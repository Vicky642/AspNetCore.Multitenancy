using Amazon.S3;
using Amazon.S3.Model;

namespace AspNetCore.Multitenancy.FileStorage.Providers;

/// <summary>
/// Tenant-isolated AWS S3 storage provider.
/// All objects are stored with a <c>tenant-{tenantId}/</c> key prefix,
/// so tenant A can never access tenant B's objects even in a shared bucket.
/// </summary>
public class S3TenantStorageProvider : ITenantStorageProvider
{
    private readonly IAmazonS3 _s3Client;
    private readonly TenantStoragePathBuilder _pathBuilder;
    private readonly string _bucketName;

    /// <param name="s3Client">The configured AWS S3 client.</param>
    /// <param name="pathBuilder">The tenant-aware path builder.</param>
    /// <param name="bucketName">The S3 bucket name to use.</param>
    public S3TenantStorageProvider(IAmazonS3 s3Client, TenantStoragePathBuilder pathBuilder, string bucketName)
    {
        _s3Client = s3Client;
        _pathBuilder = pathBuilder;
        _bucketName = bucketName;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(string path, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var key = _pathBuilder.Build(path);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = content,
            ContentType = contentType ?? "application/octet-stream",
            AutoCloseStream = false,
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
        return key;
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var key = _pathBuilder.Build(path);
        var response = await _s3Client.GetObjectAsync(_bucketName, key, cancellationToken);
        return response.ResponseStream;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var key = _pathBuilder.Build(path);
        await _s3Client.DeleteObjectAsync(_bucketName, key, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> ListAsync(string? prefix = null, CancellationToken cancellationToken = default)
    {
        var tenantPrefix = _pathBuilder.Build(prefix ?? string.Empty);

        var request = new ListObjectsV2Request
        {
            BucketName = _bucketName,
            Prefix = tenantPrefix,
        };

        var response = await _s3Client.ListObjectsV2Async(request, cancellationToken);
        return response.S3Objects.Select(o => _pathBuilder.StripPrefix(o.Key));
    }
}
