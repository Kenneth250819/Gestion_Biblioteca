using Gestion_Biblioteca.Controllers;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Registrar HttpClient 
builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar UsuarioApiService con HttpClient inyectado
builder.Services.AddHttpClient<HomeService>();
builder.Services.AddHttpClient<LibroService>();
builder.Services.AddHttpClient<PrestamoService>();
builder.Services.AddHttpClient<UsuarioService>();


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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
