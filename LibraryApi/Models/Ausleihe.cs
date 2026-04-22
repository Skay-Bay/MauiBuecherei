using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApi.Models
{
    public class Ausleihe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Buchnummer { get; set; } = string.Empty;

        public int? Ausweisnummer { get; set; }

        [Required]
        public DateTime Ausleihdatum { get; set; }

        public DateTime? Rueckgabedatum { get; set; }

        [ForeignKey(nameof(Buchnummer))]
        public Buch? Buch { get; set; }

        [ForeignKey(nameof(Ausweisnummer))]
        public SchülerIn? SchülerIn { get; set; }
    }
}