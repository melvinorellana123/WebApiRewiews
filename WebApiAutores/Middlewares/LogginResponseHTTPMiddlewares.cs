namespace WebApiAutores.Middlewares;

public static class LoggingResponseHTTPMiddlewaresExtensions
{
    public static IApplicationBuilder UseLogginResponseHTTP(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LogginResponseHTTPMiddlewares>();
    }
}

public class LogginResponseHTTPMiddlewares
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LogginResponseHTTPMiddlewares> _logger;

    public LogginResponseHTTPMiddlewares(RequestDelegate next, ILogger<LogginResponseHTTPMiddlewares> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)//contexto es donde viene la peticion y siguiente es para que siga con la peticion
    {
        using (var ms = new MemoryStream())
        {
            var cuerpoOriginalRespuesta = context.Response.Body;
            context.Response.Body = ms;
            await _next(context);
                
            ms.Seek(0, SeekOrigin.Begin);
            string respuesta = new StreamReader(ms).ReadToEnd();
                
            ms.Seek(0, SeekOrigin.Begin);
            await ms.CopyToAsync(cuerpoOriginalRespuesta);
                
            context.Response.Body = cuerpoOriginalRespuesta;
            _logger.LogInformation(respuesta);
        }
    }
}