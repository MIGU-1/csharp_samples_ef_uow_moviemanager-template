using MovieManager.Core.Entities;

namespace MovieManager.Core.Contracts
{
    public interface IMovieRepository
    {
        void AddRange(Movie[] movies);
        Movie GetLongestMovie();
        int GetYearWithMostActionFilms();
    }
}
