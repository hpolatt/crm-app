namespace PKT.Application.DTOs.Logging;

public class RequestLogDto
{
    public DateTime Timestamp { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string QueryString { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public Dictionary<string, string> RequestHeaders { get; set; } = new();
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public Dictionary<string, object> CustomFields { get; set; } = new();
}
