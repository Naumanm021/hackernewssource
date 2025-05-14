using HackerNewsReader.Api.Services;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
 
builder.Services.AddHttpClient<IHackerNewsService, HackerNewsService>();
builder.Services.AddMemoryCache(); 

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder.WithOrigins("http://localhost:4200")
                         .AllowAnyHeader()
                         .AllowAnyMethod());
});

var app = builder.Build();
 
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();
app.Run();