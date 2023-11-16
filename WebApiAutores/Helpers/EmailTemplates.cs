using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApiAutores.Helpers;

public class EmailTemplates
{
    public static string LoginTemplate(string email)
    {
        return $@"
            <h1>¡Bienvenido!</h1>
            <p>Has iniciado sesión con el correo {email}</p>
            <p>Fecha y hora de inicio de sesión: {DateTime.Now.ToString("MM/dd/yyyy")}</p>
            <p>¡Gracias por usar nuestra aplicación!</p>
            ";
    }
}