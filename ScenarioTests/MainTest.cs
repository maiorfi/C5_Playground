using System;
using System.Linq;
using Bogus;
using ScenarioTests.Entities;
using Xunit;
using Xunit.Abstractions;
using Person = ScenarioTests.Entities.Person;

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
        public void Test_Indexed_Objects_1()
        {
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
            var persons = new Indexed<Person>(
                new IndexMaker<Person, string>("name", p => p.FirstName),
                new IndexMaker<Person, int>("year", p => p.Date / 10000),
                new IndexMaker<Person, int>("day", p => p.Date % 100),
                new IndexMaker<Person, string>("month", p => months[p.Date / 100 % 100 - 1])
            );

            var fakerRandomizer = new Bogus.Randomizer();
            var fakerNameDataset = new Bogus.DataSets.Name("en");

            for (var i = 0; i < 1000000; i++)
            {
                persons.Add(new Person(
                    fakerNameDataset.FirstName(),
                    fakerNameDataset.LastName(),
                    fakerRandomizer.Int(1900, 2000) * 10000 + fakerRandomizer.Int(1, 12) * 100 + fakerRandomizer.Int(1, 31)));
            }

            _output.WriteLine("\nBorn in 1964:");

            if (persons["year"][1964].Any())
            {
                foreach (var p in persons["year"][1964])
                {
                    _output.WriteLine(p.ToString());
                }
            }
            else _output.WriteLine("None.");

            _output.WriteLine("\nBorn in June:");

            if (persons["month"]["Jun"].Any())
            {
                foreach (var p in persons["month"]["Jun"])
                {
                    _output.WriteLine(p.ToString());
                }
            }
            else _output.WriteLine("None.");

            _output.WriteLine("\nNamed John:");

            if (persons["name"]["John"].Any())
            {
                foreach (var p in persons["name"]["John"])
                {
                    _output.WriteLine(p.ToString());
                }
            }
            else _output.WriteLine("None.");
        }
    }
}