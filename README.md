# MyMediator

MyMediator is a lightweight and extensible mediator library for .NET, designed to facilitate clean separation of concerns and decoupled communication between components. It allows you to implement the mediator pattern, handling requests and notifications in a simple and testable manner.

## Features

- Request/response and notification handling
- Supports custom handlers and pipelines
- Extensible and testable
- Compatible with .NET Standard and .NET Core

## Installation

You can add MyMediator to your project using NuGet:

```shell
dotnet add package MyMediator
```

Or via the Visual Studio NuGet Package Manager.

## Getting Started

Below is a basic example of using MyMediator in a C# project.

### 1. Define a Request

```csharp
public class PingRequest : IRequest<string>
{
    public string Message { get; set; }
}
```

### 2. Implement a Handler

```csharp
public class PingHandler : IRequestHandler<PingRequest, string>
{
    public string Handle(PingRequest request)
    {
        return $"Handled: {request.Message}";
    }
}
```

### 3. Register and Use the Mediator

```csharp
using MyMediator;

var mediator = new Mediator();
mediator.RegisterHandler<PingRequest, string>(new PingHandler());

var response = mediator.Send(new PingRequest { Message = "Hello World" });
Console.WriteLine(response); // Output: Handled: Hello World
```

## Documentation

- [API Reference](docs/API.md) _(add your API details here)_
- [Examples](docs/examples.md) _(add usage examples here)_

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Repository Structure

```
src/
  MyMediator/         # Core library source
tests/                # Unit tests
README.md
LICENSE
```

## Support

If you have any issues or questions, please create an [issue](https://github.com/mpaulosky/MyMediator/issues).

---

Happy coding!