namespace AspNetCore.Multitenancy.FileStorage.Providers;

/// <summary>
/// Tenant-isolated local disk storage provider.
/// Stores files under <c>{BasePath}/tenant-{tenantId}/{path}</c>.
/// Use for development, single-server deployments, or CI environments.
/// </summary>
public class LocalTenantStorageProvider : ITenantStorageProvider
{
    private readonly TenantStoragePathBuilder _pathBuilder;
    private readonly string _basePath;

    /// <param name="pathBuilder">The tenant-aware path builder.</param>
    /// <param name="basePath">Root directory on disk. Defaults to <c>./tenant-storage</c>.</param>
    public LocalTenantStorageProvider(TenantStoragePathBuilder pathBuilder, string basePath = "./tenant-storage")
    {
        _pathBuilder = pathBuilder;
        _basePath = basePath;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(string path, Stream content, string? contentType = null, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, _pathBuilder.Build(path).Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var file = File.Create(fullPath);
        await content.CopyToAsync(file, cancellationToken);

        return _pathBuilder.Build(path);
    }

    /// <inheritdoc />
    public Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, _pathBuilder.Build(path).Replace('/', Path.DirectorySeparatorChar));
        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult(stream);
    }

    /// <inheritdoc />
    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, _pathBuilder.Build(path).Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> ListAsync(string? prefix = null, CancellationToken cancellationToken = default)
    {
        var tenantPath = Path.Combine(_basePath, $"tenant-{prefix}");
        if (!Directory.Exists(tenantPath))
            return Task.FromResult(Enumerable.Empty<string>());

        var searchPrefix = prefix is not null ? _pathBuilder.Build(prefix) : string.Empty;
        var fullSearch = Path.Combine(_basePath, searchPrefix.Replace('/', Path.DirectorySeparatorChar));

        var files = Directory.EnumerateFiles(fullSearch, "*", SearchOption.AllDirectories)
            .Select(f => _pathBuilder.StripPrefix(
                Path.GetRelativePath(_basePath, f).Replace(Path.DirectorySeparatorChar, '/')));

        return Task.FromResult(files);
    }
}
