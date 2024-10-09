using XApi.API.DependencyInjection;
using XApi.API.Page.Endpoints;
using XApi.API.Pornstars.DependencyInjection;
using XApi.API.Pornstars.Endpoints;
using XApi.API.Search.DependencyInjection;
using XApi.API.Search.Endpoints;
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
builder.Services.AddPornstarsDependencies();
builder.Services.AddSearchDependencies();
builder.Services.AddSeoDependencies();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Custom endpoints registration
app.MapTagEndpoints();
app.MapPornstarEndpoints();
app.MapSearchEndpoints();
app.MapPageEndpoints();
#endregion

app.Run();