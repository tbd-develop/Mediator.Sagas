
namespace TbdDevelop.Mediator.Sagas.SqlServer.Models;

public class Saga
{
    public int Id { get; set; }
    public Guid OrchestrationIdentifier { get; set; }
    public string State { get; set; } = null!;
}