using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Autores;
using WebApiAutores.Entities;

namespace WebApiAutores.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        MapsForBooks();
        MapsForAutores();
    }

    private void MapsForBooks()
    {
        //CreateMap<BookDto, Book>().ReverseMap();
        CreateMap<Book, BookDto>().ForPath(dest => dest.AutorNombre, opt => opt.MapFrom(src => src.Autor.Name));
        
        CreateMap<BookCreateDto, Book>();
    }
    
    private void MapsForAutores()
    {
        CreateMap<Autor, AutorDto>();
        CreateMap<AutorCreateDto, Autor>();
        CreateMap<Autor, AutorCreateDto>();
        CreateMap<Autor, AutorGetByIdDto>();
    }
}