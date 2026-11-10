using System.Text.Json;
using System.Text;

namespace OA.Test.Integration;

public static class HttpClientExtensions
{
    static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    public static StringContent SerializeAndCreateContent<T>(T model)
    {
        string jsonContent = JsonSerializer.Serialize(model, DefaultJsonOptions);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        return content;
    }

    public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, DefaultJsonOptions)
               ?? throw new InvalidOperationException($"Response body could not be deserialized to {typeof(T).Name}: {json}");
    }

    public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T model)
        => client.PostAsync(url, SerializeAndCreateContent(model));

    public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient client, string url, T model)
        => client.PutAsync(url, SerializeAndCreateContent(model));
}
