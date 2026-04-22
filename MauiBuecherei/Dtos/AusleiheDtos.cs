using System;

namespace MauiBuecherei.Dtos
{
    public class AusleiheDto
    {
        public int Id { get; set; }
        public string Buchnummer { get; set; } = string.Empty;
        public int Ausweisnummer { get; set; }
        public DateTime Ausleihdatum { get; set; }
        public DateTime? Rueckgabedatum { get; set; }
    }
}