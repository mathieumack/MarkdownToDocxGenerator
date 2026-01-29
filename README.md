
# MarkdownToDocxGenerator

[![.NET](https://github.com/mathieumack/MarkdownToDocxGenerator/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MarkdownToDocxGenerator/actions/workflows/ci.yml)
![NuGet Version](https://img.shields.io/nuget/v/MarkdownToDocxGenerator)

> **Transform your Markdown documentation into professional Word documents effortlessly.**  
> Stop manual document creation. Automate your documentation workflow with .NET.

---

## 📖 What is MarkdownToDocxGenerator?

**MarkdownToDocxGenerator** is a powerful .NET library that converts Markdown files into professional DOCX (Word) documents. Whether you're generating technical documentation, reports, or automated documentation workflows, this library seamlessly integrates into your .NET applications with minimal configuration.

### ✨ Key Features

- **📄 Markdown to DOCX Conversion**: Transform a folder of `.md` files into a single, formatted Word document
- **🎨 Custom Templates**: Apply your own `.dotx` Word template for consistent branding and styling
- **💉 Dependency Injection Ready**: Easily register as singleton or transient in your DI container
- **📊 Logging Support**: Integrates with `ILogger` for comprehensive diagnostics and monitoring
- **🔧 Pre/Post Processing Hooks**: Customize the Word document before and after Markdown integration
- **🚀 Stream Support**: Generate documents in-memory for web APIs and advanced scenarios
- **🌐 Cross-Platform**: Works on Windows, Linux, and containers with proper configuration

---

## 🚀 Getting Started

### Prerequisites

- **.NET Requirements**: .NET Standard 2.0+ / .NET 8+ / .NET 9+ / .NET 10+
- **Linux/Containers**: Install `libgdiplus` for DOCX/image processing

### Installation

Install the NuGet package via the Package Manager Console:

```powershell
Install-Package MarkdownToDocxGenerator
```

Or using the .NET CLI:

```bash
dotnet add package MarkdownToDocxGenerator
```

**Supported frameworks**: `netstandard2.0`, `net9.0`, `net10.0`

### Platform-Specific Configuration

#### Linux/Docker Containers

You must install `libgdiplus` for DOCX and image processing. Add this to your Dockerfile:

```dockerfile
RUN apt-get update && apt-get install -y libgdiplus
```

For Alpine Linux:

```dockerfile
RUN apk add --no-cache libgdiplus
```

> **💡 Tip**: If you encounter GDI+ or image processing errors, ensure this library is installed.

### Dependencies

This library uses the following packages:
- [Markdig](https://github.com/xoofx/markdig) - Fast and extensible Markdown processor
- [OpenXMLSDK.Engine](https://github.com/OfficeDev/Open-XML-SDK) - Office document manipulation
- [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) - DI support
- [Microsoft.Extensions.Logging](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging) - Logging infrastructure

---

## 💻 Usage

### Basic Usage

#### 1. Register the Service

In your `Program.cs` or `Startup.cs`:

```csharp
services.AddMarkdownToDocxGenerator();

// Or for advanced control:
// services.RegisterMarkdownToDocxGenerator(asSingleton: true);
```

> **⚠️ Important**: You must register a logger (`ILogger`) before calling `AddMarkdownToDocxGenerator`.

#### 2. Inject and Use the Generator

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

**Transform Parameters:**
- `outputPath`: Path to the output DOCX file
- `rootFolder`: Path to the folder containing Markdown files
- `templatePath`: (Optional) Path to a `.dotx` template file

---

### Advanced Usage

#### In-Memory Document Generation

Use `TransformWithStream` to generate a DOCX as a `Stream` (ideal for web APIs and cloud services):

```csharp
var markdownContents = new List<string> { "# Title", "Some content..." };
using var templateStream = File.OpenRead("template.dotx");
using var docxStream = reportGenerator.TransformWithStream(markdownContents, templateStream);
// Save or return docxStream as needed
```

#### Pre/Post Processing Hooks

Customize the Word document before or after Markdown integration:

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

## 🎯 What You Get

### Benefits

- **⏱️ Time Savings**: Automate documentation generation instead of manual Word editing
- **📝 Consistency**: Use templates to ensure uniform formatting across all documents
- **🔄 Automation**: Integrate into CI/CD pipelines for automatic documentation updates
- **🧪 Testable**: Unit tests included - see the test project for real-world examples
- **🛡️ Code Quality**: Built with CodeQL security analysis for safe, reliable code

### Real-World Use Cases

- **Technical Documentation**: Generate API documentation, user guides, and technical specifications
- **Report Generation**: Create automated business reports, project status updates
- **Documentation Pipelines**: Integrate with CI/CD to keep documentation in sync with code
- **Multi-Format Publishing**: Convert Markdown content to Word for further distribution

---

## 🧪 Testing & Examples

Explore the [unit tests](./src/MarkdownToDocxGenerator.UnitTests) in the repository for real-world usage examples and edge cases.

---

## 🔧 Troubleshooting

| Issue | Solution |
|-------|----------|
| **GDI+ or Image Errors (Linux/Containers)** | Ensure `libgdiplus` is installed in your environment |
| **Logging Not Working** | The library uses `ILogger` - verify logging is configured in your application |
| **Template Issues** | If the template file is missing or invalid, check logs for warnings. The generator will skip invalid templates |

---

## 🤝 Contributing

We welcome contributions! Here's how you can help:

- 🐛 **Found a bug?** [Open an issue](https://github.com/mathieumack/MarkdownToDocxGenerator/issues)
- 💡 **Have a feature idea?** [Submit a feature request](https://github.com/mathieumack/MarkdownToDocxGenerator/issues/new)
- 🔧 **Want to contribute code?** Check out our [contribution guidelines](./CONTRIBUTING.md)

If you find this project useful, please ⭐ star it on GitHub!

---

## 📄 License

This project is licensed under the terms specified in the [LICENSE](./LICENSE) file.

---

## ☕ Support the Author

If you find this project helpful, consider supporting the author:

[![Buy Me A Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/mathieumack)