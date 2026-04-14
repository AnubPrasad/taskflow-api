namespace TaskFlow_API.DTOs;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public Dictionary<string, string>? Fields { get; set; }
}