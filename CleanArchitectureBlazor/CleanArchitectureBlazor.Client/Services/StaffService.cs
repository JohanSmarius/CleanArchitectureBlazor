using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using CleanArchitectureBlazor.Client.Models;

namespace CleanArchitectureBlazor.Client.Services
{
    /// <summary>
    /// Service for managing staff data via HTTP API.
    /// </summary>
    public class StaffService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffService"/> class.
        /// </summary>
        /// <param name="httpClient">The injected HttpClient instance.</param>
        public StaffService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets the list of staff members.
        /// </summary>
        /// <returns>A list of staff members.</returns>
        public async Task<List<Staff>> GetStaffAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Staff>>("api/staff");
        }

        /// <summary>
        /// Gets a staff member by ID.
        /// </summary>
        /// <param name="id">The staff ID.</param>
        /// <returns>The staff member, or null if not found.</returns>
        public async Task<Staff?> GetStaffByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Staff>($"api/staff/{id}");
        }

        /// <summary>
        /// Creates a new staff member.
        /// </summary>
        /// <param name="staff">The staff member to create.</param>
        /// <returns>The created staff member.</returns>
        public async Task<Staff?> CreateStaffAsync(Staff staff)
        {
            // Age validation: must be at least 18 years old
            var today = DateTime.Today;
            var age = today.Year - staff.DateOfBirth.Year;
            if (staff.DateOfBirth.Date > today.AddYears(-age)) age--;
            if (age < 18)
            {
                throw new InvalidOperationException("Staff member must be at least 18 years old.");
            }

            var response = await _httpClient.PostAsJsonAsync("api/staff", staff);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Staff>();
        }

        /// <summary>
        /// Updates an existing staff member.
        /// </summary>
        /// <param name="id">The staff ID.</param>
        /// <param name="staff">The updated staff data.</param>
        public async Task UpdateStaffAsync(int id, Staff staff)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/staff/{id}", staff);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Deletes a staff member by ID.
        /// </summary>
        /// <param name="id">The staff ID.</param>
        public async Task DeleteStaffAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/staff/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
