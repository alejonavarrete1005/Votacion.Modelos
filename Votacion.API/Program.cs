using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// ===============================
// Servicios
// ===============================
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VotacionAPIContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("VotacionAPIContext")
        ?? throw new InvalidOperationException("Connection string not found.")
    )
);

var app = builder.Build();

// ===============================
// Middleware
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
