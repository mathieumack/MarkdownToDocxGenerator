# Docker Compose Example for MarkdownToDocxGenerator

This example demonstrates how to use MarkdownToDocxGenerator with Docker Compose.

## Prerequisites

- Docker and Docker Compose installed
- Markdown files ready for conversion
- A `.dotx` template file (optional)

## Directory Structure

Create the following directory structure:

```
.
├── docker-compose.yml
├── markdown/           # Place your .md files here
├── templates/          # Place your .dotx template here
└── output/            # Generated .docx files will appear here
```

## Usage

1. Place your Markdown files in the `markdown/` directory
2. Place your template file in the `templates/` directory (e.g., `template.dotx`)
3. Run the conversion:

```bash
docker-compose run --rm converter \
  /markdown \
  /templates/template.dotx \
  "1.0.0" \
  /output/document.docx \
  "My Project" \
  "PROJECT-001"
```

## Parameters

The converter requires 6 parameters:

1. **Root folder**: Path to markdown files (e.g., `/markdown`)
2. **Template path**: Path to .dotx template (e.g., `/templates/template.dotx`)
3. **Version**: Version string to embed in document (e.g., `"1.0.0"`)
4. **Output path**: Where to save the .docx file (e.g., `/output/document.docx`)
5. **Project name**: Name of your project (e.g., `"My Project"`)
6. **Project index**: Project identifier (e.g., `"PROJECT-001"`)

## Example with Test Files

To test with the included unit test files:

```bash
# From the repository root
docker-compose run --rm converter \
  /markdown \
  /templates/sample.dotx \
  "1.0.0" \
  /output/test-output.docx \
  "MarkdownToDocxGenerator Test" \
  "TEST-001"
```

The generated file will be available at `./output/test-output.docx`.
