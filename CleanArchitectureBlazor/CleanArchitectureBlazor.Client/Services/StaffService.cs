using System.Net.Http.Json;
using CleanArchitectureBlazor.Client.Models;

namespace CleanArchitectureBlazor.Client.Services;

public class StaffService
{
    private readonly HttpClient _httpClient;

    public StaffService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Staff>> GetStaffAsync()
    {
        var staff = await _httpClient.GetFromJsonAsync<List<Staff>>("api/staff");
        return staff ?? new List<Staff>();
    }

    public async Task<Staff?> CreateStaffAsync(Staff staff)
    {
        var response = await _httpClient.PostAsJsonAsync("api/staff", staff);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Staff>();
    }
}