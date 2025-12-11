using Microsoft.Extensions.Logging;
using OpenXMLSDK.Engine.Word.ReportEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownToDocxGenerator.Helpers;

public static class ReportsReader
{
    public static List<Report> GetReports(string rootFolder, string folder, MdToOxmlEngine parser, ILogger logger)
    {
        var reports = new List<Report>();

        logger.LogInformation("Check folder {folder}", folder);

        var orderFilePath = Path.Combine(folder, ".order");
        if (File.Exists(orderFilePath))
        {
            logger.LogInformation(".order founded");

            var pages = File.ReadAllLines(Path.Combine(folder, ".order"));
            foreach (var page in pages)
            {
                var filePath = Path.Combine(folder, page + ".md");
                logger.LogInformation("Check file {filePath}", filePath);

                var fileContent = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    var report = parser.Transform(fileContent, rootFolder);
                    reports.Add(report);
                }
                else
                    logger.LogInformation("File is empty");

                // Check sub pages :
                var subFolderPath = Path.Combine(folder, page);
                if (Directory.Exists(subFolderPath))
                {
                    logger.LogInformation("Check sub pages {subFolderPath}", subFolderPath);
                    reports.AddRange(GetReports(rootFolder, subFolderPath, parser, logger));
                }
            }
        }
        else
        {
            logger.LogInformation($"{orderFilePath} not founded");

            var files = Directory.GetFiles(folder, "*.md");
            foreach (var filePath in files.OrderBy(e => e))
            {
                logger.LogInformation("Check file {filePath}", filePath);

                var fileContent = File.ReadAllText(Path.Combine(folder, filePath));
                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    var report = parser.Transform(fileContent, rootFolder);
                    reports.Add(report);

                    // Check sub pages :
                    var subFolderPath = Path.Combine(folder, filePath.Replace(".md", ""));
                    if (Directory.Exists(subFolderPath))
                    {
                        logger.LogInformation("Check sub pages {subFolderPath}", subFolderPath);
                        reports.AddRange(GetReports(rootFolder, subFolderPath, parser, logger));
                    }

                }
                else
                    logger.LogInformation("File is empty");
            }
        }

        return reports;
    }
}
