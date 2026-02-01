# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["src/MarkdownToDocxGenerator.Console/MarkdownToDocxGenerator.Console.csproj", "src/MarkdownToDocxGenerator.Console/"]
COPY ["src/MarkdownToDocxGenerator/MarkdownToDocxGenerator.csproj", "src/MarkdownToDocxGenerator/"]

# Restore dependencies
RUN dotnet restore "src/MarkdownToDocxGenerator.Console/MarkdownToDocxGenerator.Console.csproj"

# Copy the rest of the source code
COPY . .

# Build and publish the application
WORKDIR "/src/src/MarkdownToDocxGenerator.Console"
RUN dotnet publish "MarkdownToDocxGenerator.Console.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

# Install libgdiplus for DOCX and image processing (required for Linux)
RUN apt-get update && apt-get install -y libgdiplus && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "MarkdownToDocxGenerator.Console.dll"]
