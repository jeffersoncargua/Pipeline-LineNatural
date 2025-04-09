using LineNatural.Context;
using LineNatural.Entities;
using LineNatural.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LineNatural.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Category entity)
        {
            _db.CategoryTbl.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
