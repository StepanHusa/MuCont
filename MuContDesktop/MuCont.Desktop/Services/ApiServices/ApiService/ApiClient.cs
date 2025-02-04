using MuCont.Desktop.Constatns;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services.ApiServices.ApiService;


public class ApiClient:IApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient()
    {
        _httpClient = new HttpClient { 
            BaseAddress = new Uri(new Uri(GlobalDefaultSettings.APIUrl), "api/")
        };
    }

    public async Task<T?> GetAsAsync<T>(string endpoint)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<T>(endpoint);
        }
        catch (Exception e)
        {
            Console.WriteLine($"GET request error ({endpoint}): {e.Message}");
            return default;
        }
    }

    public async Task<TResponse?> PostAsAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(endpoint, requestData);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"POST request error ({endpoint}): {e.Message}");
            return default;
        }
    }
}


