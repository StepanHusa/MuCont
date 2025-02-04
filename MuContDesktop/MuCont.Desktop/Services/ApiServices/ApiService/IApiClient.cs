using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services.ApiServices.ApiService;
public interface IApiClient
{
    Task<T?> GetAsAsync<T>(string endpoint);
    Task<TResponse?> PostAsAsync<TRequest, TResponse>(string endpoint, TRequest requestData);
}
