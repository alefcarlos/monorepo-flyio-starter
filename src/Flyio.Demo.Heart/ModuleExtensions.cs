using Microsoft.Extensions.Hosting;

namespace Flyio.Demo.Heart;

public static class HostApplicationBuilderExtensions
{

    extension<TBuilder>(TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        public TBuilder AddHeartModule()
        {
            return builder;
        }
    }
}