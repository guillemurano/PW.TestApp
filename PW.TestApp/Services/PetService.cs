using PW.TestApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PW.TestApp.Services
{
    public class PetService : IPetService
    {
        private List<Pet> _pets = new List<Pet>
        {
            new Pet { Id = 1, Name = "Daisy" },
            new Pet { Id = 2, Name = "Max" },
            new Pet { Id = 3, Name = "Buddy" },
            new Pet { Id = 4, Name = "Lucy" }
        };

        /// <inheritdoc/>
        public async Task<Pet> AddAsync(Pet pet)
        {
            pet.Id = _pets.Last().Id + 1;
            _pets.Add(pet);

            return await Task.FromResult(pet);
        }

        /// <inheritdoc/>
        public async Task<IList<Pet>> GetAsync(Func<Pet, bool> predicate = null)
        {
            if (predicate != null)
            {
                return await Task.FromResult(_pets.Where(predicate).ToList());
            }

            return await Task.FromResult(_pets);
        }

        /// <inheritdoc/>
        public async Task<Pet> SingleOrDefaultAsync(Func<Pet, bool> predicate = null)
        {
            return await Task.FromResult(_pets.SingleOrDefault(predicate));
        }
    }
}
