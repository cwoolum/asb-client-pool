# Azure Service Bus Client Pool

![.NET Core](https://github.com/cwoolum/asb-client-pool/workflows/.NET%20Core/badge.svg)

This is a library for easily sending Service Bus Topic and Queue messages. Using this Client Pool, you can map messages to topic and queue endpoints. 

Additionally, you inly need to inject one service to access all of your queue and 
topic clients within your services. This eliminates the need to create instances of the clients in your constructors or create overriden implemnentations of `TopicClient` and `QueueClient`.

## Usage

To use the client pool, you can just use the following code in your `ConfigureServices` method.

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddAzureServiceBusConnectionPool("my-service-bus-name", (builder) => {
        builder.AddQueueClient<MyMessage>("items.shipped");
    });
}
```

In your service, just inject `IServiceBusClientPool` and use it like this:

``` csharp
private readonly IServiceBusClientPool _clientPool;_

public MyService(IServiceBusClientPool clientPool)
{
    _clientPool = clientPool;_
}


public async Task DoSomeWork()
{
    var message = new MyMessage();
    await _clientPool.SendQueueMessageAsync(message);
}
```