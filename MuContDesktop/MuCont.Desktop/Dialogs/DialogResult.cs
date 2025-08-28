namespace MuCont.Desktop.Dialogs;

/// <summary>
/// Represents the result of a dialog operation
/// </summary>
public class DialogResult
{
    public bool Success { get; set; }
    public bool WasCancelled { get; set; }
    public string? ErrorMessage { get; set; }

    public static DialogResult Ok() => new() { Success = true };
    public static DialogResult Cancel() => new() { WasCancelled = true };
    public static DialogResult Error(string message) => new() { ErrorMessage = message };
}

/// <summary>
/// Represents the result of a dialog operation with data
/// </summary>
public class DialogResult<T> : DialogResult
{
    public T? Data { get; set; }

    public static DialogResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static new DialogResult<T> Cancel() => new() { WasCancelled = true };
    public static new DialogResult<T> Error(string message) => new() { ErrorMessage = message };
}
