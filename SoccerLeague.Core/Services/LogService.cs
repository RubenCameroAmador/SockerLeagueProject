namespace SoccerLeague.Core.Services;

public class LogService
{
    public void Exception(Exception ex)
    {
#if DEBUG
        Log("Exception", ex?.StackTrace ?? ex?.Message ?? ex?.ToString() ?? string.Empty);
#else
        Log("Error", ex.Message ?? ex.ToString());
#endif
    }

    public void Info(params string[] message)
    {
        Log("Info", message);
    }

    private void Log(string type, params string[] messages)
    {
        Console.WriteLine($"{type}: {string.Join(" ", messages)}");
    }

}
