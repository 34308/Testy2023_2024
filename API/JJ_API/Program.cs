using JJ_API.Service.Authenthication;
using Microsoft.OpenApi.Models;
using JJ_API.Service;
using JJ_API.Interfaces;
using JJ_API.Service.Buisneess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (!File.Exists(PropertiesSingletonBase.path) && !File.Exists(PropertiesSingletonBase.pathBackup))
{
    PropertiesSingletonBase.Save(new PropertiesSingleton());
}
var properties = PropertiesSingletonBase.Load();
PropertiesSingleton propertiesSingleton = properties as PropertiesSingleton;
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICommentServiceWrapper, CommentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddAuthentication("HeaderAuthentication")
    .AddScheme<HeaderAuthenticationOptions, HeaderAuthenticationHandler>("HeaderAuthentication", options =>
    {
        options.SecretKey = propertiesSingleton.jwtSettings.SecretKey;
        options.Issuer = propertiesSingleton.jwtSettings.Issuer;
        options.Audience = propertiesSingleton.jwtSettings.Audience;

    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
   {
     new OpenApiSecurityScheme
     {
       Reference = new OpenApiReference
       {
         Type = ReferenceType.SecurityScheme,
         Id = "Bearer"
       }
      },
      new string[] { }
    }
  });
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
