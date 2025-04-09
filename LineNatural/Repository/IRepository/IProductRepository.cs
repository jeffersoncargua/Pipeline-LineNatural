using LineNatural.Entities;

namespace LineNatural.Repository.IRepository
{
    public interface IProductRepository : IRepository<Producto>
    {
        Task UpdateAsync(Producto entity);
        
    }
}
