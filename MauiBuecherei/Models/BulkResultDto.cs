namespace MauiBuecherei.Models
{
    public class BulkResultDto
    {
        public string BuchNummer { get; set; } = string.Empty;
        public bool Erfolg { get; set; }
        public string? Fehlermeldung { get; set; }
    }
}