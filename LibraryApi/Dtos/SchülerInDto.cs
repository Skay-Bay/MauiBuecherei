namespace LibraryApi.Dtos
{
    public class SchülerInDto
    {
        public int Ausweisnummer { get; set; }
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
    }
}