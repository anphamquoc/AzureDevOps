using Microsoft.EntityFrameworkCore;
using Microsoft.SharePoint.Client;
using SharepointPermission.Entities;
using SharepointPermission.Interfaces;
using SharepointPermission.Provider;
using SharepointPermission.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true);
builder.Services.AddSingleton<ClientContext>(sp =>
{
    var siteUrl = builder.Configuration["Sharepoint:SiteUrl"];
    var username = builder.Configuration["Sharepoint:Username"];
    var password = builder.Configuration["Sharepoint:Password"];

    var clientContextProvider = new ClientContextProvider(siteUrl, username, password);

    return clientContextProvider.GetService(typeof(ClientContext)) as ClientContext;
});
builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
});

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