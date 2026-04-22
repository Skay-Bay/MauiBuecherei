using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryApi.Models
{
    public class SchülerIn
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Ausweisnummer { get; set; }

        [Required]
        [StringLength(100)]
        public string Vorname { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nachname { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<Ausleihe> Ausleihen { get; set; } = new List<Ausleihe>();
    }
}