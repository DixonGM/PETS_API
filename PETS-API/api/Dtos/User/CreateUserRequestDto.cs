using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.User
{
    public class CreateUserRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
         public List<CreatePetDto> Pets { get; set; }   //para poder crear un user y poderle asignar un pet
        
        
    }

    //para poder crear un user y poderle asignar un pet
     public class CreatePetDto
    {
        public string Name { get; set; }
        public string Animal { get; set; }
    }
}