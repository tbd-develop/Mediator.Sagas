using System.Data.Common;
using System.Data.SqlClient;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace SqlServer.Integration.Tests.Infrastructure;

public class Initializer(String connectionString)
{
    public async Task Initialize()
    {
        await using var connection = new SqlConnection(connectionString);

        var script = File.ReadAllText("Scripts/InitialDatabase.sql");

        foreach (var statement in script.Split("GO"))
        {
            await connection.ExecuteAsync(statement);
        }
    }
}