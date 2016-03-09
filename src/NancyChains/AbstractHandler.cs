namespace NancyChains
{
    using System;
    using Nancy;

    /// <summary>
    ///     A base class for <see cref="IHandler" /> implementations.
    /// </summary>
    public abstract class AbstractHandler : IHandler
    {
        private IHandler _successor;

        /// <summary>
        ///     Sets the next link in the chain to <paramref name="successor" />.
        /// </summary>
        /// <param name="successor">The next link in the chain of responsibility.</param>
        public void SetSuccessor(IHandler successor)
        {
            _successor = successor;
        }

        public abstract Response Handle<TRequest>(TRequest request, Func<TRequest, Response> handler);

        /// <summary>
        ///     Passes the <paramref name="request" /> through to the next handler in the chain, or executes the
        ///     <paramref name="handler" /> if the end of the chain has been reached.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="handler">The final handler.</param>
        /// <returns>The response of a <see cref="IHandler" />, or the response of the <paramref name="handler" />.</returns>
        protected Response Next<TRequest>(TRequest request, Func<TRequest, Response> handler)
        {
            if (_successor != null)
            {
                return _successor.Handle(request, handler);
            }

            return handler(request);
        }
    }
}
