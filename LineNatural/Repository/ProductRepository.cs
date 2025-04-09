using LineNatural.Context;
using LineNatural.Entities;
using LineNatural.Repository.IRepository;

namespace LineNatural.Repository
{
    public class ProductRepository : Repository<Producto>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task UpdateAsync(Producto entity)
        {
            _db.ProductoTbl.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
