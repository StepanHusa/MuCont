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
    public int WindowWidth { get; set; } = 800;
    public int WindowHeight { get; set; } = 600;
}
