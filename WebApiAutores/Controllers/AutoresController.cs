using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Dtos;
using WebApiAutores.Dtos.Autores;
using WebApiAutores.Entities;
using WebApiAutores.Filters;

namespace WebApiAutores.Controllers
{
    [Route("api/autores")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AutoresController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AutorDto>>> Get()
        {
            var autoresDb = await _context.Autores.ToListAsync();
            var autoresDto = _mapper.Map<List<AutorDto>>(autoresDb);
            return Ok(new ResponseDto<IReadOnlyList<AutorDto>>
            {
                Status = true,
                Data = autoresDto
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto<AutorDto>>> GetOneById(int id)
        {
			var autorDb = await _context.Autores.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
            
            if (autorDb is null)
            {
                return NotFound(new ResponseDto<AutorDto>
                {
                    Status = false,
                    Message = $"No existe el autor con el id: {id}"
                });
            }
            
            var autorDto = _mapper.Map<AutorGetByIdDto>(autorDb);
            
            return Ok(new ResponseDto<AutorDto>
            {
                Status = true,
                Data = autorDto
            });
        }
        
        [HttpPost]
        public async Task<ActionResult<ResponseDto<AutorDto>>> Post(AutorCreateDto dto)
        {
            var autor = _mapper.Map<Autor>(dto);
            
            _context.Add(autor);
            await _context.SaveChangesAsync();
            
            var autorDto = _mapper.Map<AutorDto>(autor);
            
            return StatusCode(StatusCodes.Status201Created, new ResponseDto<AutorDto>
            {
                Status = true,
                Message = "El autor se creo con exito",
                Data = autorDto
            });// para que sepa que se creo el recurso
        }
        
        [HttpPut("{id:int}")] //para actualizar y como ocupamos el id, lo ponemos en la ruta
        public async Task<ActionResult<ResponseDto<AutorDto>>> Put(int id, AutorUpdateDto dto)
        {
            var autorDb = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autorDb is null)
            {
                return NotFound(new ResponseDto<AutorDto>
                {
                    Status = false,
                    Message = $"No existe el autor con el id: {id}"
                });
            }

            _mapper.Map<AutorUpdateDto, Autor>(dto, autorDb);  
            
            _context.Update(autorDb);
            await _context.SaveChangesAsync();
            
            var autorDto = _mapper.Map<AutorDto>(autorDb);
            
            return Ok(new ResponseDto<AutorDto>
            {
                Status = true,
                Message = "El autor se actualizo con exito",
                Data = autorDto
            });
        }
        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto<string>>> Delete(int id)
        {
            var autor = await _context.Autores.FirstOrDefaultAsync(a => a.Id == id);
            if (autor is null)
            {
                return NotFound(new ResponseDto<string>
                {
                    Status = false,
                    Message = $"No existe el autor con el id: {id}"
                });
            }
            
            _context.Remove(autor);
            await _context.SaveChangesAsync();
            
            return Ok(new ResponseDto<string>
            {
                Status = true,
                Message = "El autor se elimino con exito"
            });
        }
    }
}
