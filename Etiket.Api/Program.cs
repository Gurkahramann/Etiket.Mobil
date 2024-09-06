using Etiket.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
DotNetEnv.Env.Load();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<KumasRepository>();
string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
        {
            { "ConnectionStrings:DefaultConnection", connectionString }
        });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
