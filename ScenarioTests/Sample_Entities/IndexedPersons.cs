using ScenarioTests.C5_Extensions;

namespace ScenarioTests.Sample_Entities
{
// Sample class with two fields but many possible indexes
    public class Person
    {
        public string FirstName { get; }
        public string LastName { get; }
        public int Date { get; } // YYYYMMDD as in 20070725

        public Person(string fname, string lname, int date)
        {
            FirstName = fname;
            LastName = lname;
            Date = date;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Date})";
        }
    }

    // The interface of a strongly typed indexing of PersonIndexed objects:
    // (Not yet used)
    public interface IPersonIndexes
    {
        IIndex<string, Person> Name { get; }
        IIndex<int, Person> Year { get; }
        IIndex<int, Person> Day { get; }
        IIndex<string, Person> Month { get; }
    }
}