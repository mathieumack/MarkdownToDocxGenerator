# GitHub Actions Integration Examples

This directory contains example GitHub Actions workflows for using MarkdownToDocxGenerator in CI/CD pipelines.

## Example 1: Generate Documentation on Push

File: `generate-docs.yml`

Automatically generates documentation whenever changes are pushed to the main branch.

## Example 2: Generate Documentation on Release

File: `generate-docs-on-release.yml`

Generates documentation when a new release is created and uploads it as a release asset.

## Example 3: Generate Documentation with Artifacts

File: `generate-docs-with-artifacts.yml`

Generates documentation and uploads it as a build artifact.

## Setup Instructions

1. Copy the desired workflow file to your repository's `.github/workflows/` directory
2. Adjust the paths and parameters according to your project structure
3. Ensure you have:
   - Markdown files in the specified directory
   - A `.dotx` template file (or remove the template parameter to use default formatting)
4. Push changes to trigger the workflow

## Customization

All examples can be customized by modifying:
- **Source directory**: Where your Markdown files are located
- **Template path**: Path to your custom `.dotx` template
- **Output filename**: Name of the generated DOCX file
- **Project name and index**: Metadata embedded in the document
- **Version**: Can be set to git tag, commit SHA, or custom value
