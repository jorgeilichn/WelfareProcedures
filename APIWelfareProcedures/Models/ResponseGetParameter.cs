namespace APIWelfareProcedures.Models;

public record ResponseGetParameter
{
    public bool success { get; set; }
    
    public List<string> data { get; set; }
}