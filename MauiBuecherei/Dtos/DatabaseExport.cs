using System.Collections.Generic;

namespace MauiBuecherei.Dtos
{
    public class DatabaseExport
    {
        public List<BuchDto> Buecher { get; set; } = new();
        public List<SchülerInDto> SchülerIn { get; set; } = new();
        public List<AusleiheDto> Ausleihen { get; set; } = new();
    }
}