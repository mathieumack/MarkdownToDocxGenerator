using Microsoft.Extensions.Logging;
using OpenXMLSDK.Engine.Word;
using OpenXMLSDK.Engine.Word.ReportEngine;
using System;
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
        /// Read some markdown files and transform it to a DocumentModel
        /// </summary>
        /// <param name="outputPath">Full folder path that contains all images (and references from the markdown)</param>
        /// <param name="rootFolder">Full folder path that contains all images (and references from the markdown)</param>
        /// <param name="templatePath">Dotx template file path. Optional</param>
        /// <param name="preHook">Action executed on the word document before integrate the md files. Optional</param>
        /// <param name="preHook">Action executed on the word document after integrate the md files. Optional</param>
        public void Transform(string outputPath,
                                string rootFolder,
                                string templatePath = null,
                                Action<WordManager> preHook = null,
                                Action<WordManager> postHook = null)
        {
            var reports = new List<Report>();

            foreach (var filePath in Directory.GetFiles(rootFolder))
            {
                if (filePath.EndsWith(".md"))
                {
                    var report = parser.TransformByFile(filePath, rootFolder);
                    reports.Add(report);
                }
            }

            var culture = new CultureInfo("en-US");

            using (var word = new WordManager())
            {
                if(!string.IsNullOrWhiteSpace(templatePath))
                    word.OpenDocFromTemplate(templatePath, outputPath, true);

                // Pre hook :
                preHook?.Invoke(word);
                
                // Append documentation :
                word.AppendSubDocument(reports, true, culture);

                // Post hook :
                postHook?.Invoke(word);

                word.SaveDoc();
                word.CloseDoc();
            }
        }


        /// <summary>
        /// Read some markdown files and transform it to a DocumentModel Returs a stream to the generated file
        /// </summary>
        /// <param name="markdownFilesContent">A list of markdown files</param>
        /// <param name="templateDoument">Dotx template file path. Optional</param>
        /// <param name="preHook">Action executed on the word document before integrate the md files. Optional</param>
        /// <param name="preHook">Action executed on the word document after integrate the md files. Optional</param>
        public Stream Transform(List<string> markdownFilesContent,
                                Stream templateDoument = null,
                                Action<WordManager> preHook = null,
                                Action<WordManager> postHook = null)
        {
            // Launch transformation :
            var reports = new List<Report>();

            foreach (var fileContent in markdownFilesContent)
            {
                var report = parser.Transform(fileContent, "");
                reports.Add(report);
            }

            var culture = new CultureInfo("en-US");

            using (var word = new WordManager())
            {
                if (templateDoument is null || templateDoument == Stream.Null)
                    word.OpenDoc(templateDoument, true);

                // Pre hook :
                preHook?.Invoke(word);

                // Append documentation :
                word.AppendSubDocument(reports, true, culture);

                // Post hook :
                postHook?.Invoke(word);

                word.SaveDoc();

                return word.GetMemoryStream();
            }
        }
    }
}