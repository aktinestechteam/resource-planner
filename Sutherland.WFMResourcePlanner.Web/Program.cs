using System.Data.SqlClient;
using System.Data;
using Sutherland.WFMResourcePlanner.Repository.Implementation;
using Sutherland.WFMResourcePlanner.Repository.Inerface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IDbConnection>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
    return connection;
});
builder.Services.AddScoped<IPlanRepository, PlanRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=w3crm}/{action=Login}/{id?}");

app.Run();
