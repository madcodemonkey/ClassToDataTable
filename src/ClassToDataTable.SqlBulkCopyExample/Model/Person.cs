using System;
using System.Collections.Generic;
using ClassToDataTable;
using ClassToDataTable.TypeConverters;

namespace SqlBulkCopyExample
{
    public enum PersonGender : Int16 { Female = 1, Male = 2,  Unspecified = 3 }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        [ClassToDataTableConverter(typeof(ChangeTypeCtodTypeConverter), TargetPropertyType = typeof(Int16?))]
        public PersonGender? Gender { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? Birthday { get; set; }
        
        public static List<Person> CreatePeople(int numberToCreate)
        {
            var result = new List<Person>(numberToCreate);
            Random rand = new Random(DateTime.Now.Millisecond);

            for (int i = 1; i <= numberToCreate; i++)
            {
                var newPerson = new Person
                {
                    FirstName = $"First{i}",
                    LastName = $"Last{i}",
                    Age = rand.Next(5, 65),
                    CreateDate = DateTime.Now
                };

                bool hasBirthday = rand.Next(1, 100) > 50;
                newPerson.Birthday = hasBirthday ? new DateTime(DateTime.Now.Year - newPerson.Age, rand.Next(1, 12), rand.Next(1, 28)) : (DateTime?) null;
                
                bool genderSpecified = rand.Next(1, 100) > 50;
                newPerson.Gender = genderSpecified ? (PersonGender) rand.Next(1, 4) : (PersonGender?) null;

                result.Add(newPerson);
            }

            return result;
        }
    }
}
