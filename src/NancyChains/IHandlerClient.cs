namespace NancyChains
{
    using System;
    using Nancy;

    public interface IHandlerClient
    {
        /// <summary>
        ///     The entry point to execute a Chain of Responsibility.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="handler">The final handler function to execute.</param>
        /// <returns>The result of the Chain of Responsibility.</returns>
        Response Handle<TRequest>(Func<TRequest, Response> handler);
    }
}
