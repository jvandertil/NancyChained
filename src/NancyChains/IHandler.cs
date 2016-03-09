namespace NancyChains
{
    using System;
    using Nancy;

    /// <summary>
    ///     Represents a link in a Chain of Responsibility.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        ///     The (optional) successor of this link in the Chain of Responsibility.
        /// </summary>
        /// <param name="successor">The successor of this link.</param>
        void SetSuccessor(IHandler successor);

        /// <summary>
        ///     Executes this step and any successors in the Chain of Responsibility.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="handler">The final handle function.</param>
        /// <returns>The result of the Chain of Responsibility.</returns>
        Response Handle<TRequest>(TRequest request, Func<TRequest, Response> handler);
    }
}
