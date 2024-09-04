
using api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Dtos.User;
using api.Mappers;

namespace api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll(){
            var users = await _context.Users.Include(user => user.pets).ToListAsync();
            var usersDto = users.Select(users => users.ToDto());
            return Ok(usersDto);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> getById([FromRoute] int id){

            var user =  await _context.Users.Include(user => user.pets).FirstOrDefaultAsync(u => u.id == id);
            if(user == null){
                return NotFound();
            }
            return Ok(user.ToDto());
        }

        //create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequestDto userDto){
            var userModel = userDto.ToUserFromCreateDto();
            await _context.Users.AddAsync(userModel);
            await _context.SaveChangesAsync();  //necesita este save changes para agregarlo a la base de datos, porque solo con add no lo agrega
            return CreatedAtAction(nameof(getById), new { id = userModel.id}, userModel.ToDto());  //retorna el dto para no mostrar el id por seguridad
        }


        
        //editar
        [HttpPut]
        [Route("{id}")]
       public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserRequestDto userDto){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.id == id);
            if (userModel == null){
                return NotFound();
            }
            userModel.Age = userDto.Age;
            userModel.FirstName = userDto.FirstName;
            userModel.LastName = userDto.LastName;

            await _context.SaveChangesAsync();   //sin esto no me edita en base de datos.

            return Ok(userModel.ToDto());
        }


        //delete
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id){
            var userModel = await _context.Users.FirstOrDefaultAsync(user => user.id == id);
            if (userModel == null){
                return NotFound();
            }
            _context.Users.Remove(userModel);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpPost("{userId}/assign-pet/{petId}")]
        public async Task<IActionResult> AssignPet([FromRoute] int userId, [FromRoute] int petId)
        {
           
            var user = await _context.Users.Include(u => u.pets).FirstOrDefaultAsync(u => u.id == userId);
            var pet = await _context.Pets.FirstOrDefaultAsync(p => p.id == petId);

            // Verifico si el usuario y la mascota existen
            if (user == null)
            {
                return NotFound($"Usuario con ID {userId} no encontrado.");
            }

            if (pet == null)
            {
                return NotFound($"Mascota con ID {petId} no encontrada.");
            }

            // Verifico si la mascota ya esta asignada al usuario
            if (user.pets.Any(p => p.id == petId))
            {
                return BadRequest($"La mascota con ID {petId} ya está asignada al usuario con ID {userId}.");
            }

            // Asigno la mascota al usuario
            user.pets.Add(pet);
            

            pet.UserId = userId;

            
            await _context.SaveChangesAsync();

            
            var userName = $"{user.FirstName} {user.LastName}";
            var petName = pet.Name;
            var petAnimal = pet.Animal;

            
            return Ok(new { message = $"Nombre de Mascota '{petName}' Animal '{petAnimal}' asignada al usuario '{userName}' correctamente." });
        }



    }
}