﻿namespace WebApiAutores.Dtos;

public class BookDto
{
    public Guid Id { get; set; }
    public string ISBN { get; set; }
    public string Title { get; set; }
    public DateTime PublicationDate { get; set; }
    public int AutorId { get; set; }
    public string AutorNombre { get; set; }
}