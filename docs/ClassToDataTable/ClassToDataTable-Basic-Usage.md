# ClassToDataTableService - Basic usage

```c#
var service = new ClassToDataTableService<Person>();

// Create a bunch of people
Random rand = new Random(DateTime.Now.Second);
int numberToCreate = rand.Next(10, 100);

for (int i = 0; i < numberToCreate; i++)
{
	var newPerson = new Person()
	{
		FirstName = $"First{rand.Next(1, 5000)}",
		LastName = $"Last{rand.Next(1, 5000)}",
		Age = rand.Next(5, 80),
		PercentageBodyFat = rand.Next(1, 20) / 1.2m,
		AvgHeartRate = rand.Next(60, 80) / 1.1
	};

    // Add person!!
    // Add person!!
    // Add person!!
	service.AddRow(newPerson);
}

// TODO: Do something with the table:  service.Table
```
