using Application;
using ClientInfrastructure;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register application services for WebAssembly
builder.Services.AddScoped<IExternalStaffRepository, ExternalStaffRepository>();
builder.Services.AddScoped<IAddStaffUseCase, AddStaffUseCase>();

await builder.Build().RunAsync();
