﻿using WebApiTest1.Data;
using WebApiTest1.Interfaces;
using WebApiTest1.Models;

namespace WebApiTest1.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly DataContext _dataContext;

        public CategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public bool CategoryExists(int id)
        {
            return _dataContext.Categories.Any(c => c.Id == id);
        }

        public bool CreateCategory(Category category)
        {
            _dataContext.Add(category);
            return Save();
        }

        public ICollection<Category> GetCategories()
        {
            return _dataContext.Categories.ToList();
        }

        public Category GetCategory(int id)
        {
            return _dataContext.Categories.Where(c => c.Id == id).FirstOrDefault();
        }

        public Category GetCategoryByPokemon(int id)
        {
            return _dataContext.PokemonCategories.Where(c => c.PokemonId == id).Select(c => c.Category).FirstOrDefault();
        }

        public ICollection<Pokemon> GetPokemonsByCategory(int id)
        {
            return _dataContext.PokemonCategories.Where(p => p.CategoryId == id).Select(p => p.Pokemon).ToList();
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateCategory(Category category)
        {
            _dataContext.Update(category);
            return Save();
        }
        public bool DeleteCategory(Category category)
        {
            _dataContext.Remove(category); 
            
           return Save();
        }

    }
}
