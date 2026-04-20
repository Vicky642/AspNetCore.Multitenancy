namespace AspNetCore.Multitenancy.FileStorage;

/// <summary>
/// Abstraction for tenant-isolated file storage.
/// All paths are automatically prefixed with the current tenant ID —
/// tenant A can never access tenant B's files even in a shared bucket.
/// </summary>
public interface ITenantStorageProvider
{
    /// <summary>
    /// Uploads a file. The path is automatically prefixed with the tenant ID.
    /// </summary>
    /// <param name="path">Logical path (e.g., "invoices/jan.pdf").</param>
    /// <param name="content">File content stream.</param>
    /// <param name="contentType">Optional MIME type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The full storage path including tenant prefix.</returns>
    Task<string> UploadAsync(string path, Stream content, string? contentType = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file by its logical path.
    /// </summary>
    /// <param name="path">Logical path (e.g., "invoices/jan.pdf").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file by its logical path.
    /// </summary>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all files under the given prefix (within the current tenant's namespace).
    /// </summary>
    /// <param name="prefix">Optional path prefix to filter results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IEnumerable<string>> ListAsync(string? prefix = null, CancellationToken cancellationToken = default);
}
