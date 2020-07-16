# Azure Service Bus Client Pool

[![Build Status](https://dev.azure.com/turnoutt/Public%20Projects/_apis/build/status/cwoolum.asb-client-pool?branchName=master)](https://dev.azure.com/turnoutt/Public%20Projects/_build/latest?definitionId=34&branchName=master)

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
