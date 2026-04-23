// Services/BuchApiService.cs
using System.Net.Http.Json;
using MauiBuecherei.Models;

namespace MauiBuecherei.Services
{
    public class BuchApiService
    {
        private readonly HttpClient _httpClient;

        public BuchApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/buch
        public async Task<List<BuchDto>?> GetBuecherAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("buch");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<BuchDto>>();
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API-Fehler {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetBuecherAsync Fehler: {ex}");
                throw;
            }
        }

        // GET: api/buch/{buchNummer}
        public async Task<BuchDto?> GetBuchAsync(string buchNummer)
        {
            try
            {
                var response = await _httpClient.GetAsync($"buch/{buchNummer}");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<BuchDto>();
                else
                    return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetBuchAsync Fehler: {ex}");
                throw;
            }
        }

        // GET: api/buch/suche/{suchbegriff}
        public async Task<List<BuchDto>?> SucheBuecherAsync(string suchbegriff)
        {
            try
            {
                var response = await _httpClient.GetAsync($"buch/suche/{suchbegriff}");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<List<BuchDto>>();
                else
                    return new List<BuchDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SucheBuecherAsync Fehler: {ex}");
                throw;
            }
        }

        // POST: api/buch (Buchnummer = "0" => automatisch generieren)
        public async Task<BuchDto?> CreateBuchAsync(BuchDto buch)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("buch", buch);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<BuchDto>();
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Fehler beim Anlegen: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateBuchAsync Fehler: {ex.Message}");
                throw;
            }
        }

        // PUT: api/buch/{buchNummer}
        public async Task<bool> UpdateBuchAsync(string buchNummer, BuchDto buch)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"buch/{buchNummer}", buch);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateBuchAsync Fehler: {ex.Message}");
                throw;
            }
        }

        // DELETE: api/buch/{buchNummer}
        public async Task<bool> DeleteBuchAsync(string buchNummer)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"buch/{buchNummer}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteBuchAsync Fehler: {ex.Message}");
                throw;
            }
        }
    }
}