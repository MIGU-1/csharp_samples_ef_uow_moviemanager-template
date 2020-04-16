using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace MovieManager.Persistence
{
    internal class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (Category, int) GetCategorieWithMostTitles() => _dbContext.Categories
            .Select(c => new
            {
                Category = c,
                Count = c.Movies.Count()
            })
            .OrderByDescending(c => c.Count)
            .AsEnumerable()
            .Select(t => (t.Category, t.Count))
            .First();
        public List<(string, int, int)> GetCategoryStatistics1() => _dbContext.Categories
            .Select(c => new
            {
                CategoryName = c.CategoryName,
                MovieCount = c.Movies.Count(),
                DurationSum = c.Movies
                                .Select(m => m.Duration)
                                .Sum()

            })
            .AsEnumerable()
            .Select(t => (t.CategoryName, t.MovieCount, t.DurationSum))
            .OrderByDescending(t => t.CategoryName)
            .ToList();
        public List<(string, double)> GetCategoryStatistics2() => _dbContext.Categories
             .Select(c => new
             {
                 CategoryName = c.CategoryName,
                 DurationAvg = c.Movies
                                .Select(m => m.Duration)
                                .Average()
             })
            .AsEnumerable()
            .Select(t => (t.CategoryName, t.DurationAvg))
            .OrderByDescending(t => t.DurationAvg)
            .ToList();
    }
}