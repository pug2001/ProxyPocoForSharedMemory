using System;
using System.Collections.Generic;
using DataStore.Models;

namespace DataStoreNet4
{
    class Program
    {
        static void Main(string[] args)
        {
            var people = DataStore.DataStore.GetPeople(true);

            foreach (KeyValuePair<string,Dictionary<string,Person>> withSurname in people)
            {
                Console.WriteLine($"People with surname {withSurname.Key}");
                foreach (var person in withSurname.Value)
                {
                    Console.WriteLine($"    {person.Value.FirstName} {person.Value.LastName}");
                }
            }
        }
    }
}
