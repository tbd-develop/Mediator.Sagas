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

First, define a saga

```csharp

public class SampleNotification : INotification, IOrchestratedNotification
{
    public Guid OrchestrationIdentifier {get;set;}
    public Guid SampleIdentifier {set;set;}
}

public class SampleSagaState
{
    public int Value {get;set;}
}

public class SampleSaga : Saga<SampleSagaState>,
    IAmStartedBy<SampleNotification>,
    IHandle<SampleNotification>
{
    public void Handle(SampleNotification @event)
    {
        // Do code in here 
    }
}

```

Configure Sagas in your application, i.e.

With InMemoryPersistence;
```csharp
var host = Host.CreateApplicationBuilder();

host.AddSagas(configure => { 
    configure.RegisterSaga<SampleSaga>();

    configure.UseInMemoryPersistence();
});
```

Also available, SQL and MongoDB Persistence

## Installation

Include Nuget for TbdDevelop.Mediator.Sagas, optionally include 
TbdDevelop.Mediator.Sagas.MongoDb, TbdDevelop.Mediator.Sagas.SqlServer

### References

- Mediator - https://github.com/martinothamar/Mediator
- Saga - https://learn.microsoft.com/en-us/azure/architecture/reference-architectures/saga/saga
- Source Generators - https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/
