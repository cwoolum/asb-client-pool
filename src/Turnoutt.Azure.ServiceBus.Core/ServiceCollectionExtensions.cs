using System;
using System.Runtime.CompilerServices;

using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;

[assembly:InternalsVisibleTo("Turnoutt.Azure.ServiceBus.Core.UnitTests")]

namespace Turnoutt.Azure.ServiceBus.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Used to register Azure Service Bus as well as add a mapping for messages to topics or queues. This method will use managed identity and just take in a service bus name t configure the connection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceBusName"></param>
        /// <param name="connectionBuilder"></param>
        /// <param name="transportType"></param>
        /// <param name="retryPolicy"></param>
        public static void AddAzureServiceBusConnectionPool(this IServiceCollection services, string serviceBusName, Action<ServiceBusClientPoolBuilder> connectionBuilder, TransportType transportType = TransportType.Amqp, RetryPolicy retryPolicy = null)
        {
            services.AddSingleton(provider =>
            {
                var serviceBusConnection = new ServiceBusConnection($"{serviceBusName}.servicebus.windows.net", transportType, retryPolicy) {
                    TokenProvider = TokenProvider.CreateManagedIdentityTokenProvider()
                };

                return serviceBusConnection;
            });

            services.AddSingleton<IServiceBusClientPool>(provider =>
           {
               var serviceBusConnection = provider.GetRequiredService<ServiceBusConnection>();

               var poolBuilder = new ServiceBusClientPoolBuilder(serviceBusConnection);

               connectionBuilder(poolBuilder);

               return new ServiceBusClientPool(poolBuilder);
           });
        }

        /// <summary>
        /// Used to register Azure Service Bus as well as add a mapping for messages to topics or queues. This method takes in a connection string builder to configure the Service Bus connection.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceBusName"></param>
        /// <param name="connectionBuilder"></param>
        /// <param name="transportType"></param>
        /// <param name="retryPolicy"></param>
        public static void AddAzureServiceBusConnectionPool(this IServiceCollection services, ServiceBusConnectionStringBuilder connectionStringBuilder, Action<ServiceBusClientPoolBuilder> connectionBuilder)
        {
            services.AddSingleton(provider =>
            {
                var serviceBusConnection = new ServiceBusConnection(connectionStringBuilder);

                return serviceBusConnection;
            });

            services.AddSingleton<IServiceBusClientPool>(provider =>
           {
               var serviceBusConnection = provider.GetRequiredService<ServiceBusConnection>();

               var poolBuilder = new ServiceBusClientPoolBuilder(serviceBusConnection);

               connectionBuilder(poolBuilder);

               return new ServiceBusClientPool(poolBuilder);
           });
        }

        /// <summary>
        /// Used to register Azure Service Bus as well as add a mapping for messages to topics or queues. This method will use the Service Bus connection is passed in for all sending.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceBusName"></param>
        /// <param name="connectionBuilder"></param>
        /// <param name="transportType"></param>
        /// <param name="retryPolicy"></param>
        public static void AddAzureServiceBusConnectionPool(this IServiceCollection services, ServiceBusConnection connection, Action<ServiceBusClientPoolBuilder> connectionBuilder)
        {
            services.AddSingleton<IServiceBusClientPool>(provider =>
           {
               var poolBuilder = new ServiceBusClientPoolBuilder(connection);

               connectionBuilder(poolBuilder);

               return new ServiceBusClientPool(poolBuilder);
           });
        }
    }
}