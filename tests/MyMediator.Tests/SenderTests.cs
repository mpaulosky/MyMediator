// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     SenderTests.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  MyMediator.Tests
// =======================================================

using System.Diagnostics.CodeAnalysis;

namespace MyMediator;

// Custom IServiceProvider for dynamic type resolution
[ExcludeFromCodeCoverage]
internal class TestServiceProvider : IServiceProvider
{

	private readonly Dictionary<Type, object> _map;

	public TestServiceProvider(Dictionary<Type, object> map)
	{
		_map = map;
	}

	public object GetService(Type serviceType)
	{
		if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));

		if (_map.TryGetValue(serviceType, out var handler)) return handler;

		throw new InvalidOperationException($"No service for type '{serviceType}' has been registered.");
	}

}

/// <summary>
/// Unit tests for the <see cref="Sender"/> class.
/// </summary>
public class SenderTests
{

	public class TestRequest : IRequest<string>;

	public class IntRequest : IRequest<int>;

	public class BoolRequest : IRequest<bool>;

	public class CustomRequest : IRequest<DateTime>;

	[Fact]
	public async Task Send_HandlerNotRegistered_ThrowsInvalidOperationException()
	{
		// Arrange
		var request = new TestRequest();
		var handlerMap = new Dictionary<Type, object>(); // No handler registered
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		Func<Task> act = async () => await sender.Send(request);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("*No service for type*");
	}

	[Fact]
	public async Task Send_HandlerThrowsException_PropagatesException()
	{
		// Arrange
		var request = new TestRequest();
		var handler = Substitute.For<IRequestHandler<TestRequest, string>>();

		handler.Handle(request, CancellationToken.None)
				.Returns(Task.FromException<string>(new InvalidOperationException("Handler failed")));

		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(string));
		var handlerMap = new Dictionary<Type, object> { [handlerType] = handler };
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		Func<Task> act = async () => await sender.Send(request);

		// Assert
		await act.Should().ThrowAsync<InvalidOperationException>()
				.WithMessage("Handler failed");
	}

	[Fact]
	public async Task Send_CustomRequest_ReturnsExpectedDateTime()
	{
		// Arrange
		var request = new CustomRequest();
		var expected = DateTime.UtcNow;
		var handler = Substitute.For<IRequestHandler<CustomRequest, DateTime>>();
		handler.Handle(request, CancellationToken.None).Returns(Task.FromResult(expected));
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(DateTime));
		var handlerMap = new Dictionary<Type, object> { [handlerType] = handler };
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		var result = await sender.Send(request);

		// Assert
		result.Should().Be(expected);
		await handler.Received(1).Handle(request, CancellationToken.None);
	}

	[Fact]
	public async Task Send_DefaultCancellationToken_IsNone()
	{
		// Arrange
		var request = new TestRequest();
		var handler = Substitute.For<IRequestHandler<TestRequest, string>>();
		handler.Handle(request, CancellationToken.None).Returns(Task.FromResult("defaultToken"));
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(string));
		var handlerMap = new Dictionary<Type, object> { [handlerType] = handler };
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		var result = await sender.Send(request);

		// Assert
		result.Should().Be("defaultToken");
		await handler.Received(1).Handle(request, CancellationToken.None);
	}

	[Fact]
	public async Task Send_MultipleCalls_HandlerCalledCorrectNumberOfTimes()
	{
		// Arrange
		var request = new TestRequest();
		var handler = Substitute.For<IRequestHandler<TestRequest, string>>();
		handler.Handle(request, CancellationToken.None).Returns(Task.FromResult("multi"));
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(string));
		var handlerMap = new Dictionary<Type, object> { [handlerType] = handler };
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		var result1 = await sender.Send(request);
		var result2 = await sender.Send(request);

		// Assert
		result1.Should().Be("multi");
		result2.Should().Be("multi");
		await handler.Received(2).Handle(request, CancellationToken.None);
	}

	[Fact]
	public void Sender_NullProvider_ThrowsArgumentNullException()
	{
		// Arrange
		IServiceProvider provider = null!;

		// Act
		Action act = () => new Sender(provider);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	// ...existing code...
	[Fact]
	public async Task Send_ValidRequest_ReturnsExpectedResponse()
	{
		// Arrange
		var request = new TestRequest();
		var handler = Substitute.For<IRequestHandler<TestRequest, string>>();
		handler.Handle(request, CancellationToken.None).Returns(Task.FromResult("expected"));
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(string));
		var handlerMap = new Dictionary<Type, object> { [handlerType] = handler };
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		var result = await sender.Send(request);

		// Assert
		result.Should().Be("expected");
		await handler.Received(1).Handle(request, CancellationToken.None);
	}

	[Fact]
	public async Task Send_WithCancellationToken_PassesTokenToHandler()
	{
		// Arrange
		var request = new TestRequest();
		var handler = Substitute.For<IRequestHandler<TestRequest, string>>();
		var token = new CancellationTokenSource().Token;
		handler.Handle(request, token).Returns(Task.FromResult("cancelled"));
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(string));
		var handlerMap = new Dictionary<Type, object> { [handlerType] = handler };
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		// Act
		var result = await sender.Send(request, token);

		// Assert
		result.Should().Be("cancelled");
		await handler.Received(1).Handle(request, token);
	}

	[Fact]
	public async Task Send_NullRequest_ThrowsArgumentNullException()
	{
		// Arrange
		var provider = Substitute.For<IServiceProvider>();
		var sender = new Sender(provider);
		IRequest<string> request = null!;

		// Act
		Func<Task> act = async () => await sender.Send(request);

		// Assert
		await act.Should().ThrowAsync<ArgumentNullException>();
	}

	[Theory]
	[InlineData("string", "response")]
	[InlineData("int", 42)]
	[InlineData("bool", true)]
	public async Task Send_VariousRequests_ReturnsExpectedResponses(string type, object expected)
	{
		// Arrange
		var handlerMap = new Dictionary<Type, object>();
		object request;
		object handler;
		Type handlerType;

		if (type == "string")
		{
			request = new TestRequest();
			var typedHandler = Substitute.For<IRequestHandler<TestRequest, string>>();
			typedHandler.Handle((TestRequest)request, CancellationToken.None).Returns(Task.FromResult((string)expected));
			handler = typedHandler;
			handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(string));
		}
		else if (type == "int")
		{
			request = new IntRequest();
			var typedHandler = Substitute.For<IRequestHandler<IntRequest, int>>();
			typedHandler.Handle((IntRequest)request, CancellationToken.None).Returns(Task.FromResult((int)expected));
			handler = typedHandler;
			handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(int));
		}
		else
		{
			request = new BoolRequest();
			var typedHandler = Substitute.For<IRequestHandler<BoolRequest, bool>>();
			typedHandler.Handle((BoolRequest)request, CancellationToken.None).Returns(Task.FromResult((bool)expected));
			handler = typedHandler;
			handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(bool));
		}

		handlerMap[handlerType] = handler;
		var provider = new TestServiceProvider(handlerMap);
		var sender = new Sender(provider);

		object result;

		if (type == "string")
		{
			result = await sender.Send((TestRequest)request);
		}
		else if (type == "int")
		{
			result = await sender.Send((IntRequest)request);
		}
		else
		{
			result = await sender.Send((BoolRequest)request);
		}

		// Assert
		result.Should().Be(expected);
	}

}