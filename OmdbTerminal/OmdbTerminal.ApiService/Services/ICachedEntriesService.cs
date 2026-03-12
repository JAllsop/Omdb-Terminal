using OmdbTerminal.ApiService.Data;

namespace OmdbTerminal.ApiService.Services
{
    public interface ICachedEntriesService
    {
        IQueryable<MovieEntity> GetQueryable();

        Task<MovieEntity?> GetByIdAsync(string id);

        Task<bool> CreateAsync(MovieEntity movie);

        Task<bool> UpdateAsync(string id, MovieEntity updatedMovie);

        Task<bool> DeleteAsync(string id);

        Task<int> ClearCacheAsync();
    }
}
