using System.Collections.Generic;
using LibraryApi.Dtos;

namespace LibraryApi.Dtos
{
    public class DatabaseExport
    {
        public List<BuchDto> Buecher { get; set; } = new();
        public List<SchülerInDto> SchülerIn { get; set; } = new();
        public List<AusleiheDto> Ausleihen { get; set; } = new();
    }
}