using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Auth;

public class RegisterUserDto
{
    [Display(Name = "Correo ELectrónico")]
    [Required(ErrorMessage = "El {0} es requerido.")]
    [DataType(DataType.EmailAddress)]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", 
        ErrorMessage = "Ingrese un {0} válido.")]
    public string Email { get; set; }

    [Display(Name = "Contraseñá")]
    [Required(ErrorMessage = "La {0} es requerida.")]
    public string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; }
}