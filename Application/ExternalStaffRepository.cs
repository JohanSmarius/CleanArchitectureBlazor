using System.Net.Http.Json;
using Domain;

namespace Application;

/// <summary>
/// HTTP implementation for staff operations consumed by interactive client pages.
/// </summary>
public class ExternalStaffRepository(HttpClient httpClient) : IExternalStaffRepository
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<Staff>> GetAllStaffAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Staff>>("api/staff") ?? new List<Staff>();
    }

    public Task<Staff?> GetStaffByIdAsync(int id)
    {
        return _httpClient.GetFromJsonAsync<Staff>($"api/staff/{id}");
    }

    public async Task<Staff> AddStaffAsync(Staff staff)
    {
        var response = await _httpClient.PostAsJsonAsync("api/staff", staff);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Staff>()
            ?? throw new InvalidOperationException("Staff API returned an empty response.");
    }

    public async Task UpdateStaffAsync(Staff staff)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/staff/{staff.Id}", staff);
        response.EnsureSuccessStatusCode();
    }
}
