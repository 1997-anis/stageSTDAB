using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nest;
using StageStdab.Areas.Identity.Data;
using StageStdab.Areas.Identity.Data.JwtFeatures;
using StageStdab.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var appSettings = builder.Configuration.GetSection("ApplicationSettings");
builder.Services.Configure<ApplicationSettings>(appSettings);

var appSettingsSecretKey = appSettings.Get<ApplicationSettings>();
var key = Encoding.UTF8.GetBytes(appSettingsSecretKey.JWT_Secret);


var connectionString = builder.Configuration.GetConnectionString("StageStdabContextConnection") ?? throw new InvalidOperationException("Connection string 'StageStdabContextConnection' not found.");
//builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddDbContext<StageStdabContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<StageStdabUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<Role>()
    .AddEntityFrameworkStores<StageStdabContext>()
    .AddSignInManager<SignInManager<StageStdabUser>>()
    .AddRoleManager<RoleManager<Role>>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddSingleton<ApplicationSettings>();

builder.Services.AddAutoMapper(typeof(Program));
// Add services to the container.
builder.Services.AddScoped<JwtHandler>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*")
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        ;
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{   
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();;
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
