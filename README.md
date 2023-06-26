# MarkdownToDocxGenerator
This library is used to generate a docx file from a folder containing markdown files.

==========

# Onboarding Instructions 

## Installation

1. Add nuget package: 

> Install-Package MarkdownToDocxGenerator

2. In your application, you can initialize the tools in the service collection. Open your program.cs or Startup.cs and add the following code:

```csharp
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMarkdownToDocxGenerator();
            });
```


Note : The library used the ILogger class to generate logs. So you need to register the logger before calling the AddMarkdownToDocxGenerator function.

A MdReportGenenerator object is now registered in the dependency injection container. You can now inject it in your services or controllers.

```csharp
    public class MyService
    {
        private readonly MdReportGenenerator reportGenerator;

        public MyService(MdReportGenenerator reportGenerator)
        {
            this.reportGenerator = reportGenerator;
        }
    }
```

## Usage

To work, you need to call the function GenerateReport :

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

The function Transform has 3 parameters :

- outputPath : The path of the output file (Docx file)
- rootFolder : The path of the folder containing the markdown files (MdFiles/*.md)
- templatePath : The path of the template file (Optional. It let you to use a dotx file as template)


# IC
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mathieumack_MarkdownToDocxGenerator&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mathieumack_MarkdownToDocxGenerator)
[![.NET](https://github.com/mathieumack/MarkdownToDocxGenerator/actions/workflows/ci.yml/badge.svg)](https://github.com/mathieumack/MarkdownToDocxGenerator/actions/workflows/ci.yml)
[![NuGet package](https://buildstats.info/nuget/MarkdownToDocxGenerator?includePreReleases=true)](https://nuget.org/packages/MarkdownToDocxGenerator)

# Documentation : I want more

Do not hesitate to check unit tests on the solution. It's a good way to check how transformations are tested.

Also, to get more samples, go to the [Wiki](https://github.com/mathieumack/MarkdownToDocxGenerator/wiki). 

Do not hesitate to contribute.


# Support / Contribute
> If you have any questions, problems or suggestions, create an issue or fork the project and create a Pull Request.
