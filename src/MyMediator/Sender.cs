// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     Sender.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  MyMediator
// =======================================================

namespace MyMediator;

public class Sender
(
		IServiceProvider provider
) : ISender
{

	public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
	{
		var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
		dynamic handler = provider.GetRequiredService(handlerType);

		return handler.Handle((dynamic)request, cancellationToken);
	}

}