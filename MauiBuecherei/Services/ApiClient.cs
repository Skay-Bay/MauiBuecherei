using MauiBuecherei.Dtos;
using System.Net.Http.Json;

namespace MauiBuecherei.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ===== Schüler =====
        public async Task<List<SchülerInDto>> GetSchülerInAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<SchülerInDto>>("api/schülerIn") ?? new();
        }

        public async Task<SchülerInDto?> GetSchülerInAsync(int ausweisnummer)
        {
            return await _httpClient.GetFromJsonAsync<SchülerInDto>($"api/schülerIn/{ausweisnummer}");
        }

        public async Task<SchülerInDto> CreateSchülerInAsync(SchülerInDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/schülerIn", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SchülerInDto>() ?? throw new InvalidOperationException();
        }

        public async Task UpdateSchülerInAsync(int ausweisnummer, SchülerInDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/schülerIn/{ausweisnummer}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteSchülerInAsync(int ausweisnummer)
        {
            var response = await _httpClient.DeleteAsync($"api/schülerIn/{ausweisnummer}");
            response.EnsureSuccessStatusCode();
        }

        // ===== Bücher =====
        public async Task<List<BuchDto>> GetBuecherAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BuchDto>>("api/buch") ?? new();
        }

        public async Task<BuchDto?> GetBuchAsync(string buchnummer)
        {
            return await _httpClient.GetFromJsonAsync<BuchDto>($"api/buch/{buchnummer}");
        }

        public async Task<List<BuchDto>> SucheBuecherAsync(string suchbegriff)
        {
            return await _httpClient.GetFromJsonAsync<List<BuchDto>>($"api/buch/suche/{suchbegriff}") ?? new();
        }

        public async Task<BuchDto> CreateBuchAsync(BuchDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/buch", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BuchDto>() ?? throw new InvalidOperationException();
        }

        public async Task UpdateBuchAsync(string buchnummer, BuchDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/buch/{buchnummer}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteBuchAsync(string buchnummer)
        {
            var response = await _httpClient.DeleteAsync($"api/buch/{buchnummer}");
            response.EnsureSuccessStatusCode();
        }

        // ===== Ausleihen =====
        public async Task<List<AusleiheDto>> GetAusleihenAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AusleiheDto>>("api/ausleihen") ?? new();
        }

        public async Task<AusleiheDto?> GetAusleiheByBuchAsync(string buchnummer)
        {
            return await _httpClient.GetFromJsonAsync<AusleiheDto>($"api/ausleihen/{buchnummer}");
        }

        public async Task<AusleiheDto> CreateAusleiheAsync(AusleiheDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ausleihen", dto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AusleiheDto>() ?? throw new InvalidOperationException();
        }

        public async Task ReturnBookAsync(string buchnummer)
        {
            var response = await _httpClient.PutAsync($"api/ausleihen/{buchnummer}", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAusleiheAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ausleihen/{id}");
            response.EnsureSuccessStatusCode();
        }

        // ===== Statistik =====
        public async Task<int> GetGesamtAusleihenAsync()
        {
            return await _httpClient.GetFromJsonAsync<int>("api/statistik/ausleihen/gesamt");
        }

        public async Task<int> GetAktiveAusleihenAsync()
        {
            return await _httpClient.GetFromJsonAsync<int>("api/statistik/ausleihen/aktiv");
        }

        public async Task<List<TopBuchDto>> GetTopBuecherAsync(int take = 10)
        {
            return await _httpClient.GetFromJsonAsync<List<TopBuchDto>>($"api/statistik/top-buecher?take={take}") ?? new();
        }
        public async Task<DatabaseExport> ExportDatabaseAsync()
        {
            return await _httpClient.GetFromJsonAsync<DatabaseExport>("api/exportimport/export") ?? new();
        }

        public async Task<ImportResult> ImportDatabaseAsync(DatabaseExport data)
        {
            var response = await _httpClient.PostAsJsonAsync("api/exportimport/import", data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ImportResult>() ?? throw new InvalidOperationException();
        }
        public async Task<List<BulkResultDto>> BulkAusleihenAsync(BulkAusleiheDto bulkDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ausleihen/bulk", bulkDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<BulkResultDto>>() ?? new();
        }
    }
}