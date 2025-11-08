# NuGet Package Configuration

## MyMediator NuGet Package

This repository publishes the MyMediator library as a NuGet package to both GitHub Packages and NuGet.org.

### Package Information

- **Package ID**: MyMediator
- **Author**: mpaulosky
- **License**: MIT
- **Repository**: https://github.com/mpaulosky/MyMediator

### Publishing

The package is automatically published through GitHub Actions when:

1. **GitHub Packages**: Published on every push to the `main` branch
2. **NuGet.org**: Published when a version tag (e.g., `v1.0.0`) is pushed

### Manual Publishing

You can manually trigger the publishing workflow:

1. Go to the Actions tab in GitHub
2. Select "Publish NuGet Package" workflow
3. Click "Run workflow"
4. Optionally specify a version number

### Version Management

Versioning is managed using GitVersion. The version is automatically calculated based on:
- Git commit history
- Branch name
- Tags

### Configuration

#### GitHub Packages

No additional configuration is needed for GitHub Packages. The workflow uses the `GITHUB_TOKEN` automatically.

#### NuGet.org

To publish to NuGet.org, you need to:

1. Create an API key at https://www.nuget.org/account/apikeys
2. Add it as a repository secret named `NUGET_API_KEY`:
   - Go to repository Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Your NuGet API key

### Using the Package

#### From GitHub Packages

Add a `nuget.config` file to your project:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/mpaulosky/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_GITHUB_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_GITHUB_PAT" />
    </github>
  </packageSourceCredentials>
</configuration>
```

Then install:
```bash
dotnet add package MyMediator
```

#### From NuGet.org

Simply install:
```bash
dotnet add package MyMediator
```

### Local Testing

To test package creation locally:

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build --configuration Release

# Create package
dotnet pack src/MyMediator/MyMediator.csproj --configuration Release --output ./artifacts

# Test package locally
dotnet nuget push ./artifacts/MyMediator.*.nupkg --source ./local-feed
```

### Package Contents

The package includes:
- MyMediator library assemblies
- XML documentation
- Symbol files (.snupkg) for debugging
- README.md
