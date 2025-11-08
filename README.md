# MyMediator

## A Lightweight Mediator Pattern Implementation for .NET

MyMediator is a simple, efficient, and lightweight mediator pattern implementation for .NET applications. It provides a clean way to implement request/response patterns with full dependency injection support.

****
![GitHub](https://img.shields.io/github/license/mpaulosky/MyMediator?logo=github)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mpaulosky/MyMediator?logo=github)
[![Build and Publish NuGet Package](https://github.com/mpaulosky/MyMediator/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/mpaulosky/MyMediator/actions/workflows/nuget-publish.yml)
****
[![Open Issues](https://img.shields.io/github/issues/mpaulosky/MyMediator.svg?style=flat-square&logo=github&label=Open%20Issues)](https://github.com/mpaulosky/MyMediator/issues)
[![Closed Issues](https://img.shields.io/github/issues-closed/mpaulosky/MyMediator.svg?style=flat-square&logo=github&label=Closed%20Issues)](https://github.com/mpaulosky/MyMediator/issues?q=sort%3Aupdated-desc+is%3Aissue+is%3Aclosed)
****
![GitHub pull requests](https://img.shields.io/github/issues-pr/mpaulosky/MyMediator?label=pull%20requests&logo=github)
![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed/mpaulosky/MyMediator?logo=github)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/mpaulosky/MyMediator/main?label=last%20commit%20main&logo=github)
****

## Features

- 🚀 **Lightweight** - Minimal dependencies and overhead
- 💉 **Dependency Injection** - Full DI support with automatic handler registration
- 🎯 **Simple API** - Easy-to-use request/response pattern
- 🔧 **Flexible** - Works with any .NET application using Microsoft.Extensions.DependencyInjection
- ✅ **Well Tested** - Comprehensive unit test coverage

## Installation

### From GitHub Packages

To install MyMediator from GitHub Packages, you need to add the GitHub NuGet source:

```bash
dotnet nuget add source --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/mpaulosky/index.json"
```

Then install the package:

```bash
dotnet add package MyMediator
```

## Usage

### 1. Define a Request

```csharp
public record GetUserQuery(int UserId) : IRequest<UserDto>;
```

### 2. Create a Request Handler

```csharp
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository _repository;

    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.UserId, cancellationToken);
        return new UserDto(user.Id, user.Name, user.Email);
    }
}
```

### 3. Register MyMediator in Your DI Container

```csharp
services.AddMyMediator();
```

Or specify an assembly explicitly:

```csharp
services.AddMyMediator(typeof(GetUserQueryHandler).Assembly);
```

### 4. Send Requests

```csharp
public class UserService
{
    private readonly ISender _sender;

    public UserService(ISender sender)
    {
        _sender = sender;
    }

    public async Task<UserDto> GetUserAsync(int userId)
    {
        var query = new GetUserQuery(userId);
        return await _sender.Send(query);
    }
}
```

## How It Works

MyMediator automatically discovers and registers all `IRequestHandler<TRequest, TResponse>` implementations in your assembly. When you call `Send`, it resolves the appropriate handler from the DI container and executes it.

## Requirements

- .NET 9.0 or later
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.DependencyInjection.Abstractions

## How to Engage, Contribute, and Give Feedback

Review the [Code Of Conduct](./docs/CODE_OF_CONDUCT.md).

Some of the best ways to contribute are to try things out, file issues, and make pull-requests.

Check out the [contributing page](./docs/CONTRIBUTING.md) to see the best places to log issues and start discussions.

****

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Code of Conduct

See the [CODE-OF-CONDUCT](./docs/CODE_OF_CONDUCT.md) document
