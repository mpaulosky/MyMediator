// =======================================================
// Copyright (c) 2025. All rights reserved.
// File Name :     ISender.cs
// Company :       mpaulosky
// Author :        Matthew
// Solution Name : TailwindBlog
// Project Name :  MyMediator
// =======================================================

namespace MyMediator;

public interface ISender
{

	Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

}