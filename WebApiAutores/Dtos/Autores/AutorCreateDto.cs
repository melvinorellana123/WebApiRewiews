using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Dtos.Autores;

public class AutorCreateDto
{
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(70, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres")]
    public string Name { get; set; }
}