namespace NancyChains
{
    using Nancy;

    /// <summary>
    ///     A simple wrapper that allows the <see cref="NancyContext" /> to be registered in a DI Container.
    /// </summary>
    public class NancyContextWrapper
    {
        public NancyContextWrapper(NancyContext context)
        {
            Context = context;
        }

        public NancyContext Context { get; }
    }
}
