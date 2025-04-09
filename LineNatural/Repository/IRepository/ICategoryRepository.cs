using LineNatural.Entities;

namespace LineNatural.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task UpdateAsync(Category entity);

    }
}
