# MyMediator

## A Lightweight Mediator Implementation for .NET

A simple, efficient mediator pattern implementation for .NET applications that supports request/response workflows with dependency injection.

****
![GitHub](https://img.shields.io/github/license/mpaulosky/MyMediator?logo=GitHub)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/mpaulosky/MyMediator?logo=Github)
****
[![Open Issues](https://img.shields.io/github/issues/mpaulosky/MyMediator.svg?style=flatsquare&logo=github&label=Open%20Issues)](https://github.com/mpaulosky/MyMediator/issues)
[![Closed Issues](https://img.shields.io/github/issues-closed/mpaulosky/MyMediator.svg?style=flatsquare&logo=github&label=Closed%20Issues)](https://github.com/mpaulosky/MyMediator/issues?q=sort%3Aupdated-desc+is%3Aissue+is%3Aclosed)
****
![GitHub pull requests](https://img.shields.io/github/issues-pr/mpaulosky/MyMediator?label=pull%20requests&logo=github)
![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed/mpaulosky/MyMediator?logo=github)
![GitHub last commit (branch)](https://img.shields.io/github/last-commit/mpaulosky/MyMediator/main?label=last%20commit%20main&logo=github)
****
[![.NET Build](https://github.com/mpaulosky/MyMediator/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mpaulosky/MyMediator/actions/workflows/dotnet.yml)
[![Publish NuGet](https://github.com/mpaulosky/MyMediator/actions/workflows/nuget-publish.yml/badge.svg)](https://github.com/mpaulosky/MyMediator/actions/workflows/nuget-publish.yml)
****

## Installation

### From NuGet.org

```bash
dotnet add package MyMediator
```

### From GitHub Packages

See [NuGet Documentation](./docs/NUGET.md) for details on using GitHub Packages.

## Features

- Simple request/response pattern
- Dependency injection integration
- Lightweight with minimal dependencies
- Supports async operations
- Easy to test and mock

## Usage

### 1. Define a Request

```csharp
public record GetUserQuery(int UserId) : IRequest<User>;
```

### 2. Create a Handler

```csharp
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
{
    private readonly IUserRepository _repository;
    
    public GetUserQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.UserId, cancellationToken);
    }
}
```

### 3. Register Services

```csharp
services.AddMyMediator();
services.AddTransient<IRequestHandler<GetUserQuery, User>, GetUserQueryHandler>();
```

### 4. Send Requests

```csharp
public class UserController : ControllerBase
{
    private readonly ISender _sender;
    
    public UserController(ISender sender)
    {
        _sender = sender;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _sender.Send(new GetUserQuery(id));
        return Ok(user);
    }
}
```

## How to Engage, Contribute, and Give Feedback

Review the [Code Of Conduct](./docs/CODE_OF_CONDUCT.md).

Some of the best ways to contribute are to try things out, file issues, and make pull-requests.

Check out the [contributing page](./docs/CONTRIBUTING.md) to see the best places to log issues and start discussions.

****

## Software References

* .NET 10 (Preview)
* C#
* Microsoft.Extensions.DependencyInjection

## Code of conduct

See the [CODE-OF-CONDUCT](./docs/CODE_OF_CONDUCT.md) document
