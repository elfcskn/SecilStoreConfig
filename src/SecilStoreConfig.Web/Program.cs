using SecilStoreConfig.Library.Data;
using SecilStoreConfig.Library.Repositories;
using SecilStoreConfig.Library.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = SharedDatabase.ConnectionString;

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IConfigurationRepository>(_ => new ConfigurationRepository(connectionString));
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();

var app = builder.Build();

await DbInitializer.InitializeAsync(connectionString);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Config}/{action=Index}/{id?}");

app.Run();
