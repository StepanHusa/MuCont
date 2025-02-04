using MuCont.Desktop.Dialogs.ViewModels;
using MuCont.Desktop.Services.ApiServices.ApiService;
using MuCont.Desktop.Services.ApiServices.BaseClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services.ApiServices.SystemsService;
internal class SystemService: ServiceBase,ISystemService
{
    private readonly IApiClient _apiClient;
    private const string _basePath = "api/systems";

    public SystemService(IApiClient apiClient): base(apiClient, _basePath)
    {
        _apiClient = apiClient;
    }

    public Task<IEnumerable<SystemModel>?> GetSystems()
        => GetAsAsync<IEnumerable<SystemModel>>();

    public Task<SystemModel?> PostNewService(NewSystemPost post)
        => PostAsAsync<NewSystemPost, SystemModel>("new", post);

}

public class SystemModel
{
    uint Id { get; set; }
    string Name { get; set; }
    IEnumerable<string> Info { get; set; }
    IEnumerable<string> Notes { get; set; }

    IEnumerable<SymbolModel> Variables { get; set; }    

}

internal class DiagramsModel
{
    public uint SystemId { get; set; }
    string Name { get; set; }

    IEnumerable<CurveModel> Curves{ get; set; }

}

internal class CurveModel // This is for the curves continuated in coordinates+parameters space, the integrated curves (with respect to time) will be in a different model
{
    uint Id { get; set; }
    uint SystemId { get; set; }

    string Name { get; set; }

    CurveDataModel Data { get; set; }
}

internal class CurveDataModel
{
    // 1. dimension is Points
    // 2. dimension is Coordinates+Parameters
    // 3. dimension is potencionaly Colocation of cycles
    double[,,] Array { get; set; } 

    string[] FreeParameters // this string is the key to find SymbolModel in the system
    { get; set; }


}



internal class SymbolModel
{
    string Name { get; set; }
    string LatexName { get; set; }

    SymbolType Type { get; set; }
}

internal enum SymbolType
{
    Variable,
    Coordinate,
    Time
}

//internal class ParametersModel
//{
//    IEnumerable<ParameterModel> Parameters { get; set; }
//}

//internal class ParameterModel
//{
//    string Name { get; set; }
//    string LatexName { get; set; }
//}

//internal class VariablesModel
//{
//    IEnumerable<VariableModel> Variables { get; set; }
//}

//internal class VariableModel
//{
//    string Name { get; set; }
//    string LatexName { get; set; }

//}