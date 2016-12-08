using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace katanaIntro
{
    using System.IO;
    using System.Web.Http;
    using AppFunc = Func<IDictionary<string, object>, Task>;
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:8080";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Statrted");
                Console.ReadKey();
                Console.WriteLine("Stopping");
            }

        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            //app.Use( async (environment, next) =>
            //{
            //    foreach (var pair in environment.Environment)
            //    {
            //        Console.WriteLine("{0}:{1}", pair.Key, pair.Value);
            //    }

            //    await next();
            //});
            // 1 app.UseWelcomePage();
            //2 app.Run(ctx =>
            //  {
            //    return ctx.Response.WriteAsync("Hello World");
            //  });
            // 3rd

            app.Use(async (environment, next) =>
            {
                Console.WriteLine("Requesting : " + environment.Request.Path);
                await next();
                Console.WriteLine("Response : " + environment.Response.StatusCode);
            });

            ConfigureWebApi(app);

            app.Use<HelloWorldComponent>();
        }

        private void ConfigureWebApi(IAppBuilder app)
        {
            var  config = new HttpConfiguration();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}",
            new { id = RouteParameter.Optional });

            app.UseWebApi(config);
        }
    }

    public static class AppBuilderExtensions
    {
        public static void UseHelloWorld(this IAppBuilder app)
        {
            app.Use<HelloWorldComponent>();
        }
    }

    public class HelloWorldComponent
    {
        AppFunc _next;
        public HelloWorldComponent(AppFunc next )
        {
            _next = next;
        }

        public  Task Invoke(IDictionary<string, object> environment)
        {
            var response = environment["owin.ResponseBody"] as Stream;
            using (var writer = new StreamWriter(response))
            {
                return writer.WriteAsync("Hellooo !!!");
            }
        }
    }

}
