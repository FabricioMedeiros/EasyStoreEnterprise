﻿using Microsoft.Extensions.Configuration;

namespace ESE.Core.Utils
{
    public static class ConfigurationExtension
    {
        public static string GetMessageQueueConnection(this IConfiguration configuration, string name)
        {
            return configuration?.GetSection("MessageQueueConnection")?[name];
        }
    }
}
