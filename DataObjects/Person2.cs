using System;
using System.Collections.Generic;
using System.Text;
using DataStore.Models;

namespace DataObjects
{
    public class Person2 : Person
    {
        public new string FirstName { get; set; }
        public new string LastName { get; set; }
        public new Sex  Sex { get; set; }
        public new bool Married { get; set; }
    }
}
