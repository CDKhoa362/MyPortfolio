namespace MyWebsite.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);

        // Create
        Task AddAsync(T entity);
        
        // Update
        Task UpdateAsync(T entity);

        // Delete
        Task DeleteAsync(string id);    

    }
}
