namespace LibraryApi.Dtos
{
    public class BulkAusleiheDto
    {
        public int SchülerInAusweisnummer { get; set; }
        public List<string> BuchNummern { get; set; } = new();
    }
}
