using System.Net.Http.Json;
using MauiBuecherei.Models;

namespace MauiBuecherei.Services
{
    public class StatistikApiService
    {
        private readonly HttpClient _httpClient;

        public StatistikApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetGesamtAusleihenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("statistik/ausleihen/gesamt");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return int.Parse(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetGesamtAusleihen Fehler: {ex}");
                throw;
            }
        }

        public async Task<int> GetAktiveAusleihenAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("statistik/ausleihen/aktiv");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return int.Parse(json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAktiveAusleihen Fehler: {ex}");
                throw;
            }
        }

        public async Task<List<TopBuchDto>> GetTopBuecherAsync(int take = 10)
        {
            try
            {
                var response = await _httpClient.GetAsync($"statistik/top-buecher?take={take}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<TopBuchDto>>()
                       ?? new List<TopBuchDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTopBuecher Fehler: {ex}");
                throw;
            }
        }
    }
}