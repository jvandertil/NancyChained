namespace NancyChains
{
    using System;
    using Nancy;
    using Nancy.ModelBinding;

    /// <summary>
    ///     Utility class to simplify the execution of Chains of Responsibility using Nancy.
    /// </summary>
    public class HandlerClient : IHandlerClient
    {
        private readonly IModelBinderLocator _binderLocator;
        private readonly NancyContext _context;
        private readonly IHandler _handler;

        /// <summary>
        ///     Initializes a new instance of the HandlerClient class.
        /// </summary>
        /// <param name="binderLocator">The Nancy <see cref="IModelBinderLocator" />.</param>
        /// <param name="context">The current <see cref="NancyContext" />.</param>
        /// <param name="handler">The Chain of Responsibility to execute.</param>
        public HandlerClient(IModelBinderLocator binderLocator, NancyContextWrapper context, IHandler handler)
        {
            _binderLocator = binderLocator;
            _context = context.Context;
            _handler = handler;
        }

        /// <summary>
        ///     Executes the Chain of Responsibility for the given handler.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="handler">The final handler in the chain.</param>
        /// <returns>The result of the Chain of Responsibility.</returns>
        public Response Handle<TRequest>(Func<TRequest, Response> handler)
        {
            IBinder binder = _binderLocator.GetBinderForType(typeof (TRequest), _context);

            object instance = Activator.CreateInstance(typeof (TRequest), true);
            var request = (TRequest) binder.Bind(_context, typeof (TRequest), instance, BindingConfig.Default);

            return _handler.Handle(request, handler);
        }
    }
}
