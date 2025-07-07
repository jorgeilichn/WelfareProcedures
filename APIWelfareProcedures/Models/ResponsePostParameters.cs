using System.ComponentModel.DataAnnotations;

namespace APIWelfareProcedures.Models;

public record ResponsePostParameters
{
    public string access_token { get; init; }
    
    public string refresh_token { get; init; }
    
    public string token_type { get; init; }
    
    public int expires_in { get; init; }
}