# Nancy.Chains
An implementation of the Chain-of-Responsibility pattern for creating handlers in Nancy.
Although currently less powerful, they can be compared to the behaviour chains in FubuMVC, or Action Filters in ASP.NET MVC.
Somewhat comparable actions can be done using the Before and After hooks already provided by Nancy, but putting cross-cutting concerns such as model binding and validation in them is not ideal.

Currently there is support for single chain of responsibility in a Nancy application.

## Example usage
First register the components in the IoC container for Nancy.

```c#
protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
{
    base.RequestStartup(container, pipelines, context);

    // Wrapper is needed due to: https://github.com/NancyFx/Nancy/issues/2346
    container.Register(new NancyContextWrapper(context));
    container.Register((ctx, _) =>
    {
        var builder = new HandlerChainBuilder(ctx.Resolve);

        // use the builder to ease building the chain
        builder
            .Add<TestHandler>();

        return builder.Build();
    });
    container.Register<IHandlerClient>((ctx, _) => ctx.Resolve<HandlerClient>());
}
```

The `TestHandler` used in the chain is quite simple:
```c#
public class TestHandler : AbstractHandler
{
    public override Response Handle<TRequest>(TRequest request, Func<TRequest, Response> handler)
    {
        // Demo code, obviously.
        var model = request as TestModel;

        if (model != null && model.Name == "1337")
        {
            return new Response { StatusCode = HttpStatusCode.BadRequest };
        }

        return Next(request, handler);
    }
}
```

Then our module will look like this:
```c#
public class TestModule : NancyModule
{
    public TestModule(IHandlerClient client)
    {
        // Actual handler is only the happy flow.
        Post["/test"] = _ => client.Handle<TestModel>(request => Ok());
    }

    private Response Ok()
    {
        return new Response { StatusCode = HttpStatusCode.OK };
    }
}

// And the model
public class TestModel
{
    public string Name { get; set; }
}
```

Using this setup all request where the 'Name' field on 'TestModel' is '1337' will result in a HTTP 400 BadRequest response.
All other values will result in a HTTP 200 OK, which is the happy path for the '/test' route.

Obviously the toy example demonstrated here does not show the real power of this pattern.
But you can also create Handlers that can catch exceptions, create database transactions, etc.
