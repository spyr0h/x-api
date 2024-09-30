using XApi.API.DependencyInjection;
using XApi.API.Tags.DependencyInjection;
using XApi.API.Tags.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

#region Custom dependencies DI
builder.Services.AddMapster();
builder.Services.AddMemoryCache();
builder.Services.AddTagsDependencies();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Custom endpoints registration
app.MapTagsEndpoints();
#endregion

app.Run();