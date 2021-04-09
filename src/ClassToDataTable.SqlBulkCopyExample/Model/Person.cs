using System;
using System.Collections.Generic;

namespace SqlBulkCopyExample
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
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
                
                result.Add(newPerson);
            }

            return result;
        }
    }
}
