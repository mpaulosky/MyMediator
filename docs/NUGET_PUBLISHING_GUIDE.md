# MyMediator NuGet Package Publishing Setup

## Overview
This document describes the complete setup for packaging and publishing the MyMediator library as a NuGet package to GitHub Packages.

## Changes Implemented

### 1. Project Configuration Updates

#### Target Framework
- **Changed from**: .NET 10.0 (preview)
- **Changed to**: .NET 9.0 (stable)
- **Reason**: Build environment compatibility and stable dependency versions

#### Dependency Versions
Updated in `Directory.Packages.props`:
- `Microsoft.Extensions.DependencyInjection`: 9.0.0 (was 10.0.0-rc.2.25502.107)
- `Microsoft.Extensions.DependencyInjection.Abstractions`: 9.0.0 (was 10.0.0-rc.2.25502.107)
- Added: `Microsoft.SourceLink.GitHub`: 8.0.0

### 2. NuGet Package Metadata

Added comprehensive package metadata to `src/MyMediator/MyMediator.csproj`:

```xml
<!-- NuGet Package Metadata -->
<PackageId>MyMediator</PackageId>
<Authors>mpaulosky</Authors>
<Company>mpaulosky</Company>
<Description>A lightweight mediator pattern implementation for .NET applications...</Description>
<PackageTags>mediator;cqrs;request-response;dotnet;dependency-injection</PackageTags>
<PackageLicenseExpression>MIT</PackageLicenseExpression>
<PackageProjectUrl>https://github.com/mpaulosky/MyMediator</PackageProjectUrl>
<RepositoryUrl>https://github.com/mpaulosky/MyMediator.git</RepositoryUrl>
<PackageReadmeFile>README.md</PackageReadmeFile>

<!-- Source Link and Debugging -->
<PublishRepositoryUrl>true</PublishRepositoryUrl>
<EmbedUntrackedSources>true</EmbedUntrackedSources>
<IncludeSymbols>true</IncludeSymbols>
<SymbolPackageFormat>snupkg</SymbolPackageFormat>
```

**Key Features**:
- README.md included in package
- Source Link enabled for step-through debugging
- Symbol packages (.snupkg) generated for debugging support
- Proper repository and project URL references

### 3. GitVersion Configuration

Created `GitVersion.yml` for automatic semantic versioning:

**Branch Strategies**:
- **main**: Stable releases (no prerelease tag), patch increment
- **develop**: Alpha prerelease versions, minor increment
- **feature/**: Branch name as prerelease tag
- **release/**: Beta prerelease versions
- **hotfix/**: Beta prerelease versions, patch increment
- **pull-request**: PR number as prerelease tag

**Version Bump Messages**:
- `+semver: breaking` or `+semver: major` → Major version bump
- `+semver: feature` or `+semver: minor` → Minor version bump
- `+semver: fix` or `+semver: patch` → Patch version bump
- `+semver: none` or `+semver: skip` → No version bump

### 4. GitHub Actions Workflow

Created `.github/workflows/nuget-publish.yml`:

**Trigger Events**:
- Push to `main` branch
- Pull requests to `main` branch
- Manual workflow dispatch

**Workflow Steps**:
1. **Checkout**: Fetches full git history for GitVersion
2. **Setup .NET**: Installs .NET SDK based on global.json
3. **Install GitVersion**: Version 6.0
4. **Determine Version**: Calculates semantic version
5. **Restore Dependencies**: Restores NuGet packages
6. **Build**: Builds in Release configuration with version
7. **Test**: Runs all unit tests
8. **Pack**: Creates NuGet package (.nupkg) and symbols (.snupkg)
9. **Upload Artifacts**: Saves packages as GitHub artifacts
10. **Publish to GitHub Packages**: Publishes on main branch pushes
11. **Create GitHub Release**: Creates release for stable versions

**Environment Variables**:
- Uses `GITHUB_TOKEN` for authentication
- Configures NuGet package caching

### 5. Documentation Updates

Updated `README.md`:
- Corrected project description (removed TailwindBlog references)
- Added comprehensive feature list
- Added installation instructions for GitHub Packages
- Added complete usage examples
- Added GitHub badges for build status, issues, and PRs
- Added license and contribution information

### 6. NuGet Configuration

Created `nuget.config`:
- Configures GitHub Packages as a package source
- Includes authentication setup instructions
- Supports both nuget.org and GitHub Packages

### 7. Git Ignore Updates

Updated `.gitignore`:
- Added `test-artifacts/` directory
- Added `artifacts/` directory  
- Added `*.nupkg` and `*.snupkg` files

## How to Use

### Publishing a New Version

1. **Make Changes**: Commit your code changes to a feature branch
2. **Create PR**: Open a pull request to main
3. **Merge to Main**: Once approved, merge the PR
4. **Automatic Process**:
   - GitHub Actions workflow triggers
   - GitVersion calculates new version
   - Package is built and tested
   - Package is published to GitHub Packages
   - GitHub Release is created (for stable versions)

### Version Control

Use commit messages to control versioning:

```bash
# Patch version bump (1.0.0 → 1.0.1)
git commit -m "Fix validation bug +semver: patch"

# Minor version bump (1.0.0 → 1.1.0)
git commit -m "Add new feature +semver: minor"

# Major version bump (1.0.0 → 2.0.0)
git commit -m "Breaking change in API +semver: major"
```

### Installing the Package

1. **Add GitHub Packages Source**:
```bash
dotnet nuget add source \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_TOKEN \
  --store-password-in-clear-text \
  --name github \
  "https://nuget.pkg.github.com/mpaulosky/index.json"
```

2. **Install Package**:
```bash
dotnet add package MyMediator
```

### Local Testing

To test package creation locally:

```bash
# Build and pack
dotnet pack src/MyMediator/MyMediator.csproj \
  --configuration Release \
  --output ./artifacts

# Inspect package contents
unzip -l ./artifacts/MyMediator.*.nupkg
```

## Package Contents

The generated NuGet package includes:
- **Assembly**: MyMediator.dll (targeting .NET 9.0)
- **README**: README.md with usage instructions
- **Dependencies**: 
  - Microsoft.Extensions.DependencyInjection 9.0.0
  - Microsoft.Extensions.DependencyInjection.Abstractions 9.0.0
- **Symbols**: Source link information for debugging
- **Metadata**: License, authors, description, tags

## Debugging Support

The package includes SourceLink configuration, allowing developers to:
- Step into MyMediator source code while debugging
- View exact source code from the repository
- Navigate to specific commits that built the package

## Security Considerations

- Uses `GITHUB_TOKEN` provided by GitHub Actions (no manual secrets needed)
- SourceLink embeds repository information but not actual source code
- Symbol packages (.snupkg) are separate and optional

## Future Enhancements

Potential improvements:
1. **NuGet.org Publishing**: Add workflow step to publish to public NuGet.org
2. **Multi-targeting**: Support additional target frameworks (net8.0, net6.0)
3. **Code Coverage**: Integrate code coverage reports
4. **Release Notes**: Auto-generate release notes from commits
5. **Pre-release Packages**: Publish develop branch as prerelease packages

## Testing Status

✅ All 19 unit tests pass
✅ Package builds without warnings
✅ Symbol package generation works
✅ README included in package
✅ SourceLink configuration valid

## References

- [GitVersion Documentation](https://gitversion.net/)
- [GitHub Packages for NuGet](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry)
- [SourceLink Documentation](https://github.com/dotnet/sourcelink)
- [NuGet Package Metadata](https://docs.microsoft.com/en-us/nuget/reference/msbuild-targets)
