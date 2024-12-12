using MinimalAPIMoviez.Entities;

namespace MinimalAPIMoviez.Repositories
{
    public interface IGenresRepository
    {
        Task<int> Create(Genre genre);

        Task<List<Genre>> GetAll();
        Task<Genre?> GetbyID(int id);

        //Update
        // we use exist Interface to find out, if the Data exists
        Task<bool> Exist(int id);
        //If the Data exists then we use Update and it doesnt have any generic Type
        Task Update(Genre genre);
        Task Delete(int id);
        Task<List<int>> Exists(List<int> ids);
        Task<bool> Exists(int id, string name);
    }
}














//Task<int> Create(Genre genre);
//Task<List<Genre>> GetAll();
//Task<Genre?> GetByID(int id);