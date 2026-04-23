using System.Net.Http.Json;
using MauiBuecherei.Models;

namespace MauiBuecherei.Services
{
    public class AusleiheApiService
    {
        private readonly HttpClient _httpClient;

        public AusleiheApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/ausleihen
        public async Task<List<AusleiheDto>?> GetAusleihenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("ausleihen");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<AusleiheDto>>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAusleihen Fehler: {ex}");
                throw;
            }
        }

        // GET: api/ausleihen/{buchNummer}
        public async Task<AusleiheDto?> GetAusleiheByBuchAsync(string buchNummer)
        {
            try
            {
                var response = await _httpClient.GetAsync($"ausleihen/{buchNummer}");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<AusleiheDto>();
                else
                    return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAusleiheByBuch Fehler: {ex}");
                throw;
            }
        }

        // POST: api/ausleihen
        public async Task<AusleiheDto?> CreateAusleiheAsync(AusleiheDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ausleihen", dto);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<AusleiheDto>();
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Fehler beim Erstellen: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateAusleihe Fehler: {ex}");
                throw;
            }
        }

        // PUT: api/ausleihen/{buchNummer} → Rückgabe + Anonymisierung
        public async Task<bool> ReturnBookAsync(string buchNummer)
        {
            try
            {
                var response = await _httpClient.PutAsync($"ausleihen/{buchNummer}", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReturnBook Fehler: {ex}");
                throw;
            }
        }

        // DELETE: api/ausleihen/{id}
        public async Task<bool> DeleteAusleiheAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"ausleihen/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAusleihe Fehler: {ex}");
                throw;
            }
        }

        // POST: api/ausleihen/bulk
        public async Task<List<BulkResultDto>?> BulkAusleiheAsync(BulkAusleiheDto bulk)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("ausleihen/bulk", bulk);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<BulkResultDto>>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BulkAusleihe Fehler: {ex}");
                throw;
            }
        }
    }
}
