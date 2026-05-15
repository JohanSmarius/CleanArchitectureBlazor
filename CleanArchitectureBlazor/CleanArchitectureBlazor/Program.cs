using CleanArchitectureBlazor.Client.Pages;
using CleanArchitectureBlazor.Components;
using CleanArchitectureBlazor.Components.Account;
using CleanArchitectureBlazor.Configuration;
using CleanArchitectureBlazor.Data;
using Application;
using Application.Commands;
using Application.Queries;
using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var useSqlite = connectionString.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
        options.UseSqlite(connectionString);
    else
        options.UseSqlServer(connectionString);
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Register our application services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventCommandRepository>(sp => sp.GetRequiredService<IEventRepository>());
builder.Services.AddScoped<IEventQueryRepository>(sp => sp.GetRequiredService<IEventRepository>());
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffAssignmentRepository, StaffAssignmentRepository>();
builder.Services.AddScoped<ICreateEventUseCase, CreateEventUseCase>();
builder.Services.AddScoped<IUpdateEventUseCase, UpdateEventUseCase>();

// CQRS command handlers
builder.Services.AddScoped<ICreateEventCommandHandler, CreateEventCommandHandler>();
builder.Services.AddScoped<IUpdateEventCommandHandler, UpdateEventCommandHandler>();
builder.Services.AddScoped<IDeleteEventCommandHandler, DeleteEventCommandHandler>();
builder.Services.AddScoped<ICreateStaffCommandHandler, CreateStaffCommandHandler>();
builder.Services.AddScoped<IUpdateStaffCommandHandler, UpdateStaffCommandHandler>();
builder.Services.AddScoped<IDeleteStaffCommandHandler, DeleteStaffCommandHandler>();

// CQRS query handlers
builder.Services.AddScoped<IGetAllEventsQueryHandler, GetAllEventsQueryHandler>();
builder.Services.AddScoped<IGetEventByIdQueryHandler, GetEventByIdQueryHandler>();
builder.Services.AddScoped<IGetUpcomingEventsQueryHandler, GetUpcomingEventsQueryHandler>();
builder.Services.AddScoped<IGetEventsByDateRangeQueryHandler, GetEventsByDateRangeQueryHandler>();
builder.Services.AddScoped<IGetAllStaffQueryHandler, GetAllStaffQueryHandler>();
builder.Services.AddScoped<IGetStaffByIdQueryHandler, GetStaffByIdQueryHandler>();
builder.Services.AddScoped<IIsStaffEmailUniqueQueryHandler, IsStaffEmailUniqueQueryHandler>();

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.SectionName)
);

// Register IOptions<EmailOptions> if you need to inject it directly elsewhere
builder.Services.AddOptions<EmailOptions>().Bind(builder.Configuration.GetSection(EmailOptions.SectionName));

var app = builder.Build();

// In development, ensure the SQLite schema is created automatically so the app
// can be run without running `dotnet-ef database update` manually.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (useSqlite)
        db.Database.EnsureCreated();
    else
        db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found");

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CleanArchitectureBlazor.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
