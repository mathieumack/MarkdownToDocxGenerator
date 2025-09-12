
# MarkdownToDocxGenerator

Generate professional DOCX (Word) documents from a folder of Markdown files with .NET. Easily integrate into your .NET applications, use custom templates, and automate documentation workflows.

---

## Features

- **Convert Markdown to DOCX**: Transform a folder of `.md` files into a single Word document.
- **Custom Templates**: Use your own `.dotx` Word template for consistent branding.
- **Dependency Injection Ready**: Register as singleton or transient in your DI container.
- **Logging**: Integrates with `ILogger` for diagnostics.
- **Pre/Post Processing Hooks**: Customize the Word document before/after Markdown integration.
- **Stream Support**: Generate documents in-memory for advanced scenarios.

---

## Installation

Install the NuGet package:

```powershell
Install-Package MarkdownToDocxGenerator
```

Supported frameworks: `netstandard2.0`, `net9.0`, `net10.0`

---

## Dependencies

- [.NET Standard 2.0+ / .NET 8+ / .NET 9+ / .NET 10+](https://dotnet.microsoft.com/)
- [Markdig](https://github.com/xoofx/markdig)
- [OpenXMLSDK.Engine](https://github.com/OfficeDev/Open-XML-SDK)
- [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [Microsoft.Extensions.Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)

---

## Platform Requirements

**Linux/Containers:** You must install `libgdiplus` for DOCX/image processing. For example, in your Dockerfile:

```dockerfile
RUN apt-get update && apt-get install -y libgdiplus
```

For Alpine Linux, use:

```dockerfile
RUN apk add --no-cache libgdiplus
```

If you see errors related to GDI+ or image processing, ensure this library is present.

---

## Quick Start

### 1. Register the Service

In your `Program.cs` or `Startup.cs`:

```csharp
services.AddMarkdownToDocxGenerator();
// or for advanced control:
// services.RegisterMarkdownToDocxGenerator(asSingleton: true);
```

> **Note:** You must register a logger (ILogger) before calling `AddMarkdownToDocxGenerator`.

### 2. Inject and Use the Generator

```csharp
public class MyService
{
    private readonly MdReportGenenerator reportGenerator;

    public MyService(MdReportGenenerator reportGenerator)
    {
        this.reportGenerator = reportGenerator;
    }

    public void GenerateReport()
    {
        var rootFolder = Path.Combine(Environment.CurrentDirectory, "MdFiles");
        var templatePath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.dotx");
        var outputPath = Path.Combine(Environment.CurrentDirectory, "Dotx/sample.docx");

        reportGenerator.Transform(outputPath, rootFolder, templatePath);
    }
}
```

#### Transform Parameters
- `outputPath`: Path to the output DOCX file
- `rootFolder`: Path to the folder containing Markdown files
- `templatePath`: (Optional) Path to a `.dotx` template file

---

## Advanced Usage

### In-Memory Document Generation

Use `TransformWithStream` to generate a DOCX as a `Stream` (for web APIs, etc):

```csharp
var markdownContents = new List<string> { "# Title", "Some content..." };
using var templateStream = File.OpenRead("template.dotx");
using var docxStream = reportGenerator.TransformWithStream(markdownContents, templateStream);
// Save or return docxStream as needed
```

### Pre/Post Processing Hooks

You can pass actions to run before or after Markdown integration:

```csharp
reportGenerator.Transform(
    outputPath,
    rootFolder,
    templatePath,
    preHook: word => { /* customize WordManager before content */ },
    postHook: word => { /* finalize document */ }
);
```

---

## Testing & Samples

- Check the unit tests in the repository for real-world usage and edge cases.
- More samples and advanced scenarios are available in the [Wiki](https://github.com/mathieumack/MarkdownToDocxGenerator/wiki).

---

## Troubleshooting

- **Linux/Containers:** Ensure `libgdiplus` is installed if you see GDI+ or image errors.
- **Logging:** The library uses `ILogger` for diagnostics. Make sure logging is configured in your app.
- **Template Issues:** If the template file is missing or invalid, the generator will log a warning and skip it.

---

## Contributing & Support

- Found a bug or have a feature request? [Open an issue](https://github.com/mathieumack/MarkdownToDocxGenerator/issues) or submit a PR.
- Contributions are welcome! Please see the [contribution guidelines](https://github.com/mathieumack/MarkdownToDocxGenerator/blob/main/CONTRIBUTING.md) if available.
- If you like this project, please star it!

---

## Badges

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mathieumack_MarkdownToDocxGenerator&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mathieumack_MarkdownToDocxGenerator)
[![.NET](https://github.com/mathieumack/MarkdownToDocxGenerator/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MarkdownToDocxGenerator/actions/workflows/ci.yml)
![NuGet Version](https://img.shields.io/nuget/v/MarkdownToDocxGenerator)

---

## Support the Author

If you find this project useful, you can support the author with a coffee:

[![](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/mathieumack)