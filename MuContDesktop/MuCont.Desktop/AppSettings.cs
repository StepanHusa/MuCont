using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MuCont.Desktop;

public class AppSettings
{
    public string Theme { get; set; } = "Light";
    public string LastOpenedFile { get; set; } = string.Empty;
    public bool StartMaximized { get; set; } = false;
    public bool UseLocalApi { get; set; } = true;
    public string ApiAddress { get; set; } = "http://localhost:5000";
}
