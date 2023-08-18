var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
int hostPort = Int16.Parse(builder.Configuration["Host:Port"]);

builder.WebHost.UseKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, hostPort);

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

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
