namespace NancyChains
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Builder class to ease the creation of a chain of responsibility using <see cref="IHandler" />s.
    /// </summary>
    public sealed class HandlerChainBuilder
    {
        private readonly Func<Type, object> _handlerFactory;
        private readonly LinkedList<Type> _handlers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HandlerChainBuilder" />.
        /// </summary>
        /// <param name="handlerFactory">The factory method that can be used to create instances in the build phase.</param>
        public HandlerChainBuilder(Func<Type, object> handlerFactory)
        {
            _handlerFactory = handlerFactory;

            _handlers = new LinkedList<Type>();
        }

        /// <summary>
        ///     Adds a new <see cref="IHandler" /> to the chain of responsibility.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler to add.</typeparam>
        /// <returns>The builder.</returns>
        public HandlerChainBuilder Add<THandler>() where THandler : IHandler
        {
            _handlers.AddFirst(typeof (THandler));

            return this;
        }

        /// <summary>
        ///     Builds the chain of responsibility created using the <see cref="Add{THandler}" /> method.
        /// </summary>
        /// <returns>The chain of responsibility as they were added using <see cref="Add{THandler}" />, or null if none are given.</returns>
        public IHandler Build()
        {
            IHandler prevHandler = null;

            foreach (Type type in _handlers)
            {
                var currentHandler = (IHandler) _handlerFactory(type);

                prevHandler?.SetSuccessor(currentHandler);

                prevHandler = currentHandler;
            }

            return prevHandler;
        }
    }
}
