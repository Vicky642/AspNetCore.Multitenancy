namespace AspNetCore.Multitenancy;

/// <summary>
/// Fluent builder returned by <see cref="ServiceCollectionExtensions.AddMultitenancy"/>.
/// Use the <c>With*</c> extension methods from each package to register features.
/// </summary>
public class MultitenancyBuilder
{
    /// <summary>Gets the underlying service collection.</summary>
    public IServiceCollection Services { get; }

    /// <summary>Gets the configured options.</summary>
    public MultitenancyOptions Options { get; }

    internal MultitenancyBuilder(IServiceCollection services, MultitenancyOptions options)
    {
        Services = services;
        Options = options;
    }
}
