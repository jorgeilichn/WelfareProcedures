namespace APIWelfareProcedures.Models
{
    public record RequestPostBodyParameters
    {
        public string grant_type { get; init; }
        public string client_id { get; init; }
        public string client_secret{ get; init; }
        public string scope { get; init; }
        public string username { get; init; }
        public string password { get; init; }
    };
}