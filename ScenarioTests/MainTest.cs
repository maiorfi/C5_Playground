using System;
using System.Diagnostics;
using System.Linq;
using Bogus;
using ScenarioTests.C5_Extensions;
using Xunit;
using Xunit.Abstractions;
using Person = ScenarioTests.Sample_Entities.Person;

namespace ScenarioTests
{
    public class MainTester
    {
        private readonly ITestOutputHelper _output;

        public MainTester(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_Indexed_Persons_Find_By_Index()
        {
            const int ITEMS_COUNT = 1000000;

            const string INDEX_NAME_YEAR = "year";
            const string INDEX_NAME_MONTH = "month";
            const string INDEX_NAME_DAY = "day";
            const string INDEX_NAME_NAME = "name";
            // Lookup
            string[] months =
            {
                "Jan",
                "Feb",
                "Mar",
                "Apr",
                "May",
                "Jun",
                "Jul",
                "Aug",
                "Sep",
                "Oct",
                "Nov",
                "Dec"
            };

            // definizione indici su entità Person
            var persons = new Index<Sample_Entities.Person>(
                new IndexMaker<Person, string>(INDEX_NAME_NAME, p => p.FirstName),
                new IndexMaker<Person, int>(INDEX_NAME_YEAR, p => p.Date / 10000),
                new IndexMaker<Person, int>(INDEX_NAME_DAY, p => p.Date % 100),
                new IndexMaker<Person, string>(INDEX_NAME_MONTH, p => months[p.Date / 100 % 100 - 1])
            );

            // Generiamo ITEMS_COUNT entità casuali
            var fakerRandomizer = new Bogus.Randomizer();
            var fakerNameDataset = new Bogus.DataSets.Name("en");

            for (var i = 0; i < ITEMS_COUNT; i++)
            {
                persons.Add(new Person(
                    fakerNameDataset.FirstName(),
                    fakerNameDataset.LastName(),
                    fakerRandomizer.Int(1900, 2000) * 10000 + fakerRandomizer.Int(1, 12) * 100 + fakerRandomizer.Int(1, 31)));
            }

            var sw=new Stopwatch();
            
            sw.Reset();

            // Ricerca su indice year

            const int YEAR_TO_SEARCH_FOR = 1971;
            
            if (persons[INDEX_NAME_YEAR][YEAR_TO_SEARCH_FOR].Any())
            {
                _output.WriteLine($"Indice '{INDEX_NAME_YEAR}', ricerca su valore {YEAR_TO_SEARCH_FOR}, trovati {persons[INDEX_NAME_YEAR][YEAR_TO_SEARCH_FOR].Count} item");
            }
            else _output.WriteLine("Nessuno.");
            
            _output.WriteLine($"Tempo impiegato: {sw.ElapsedMilliseconds} msec\n");
            
            sw.Restart();
            
            // Ricerca su indice month

            const string MONTH_TO_SEARCH_FOR = "Nov";
            
            if (persons[INDEX_NAME_MONTH][MONTH_TO_SEARCH_FOR].Any())
            {
                _output.WriteLine($"Indice '{INDEX_NAME_MONTH}', ricerca su valore {MONTH_TO_SEARCH_FOR}, trovati {persons[INDEX_NAME_MONTH][MONTH_TO_SEARCH_FOR].Count} item");
            }
            else _output.WriteLine("Nessuno.");
            
            _output.WriteLine($"Tempo impiegato: {sw.ElapsedMilliseconds} msec\n");
            
            // Ricerca su indice day
            
            sw.Restart();

            const int DAY_TO_SEARCH_FOR = 17;
            
            if (persons[INDEX_NAME_DAY][DAY_TO_SEARCH_FOR].Any())
            {
                _output.WriteLine($"Indice '{INDEX_NAME_DAY}', ricerca su valore {DAY_TO_SEARCH_FOR}, trovati {persons[INDEX_NAME_DAY][DAY_TO_SEARCH_FOR].Count} item");
            }
            else _output.WriteLine("Nessuno.");
            
            _output.WriteLine($"Tempo impiegato: {sw.ElapsedMilliseconds} msec\n");

            // Ricerca su indice name
            
            sw.Restart();
            
            const string NAME_TO_SEARCH_FOR = "Lawrence";
            
            if (persons[INDEX_NAME_NAME][NAME_TO_SEARCH_FOR].Any())
            {
                _output.WriteLine($"Indice '{INDEX_NAME_NAME}', ricerca su valore {NAME_TO_SEARCH_FOR}, trovati {persons[INDEX_NAME_NAME][NAME_TO_SEARCH_FOR].Count} item");
            }
            else _output.WriteLine("Nessuno.");
            
            _output.WriteLine($"Tempo impiegato: {sw.ElapsedMilliseconds} msec\n");
        }

        [Fact]
        public void Test_Indexed_Persons_Sorted_Paging_By_Index()
        {
            const int ITEMS_COUNT = 100;

            const string INDEX_NAME_NAME = "name";

            // definizione indice su entità Person
            var persons = new Index<Person>(
                new IndexMaker<Person, string>(INDEX_NAME_NAME, p => p.FirstName)
            );

            // Generiamo ITEMS_COUNT entità casuali
            var fakerRandomizer = new Bogus.Randomizer();
            var fakerNameDataset = new Bogus.DataSets.Name("en");

            for (var i = 0; i < ITEMS_COUNT; i++)
            {
                persons.Add(new Person(
                    fakerNameDataset.FirstName(),
                    fakerNameDataset.LastName(),
                    fakerRandomizer.Int(1900, 2000) * 10000 + fakerRandomizer.Int(1, 12) * 100 + fakerRandomizer.Int(1, 31)));
            }

            // Enumeriamo gli ultimi 10 item ordinati desc per l'indice creato
            const int PAGE_SIZE = ITEMS_COUNT / 10;
            
            _output.WriteLine($"Ultimi {PAGE_SIZE} item in ordine inverso:\n");
            
            var lastPageByName = persons[INDEX_NAME_NAME].AsEnumerable().Skip(ITEMS_COUNT-PAGE_SIZE).Take(PAGE_SIZE).Reverse();
            
            foreach (var p in lastPageByName)
            {
                _output.WriteLine($"{p.FirstName}");
            }
        }
    }
}