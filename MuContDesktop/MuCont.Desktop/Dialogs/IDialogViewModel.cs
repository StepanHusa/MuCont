using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Dialogs;
public interface IDialogViewModel<TResult>
{
    Action<TResult?> OnClose { get; set; }
}