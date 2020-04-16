using MovieManager.Core;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using MovieManager.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTableExt;

namespace MovieManager.ImportConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            InitData();
            AnalyzeData();

            Console.WriteLine();
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static void InitData()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("          Import");
            Console.WriteLine("***************************");

            Console.WriteLine("Import der Movies und Categories in die Datenbank");
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                Console.WriteLine("Datenbank löschen");
                unitOfWork.DeleteDatabase();
                Console.WriteLine("Datenbank migrieren");
                unitOfWork.MigrateDatabase();
                Console.WriteLine("Movies/Categories werden eingelesen");

                var movies = ImportController.ReadFromCsv().ToArray();
                if (movies.Length == 0)
                {
                    Console.WriteLine("!!! Es wurden keine Movies eingelesen");
                    return;
                }

                var categories = movies
                    .Select(m => m.Category)
                    .Distinct()
                    .OrderBy(c => c.CategoryName);

                Console.WriteLine($"  Es wurden {movies.Count()} Movies in {categories.Count()} Kategorien eingelesen!");

                unitOfWork.MovieRepository.AddRange(movies);

                unitOfWork.SaveChanges();
                Console.WriteLine();
            }
        }
        private static void AnalyzeData()
        {
            Console.WriteLine("***************************");
            Console.WriteLine("        Statistik");
            Console.WriteLine("***************************");


            using (IUnitOfWork uow = new UnitOfWork())
            {
                Movie longestMovie = uow.MovieRepository.GetLongestMovie();
                Console.WriteLine($"Längster Film = {longestMovie.Title}; Länge = {GetDurationAsString(longestMovie.Duration, false)} ");
                Console.WriteLine();

                (Category, int) categoriesWithCount = uow.CategoryRepository.GetCategorieWithMostTitles();
                Console.WriteLine($"Kategorie mit den meisten Filmen = '{categoriesWithCount.Item1.CategoryName}'; Filme = {categoriesWithCount.Item2}");
                Console.WriteLine();

                int yearOfActionFilms = uow.MovieRepository.GetYearWithMostActionFilms();
                Console.WriteLine($"Jahr der Actionfilme = {yearOfActionFilms}");
                Console.WriteLine();

                List<(string, int, int)> statistic1 = uow.CategoryRepository.GetCategoryStatistics1();
                ConsoleTableBuilder
                    .From(statistic1
                    .Select(o => new object[]
                    {
                        o.Item1,
                        o.Item2,
                        GetDurationAsString(o.Item3, false)
                    })
                    .ToList())
                    .WithColumn(
                        "Kategorie",
                        "Anzahl",
                        "Gesamtdauer"
                    )
                    .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                    .WithOptions(new ConsoleTableBuilderOption { DividerString = "" })
                    .ExportAndWrite();

                Console.WriteLine();
                Console.WriteLine();

                List<(string, double)> statistic2 = uow.CategoryRepository.GetCategoryStatistics2();
                ConsoleTableBuilder
                    .From(statistic2
                    .Select(o => new object[]
                    {
                        o.Item1,
                        GetDurationAsString(o.Item2, true)
                    })
                    .ToList())
                    .WithColumn(
                        "Kategorie",
                        "durchschnittliche Gesamtdauer"
                    )
                    .WithFormat(ConsoleTableBuilderFormat.MarkDown)
                    .WithOptions(new ConsoleTableBuilderOption { DividerString = "" })
                    .ExportAndWrite();
            }
        }
        private static string GetDurationAsString(double minutes, bool withSeconds = true)
        {
            TimeSpan timespan = TimeSpan.FromMinutes(minutes);
            int hr = timespan.Hours + timespan.Days * 24;
            int min = timespan.Minutes;
            int sec = timespan.Seconds;

            return withSeconds ? $"{hr}h {min}min {sec}sec" : $"{hr} h {min} min";
        }
    }
}
