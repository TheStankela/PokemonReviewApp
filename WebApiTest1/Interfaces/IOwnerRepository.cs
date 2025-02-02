﻿using WebApiTest1.Models;

namespace WebApiTest1.Interfaces
{
    public interface IOwnerRepository
    {
        public ICollection<Owner> GetOwners();
        public Owner GetOwners(int id);
        public List<Pokemon> GetPokemonsByOwner(int ownerId);
        public bool OwnerExists(int id);

        public bool CreateOwner(Owner owner);
        public bool Save();
        public bool DeleteOwner(Owner owner);
        public bool UpdateOwner(Owner owner);
    }
}
