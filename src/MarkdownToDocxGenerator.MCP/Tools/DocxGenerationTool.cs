using MarkdownToDocxGenerator.Helpers;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using OpenXMLSDK.Engine.Word;
using OpenXMLSDK.Engine.Word.Bookmarks;
using System.ComponentModel;
using System.Globalization;

namespace MarkdownToDocxGenerator.MCP.Tools;

[McpServerToolType]
public class DocxGenerationTool
{
    [McpServerTool, Description("Generate a DOCX document from Markdown.")]
    public static string GenerateDocx(MdToOxmlEngine parser, 
                                        ILogger<DocxGenerationTool> logger,
                                        [Description("Dotx full document template file path (absolute file path)")] string dotxTemplateFile,
                                        [Description("Root full folder path that contains all markdown files (absolute file path)")]string rootFolder)
    {
        if (!File.Exists(dotxTemplateFile))
            return "The template does not exists, check dotx file path.";
        if (!Directory.Exists(rootFolder))
            return "Root folder does not exists, check that folder exists and contains some md files.";

        var reports = ReportsReader.GetReports(rootFolder, rootFolder, parser, logger);

        var culture = new CultureInfo("en-US");

        using (var word = new WordManager())
        {
            var outputPath = Path.ChangeExtension(dotxTemplateFile, "docx");
            word.OpenDocFromTemplate(dotxTemplateFile, outputPath, true);

            // Fill context :
            word.SetTextOnBookmark("creationDate", DateTime.Now.ToString("g", culture));

            // Append documentation :
            word.AppendSubDocument(reports, true, culture);

            logger.LogInformation("Save file {outputPath}", outputPath);

            word.SaveDoc();
            word.CloseDoc();

            return "Document has been saved to " + outputPath;
        }
    }
}