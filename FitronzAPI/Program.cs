using FitronzService.Implementation;
using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.OpenApi.Models;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
int hostPort = Int16.Parse(builder.Configuration["Host:Port"]);
NpgsqlConnection.GlobalTypeMapper.MapComposite<SlotDetails>();
NpgsqlConnection.GlobalTypeMapper.MapComposite<GymTypeDetailz>();
builder.WebHost.UseKestrel(options =>
{
    options.Listen(System.Net.IPAddress.Any, hostPort);

});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Version = "v1",
//        Title = "Implement Swagger UI",
//        Description = "A simple example to Implement Swagger UI",
//    });
//});
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<ICloudStorageService, CloudStorageService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
var app = builder.Build();
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
//    });
//}
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        context.Response.Redirect("swagger");
    });
});
//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
