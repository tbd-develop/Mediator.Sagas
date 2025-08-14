# Mediator.Sagas
A library to bring Saga pattern to Mediator Library

[![Release to Nuget](https://github.com/tbd-develop/Mediator.Sagas/actions/workflows/release.yml/badge.svg?event=release)](https://github.com/tbd-develop/Mediator.Sagas/actions/workflows/release.yml)

### Why?

The Mediator pattern is a great way to decouple your application. The Mediator library is a
great implementation of this pattern and provides clear Command/Query separation with it's interfaces. 
It also provides a notification feature, that will publish a Notification and can be subscribed to from 
multiple subscribers. 

In developing a new application, I wanted to trigger an event when several other events
had occured. And rather than try and wedge this in to some sort of super handler, I decided that what I needed was 
a Saga. I heavily leaned on the Mediator library, and decided that using Source Generators was my best path 
to a clean Saga implementation. 

### But...

This might exist, this might not be a good idea. This might not be what the developers of Mediator expect you to do in 
this scenario. However, right now, this is what I need given what I know. If nothing else, I have learned more about 
source generators, I have stretched my legs as a developer and gone beyond my comfort zone. If you're reading this and this 
library actually proved useful, great! 

## Usage

### 1. Define orchestrated notifications
All notifications that participate in a saga must expose an OrchestrationIdentifier so the saga instance can be correlated.

```csharp
using Mediator;
using TbdDevelop.Mediator.Sagas.Contracts;

public sealed record OrderPlaced(Guid OrderId, Guid OrchestrationIdentifier) 
    : INotification, IOrchestratedNotification;

public sealed record PaymentCaptured(Guid OrderId, Guid OrchestrationIdentifier) 
    : INotification, IOrchestratedNotification;

public sealed record OrderShipped(Guid OrderId, Guid OrchestrationIdentifier) 
    : INotification, IOrchestratedNotification;
```

### 2. Create saga state (optional strongly-typed container)
```csharp
public class OrderSagaState
{
    public bool Placed { get; set; }
    public bool Paid { get; set; }
    public bool Shipped { get; set; }
    public DateTime? LastProgress { get; set; }
}
```

### 3. Implement a saga
Implement Saga<TState> and the relevant interfaces:
- IAmStartedBy<TNotification> marks the event that creates a new saga instance.
- IHandle<TNotification> handles subsequent correlated notifications.
- Optionally implement IPublishOnComplete (or IPublishOnComplete<TNotification>) if you want a notification published when IsComplete becomes true.

Handlers are asynchronous and return ValueTask.

```csharp
using TbdDevelop.Mediator.Sagas;
using TbdDevelop.Mediator.Sagas.Contracts;
using Mediator;

public sealed class OrderSaga : Saga<OrderSagaState>,
    IAmStartedBy<OrderPlaced>,
    IHandle<PaymentCaptured>,
    IHandle<OrderShipped>,
    IPublishOnComplete<OrderCompleted>
{
    public override bool IsComplete => State.Placed && State.Paid && State.Shipped;

    // Example: set up a periodic trigger every 30 seconds until complete
    public OrderSaga()
    {
        TriggerInterval = TimeSpan.FromSeconds(30); // will be polled
    }

    public ValueTask Handle(OrderPlaced @event, CancellationToken ct)
    {
        State.Placed = true;
        State.LastProgress = DateTime.UtcNow;
        return ValueTask.CompletedTask;
    }

    public ValueTask Handle(PaymentCaptured @event, CancellationToken ct)
    {
        State.Paid = true;
        State.LastProgress = DateTime.UtcNow;
        return ValueTask.CompletedTask;
    }

    public ValueTask Handle(OrderShipped @event, CancellationToken ct)
    {
        State.Shipped = true;
        State.LastProgress = DateTime.UtcNow;
        return ValueTask.CompletedTask;
    }

    public override Task TriggerImpl(CancellationToken cancellationToken)
    {
        // Optional periodic work (timeouts, reminders, etc.)
        // You can adjust NextTriggerTime manually or rely on TriggerInterval auto-advance.
        return Task.CompletedTask;
    }

    public OrderCompleted Publish() => new(State.Placed, State.Paid, State.Shipped, OrchestrationIdentifier);
}

public sealed record OrderCompleted(bool Placed, bool Paid, bool Shipped, Guid OrchestrationIdentifier) : INotification;
```

### 4. Register the saga and choose persistence
AddSagas registers the background trigger service and lets you configure persistence. In-memory is default/simple; MongoDB and SQL Server are available via optional packages.

```csharp
using Microsoft.Extensions.Hosting;
using TbdDevelop.Mediator.Sagas.Configuration;
using TbdDevelop.Mediator.Sagas.MongoDb.Infrastructure; // if using Mongo
using TbdDevelop.Mediator.Sagas.SqlServer.Infrastructure; // if using SQL Server

var builder = Host.CreateApplicationBuilder(args);

builder.AddSagas(cfg =>
{
    cfg.RegisterSaga<OrderSaga>()
       .UseInMemoryPersistence();
    // or: .UseMongoDb("mongodb://localhost:27017", "SagasDb")
    // or: .UseSqlServer(builder.Configuration.GetConnectionString("Sagas")!);
});
```

### 5. Configure trigger polling (optional)
The background service polls for sagas that need triggers. Configure interval (milliseconds) in appsettings:

```json
{
  "sagas": {
    "triggers": {
      "IntervalMs": 2000
    }
  }
}
```
If omitted, default interval is 1000 ms.

### 6. Publishing orchestrated notifications
When you publish any IOrchestratedNotification with a shared OrchestrationIdentifier, the correct saga instance is loaded and the matching handler invoked. The first notification handled by an IAmStartedBy<T> handler creates the saga instance.

### 7. Completion publication
If the saga implements IPublishOnComplete / IPublishOnComplete<T>, once IsComplete is true after handling an event (or potentially after a trigger), the Publish() result is emitted as a Mediator notification.

### 8. Triggers
To enable time-based operations:
- Set TriggerInterval to have the framework reschedule automatically.
- Or set NextTriggerTime explicitly (absolute UTC time) and clear/update after TriggerImpl runs.
A saga has a trigger if either NextTriggerTime or TriggerInterval is non-null. The background service fetches due sagas and calls Trigger(), which advances NextTriggerTime when TriggerInterval is set.

## Persistence Options

| Option      | Package                                   | Notes |
|-------------|--------------------------------------------|-------|
| In-Memory   | TbdDevelop.Mediator.Sagas (built-in)       | Non-durable, good for tests/local dev |
| MongoDB     | TbdDevelop.Mediator.Sagas.MongoDb          | Requires connection string; uses pooled DbContext with MongoDB provider |
| SQL Server  | TbdDevelop.Mediator.Sagas.SqlServer        | Runs migrations automatically on startup |

Mongo usage:
```csharp
cfg.RegisterSaga<OrderSaga>()
   .UseMongoDb("mongodb://localhost:27017", "SagasDb");
```

SQL Server usage:
```csharp
cfg.RegisterSaga<OrderSaga>()
   .UseSqlServer(builder.Configuration.GetConnectionString("Sagas")!);
```

## Installation

Add the core package plus persistence provider(s) you need:
- dotnet add package TbdDevelop.Mediator.Sagas
- dotnet add package TbdDevelop.Mediator.Sagas.MongoDb (optional)
- dotnet add package TbdDevelop.Mediator.Sagas.SqlServer (optional)

Also install Mediator:
- dotnet add package Mediator

Ensure you have the proper using statements for extension methods (e.g. TbdDevelop.Mediator.Sagas.MongoDb.Infrastructure).

## Reference Summary
- Mediator - https://github.com/martinothamar/Mediator
- Saga pattern - https://learn.microsoft.com/en-us/azure/architecture/reference-architectures/saga/saga
- Source Generators - https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/

---
Contributions and feedback welcome.
