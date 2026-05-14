using System.Net.Http.Json;
using Domain;
using DomainService;

namespace ClientInfrastructure
{
    public class ExternalStaffRepository : IExternalStaffRepository
    {
        private readonly HttpClient _httpClient;

        public ExternalStaffRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<List<Staff>> GetAllStaffAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Staff>>("api/staff");
        }

        public async Task<Staff> GetStaffByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Staff>($"api/staff/{id}");
        }

        public async Task UpdateStaffAsync(Staff staff)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/staff/{staff.Id}", staff);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Staff> AddStaffAsync(Staff staff)
        {
            var response = await _httpClient.PostAsJsonAsync("api/staff", staff);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Staff>();
        }
    }
}
