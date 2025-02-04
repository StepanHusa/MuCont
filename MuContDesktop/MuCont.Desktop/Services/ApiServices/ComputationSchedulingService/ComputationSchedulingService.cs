using MuCont.Desktop.Services.ApiServices.ApiService;
using MuCont.Desktop.Services.ApiServices.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services.ApiServices.ComputationSchedulingService;
internal class ComputationSchedulingService : ServiceBase, IComputationSchedulingService
{
    private const string _basePath = "api/systems";


    public ComputationSchedulingService(IApiClient apiClient) : base(apiClient, _basePath)
    {
    }
}
