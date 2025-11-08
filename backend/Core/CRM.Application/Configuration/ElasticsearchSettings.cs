namespace CRM.Application.Configuration;

public class ElasticsearchSettings
{
    public string Uri { get; set; } = "http://localhost:9200";
    public string DefaultIndex { get; set; } = "crm-logs";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
