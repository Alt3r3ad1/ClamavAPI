using clamavAPI.Services;
using clamavAPI.Middlewares;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
if (!builder.Environment.IsDevelopment())
{
    if (!Directory.Exists(@"/var/afKeys"))
    {
        Directory.CreateDirectory(@"/var/afKeys");
    }
    builder.Services.AddDataProtection()
                .SetApplicationName("clamavapi")
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo(@"/var/afKeys/"));
}
builder.Services.AddSwaggerGen();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ScanService>();
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddScoped<RateLimitService>();
if (builder.Environment.IsStaging())
{
    builder.WebHost.UseStaticWebAssets();
}

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHsts();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.Run();
