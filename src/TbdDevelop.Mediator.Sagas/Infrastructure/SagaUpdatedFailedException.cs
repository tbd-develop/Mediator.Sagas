namespace TbdDevelop.Mediator.Sagas.Infrastructure;

public class SagaUpdatedFailedException(string message) : Exception(message);