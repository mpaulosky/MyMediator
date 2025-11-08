// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyMediatorTests.cs
// Company :       mpaulosky
// Author :        Matthew Paulosky
// Solution Name : MyMediator
// Project Name :  MyMediator.Tests
// =======================================================

using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

namespace MyMediator;

/// <summary>
/// Unit tests for <see cref="global::MyMediator.MyMediator"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public class MyMediatorTests
{

	private class TestRequest : IRequest<string> { }

	private class TestHandler : IRequestHandler<TestRequest, string>
	{

		public Task<string> Handle(TestRequest request, CancellationToken cancellationToken) => Task.FromResult("ok");

	}

	private class AnotherTestHandler : IRequestHandler<TestRequest, string>
	{

		public Task<string> Handle(TestRequest request, CancellationToken cancellationToken) => Task.FromResult("another");

	}

	/// <summary>
	/// Verifies that ISender is registered.
	/// </summary>
	[Fact]
	public void AddMyMediator_RegistersISender()
	{
		var services = new ServiceCollection();
		services.AddMyMediator(typeof(TestHandler).Assembly);
		var provider = services.BuildServiceProvider();
		var sender = provider.GetService<ISender>();
		sender.Should().NotBeNull();
	}

	/// <summary>
	/// Verifies that IRequestHandler implementations are registered.
	/// </summary>
	[Fact]
	public void AddMyMediator_RegistersRequestHandlers()
	{
		var services = new ServiceCollection();
		services.AddMyMediator(typeof(TestHandler).Assembly);
		var provider = services.BuildServiceProvider();
		var handler = provider.GetService<IRequestHandler<TestRequest, string>>();
		handler.Should().NotBeNull();
		handler.Should().BeOfType<AnotherTestHandler>();
	}

	/// <summary>
	/// Verifies that the returned IServiceCollection is the same instance.
	/// </summary>
	[Fact]
	public void AddMyMediator_ReturnsSameServiceCollection()
	{
		var services = new ServiceCollection();
		var result = services.AddMyMediator(typeof(TestHandler).Assembly);
		result.Should().BeSameAs(services);
	}

	/// <summary>
	/// Verifies that AddMyMediator uses the calling assembly when assembly is null.
	/// </summary>
	[Fact]
	public void AddMyMediator_UsesCallingAssembly_WhenAssemblyIsNull()
	{
		var services = new ServiceCollection();
		services.AddMyMediator();
		var provider = services.BuildServiceProvider();
		var sender = provider.GetService<ISender>();
		sender.Should().NotBeNull();
	}

	/// <summary>
	/// Verifies that AddMyMediator does not fail if no handlers are found.
	/// </summary>
	[Fact]
	public void AddMyMediator_NoHandlers_DoesNotThrow()
	{
		var services = new ServiceCollection();
		var emptyAssembly = typeof(string).Assembly; // System.String has no IRequestHandler
		var act = () => services.AddMyMediator(emptyAssembly);
		act.Should().NotThrow();
		var provider = services.BuildServiceProvider();
		provider.GetService<ISender>().Should().NotBeNull();
	}

	/// <summary>
	/// Verifies that duplicate handler registrations do not throw and last registration is used.
	/// </summary>
	[Fact]
	public void AddMyMediator_DuplicateHandlers_RegistersAll()
	{
		var services = new ServiceCollection();

		// Manually register two handlers for the same request
		services.AddScoped<IRequestHandler<TestRequest, string>, TestHandler>();
		services.AddScoped<IRequestHandler<TestRequest, string>, AnotherTestHandler>();
		services.AddMyMediator(typeof(TestHandler).Assembly);
		var provider = services.BuildServiceProvider();
		var handler = provider.GetServices<IRequestHandler<TestRequest, string>>();
		handler.Should().Contain(h => h is TestHandler);
		handler.Should().Contain(h => h is AnotherTestHandler);
	}

	/// <summary>
	/// Verifies that passing a null IServiceCollection throws ArgumentNullException.
	/// </summary>
	[Fact]
	public void AddMyMediator_NullServiceCollection_Throws()
	{
		IServiceCollection? services = null;
		var act = () => MyMediator.AddMyMediator(services!);
		act.Should().Throw<ArgumentNullException>();
	}

}