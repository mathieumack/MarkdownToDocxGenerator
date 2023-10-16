using Markdig;
using Markdig.Extensions.Tables; 
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.Extensions.Logging;
using OpenXMLSDK.Engine.ReportEngine.DataContext;
using OpenXMLSDK.Engine.Word;
using OpenXMLSDK.Engine.Word.ReportEngine;
using OpenXMLSDK.Engine.Word.ReportEngine.Models;
using OpenXMLSDK.Engine.Word.ReportEngine.Models.ExtendedModels;
using OpenXMLSDK.Engine.Word.Tables;
using OpenXMLSDK.Engine.Word.Tables.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Table = OpenXMLSDK.Engine.Word.ReportEngine.Models.Table;

namespace MarkdownToDocxGenerator
{
    public class MdToOxmlEngine
    {
        private readonly ILogger logger;

        private string rootFolder;

        public MdToOxmlEngine(ILogger<MdToOxmlEngine> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Read a markdown file and transform it to a DocumentModel
        /// </summary>
        /// <param name="markdownContentFilePath">Full file path to the markdown file</param>
        /// <param name="rootFolder">Full folder path that contains all images (and references from the markdown)</param>
        public Report TransformByFile(string markdownContentFilePath,
                                        string rootFolder)
        {
            var fileContent = File.ReadAllText(markdownContentFilePath);

            return Transform(fileContent, rootFolder);
        }

        /// <summary>
        /// Read a markdown file and transform it to a DocumentModel
        /// </summary>
        /// <param name="markdownContent">Full file path t the markdown file</param>
        /// <param name="rootFolder">Full folder path that contains all images (and references from the markdown)</param>
        public Report Transform(string fileContent, 
                                string rootFolder)
        {
            this.rootFolder = rootFolder;

            var result = new Report()
            {
                ContextModel = new ContextModel(),
                AddPageBreak = true
            };

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var mdDocument = Markdown.Parse(fileContent, pipeline);
            var currentPage = new Page();

            for (int i = 0; i < mdDocument.Count; i++)
            {
                var block = mdDocument[i];
                var blockType = block.GetType();
                if (blockType == typeof(HeadingBlock))
                {
                    // Add title :
                    var mdHead = (HeadingBlock)block;

                    var elements = GetContainerInlineText(mdHead.Inline);
                    var paragraph = elements.OfType<Paragraph>().FirstOrDefault();
                    paragraph.ParagraphStyleId = GetTitleStyle(mdHead.Level);
                    // Now we will delete label styles :
                    foreach (var label in paragraph.ChildElements.OfType<Label>())
                    {
                        label.FontColor = null;
                        label.FontSize = null;
                    }
                    currentPage.ChildElements.AddRange(elements);
                }
                else if (blockType == typeof(ParagraphBlock))
                {
                    var mdParagraph = (ParagraphBlock)block;

                    var elements = GetContainerInlineText(mdParagraph.Inline);
                    currentPage.ChildElements.AddRange(elements);
                }
                else if (blockType == typeof(FencedCodeBlock))
                {
                    var mdFencedCodeBlock = (FencedCodeBlock)block;

                    var paragraphs = GetFencedCodeBlockText(mdFencedCodeBlock);
                    currentPage.ChildElements.AddRange(paragraphs);
                }
                else if (blockType == typeof(ListBlock))
                {
                    var mdListBlock = (ListBlock)block;
                    var paragraphs = GetListBlock(mdListBlock);
                    currentPage.ChildElements.AddRange(paragraphs);
                }
                else if (blockType == typeof(LinkReferenceDefinitionGroup))
                {
                    var mdListBlock = (LinkReferenceDefinitionGroup)block;
                    foreach (var element in mdListBlock)
                    {
                        var link = element as LinkReferenceDefinition;
                        if (link != null)
                        {

                            // currentPage.ChildElements.Add(image);
                        }
                    }
                }
                else if (blockType == typeof(Markdig.Extensions.Tables.Table))
                {
                    var mdtable = (Markdig.Extensions.Tables.Table)block;
                    var table = GetTable(mdtable);
                    currentPage.ChildElements.Add(table);
                }
                else
                {
                    logger.LogInformation($"Block {blockType.FullName} unknow. I don't know how to transform it");
                }
            }

            result.Document = new Document()
            {
                Pages = new List<BaseElement>() { currentPage }
            };

            return result;
        }

        private string GetTitleStyle(int level)
        {
            return $"Titre{level}";
        }

        private List<BaseElement> GetListBlock(ListBlock block)
        {
            var result = new List<BaseElement>();
            var count = 1;
            foreach (ListItemBlock mdBlock in block)
            {
                var contentBlock = mdBlock.LastChild as ParagraphBlock;
                if (contentBlock != null)
                {
                    var prefix = "";
                    if (block.BulletType == '1')
                        prefix = count.ToString() + ".";
                    else
                        prefix = block.BulletType.ToString();
                    var elements = GetContainerInlineText(((ParagraphBlock)mdBlock.LastChild).Inline, prefix + " ");
                    result.AddRange(elements);
                }
                count++;
                //var elements = GetContainerInlineText(((LeafBlock)(mdBlock.LastChild).LastChild).Inline);
                //result.AddRange(elements);
            }

            //var label = (Label)elements.ChildElements[0];
            //label.Text = $" {block.BulletType}  {label.Text}";

            return result;
        }

        private List<Paragraph> GetFencedCodeBlockText(FencedCodeBlock block)
        {
            var result = new List<Paragraph>();

            foreach (var line in block.Lines)
            {
                var paragraph = new Paragraph()
                {
                    SpacingAfter = 0,
                    SpacingBefore = 0,
                    Shading = "EAEAEA",
                    ChildElements = new List<BaseElement>()
                    {
                        new Label()
                        {
                            Text = line.ToString(),
                            FontSize = "20",
                            SpaceProcessingModeValue = SpaceProcessingModeValues.Preserve
                        }
                    }
                };

                result.Add(paragraph);
            }

            return result;
        }

        private List<BaseElement> GetContainerInlineText(ContainerInline containerBlock, string labelPrefix = "")
        {
            var result = new List<BaseElement>();

            Paragraph paragraph = null;

            foreach (var inlineBlock in containerBlock)
            {
                var blockType = inlineBlock.GetType();
                if (blockType == typeof(LiteralInline))
                {
                    if (paragraph is null)
                    {
                        paragraph = new Paragraph()
                        {
                            ChildElements = new List<BaseElement>()
                        };
                        result.Add(paragraph);
                    }
                    var label = GetLiteralInlineText((LiteralInline)inlineBlock, labelPrefix: labelPrefix);
                    paragraph.ChildElements.Add(label);
                }
                else if (blockType == typeof(EmphasisInline))
                {
                    if (paragraph is null)
                    {
                        paragraph = new Paragraph()
                        {
                            ChildElements = new List<BaseElement>()
                        };
                        result.Add(paragraph);
                    }
                    var mdEmphasisInline = (EmphasisInline)inlineBlock;
                    foreach (var subBlock in mdEmphasisInline)
                    {
                        var label = GetLiteralInlineText((LiteralInline)subBlock, mdEmphasisInline.DelimiterChar == '*' && mdEmphasisInline.DelimiterCount == 2);
                        paragraph.ChildElements.Add(label);
                    }
                }
                else if (blockType == typeof(LinkInline))
                {
                    if (paragraph is null)
                    {
                        paragraph = new Paragraph()
                        {
                            ChildElements = new List<BaseElement>()
                        };
                        result.Add(paragraph);
                    }
                    var mdLinkInline = (LinkInline)inlineBlock;
                    var hyperlink = GetLinkInlineText(mdLinkInline);
                    if (hyperlink != null)
                        paragraph.ChildElements.Add(hyperlink);
                }
                else if (blockType == typeof(LineBreakInline))
                {
                    if (paragraph is null)
                    {
                        paragraph = new Paragraph()
                        {
                            ChildElements = new List<BaseElement>()
                        };
                        result.Add(paragraph);
                    }
                    var breakLineLabel = new Label()
                    {
                        Text = "",
                        FontSize = "20",
                        SpaceProcessingModeValue = SpaceProcessingModeValues.Preserve
                    };
                    paragraph.ChildElements.Add(breakLineLabel);
                }
                else if (blockType == typeof(PipeTableDelimiterInline))
                {
                    // Custom table ended with a '.' ?!
                    var mdTable = (PipeTableDelimiterInline)inlineBlock;
                    var table = GetTable(mdTable);
                    result.Add(table);
                    paragraph = null;
                }
                else
                {
                    logger.LogInformation($"ContainerInline {blockType} unknow. I don't know how to transform it");
                }
            }

            return result;
        }

        private Table GetTable(Markdig.Extensions.Tables.Table table)
        {
            var cells = GetCells(table);

            return GenerateTable(cells);
        }

        private Table GetTable(PipeTableDelimiterInline table)
        {
            var cells = GetCells(table);

            return GenerateTable(cells);
        }

        private Table GenerateTable(List<List<Cell>> cells)
        {
            var headerRow = cells[0];

            // Hypothesis : All row have the same number of values and the table is correctly formatted
            var tableCells = new List<int>();
            for (int i = 0; i < headerRow.Count; i++)
                tableCells.Add(5000 / headerRow.Count);

            var result = new Table()
            {
                TableWidth = new TableWidthModel() { Width = "5000", Type = TableWidthUnitValues.Pct },
                ColsWidth = tableCells.ToArray(),
                Borders = new BorderModel()
                {
                    BorderPositions = BorderPositions.NONE,
                    BorderColor = "FFFFFF",
                    BorderWidth = 0
                },
                HeaderRow = new Row
                {
                    Cells = headerRow
                },
                Rows = cells.Skip(cells.Count > 1 ? 1 : 0).Where(e => e.Count > 0).Select(e =>
                {
                    return new Row
                    {
                        Cells = e
                    };
                }).ToList()
            };

            SetHeaderRowStyle(result.HeaderRow);
            SetFirstColumnStyle(result.Rows);

            return result;
        }

        private void SetHeaderRowStyle(Row header)
        {
            header.Italic = true;
            header.Shading = "FFFFFF";
        }

        private void SetFirstColumnStyle(IList<Row> rows)
        {
            int i = 0;
            foreach (var row in rows)
            {
                row.Shading = i % 2 == 0 ? "EAEAEA" : "FFFFFF";
                var firstCell = row.Cells[0];
                firstCell.Shading = "EAEAEA";
                i++;
            }
        }

        private List<List<Cell>> GetCells(PipeTableDelimiterInline table)
        {
            List<List<Cell>> cells = new List<List<Cell>>();

            List<Cell> row = new List<Cell>();
            cells.Add(row);

            var e = table.FirstChild;
            while (e != null)
            {
                if (e.GetType() == typeof(LiteralInline))
                {
                    var label = GetLiteralInlineText((LiteralInline)e);
                    var cell = new Cell
                    {
                        Margin = new MarginModel { Left = 100 },
                        ChildElements = new List<BaseElement>()
                                            {
                                                label
                                            }
                    };
                    row.Add(cell);
                    e = e.NextSibling;
                }
                else if (e.GetType() == typeof(PipeTableDelimiterInline))
                {
                    //var subCells = GetCells((PipeTableDelimiterInline)e);
                    //row.AddRange(subCells[0]);
                    e = ((PipeTableDelimiterInline)e).FirstChild;
                }
                else if (e.GetType() == typeof(LineBreakInline))
                {
                    row = new List<Cell>();
                    cells.Add(row);
                    e = (e.NextSibling as PipeTableDelimiterInline)?.FirstChild;
                }
                else
                {
                    logger.LogInformation($"table cell of type {e.GetType()} unknow");
                    e = null;
                }
            }

            return cells;
        }

        private List<List<Cell>> GetCells(Markdig.Extensions.Tables.Table table)
        {
            List<List<Cell>> cells = new List<List<Cell>>();

            foreach (TableRow tableRow in table)
            {
                List<Cell> row = new List<Cell>();

                foreach (TableCell tableCell in tableRow)
                {
                    var mdParagraph = tableCell.LastChild as ParagraphBlock;

                    if (mdParagraph != null)
                    {
                        var elements = GetContainerInlineText(mdParagraph.Inline);
                        var cell = new Cell
                        {
                            Margin = new MarginModel { Left = 100 },
                            ChildElements = elements
                        };
                        row.Add(cell);
                    }
                    else
                    {
                        var cell = new Cell
                        {
                            Margin = new MarginModel { Left = 100 },
                            ChildElements = new List<BaseElement>() {
                                new Paragraph()
                                {
                                    ChildElements = new List<BaseElement>()
                                    {
                                        new Label()
                                        {
                                            Text = "",
                                            FontSize = "20",
                                            SpaceProcessingModeValue = SpaceProcessingModeValues.Preserve
                                        }
                                    }
                                }
                            }
                        };
                        row.Add(cell);
                    }
                }

                cells.Add(row);
            }
            //var e = table.FirstChild;
            //while (e != null)
            //{
            //    if (e.GetType() == typeof(LiteralInline))
            //    {
            //        var label = GetLiteralInlineText((LiteralInline)e);
            //        var cell = new Cell
            //        {
            //            Margin = new MarginModel { Left = 100 },
            //            ChildElements = new List<BaseElement>()
            //                                {
            //                                    label
            //                                }
            //        };
            //        row.Add(cell);
            //        e = e.NextSibling;
            //    }
            //    else if (e.GetType() == typeof(PipeTableDelimiterInline))
            //    {
            //        //var subCells = GetCells((PipeTableDelimiterInline)e);
            //        //row.AddRange(subCells[0]);
            //        e = ((PipeTableDelimiterInline)e).FirstChild;
            //    }
            //    else if (e.GetType() == typeof(LineBreakInline))
            //    {
            //        row = new List<Cell>();
            //        cells.Add(row);
            //        e = (e.NextSibling as PipeTableDelimiterInline)?.FirstChild;
            //    }
            //    else
            //    {
            //        logger.LogInformation($"table cell of type {e.GetType()} unknow");
            //        e = null;
            //    }
            //}

            return cells;
        }

        private Label GetLiteralInlineText(LiteralInline containerBlock,
                                            bool isBold = false,
                                            string labelPrefix = "")
        {
            var result = new Label()
            {
                Text = labelPrefix + containerBlock.Content.ToString(),
                FontSize = "20",
                SpaceProcessingModeValue = SpaceProcessingModeValues.Preserve,
                Bold = isBold
            };

            return result;
        }

        private BaseElement GetLinkInlineText(LinkInline containerBlock)
        {
            if (containerBlock.IsImage)
            {
                var imagePath = containerBlock.Url.StartsWith("/") ? containerBlock.Url.Substring(1) : containerBlock.Url;
                imagePath = Path.Combine(rootFolder, imagePath);
                if (!File.Exists(imagePath))
                    return null;

                // Try to get image width and height from tag :
                int? width = null;
                int? height = null;
                if (containerBlock.NextSibling is HtmlInline)
                {
                    var tag = (containerBlock.NextSibling as HtmlInline).Tag;
                    var regex = "(?<wd>width=[0-9]+)|(?<hg>height=[0-9]+)";
                    // Try to extract witdth and height if exists :
                    MatchCollection matches = Regex.Matches(tag, regex);

                    foreach (Match match in matches)
                    {
                        if (match.Groups["wd"].Success)
                        {
                            width = int.Parse(match.Groups["wd"].Value.Replace("width=", ""));
                        }
                        if (match.Groups["hg"].Success)
                        {
                            height = int.Parse(match.Groups["hg"].Value.Replace("height=", ""));
                        }
                    }
                }
                var image = new Image()
                {
                    Path = imagePath,
                    MaxWidth = 600,
                    ImagePartType = OpenXMLSDK.Engine.Packaging.ImagePartType.Png
                };

                if (width.HasValue)
                    image.Width = width.Value;
                if (height.HasValue)
                    image.Height = height.Value;
                    
                return image;
            }
            else
            {
                var hyperlink = new Hyperlink()
                {
                    WebSiteUri = containerBlock.Url,
                    ChildElements = new List<BaseElement>()
                };

                if (containerBlock.FirstChild.GetType() == typeof(LiteralInline))
                {
                    var label = GetLiteralInlineText((LiteralInline)containerBlock.FirstChild);
                    label.Underline = new UnderlineModel
                    {
                        Color = "40A6DB",
                        Val = UnderlineValues.Single
                    };
                    hyperlink.Text = label;
                }
                else
                {
                    logger.LogInformation($"{containerBlock.FirstChild.GetType()} unknow. I don't know how to transform it as hyperlink");
                }
                return hyperlink;
            }
        }
    }
}