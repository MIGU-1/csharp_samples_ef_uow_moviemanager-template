using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System.Linq;

namespace MovieManager.Persistence
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MovieRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void AddRange(Movie[] movie) => _dbContext.Movies
            .AddRange(movie);

        public Movie GetLongestMovie() => _dbContext.Movies
            .OrderByDescending(m => m.Duration)
            .First();

        public int GetYearWithMostActionFilms() => _dbContext.Movies
            .Where(m => m.Category.CategoryName == "Action")
            .GroupBy(m => m.Year)
            .Select(obj => new
            {
                Year = obj.Key,
                Count = obj.Count(),
            })
            .OrderByDescending(obj => obj.Count)
            .Select(obj => obj.Year)
            .First();

    }
}