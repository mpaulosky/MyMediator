// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Sender.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  MyMediator
// =======================================================

namespace MyMediator;

public class Sender : ISender
{

	private readonly IServiceProvider _provider;

	public Sender(IServiceProvider provider)
	{
		_provider = provider ?? throw new ArgumentNullException(nameof(provider));
	}

	public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		if (request == null)
			throw new ArgumentNullException(nameof(request));

		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
		dynamic handler = _provider.GetRequiredService(handlerType);

		return handler.Handle((dynamic)request, cancellationToken);
	}

}