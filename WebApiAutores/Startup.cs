using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApiAutores.Entities;
using WebApiAutores.Filters;
using WebApiAutores.Middlewares;
using WebApiAutores.Services;

namespace WebApiAutores;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ExceptionFilter));//aplicamos a todos los controladores el filtro
        }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler
        = ReferenceHandler.IgnoreCycles);// para solucionar el error de entra en bucle el sql porque hay una relacion de muchos a muchos
        
        //Add DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        });
        
        //Add Repositories
        services.AddTransient<IEmailSenderService, EmailSenderService>();
  
       

        services.AddTransient<MiFiltro>();
        services.AddAutoMapper(typeof(Startup));
        
        //Add Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        
        //Add Authentication Jwt
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;//para que por defecto use jwt
            
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;//para que guarde el token
            options.RequireHttpsMetadata = false; //para que no use https
            
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,//valida el emisor
                ValidateAudience = true,//valida el receptor
                //ValidateLifetime = true,//valida el tiempo de vida
                //ValidateIssuerSigningKey = true,//valida la firma
                ValidIssuer = Configuration["JWT:ValidIssuer"],//el emisor debe ser el mismo que el del token
                ValidAudience = Configuration["JWT:Audience"],//el receptor debe ser el mismo que el del token
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])),//la clave secreta debe ser la misma que la del token
                //ClockSkew = TimeSpan.Zero//para que no haya diferencia de tiempo
            };
        });
        
        // Add cache filter
        services.AddResponseCaching();

		//Add CORS
		services.AddCors(options =>
        {
            options.AddPolicy("CorsRule", rule =>
            {
                rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
            });
            
        });// para permitir que se conecte el backend con el forntend
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        /*app.Map("/semitas", app =>
        {
            app.Run(async contexto =>
            {
                await contexto.Response.WriteAsync("Interceptando la pipeline de procesos");
            });
        });*/
        //middleware
        app.UseLogginResponseHTTP();
        
        //if (env.IsDevelopment())
        //{
            app.UseSwagger();
            app.UseSwaggerUI();
        //}

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseResponseCaching();
        
        app.UseCors("CorsRule");
        
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}