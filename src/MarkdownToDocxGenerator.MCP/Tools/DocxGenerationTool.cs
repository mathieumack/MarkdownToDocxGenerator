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
                                        [Description("Local dotx full document template file path (absolute file path)")] string dotxTemplateFile,
                                        [Description("Local root full folder path that contains all markdown files (absolute file path)")]string rootFolder)
    {
        var reports = ReportsReader.GetReports(rootFolder, rootFolder, parser, logger);

        var culture = new CultureInfo("en-US");

        using (var word = new WordManager())
        {
            var outputPath = dotxTemplateFile.Replace(".dotx", ".docx");
            word.OpenDocFromTemplate(dotxTemplateFile, outputPath, true);

            // Fill context :
            word.SetTextOnBookmark("creationDate", DateTime.Now.ToString("g", culture));

            // Append documentation :
            word.AppendSubDocument(reports, true, culture);

            logger.LogInformation($"Save file {outputPath}");

            word.SaveDoc();
            word.CloseDoc();

            return "Document has been saved to " + outputPath;
        }
    }
}