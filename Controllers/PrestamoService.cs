using Gestion_Biblioteca.Models;
using Newtonsoft.Json;
using System.Text;

namespace Gestion_Biblioteca.Controllers
{
    public class PrestamoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PrestamoService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<Prestamos>> GetAllPrestamosAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Prestamo");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Prestamos>>(content);
        }

        public async Task<Prestamos> GetPrestamoByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Prestamo/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Prestamos>(content);
        }

        public async Task<Prestamos> CreatePrestamoAsync(Prestamos prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_baseUrl}/Prestamo", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<Prestamos> UpdatePrestamoAsync(int id, Prestamos prestamo)
        {
            var content = new StringContent(JsonConvert.SerializeObject(prestamo), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Prestamo/{id}", content);
            response.EnsureSuccessStatusCode();
            return prestamo;
        }

        public async Task<bool> DeletePrestamoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Prestamo/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
