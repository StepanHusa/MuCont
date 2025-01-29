using Avalonia;

namespace MuCont.Desktop.Themes;

public interface IThemeManager
{
    void Initialize(Application application);

    void Switch(int index);
}
