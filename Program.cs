using AuctionPortal.ApplicationLayer.Application;
using AuctionPortal.ApplicationLayer.IApplication;
using AuctionPortal.Common.Core;
using AuctionPortal.Common.Middleware;
using AuctionPortal.Common.Services;
using AuctionPortal.InfrastructureLayer.Infrastructure;
using AuctionPortal.InfrastructureLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// ----- CORS (allow Angular dev origin) -----
var corsPolicy = "_devCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
    // .AllowCredentials() // enable only if you use cookies
    );
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuctionPortal API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Common services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHeaderValue, HeaderValue>();
builder.Services.AddScoped<IBaseServiceConnector, BaseServiceConnector>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailServiceConnector, EmailServiceConnector>();

// Infrastructure
builder.Services.AddScoped<IUserInfrastructure, UserInfrastructure>();
builder.Services.AddScoped<IRoleInfrastructure, RoleInfrastructure>();
builder.Services.AddScoped<IApplicationUserOperationInfrastructure, ApplicationUserOperationInfrastructure>();
builder.Services.AddScoped<IClaimInfrastructure, ClaimInfrastructure>();
builder.Services.AddScoped<IEmailsInfrastructure, EmailsInfrastructure>();
builder.Services.AddScoped<IBidderInfrastructure, BidderInfrastructure>();
builder.Services.AddScoped<IBidderOperationInfrastructure, BidderOperationInfrastructure>();
builder.Services.AddScoped<IMakeInfrastructure, MakeInfrastructure>();
builder.Services.AddScoped<IModelInfrastructure, ModelInfrastructure>();
builder.Services.AddScoped<IYearInfrastructure, YearInfrastructure>();
builder.Services.AddScoped<ICategoryInfrastructure, CategoryInfrastructure>();
builder.Services.AddScoped<IProductInfrastructure, ProductInfrastructure>();
builder.Services.AddScoped<IInventoryInfrastructure, InventoryInfrastructure>();
// Application layer
builder.Services.AddScoped<IUserApplication, UserApplication>();
builder.Services.AddScoped<IRoleApplication, RoleApplication>();
builder.Services.AddScoped<IApplicationUserOperationApplication, ApplicationUserOperationApplication>();
builder.Services.AddScoped<IClaimApplication, ClaimApplication>();
builder.Services.AddScoped<IEmailsApplication, EmailsApplication>();
builder.Services.AddScoped<IBidderApplication, BidderApplication>();
builder.Services.AddScoped<IBidderOperationApplication, BidderOperationApplication>();
builder.Services.AddScoped<IMakeApplication, MakeApplication>();
builder.Services.AddScoped<IModelApplication, ModelApplication>();
builder.Services.AddScoped<IYearApplication, YearApplication>();
builder.Services.AddScoped<ICategoryApplication, CategoryApplication>();
builder.Services.AddScoped<IProductApplication, ProductApplication>();
builder.Services.AddScoped<IInventoryApplication, InventoryApplication>();
// AuthN / AuthZ
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var connector = new BaseServiceConnector(builder.Configuration);
        options.TokenValidationParameters = connector.GetValidationParameters();
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("USER_UAD", p => p.RequireClaim("perm", "USER_UAD"));
    options.AddPolicy("USER_UUP", p => p.RequireClaim("perm", "USER_UUP"));
    options.AddPolicy("USER_UAC", p => p.RequireClaim("perm", "USER_UAC"));
    options.AddPolicy("USER_UGI", p => p.RequireClaim("perm", "USER_UGI"));
    options.AddPolicy("USER_UGL", p => p.RequireClaim("perm", "USER_UGL"));

    options.AddPolicy("ROLE_RAD", p => p.RequireClaim("perm", "ROLE_RAD"));
    options.AddPolicy("ROLE_RUP", p => p.RequireClaim("perm", "ROLE_RUP"));
    options.AddPolicy("ROLE_RAC", p => p.RequireClaim("perm", "ROLE_RAC"));
    options.AddPolicy("ROLE_RGI", p => p.RequireClaim("perm", "ROLE_RGI"));
    options.AddPolicy("ROLE_RGL", p => p.RequireClaim("perm", "ROLE_RGL"));
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// In Development, DON'T force HTTPS so preflight to http://localhost:5070 works.
// In other environments, keep HTTPS redirection on.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS must run before auth and before custom token middleware.
app.UseCors(corsPolicy);

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
// Ensure your TokenValidatorMiddleware ignores OPTIONS requests if it inspects Authorization.
app.UseMiddleware<TokenValidatorMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
