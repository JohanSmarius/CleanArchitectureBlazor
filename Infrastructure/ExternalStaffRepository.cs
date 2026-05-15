using System.Net.Http.Json;
using Application;
using Entities;

namespace Infrastructure;

/// <summary>
/// HTTP-based staff repository used by WebAssembly components.
/// </summary>
public class ExternalStaffRepository : IStaffRepository
{
    private readonly HttpClient _httpClient;

    public ExternalStaffRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Staff>> GetAllStaffAsync()
        => await _httpClient.GetFromJsonAsync<List<Staff>>("api/staff") ?? [];

    public async Task<Staff?> GetStaffByIdAsync(int id)
        => await _httpClient.GetFromJsonAsync<Staff>($"api/staff/{id}");

    public async Task<Staff> CreateStaffAsync(Staff staff)
    {
        var response = await _httpClient.PostAsJsonAsync("api/staff", staff);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Staff>()
               ?? throw new InvalidOperationException("Failed to deserialize created staff response.");
    }

    public async Task<Staff> UpdateStaffAsync(Staff staff)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/staff/{staff.Id}", staff);
        response.EnsureSuccessStatusCode();
        return await GetStaffByIdAsync(staff.Id)
               ?? throw new InvalidOperationException($"Failed to reload updated staff with id {staff.Id}.");
    }

    public async Task DeleteStaffAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/staff/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<Staff>> GetActiveStaffAsync()
        => (await GetAllStaffAsync()).Where(s => s.IsActive).ToList();

    public async Task<List<Staff>> GetStaffByRoleAsync(StaffRole role)
        => (await GetAllStaffAsync()).Where(s => s.Role == role && s.IsActive).ToList();

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        var encodedEmail = Uri.EscapeDataString(email);
        var url = excludeId.HasValue
            ? $"api/staff/email-unique?email={encodedEmail}&excludeId={excludeId.Value}"
            : $"api/staff/email-unique?email={encodedEmail}";

        return await _httpClient.GetFromJsonAsync<bool>(url);
    }
}
