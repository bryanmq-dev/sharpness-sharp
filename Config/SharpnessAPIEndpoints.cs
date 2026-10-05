using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace sharpness_sharp.Config
{
    public static class SharpnessAPIEndpoints
    {
        public static WebApplication UseAPIEndpoints(this WebApplication app)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var endpointClasses = currentAssembly.GetTypes().Where(@class => @class is { IsClass: true, IsAbstract: false } && typeof(IEndpoints).IsAssignableFrom(@class));


            foreach (var endpointClass in endpointClasses)
            {
                IEndpoints endpoints = (IEndpoints)Activator.CreateInstance(endpointClass);

                endpoints?.MapEndpoints(app);
            }
            return app;
        }
    }

    public interface IEndpoints
    {
        void MapEndpoints(IEndpointRouteBuilder app);
    }
}