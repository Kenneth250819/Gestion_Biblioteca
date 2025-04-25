using Gestion_Biblioteca.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Gestion_Biblioteca.Controllers
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public UsuarioService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"];
        }

        public async Task<IEnumerable<Usuarios>> GetAllUsuariosAsync()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "/Usuario");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<Usuarios>>(content);
        }

        public async Task<Usuarios> GetUsuarioByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/Usuario/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Usuarios>(content);
        }

        public async Task<Usuarios> CreateUsuarioAsync(Usuarios usuario)
        {
            var content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseUrl + "/Usuario", content);
            response.EnsureSuccessStatusCode();
            return usuario;
        }

        public async Task<Usuarios> UpdateUsuarioAsync(int id, Usuarios usuario)
        {
            var content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/Usuario/{id}", content);
            response.EnsureSuccessStatusCode();
            return usuario;
        }

        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/Usuario/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
