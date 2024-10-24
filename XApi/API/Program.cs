using API.Page.DependencyInjection;
using API.Paging.DependencyInjection;
using XApi.API.Autocomplete.Endpoints;
using XApi.API.Categories.DependencyInjection;
using XApi.API.DependencyInjection;
using XApi.API.Linkbox.DependencyInjection;
using XApi.API.Page.Endpoints;
using XApi.API.Pornstars.DependencyInjection;
using XApi.API.Pornstars.Endpoints;
using XApi.API.Search.DependencyInjection;
using XApi.API.Search.Endpoints;
using XApi.API.Tags.DependencyInjection;
using XApi.API.Tags.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    // TODO : be more strict
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

#region Custom dependencies DI
builder.Services.AddMapster();
builder.Services.AddMemoryCache();
builder.Services.AddTagsDependencies();
builder.Services.AddCategoriesDependencies();
builder.Services.AddPornstarsDependencies();
builder.Services.AddSearchDependencies();
builder.Services.AddSeoDependencies();
builder.Services.AddPagingsDependencies();
builder.Services.AddPagesDependencies();
builder.Services.AddLinkboxesDependencies();
#endregion

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

#region Custom endpoints registration
app.MapTagEndpoints();
app.MapPornstarEndpoints();
app.MapSearchEndpoints();
app.MapPageEndpoints();
app.MapAutocompleteEndpoints();
#endregion

app.Run();