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
        [DataRow("", DisplayName = "Transform from empty document")]
        [DataRow("Dotx/sample.dotx", DisplayName = "Transform with template sample.dotx")]
        public void TransformWithFiles(string templateFileName)
        {
            // Configuration of sample data :
            var rootFolder = Path.Combine(Environment.CurrentDirectory, "MdFiles");
            var outputPath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample_file.docx");

            // Create Service provider :
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.RegisterMarkdownToDocxGenerator(true);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get the report tool :
            var parser = serviceProvider.GetRequiredService<MdReportGenenerator>();

            // Generate Word document :
            if (string.IsNullOrWhiteSpace(templateFileName))
            {
                parser.Transform(outputPath, rootFolder);
            }
            else
            {
                var templatePath = Path.Combine(Environment.CurrentDirectory, templateFileName);
                parser.Transform(outputPath, rootFolder, templatePath);
            }
        }

        /// <summary>
        /// The goal of this unit test is just to ensure that the wode does not g�n�rate exception.
        /// You can use it as it in order to test the library with your own data.
        /// </summary>
        [TestMethod]
        [DataRow("", DisplayName = "Transform with stream from empty document")]
        [DataRow("Dotx/sample.dotx", DisplayName = "Transform with stream with template sample.dotx")]
        public void TransformWithStream(string templateFileName)
        {
            // Configuration of sample data :
            var rootFolder = Path.Combine(Environment.CurrentDirectory, "MdFiles");
            var outputPath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample_stream.docx");

            // Create Service provider :
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure => configure.AddConsole());
            serviceCollection.RegisterMarkdownToDocxGenerator(true);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Now get the report tool :
            var parser = serviceProvider.GetRequiredService<MdReportGenenerator>();

            // Generate Word document :
            // Generate Word document :
            if (string.IsNullOrWhiteSpace(templateFileName))
            {
                var markdownContent = File.ReadAllText(Path.Combine(rootFolder, "0.md"));
                using var result = parser.TransformWithStream(new List<string>() { markdownContent });

                using var file = File.OpenWrite(outputPath);
                result.CopyTo(file);
            }
            else
            {
                var templatePath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.dotx");
                using var templateStream = File.OpenRead(templatePath);
                var markdownContent = File.ReadAllText(Path.Combine(rootFolder, "0.md"));
                using var result = parser.TransformWithStream(new List<string>() { markdownContent }, templateStream);

                using var file = File.OpenWrite(outputPath);
                result.CopyTo(file);
            }
        }
    }
}