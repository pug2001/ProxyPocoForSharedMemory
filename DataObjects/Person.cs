using System.Collections.Generic;

namespace DataStore.Models
{
    public enum Sex
    {
            Undefined,
            Male,
            Female
    }

    public class Person
    {
        public Person()
        {
        }

        public Person(Person father, Person mother)
        {
            Father = father;
            Mother = mother;
        }

        public Sex Sex { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MaidenName { get; set; }
        public bool Married { get; protected set; }
        public Person Partner { get; set; }
        public Person Mother { get; set; }
        public Person Father { get; set; }
        public ICollection<Person> Children { get; set; }

        public void Marry(Person partner)
        {
            partner.Partner = this;
            Partner = partner;
            partner.Married = true;
            Married = true;
            if (partner.Sex == Sex.Female)
            {
                partner.MaidenName = partner.LastName;
                partner.LastName = LastName;
            }
            else
            {
                MaidenName = LastName;
                LastName = partner.LastName;
            }
        }

        public void CreateChild()
        {
            
        }
    }
}
