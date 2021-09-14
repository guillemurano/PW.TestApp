using PW.TestApp.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PW.TestApp.Services
{
    public interface IPetService
    {
        /// <summary>
        /// Single or default Pet
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<Pet> SingleOrDefaultAsync(Func<Pet, bool> predicate);

        /// <summary>
        /// Get Pets
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Task<IList<Pet>> GetAsync(Func<Pet, bool> predicate = null);
        
        /// <summary>
        /// Add Pet
        /// </summary>
        /// <param name="pet"></param>
        /// <returns>Pet</returns>
        public Task<Pet> AddAsync(Pet pet);
    }
}
