using MuCont.Desktop.Helpers;
using MuCont.Desktop.Services.ApiServices.ApiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services.ApiServices.BaseClasses;
public abstract class ServiceBase
{
    private readonly IApiClient _apiClient;
    private readonly string _basePath;

    public ServiceBase(IApiClient apiClient, string basePath)
    {
        _apiClient = apiClient;
        _basePath = basePath;
    }
    protected Task<T?> GetAsAsync<T>(string path)
    => _apiClient.GetAsAsync<T>(ServiceHelper.Join(_basePath, path));

    protected Task<T?> GetAsAsync<T>()
    => _apiClient.GetAsAsync<T>(_basePath);

    protected Task<TResponse?> PostAsAsync<TRequest, TResponse>(string path, TRequest requestData)
    => _apiClient.PostAsAsync<TRequest, TResponse>(ServiceHelper.Join(_basePath, path), requestData);
}
