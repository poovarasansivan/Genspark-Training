namespace BankingAPI.Interfaces
{
    public interface IRepository<K, T> where T : class
    {
        Task<T> AddAsync(T item);
        Task<T> UpdateAsync(T item);
        Task<T> DeleteAsync(K id);
        Task<T> GetByIdAsync(K id);
        Task<ICollection<T>> GetAllAsync();
    }

}