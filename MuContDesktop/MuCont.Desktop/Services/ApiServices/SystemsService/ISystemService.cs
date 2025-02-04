using MuCont.Desktop.Dialogs.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Services.ApiServices.SystemsService;
internal interface ISystemService
{
    Task<IEnumerable<SystemModel>?> GetSystems();
    Task<SystemModel?> PostNewService(NewSystemPost post);
}
