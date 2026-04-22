using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryApi.Models
{
    public class Buch
    {
        [Key]
        public string Buchnummer { get; set; } = string.Empty;

        public string Sachgebiet { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        [Required]
        public string Titel { get; set; } = string.Empty;

        public string AutorInnen { get; set; } = string.Empty;

        public string Verlag { get; set; } = string.Empty;

        public string Verlagsort { get; set; } = string.Empty;

        public DateTime Erscheinungsdatum { get; set; }

        [JsonIgnore]
        public ICollection<Ausleihe> Ausleihen { get; set; } = new List<Ausleihe>();
    }
}