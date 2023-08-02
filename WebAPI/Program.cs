using Membership.Abstractions.ValueObjects;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("BearerToken",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Proporciona el valor del Token",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "BearerToken"
                    }
                },
                Array.Empty<string>()
            }
        });
});

#endregion

#region Membership configuration
builder.Services.AddMembershipCoreServices(
    jwtOptions => builder.Configuration.GetSection(JwtOptions.SectionKey)
    .Bind(jwtOptions),
    appClientInfoOptions => appClientInfoOptions.AppClients =
    builder.Configuration.GetSection(
        AppClientInfoOptions.SectionKey).Get<AppClientInfo[]>(),
    iDPClientInfoOptions => iDPClientInfoOptions.IDPClients =
    builder.Configuration.GetSection(IDPClientInfoOptions.SectionKey)
    .Get<IDPClientInfo[]>())
    .AddMembershipValidators()
    .AddMembershipMessageLocalizer()
    .AddRefreshTokenMemoryCacheService()
    .AddUserManagerAspNetIdentityService(
    aspNetIdentityOptions =>
    builder.Configuration.GetSection(AspNetIdentityOptions.SectionKey)
    .Bind(aspNetIdentityOptions));
#endregion

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

#region Authentication Scheme Configuration

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        builder.Configuration.GetSection(JwtOptions.SectionKey)
        .Bind(options.TokenValidationParameters);

        options.TokenValidationParameters.IssuerSigningKey =
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration
            .GetSection(JwtOptions.SectionKey)["SecurityKey"]));
        if(int.TryParse(
             builder.Configuration
            .GetSection(JwtOptions.SectionKey)["ClockSkewInMinutes"],
             out int value))
        {
            options.TokenValidationParameters.ClockSkew =
            TimeSpan.FromMinutes(value);
        }
    });
#endregion

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMembershipExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseMembershipEndpoints();

app.MapGet("/authorizeduser", (HttpContext context,
    IUserService userService) =>
Results.Ok($"Hello - {userService.FullName} - {context.User.Identity?.Name}"))
    .RequireAuthorization();

app.MapGet("/anonymous", () => Results.Ok("Hello, World!"));

app.Run();
