namespace NancyChains
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Owin.Builder;
    using Nancy;
    using Nancy.Bootstrapper;
    using Nancy.TinyIoc;
    using Newtonsoft.Json;
    using Owin;
    using Shouldly;

    public class TestHandler : AbstractHandler
    {
        public override Response Handle<TRequest>(TRequest request, Func<TRequest, Response> handler)
        {
            var model = request as TestModel;

            if (model != null && model.Name == "1337")
            {
                return new Response { StatusCode = HttpStatusCode.BadRequest };
            }

            return Next(request, handler);
        }
    }

    public class TestModule : NancyModule
    {
        public TestModule(IHandlerClient client)
        {
            Post["/test"] = _ => client.Handle<TestModel>(request => Ok());
        }

        private Response Ok()
        {
            return new Response { StatusCode = HttpStatusCode.OK };
        }
    }

    public class TestModel
    {
        public string Name { get; set; }
    }

    public class TestBootstrapper : DefaultNancyBootstrapper
    {
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            base.RequestStartup(container, pipelines, context);

            container.Register(new NancyContextWrapper(context));
            container.Register((ctx, _) =>
            {
                var builder = new HandlerChainBuilder(ctx.Resolve);

                builder
                    .Add<TestHandler>();

                return builder.Build();
            });
            container.Register<IHandlerClient>((ctx, _) => ctx.Resolve<HandlerClient>());
        }
    }

    public class IntegrationTests
    {
        public async Task Test()
        {
            HttpClient client = GetClient();

            var model = new { name = "1337" };
            var body = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("/test", body);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        }

        private HttpClient GetClient()
        {
            var app = new AppBuilder();
            app.UseNancy(opts => opts.Bootstrapper = new TestBootstrapper());

            var client = new HttpClient(new OwinHttpMessageHandler(app.Build()))
            {
                BaseAddress = new Uri("http://test.example.com")
            };

            return client;
        }
    }
}
