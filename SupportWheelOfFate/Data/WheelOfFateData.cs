using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SupportWheelOfFateWebApi.Data
{
    public class Person
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public override bool Equals(object obj)
        {
            var otherPerson = obj as Person;
            if (otherPerson == null) return false;
            return Id == otherPerson.Id && FirstName == otherPerson.FirstName && Surname == otherPerson.Surname;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class BAU
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int HalfOfTheDay { get; set; }
        public Person Person { get; set; }

        public override bool Equals(object obj)
        {
            var otherBAU = obj as BAU;
            if (otherBAU == null) return false;
            return Id == otherBAU.Id && Date == otherBAU.Date && HalfOfTheDay == otherBAU.HalfOfTheDay && Person.Equals(otherBAU.Person);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class BAUEqualityComparer : IEqualityComparer<BAU>
    {
        public bool Equals(BAU x, BAU y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(BAU obj)
        {
            return 0;
        }
    }

    public class PersonEqualityComparer : IEqualityComparer<Person>
    {
        public bool Equals(Person x, Person y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Person obj)
        {
            return 0;
        }
    }

}
