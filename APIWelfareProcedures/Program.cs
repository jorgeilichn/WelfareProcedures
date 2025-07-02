using System.Net.Http.Headers;
using APIWelfareProcedures.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<RequestPostBodyParameters>(
    builder.Configuration.GetSection("RequestPostBodyParameters"));
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("AuthSettings"));
builder.Services.AddHttpClient("welfare_client",config =>
{
    config.BaseAddress = new Uri(
        builder.Configuration.GetSection("AuthSettings").GetValue<string>("base_uri"));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();