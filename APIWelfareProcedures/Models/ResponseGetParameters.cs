namespace APIWelfareProcedures.Models
{
    public record ResponseGetParameters
    {
        public bool success { get; init; }
        
        public List<string> data { get; init; }
    }
}