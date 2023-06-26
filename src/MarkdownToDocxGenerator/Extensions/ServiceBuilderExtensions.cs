using Microsoft.Extensions.DependencyInjection; 

namespace MarkdownToDocxGenerator.Extensions
{
    public static class ServiceBuilderExtensions
    {
        /// <summary>
        /// Register all dedicated services in the service collection.
        /// </summary>
        /// <param name="sevices"></param>
        /// <param name="asSingleton">True = Register as singleton. False = Register as Transiant</param>
        public static void RegisterMarkdownToDocxGenerator(this ServiceCollection? sevices, bool asSingleton)
        {
            if (asSingleton)
            {
                sevices.AddSingleton<MdToOxmlEngine>();
                sevices.AddSingleton<MdReportGenenerator>();
            }
            else
            {
                sevices.AddTransient<MdToOxmlEngine>();
                sevices.AddTransient<MdReportGenenerator>();
            }
        }
    }
}
