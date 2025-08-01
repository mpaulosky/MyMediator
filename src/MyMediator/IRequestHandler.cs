// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     IRequestHandler.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  MyMediator
// =======================================================

namespace MyMediator;

public interface IRequestHandler<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
{

	Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);

}