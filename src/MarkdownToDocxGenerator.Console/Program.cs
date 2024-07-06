using MarkdownToDocxGenerator.Extensions;
using MarkdownToDocxGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenXMLSDK.Engine.Word;
using System.Globalization;
using MarkdownToDocxGenerator.Console;

// Init Service collection :
var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(configure => configure.AddConsole());
serviceCollection.RegisterMarkdownToDocxGenerator(true);
var serviceProvider = serviceCollection.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Start processing the report.");

// Check parameters :
var arguments = Environment.GetCommandLineArgs();

// Check argumens :
logger.LogInformation($"Arguments ({arguments.Length}) : {string.Join(',', arguments)}");

if (arguments.Length != 7)
{
    logger.LogWarning($"Missing parameters : {arguments.Length} sent");
    return 0;
}

var rootFolder = arguments[1];
if (!Directory.Exists(rootFolder))
{
    logger.LogWarning("Document folder does not exists");
    return 0;
}
var templatePath = arguments[2];
if (!File.Exists(templatePath))
{
    logger.LogWarning("Template file does not exists");
    return 0;
}

var version = arguments[3];

var outputPath = arguments[4];
if (string.IsNullOrWhiteSpace(outputPath))
{
    logger.LogWarning("Output file is not valid");
    return 0;
}

var projectName = arguments[5];
if (string.IsNullOrWhiteSpace(projectName))
{
    logger.LogWarning("Project name is not valid");
    return 0;
}

var projectIndex = arguments[6];
if (string.IsNullOrWhiteSpace(projectIndex))
{
    logger.LogWarning("Project index is not valid");
    return 0;
}

// Launch transformation :
var parser = serviceProvider.GetService<MdToOxmlEngine>();
var reports = ReportsReader.GetReports(rootFolder, rootFolder, parser, logger);

var culture = new CultureInfo("en-US");

using (var word = new WordManager())
{
    word.OpenDocFromTemplate(templatePath, outputPath, true);

    // Fill context :
    word.SetTextOnBookmark("version", version);
    word.SetTextOnBookmark("creationDate", DateTime.Now.ToString("g", culture));
    var projectFullName = $"{projectName} - {projectIndex}";
    for (int i = 0; i < 10; i++)
        word.SetTextOnBookmark($"projectName{i}", projectFullName);

    // Append documentation :
    word.AppendSubDocument(reports, true, culture);

    logger.LogInformation($"Save file {outputPath}");

    word.SaveDoc();
    word.CloseDoc();
}

return 0;