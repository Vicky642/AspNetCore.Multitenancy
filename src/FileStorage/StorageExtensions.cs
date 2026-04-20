using AspNetCore.Multitenancy.FileStorage.Providers;

namespace AspNetCore.Multitenancy.FileStorage;

/// <summary>
/// Specifies which storage backend to use.
/// </summary>
public enum StorageProvider
{
    /// <summary>Local disk. For development and single-server deployments.</summary>
    Local,
    /// <summary>AWS S3.</summary>
    S3,
    /// <summary>Azure Blob Storage.</summary>
    AzureBlob,
}

/// <summary>
/// Extension methods on <see cref="MultitenancyBuilder"/> for registering tenant storage providers.
/// </summary>
public static class StorageExtensions
{
    /// <summary>
    /// Registers a local disk <see cref="ITenantStorageProvider"/> for development.
    /// </summary>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="basePath">Root directory for local storage. Defaults to <c>./tenant-storage</c>.</param>
    public static MultitenancyBuilder WithLocalStorage(this MultitenancyBuilder builder, string basePath = "./tenant-storage")
    {
        builder.Services.AddScoped<TenantStoragePathBuilder>();
        builder.Services.AddScoped<ITenantStorageProvider>(sp =>
            new LocalTenantStorageProvider(
                sp.GetRequiredService<TenantStoragePathBuilder>(),
                basePath));
        return builder;
    }

    /// <summary>
    /// Registers the AWS S3 <see cref="ITenantStorageProvider"/>.
    /// Requires <c>AWSSDK.S3</c> to be configured in your service container.
    /// </summary>
    /// <param name="builder">The multitenancy builder.</param>
    /// <param name="bucketName">The S3 bucket name.</param>
    public static MultitenancyBuilder WithS3Storage(this MultitenancyBuilder builder, string bucketName)
    {
        builder.Services.AddScoped<TenantStoragePathBuilder>();
        builder.Services.AddScoped<ITenantStorageProvider>(sp =>
            new S3TenantStorageProvider(
                sp.GetRequiredService<Amazon.S3.IAmazonS3>(),
                sp.GetRequiredService<TenantStoragePathBuilder>(),
                bucketName));
        return builder;
    }

    /// <summary>
    /// Registers the Azure Blob <see cref="ITenantStorageProvider"/>.
    /// Requires a <c>BlobContainerClient</c> to be registered separately.
    /// </summary>
    public static MultitenancyBuilder WithAzureBlobStorage(this MultitenancyBuilder builder)
    {
        builder.Services.AddScoped<TenantStoragePathBuilder>();
        builder.Services.AddScoped<ITenantStorageProvider>(sp =>
            new AzureBlobTenantStorageProvider(
                sp.GetRequiredService<Azure.Storage.Blobs.BlobContainerClient>(),
                sp.GetRequiredService<TenantStoragePathBuilder>()));
        return builder;
    }

    /// <summary>
    /// Registers a storage provider by enum value.
    /// For S3/Azure, use the typed overloads for full configuration control.
    /// </summary>
    public static MultitenancyBuilder WithTenantStorage(this MultitenancyBuilder builder, StorageProvider provider)
        => provider switch
        {
            StorageProvider.Local => builder.WithLocalStorage(),
            StorageProvider.S3 => throw new InvalidOperationException("Use WithS3Storage(bucketName) for S3 configuration."),
            StorageProvider.AzureBlob => builder.WithAzureBlobStorage(),
            _ => throw new ArgumentOutOfRangeException(nameof(provider)),
        };
}
