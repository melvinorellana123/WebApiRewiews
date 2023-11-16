﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Entities;

namespace WebApiAutores.Dtos.Autores;

public class AutorDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}