using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AspNetCore.Multitenancy.FileStorage.Providers;

/// <summary>
/// Tenant-isolated Azure Blob Storage provider.
/// Each tenant's files are stored under a <c>tenant-{tenantId}/</c> blob path prefix
/// within a shared container.
/// </summary>
public class AzureBlobTenantStorageProvider : ITenantStorageProvider
{
    private readonly BlobContainerClient _containerClient;
    private readonly TenantStoragePathBuilder _pathBuilder;

    /// <param name="containerClient">The Azure Blob container client.</param>
    /// <param name="pathBuilder">The tenant-aware path builder.</param>
    public AzureBlobTenantStorageProvider(BlobContainerClient containerClient, TenantStoragePathBuilder pathBuilder)
    {
        _containerClient = containerClient;
        _pathBuilder = pathBuilder;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(string path, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var blobPath = _pathBuilder.Build(path);
        var blobClient = _containerClient.GetBlobClient(blobPath);

        var options = new BlobUploadOptions
        {
            HttpHeaders = contentType is not null
                ? new BlobHttpHeaders { ContentType = contentType }
                : null,
        };

        await blobClient.UploadAsync(content, options, cancellationToken);
        return blobPath;
    }

    /// <inheritdoc />
    public async Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var blobPath = _pathBuilder.Build(path);
        var blobClient = _containerClient.GetBlobClient(blobPath);
        var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var blobPath = _pathBuilder.Build(path);
        var blobClient = _containerClient.GetBlobClient(blobPath);
        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> ListAsync(string? prefix = null, CancellationToken cancellationToken = default)
    {
        var tenantPrefix = _pathBuilder.Build(prefix ?? string.Empty);
        var results = new List<string>();

        await foreach (var item in _containerClient.GetBlobsAsync(prefix: tenantPrefix, cancellationToken: cancellationToken))
        {
            results.Add(_pathBuilder.StripPrefix(item.Name));
        }

        return results;
    }
}
