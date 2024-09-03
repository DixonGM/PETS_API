using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    //public class PetController
    [Route("api/pet")]
    [ApiController]
    public class PetController : ControllerBase
    {

        private readonly ApplicationDBContext _context;
        public PetController(ApplicationDBContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var pets = await _context.Pets.ToListAsync();
            var petsDto = pets.Select(pets => pets.ToDto());
            return Ok(petsDto);
        }
    }
}