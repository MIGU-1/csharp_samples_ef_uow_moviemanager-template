using MovieManager.Core.Entities;
using System.Collections.Generic;

namespace MovieManager.Core.Contracts
{
    public interface ICategoryRepository
    {
        (Category, int) GetCategorieWithMostTitles();
        List<(string, int, int)> GetCategoryStatistics1();
        List<(string, double)> GetCategoryStatistics2();
    }
}
