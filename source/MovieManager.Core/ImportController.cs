using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace MovieManager.Core
{
    public class ImportController
    {
        const string Filename = "movies.csv";
        private static List<Category> categories = new List<Category>();
        const int TITLE_IDX = 0;
        const int YEAR_IDX = 1;
        const int CATEGORY_IDX = 2;
        const int DURATION_IDX = 3;

        /// <summary>
        /// Liefert die Movies mit den dazugehörigen Kategorien
        /// </summary>
        public static IEnumerable<Movie> ReadFromCsv()
        {
            var lines = MyFile.ReadStringMatrixFromCsv(Filename, true);
            ReadAllCategoriesFromLines(lines);
            return GetMoviesFromLines(lines);

        }
        private static void ReadAllCategoriesFromLines(string[][] lines)
        {
            foreach (string[] line in lines)
            {
                string category = line[CATEGORY_IDX];

                if (categories.Where(c => c.CategoryName == category).FirstOrDefault() == null)
                {
                    Category newCat = new Category() { CategoryName = category };
                    categories.Add(newCat);
                }

            }
        }
        private static IEnumerable<Movie> GetMoviesFromLines(string[][] lines)
        {
            List<Movie> movies = new List<Movie>();

            foreach (string[] line in lines)
            {
                string title = line[TITLE_IDX];
                int year = Convert.ToInt32(line[YEAR_IDX]);
                string category = line[CATEGORY_IDX];
                int duration = Convert.ToInt32(line[DURATION_IDX]);

                Category cat = categories.Where(c => c.CategoryName == category).FirstOrDefault();

                Movie newMovie = new Movie()
                {
                    Title = title,
                    Category = cat,
                    Duration = duration,
                    Year = year
                };

                cat.Movies.Add(newMovie);
                movies.Add(newMovie);
            }

            return movies;
        }
    }
}
