namespace SoccerLeague.Client;

public class ServiceActionResult
{
    public bool IsSuccess{get; set;}
    public string? Message{get; set;}

    public static ServiceActionResult Success(string message = "") => new ServiceActionResult { IsSuccess = true, Message = message };
    public static ServiceActionResult Error(string message = "Error") => new ServiceActionResult { IsSuccess = false, Message = message };
}
