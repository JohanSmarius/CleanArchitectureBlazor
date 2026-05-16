using System.Net.Http.Json;
using ClientApplication;
using ClientApplication.DataAdapters;
using Entities;

namespace InfraClient;

public class ExternalStaffRepository : IExternalStaffRepository
{
    private readonly HttpClient _httpClient;

    public ExternalStaffRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
        
    public async Task<List<StaffDTO>> GetAllStaffAsync()
    {
        var returnedStaff = await _httpClient.GetFromJsonAsync<List<Staff>>("api/staff");
        return returnedStaff?.Select(s => s.ToDTO()).ToList() ?? new List<StaffDTO>();
    }

    public async Task<StaffDTO> GetStaffByIdAsync(int id)
    {
        var returnedStaff = await _httpClient.GetFromJsonAsync<Staff>($"api/staff/{id}");
        return returnedStaff?.ToDTO() ?? new();
    }

    public async Task UpdateStaffAsync(StaffDTO staff)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/staff/{staff.Id}", staff);
        response.EnsureSuccessStatusCode();
    }

    public async Task<StaffDTO> AddStaffAsync(StaffDTO staff)
    {
        var response = await _httpClient.PostAsJsonAsync("api/staff", staff);
        response.EnsureSuccessStatusCode();
        var result =  await response.Content.ReadFromJsonAsync<Staff>();
        return result.ToDTO() ?? new();
    }
}