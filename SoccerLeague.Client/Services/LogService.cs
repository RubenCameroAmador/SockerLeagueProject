namespace SoccerLeague.Client.Services;

public class LogService
{
    public void Exception(Exception ex)
    {
#if DEBUG
        Log("Exception", ex?.StackTrace ?? ex?.Message ?? string.Empty);
#else
        Log("Error", ex.Message);
#endif
    }

    private void Log(string type, params string[] messages)
    {
        Console.WriteLine(type, messages);
    }
}
