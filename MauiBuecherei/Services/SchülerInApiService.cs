using System.Net.Http.Json;
using MauiBuecherei.Models;

namespace MauiBuecherei.Services
{
    public class SchülerInApiService
    {
        private readonly HttpClient _httpClient;

        public SchülerInApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SchülerInDto>?> GetSchülerInAsync()
        {
            try
            {
                var fullUrl = $"{_httpClient.BaseAddress}schülerIn";
                System.Diagnostics.Debug.WriteLine($"API-Aufruf: {fullUrl}");

                var response = await _httpClient.GetAsync("schülerIn");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<SchülerInDto>>();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"API-Fehler {response.StatusCode}: {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Fehler in GetSchülerInAsync: {ex}");
                throw;
            }
        }

        // POST: Neuen Schüler anlegen (Ausweisnummer = 0)
        public async Task<SchülerInDto?> CreateSchülerInAsync(SchülerInDto schueler)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("schülerIn", schueler);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SchülerInDto>();
                }
                else
                {
                    string error = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Fehler beim Anlegen: {response.StatusCode} - {error}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateSchülerInAsync Fehler: {ex.Message}");
                throw;
            }
        }

        // PUT: Schüler aktualisieren
        public async Task<bool> UpdateSchülerInAsync(int ausweisnummer, SchülerInDto schueler)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"schülerIn/{ausweisnummer}", schueler);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateSchülerInAsync Fehler: {ex.Message}");
                throw;
            }
        }

        // DELETE: Schüler löschen
        public async Task<bool> DeleteSchülerInAsync(int ausweisnummer)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"schülerIn/{ausweisnummer}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteSchülerInAsync Fehler: {ex.Message}");
                throw;
            }
        }
    }
}