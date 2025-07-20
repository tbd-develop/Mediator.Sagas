namespace TbdDevelop.Mediator.Sagas.Persistence.Exceptions;

public class SagaStateSerializationException(Type stateType) : Exception
{
    public override string Message => $"Saga State Serialization for {stateType.Name} failed";
}

public class SagaStateDeserializationException(Type stateType) : Exception
{
    public override string Message => $"Saga State Deserialization for {stateType.Name} failed";
}