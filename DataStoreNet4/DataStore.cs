using DataObjects;
using DataObjects.PocoProxyFactory;
using DataStore.Models;
using DataStore.Stores;
using DataStoreNet4;

namespace DataStore
{
    public static class DataStore
    {
        private static PeopleStore _people;

        public static PeopleStore GetPeople(bool useSharedStore = false)
        {
            if (_people == null)
            {
                _people = useSharedStore? SharedStoreFactory.Create<PeopleStore>(): new PeopleStore();
                PopulatePeople(_people);
            }
            return _people;
        }

        public static Person GetPerson(string firstName, string lastName)
        {
            var people = GetPeople();
            return people?.GetPerson(firstName,lastName);
        }

        public static void PopulatePeople(PeopleStore people)
        {
//            var a = new Person {FirstName = "Adam", LastName = "Godson", Sex = Sex.Male};
            var a = new Person2 { FirstName = "Adam", LastName = "Godson", Sex = Sex.Male };
            var adam = PocoProxyFactory<Person>.CreateProxyObject(a);
            var eve = new Person {FirstName = "Eve", LastName = "Goddaughter", Sex = Sex.Female};
            a.Marry(eve);
            people.Add(eve);
            people.Add(adam);
        }
    }
}
