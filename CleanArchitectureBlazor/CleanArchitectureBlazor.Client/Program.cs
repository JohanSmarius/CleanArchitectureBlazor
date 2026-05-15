using Application;
using Application.Commands;
using Application.Queries;
using CleanArchitectureBlazor.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IStaffRepository, ExternalStaffRepository>();

builder.Services.AddScoped<ICreateStaffCommandHandler, CreateStaffCommandHandler>();
builder.Services.AddScoped<IUpdateStaffCommandHandler, UpdateStaffCommandHandler>();
builder.Services.AddScoped<IDeleteStaffCommandHandler, DeleteStaffCommandHandler>();
builder.Services.AddScoped<IGetAllStaffQueryHandler, GetAllStaffQueryHandler>();
builder.Services.AddScoped<IGetStaffByIdQueryHandler, GetStaffByIdQueryHandler>();
builder.Services.AddScoped<IIsStaffEmailUniqueQueryHandler, IsStaffEmailUniqueQueryHandler>();

await builder.Build().RunAsync();
