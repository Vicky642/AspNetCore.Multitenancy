namespace AspNetCore.Multitenancy;

/// <summary>
/// Thrown when a tenant cannot be resolved from the current HTTP request,
/// and the application requires a valid tenant context.
/// </summary>
public class TenantNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance with a default message.
    /// </summary>
    public TenantNotFoundException()
        : base("No tenant could be resolved for the current request.")
    {
    }

    /// <summary>
    /// Initializes a new instance with the specified tenant identifier.
    /// </summary>
    /// <param name="tenantId">The tenant ID that could not be found.</param>
    public TenantNotFoundException(string tenantId)
        : base($"Tenant '{tenantId}' was not found or is inactive.")
    {
        TenantId = tenantId;
    }

    /// <summary>
    /// Initializes a new instance with a message and inner exception.
    /// </summary>
    public TenantNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Gets the tenant ID that triggered the exception, if available.
    /// </summary>
    public string? TenantId { get; }
}
