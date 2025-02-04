using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuCont.Desktop.Helpers;
public static class ServiceHelper
{

    public static string Join(params object[] values)
    {
        return string.Join("/", values);
    }
}