using System;
using System.Collections.Generic;

namespace Common.MessageBus
{
    public class RabbitMqOptions
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
