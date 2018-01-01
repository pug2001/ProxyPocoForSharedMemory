using System;
using System.Collections;
using System.Collections.Generic;
using DataStore.Models;

namespace DataStore.Stores
{
    public class PeopleStore : MarshalByRefObject,IEnumerable
    {
        private Dictionary<string, Dictionary<string, Person>> _store;

        public PeopleStore()
        {
            _store = new Dictionary<string, Dictionary<string, Person>>();
        }

        public Person GetPerson(string firstName, string lastName)
        {
            if (_store.ContainsKey(lastName))
            {
                var withLastName = _store[lastName];
                if (withLastName.ContainsKey(firstName))
                {
                    return withLastName[firstName];
                }
            }
            return null;
        }

        public void Add(Person person)
        {
            if (!_store.ContainsKey(person.LastName))
            {
                _store.Add(person.LastName,new Dictionary<string, Person>());
            }
            _store[person.LastName].Add(person.FirstName,person);
        }

        public IEnumerator GetEnumerator()
        {
            return _store.GetEnumerator();
        }
    }
}
