using ESE.Core.Utils;
using ESE.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESE.Orders.API.Configuration
{
    public static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
        }
    }
}
