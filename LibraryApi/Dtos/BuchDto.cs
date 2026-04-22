using System;

namespace LibraryApi.Dtos
{
    public class BuchDto
    {
        public string Buchnummer { get; set; } = string.Empty;
        public string Sachgebiet { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Titel { get; set; } = string.Empty;
        public string AutorInnen { get; set; } = string.Empty;
        public string Verlag { get; set; } = string.Empty;
        public string Verlagsort { get; set; } = string.Empty;
        public DateTime Erscheinungsdatum { get; set; }
    }
}