// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     MyMediator.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  MyMediator
// =======================================================

namespace MyMediator;

public static class MyMediator
{

	public static IServiceCollection AddMyMediator(this IServiceCollection services, Assembly? assembly = null)
	{

		assembly ??= Assembly.GetCallingAssembly();

		services.AddScoped<ISender, Sender>();

		var handlerInterfaceType = typeof(IRequestHandler<,>);

		var handlerTypes = assembly
				.GetTypes()
				.Where(type => !type.IsAbstract && !type.IsInterface)
				.SelectMany(type => type.GetInterfaces()
						.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerInterfaceType)
						.Select(i => new { Interface = i, Implementation = type }));

		foreach (var handler in handlerTypes)
		{
			services.AddScoped(handler.Interface, handler.Implementation);
		}

		return services;

	}

}