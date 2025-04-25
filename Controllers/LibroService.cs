using Gestion_Biblioteca.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Gestion_Biblioteca.Controllers
{
        public class LibroService
        {
            private readonly HttpClient _httpClient;
            private readonly string _baseUrl;

            public LibroService(IConfiguration configuration, HttpClient httpClient)
            {
                _httpClient = httpClient;
                _baseUrl = configuration["ApiSettings:BaseUrl"];
            }

            public async Task<IEnumerable<Libros>> GetAllLibrosAsync()
            {
                var response = await _httpClient.GetAsync(_baseUrl + "/Libro");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Libros>>(content);
            }

            public async Task<List<Libros>> BuscarLibrosAsync(string? titulo, string? autor)
            {
            string url = $"{_baseUrl}/Libro/buscar?titulo={titulo}&autor={autor}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Libros>>(content);
            }

            public async Task<Libros> GetLibroByIdAsync(int id)
            {
                var response = await _httpClient.GetAsync($"{_baseUrl + "/Libro"}/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Libros>(content);
            }

            public async Task<Libros> CreateLibroAsync(Libros libro)
            {
                var content = new StringContent(JsonConvert.SerializeObject(libro), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseUrl + "/Libro", content);
                response.EnsureSuccessStatusCode();
                return libro;
            }

            public async Task<Libros> UpdateLibroAsync(int id, Libros libro)
            {
                var content = new StringContent(JsonConvert.SerializeObject(libro), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_baseUrl + "/Libro"}/{id}", content);
                response.EnsureSuccessStatusCode();
                return libro; // ya lo tienes, no necesitas deserializar
            }

            public async Task<bool> DeleteLibroAsync(int id)
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl + "/Libro"}/{id}");
                return response.IsSuccessStatusCode;
            }
        }
}
