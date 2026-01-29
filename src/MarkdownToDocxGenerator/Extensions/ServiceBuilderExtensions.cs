using Microsoft.Extensions.DependencyInjection; 

namespace MarkdownToDocxGenerator.Extensions;

public static class ServiceBuilderExtensions
{
    /// <summary>
    /// Register all dedicated services in the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="asSingleton">True = Register as singleton. False = Register as Transiant</param>
    public static IServiceCollection RegisterMarkdownToDocxGenerator(this IServiceCollection services, bool asSingleton)
    {
        if (asSingleton)
        {
            services.AddSingleton<MdToOxmlEngine>();
            services.AddSingleton<MdReportGenenerator>();
        }
        else
        {
            services.AddTransient<MdToOxmlEngine>();
            services.AddTransient<MdReportGenenerator>();
        }

        return services;
    }
}
