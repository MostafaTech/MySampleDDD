using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Common.MessageBus
{
    public class RabbitMqMessageBus : IMessageBus, IDisposable
    {
        private readonly ILogger<RabbitMqMessageBus> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqOptions _options;
        private IConnection _connection;
        private IModel _channel;
        public RabbitMqMessageBus(
            ILogger<RabbitMqMessageBus> logger,
            IServiceProvider serviceProvider,
            IOptions<RabbitMqOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
            CloseConnection();
        }

        public void Send(object data)
        {
            this.CheckConnection();

            var messageId = Guid.NewGuid();
            var bodyType = data.GetType().AssemblyQualifiedName;
            var bodyJson = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var bodyBytes = Encoding.UTF8.GetBytes(bodyJson);

            var props = _channel.CreateBasicProperties();
            props.ContentEncoding = "utf-8";
            props.ContentType = "application/json";
            props.Headers = new Dictionary<string, object>();
            props.Headers.Add("id", messageId.ToString());
            props.Headers.Add("clrtype", bodyType);

            _channel.BasicPublish(
                exchange: "",
                routingKey: _options.QueueName,
                basicProperties: props,
                body: bodyBytes);

            _logger.LogDebug("Message sent to message bust with id: {MessageId}", messageId);
        }

        public Task StartConsume(Type[] handlerTypes, Type[] middlewares = null, CancellationToken stoppingToken = default)
        {
            // create a dictionary of messages and handlers <MessageType, HandlerType>
            var handlers = new Dictionary<Type, Type>();
            foreach (var tHandler in handlerTypes)
            {
                var messageTypes = Helpers.ReflectionHelpers.GetMessageTypesHandledByMessageHandler(tHandler);
                foreach (var t in messageTypes)
                    handlers.Add(t, tHandler);
            }

            CheckConnection();

            var consumer = new EventingBasicConsumer(_channel);
            //consumer.Shutdown += OnConsumerShutdown;
            //consumer.Registered += OnConsumerRegistered;
            //consumer.Unregistered += OnConsumerUnregistered;
            //consumer.ConsumerCancelled += OnConsumerCancelled;

            try
            {
                stoppingToken.ThrowIfCancellationRequested();

                consumer.Received += (ch, ea) =>
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        Task.Run(async () =>
                        {
                            // parse message
                            var messageId = Encoding.UTF8.GetString(ea.BasicProperties.Headers["id"] as byte[]);
                            var bodyType = Type.GetType(Encoding.UTF8.GetString(ea.BasicProperties.Headers["clrtype"] as byte[]));
                            var bodyJson = Encoding.UTF8.GetString(ea.Body.ToArray());
                            var bodyObject = Newtonsoft.Json.JsonConvert.DeserializeObject(bodyJson, bodyType);
                            var middlewareInstances = new List<IMessageConsumerMiddleware>();

                            _logger.LogDebug(
                                "Message received of type [{MessageType}] with body: {MessageBody}", 
                                bodyType.FullName, bodyJson);

                            try
                            {
                                // prepare middlewares and run their before method
                                if (middlewares != null && middlewares.Any())
                                {
                                    foreach (var tmw in middlewares)
                                    {
                                        var mwInstance = (IMessageConsumerMiddleware)scope.ServiceProvider.GetService(tmw);
                                        await mwInstance.Before();
                                        middlewareInstances.Add(mwInstance);
                                    }
                                }

                                // handle message
                                var handlerType = handlers[bodyType];
                                var handlerInstance = scope.ServiceProvider.GetService(handlerType);
                                if (handlerInstance != null)
                                {
                                    var handleMethod = Helpers.ReflectionHelpers.GetMessageHandlerHandleMethod(handlerType, bodyType);
                                    await Helpers.ReflectionHelpers.InvokeMethodAsync(handleMethod, handlerInstance, new object[] { bodyObject });
                                }

                                // run middlewares after method
                                foreach (var mwInstance in middlewareInstances)
                                    await mwInstance.After(null);
                            }
                            catch (Exception ex)
                            {
                                // run middlewares after method with error
                                foreach (var mwInstance in middlewareInstances)
                                    await mwInstance.After(ex);
                            }

                            _channel.BasicAck(ea.DeliveryTag, false);
                        }).GetAwaiter().GetResult();
                    }
                };
                _channel.BasicConsume(_options.QueueName, false, consumer);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                if (stoppingToken.IsCancellationRequested)
                    return Task.FromCanceled(stoppingToken);

                _logger.LogError(ex, "Consumer stopped because of: {Message}", ex.Message);
                return Task.FromException(ex);
            }
        }

        public void CreateConnection()
        {
            try
            {
                // create connection
                var factory = new ConnectionFactory
                {
                    HostName = _options.Host,
                    Port = Convert.ToInt32(_options.Port),
                    UserName = _options.Username,
                    Password = _options.Password
                };
                _connection = factory.CreateConnection();
                _connection.CallbackException += OnConnectionCallbackException;
                _connection.ConnectionShutdown += OnConnectionShutdown;

                // create channel to default queue
                _channel = _connection.CreateModel();
                _channel.QueueDeclare(queue: _options.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                _logger.LogDebug("Message Bus Connection established");
            }
            catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
            {
                const int retry_timeout = 5;
                _logger.LogError($"Message Bus host is not reachable. Trying again in {retry_timeout} seconds ...");
                Thread.Sleep(retry_timeout * 1000);
                CreateConnection();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Message Bus connection could not be made");
            }
        }

        public void CloseConnection()
        {
            if (_connection != null)
            {
                _channel.Close(0, "User demand");
                _connection.Close(0, "User demand");
                _logger.LogDebug("Message Bus Connection closed due to user demand");
            }
        }

        public bool CheckConnection()
        {
            if (_connection != null)
                return true;

            CreateConnection();

            return _connection != null;
        }

        private void OnConnectionCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger.LogError(e.Exception, "Exception thrown on message bus");
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogError("Message bus connection lost. {ReplyText}", e.ReplyText);
        }
    }
}
