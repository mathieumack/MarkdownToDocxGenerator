using Microsoft.Extensions.Logging;
using OpenXMLSDK.Engine.Word;
using OpenXMLSDK.Engine.Word.ReportEngine;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MarkdownToDocxGenerator
{
    public class MdReportGenenerator
    {
        private readonly ILogger logger;
        private readonly MdToOxmlEngine parser;

        public MdReportGenenerator(ILogger<MdReportGenenerator> logger,
                                    MdToOxmlEngine engine)
        {
            this.logger = logger;
            this.parser = engine;
        }

        /// <summary>
        /// Read a markdown file and transform it to a DocumentModel
        /// </summary>
        /// <param name="outputPath">Full folder path that contains all images (and references from the markdown)</param>
        /// <param name="rootFolder">Full folder path that contains all images (and references from the markdown)</param>
        /// <param name="templatePath">Dotx template file path. Can be optional</param>
        public void Transform(string outputPath,
                                string rootFolder,
                                string templatePath = null)
        {
            // Launch transformation :
            var reports = new List<Report>();

            foreach (var filePath in Directory.GetFiles(rootFolder))
            {
                if (filePath.EndsWith(".md"))
                {
                    var report = parser.Transform(filePath, rootFolder);
                    reports.Add(report);
                }
            }

            var culture = new CultureInfo("en-US");

            using (var word = new WordManager())
            {
                if(!string.IsNullOrWhiteSpace(templatePath))
                    word.OpenDocFromTemplate(templatePath, outputPath, true);

                // Fill context :
                //word.SetTextOnBookmark("version", version);
                //word.SetTextOnBookmark("creationDate", DateTime.Now.ToString("g", culture));
                //var projectFullName = $"{projectName} - {projectIndex}";
                //for (int i = 0; i < 10; i++)
                //    word.SetTextOnBookmark($"projectName{i}", projectFullName);

                // Append documentation :
                word.AppendSubDocument(reports, true, culture);
                word.SaveDoc();
                word.CloseDoc();
            }
        }
    }
}