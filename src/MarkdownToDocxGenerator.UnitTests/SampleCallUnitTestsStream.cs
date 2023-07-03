using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; 
using MarkdownToDocxGenerator.Extensions;

namespace MarkdownToDocxGenerator.UnitTests
{
    [TestClass]
    public class SampleCallUnitTestsStream
    {
        /// <summary>
        /// The goal of this unit test is just to ensure that the wode does not g�n�rate exception.
        /// You can use it as it in order to test the library with your own data.
        /// </summary>
        [TestMethod]
        public void TransformWithFiles()
        {
            // Configuration of sample data :
            var rootFolder = Path.Combine(Environment.CurrentDirectory, "MdFiles");
            var templatePath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.dotx");
            var outputPath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample_file.docx");

            // Create Service provider :
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.RegisterMarkdownToDocxGenerator(true);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get the report tool :
            var parser = serviceProvider.GetRequiredService<MdReportGenenerator>();

            // Generate Word document :
            using var templateStream = File.OpenRead(templatePath);
            var markdownContent = File.ReadAllText(Path.Combine(rootFolder, "1.md"));
            var markdownContent2 = File.ReadAllText(Path.Combine(rootFolder, "1.md"));

            parser.Transform(outputPath, rootFolder, templatePath);
        }

        /// <summary>
        /// The goal of this unit test is just to ensure that the wode does not g�n�rate exception.
        /// You can use it as it in order to test the library with your own data.
        /// </summary>
        [TestMethod]
        public void TransformWithStream()
        {
            // Configuration of sample data :
            var rootFolder = Path.Combine(Environment.CurrentDirectory, "MdFiles");
            var templatePath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.dotx");
            var outputPath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample_stream.docx");

            // Create Service provider :
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.RegisterMarkdownToDocxGenerator(true);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get the report tool :
            var parser = serviceProvider.GetRequiredService<MdReportGenenerator>();

            // Generate Word document :
            using var templateStream = File.OpenRead(templatePath);
            var markdownContent = File.ReadAllText(Path.Combine(rootFolder, "0.md"));
            //var markdownContent2 = File.ReadAllText(Path.Combine(rootFolder, "1.md"));

            using var result = parser.TransformWithStream(new List<string>() { markdownContent }, templateStream);

            using var file = File.OpenWrite(outputPath);
            result.CopyTo(file);
        }
    }
}