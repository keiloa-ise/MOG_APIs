namespace APIs.Models
{
    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    }
}
