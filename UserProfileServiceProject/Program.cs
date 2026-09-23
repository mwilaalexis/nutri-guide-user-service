using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using UserProject.DataAccess.Data;
using UserProfileServiceProject.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddConfigurationAndServices();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();


app.UseStaticFiles(new StaticFileOptions
 {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")
    ),
        RequestPath = ""
});

app.UseCors("AllowAll");
try
{
    SeedData.Initialize(app);
}
catch (Exception ex)
{
    Console.WriteLine("Seed data skipped: " + ex.Message);
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    Console.WriteLine($"Auth services received → {context.Request.Method} {context.Request.Path}");
    Console.WriteLine($"   Authorization Header: {context.Request.Headers["Authorization"]}");
    Console.WriteLine($"   content Boby: {context.Request.Body.ToString()}");
    await next.Invoke();
});


app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
