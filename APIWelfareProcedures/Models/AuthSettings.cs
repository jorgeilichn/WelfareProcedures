using System.ComponentModel.DataAnnotations;

namespace APIWelfareProcedures.Models
{
    public record AuthSettings
    {
        public string base_uri { get; init; }
        public string post_uri { get; init; }
    
        public string get_uri { get; init; }
        public string bearerToken { get; init; }
    }
}