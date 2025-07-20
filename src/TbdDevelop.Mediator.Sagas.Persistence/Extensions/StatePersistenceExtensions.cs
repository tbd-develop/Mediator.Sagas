using System.Text.Json;
using TbdDevelop.Mediator.Sagas.Persistence.Exceptions;

namespace TbdDevelop.Mediator.Sagas.Persistence.Extensions;

public static class StatePersistenceExtensions
{
    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static string AsJson<T>(this T state)
    {
        return JsonSerializer.Serialize(state, JsonSerializerOptions) ??
               throw new SagaStateSerializationException(typeof(T));
    }

    public static object FromJson(this string state, Type stateType)
    {
        return JsonSerializer.Deserialize(state, stateType, JsonSerializerOptions) ??
               throw new SagaStateDeserializationException(stateType);
    }

    public static TState? FromJson<TState>(this string json)
    {
        return JsonSerializer.Deserialize<TState>(json, JsonSerializerOptions);
    }
}