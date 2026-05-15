using System.Net.Http.Json;
using Domain;

namespace Application;

/// <summary>
/// HTTP implementation for staff operations consumed by interactive client pages.
/// </summary>
public class ExternalStaffRepository(HttpClient httpClient) : IExternalStaffRepository
{
    public async Task<List<Staff>> GetAllStaffAsync()
    {
        return await httpClient.GetFromJsonAsync<List<Staff>>("api/staff") ?? new();
    }

    public Task<Staff?> GetStaffByIdAsync(int id)
    {
        return httpClient.GetFromJsonAsync<Staff>($"api/staff/{id}");
    }

    public async Task<Staff> AddStaffAsync(Staff staff)
    {
        var response = await httpClient.PostAsJsonAsync("api/staff", staff);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Staff>()
            ?? throw new InvalidOperationException("Invalid response received from staff API.");
    }

    public async Task UpdateStaffAsync(Staff staff)
    {
        var response = await httpClient.PutAsJsonAsync($"api/staff/{staff.Id}", staff);
        response.EnsureSuccessStatusCode();
    }
}
