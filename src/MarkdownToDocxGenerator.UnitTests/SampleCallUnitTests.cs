using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 
using MarkdownToDocxGenerator.Extensions;

namespace MarkdownToDocxGenerator.UnitTests
{
    [TestClass]
    public class SampleCallUnitTests
    {
        /// <summary>
        /// The goal of this unit test is just to ensure that the wode does not g�n�rate exception.
        /// You can use it as it in order to test the library with your own data.
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            // Configuration of sample data :
            var rootFolder = Path.Combine(Environment.CurrentDirectory, "MdFiles");
            var templatePath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.dotx");
            var outputPath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.docx");

            // Create Service provider :
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.RegisterMarkdownToDocxGenerator(true);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get the report tool :
            var parser = serviceProvider.GetRequiredService<MdReportGenenerator>();

            // Generate Word document :
            parser.Transform(outputPath, rootFolder, templatePath);
        }
    }
}