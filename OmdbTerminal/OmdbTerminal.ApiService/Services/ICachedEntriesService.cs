using OmdbTerminal.ApiService.Data;

namespace OmdbTerminal.ApiService.Services
{
    public interface ICachedEntriesService
    {
        IQueryable<MovieEntity> GetQueryable();

        Task<MovieEntity?> GetByIdAsync(string id);

        Task<List<MovieEntity>> GetByIdsAsync(IEnumerable<string> ids);

        Task<bool> CreateAsync(MovieEntity movie);

        Task<bool> UpdateAsync(string id, MovieEntity updatedMovie);

        Task<bool> DeleteAsync(string id);

        Task<SearchCacheEntity?> GetSearchCacheAsync(string query, int page);

        Task<bool> SaveSearchCacheAsync(SearchCacheEntity searchCache, IEnumerable<MovieEntity> moviesFromSearch);

        Task<int> ClearCacheAsync();
    }
}
