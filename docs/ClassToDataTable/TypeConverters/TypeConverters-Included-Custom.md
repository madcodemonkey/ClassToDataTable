 # Creating a DataTable: Custom type converters included

There are two custom type converters included with this project that you can use:
- **PercentCtodTypeConverter** - Converts a string with a percentage sign in it into a decimal and then divides it by 100.  If no percentage symbol is found, it still tries to convert the string into a decimal and divide by 100.  It it cannot parse the string, it throws an exception.  Nulls are ignored.</summary>
- **ChangeTypeCtodTypeConverter** - Runs the value encountered into the System.Convert.ChangeType method where the type is the TargetPropertyType from the attribute. If null is found, null is returned.

# PercentCtodTypeConverter
---
To use them, decorate the class property with the ClassToDataTableConverter:

```c#
public class PercentageTestData
{
    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter))]
    public string SomeTestProperty { get; set; }
    
    public int SomeIntProperty { get; set; }
}
```

# ChangeTypeCtodTypeConverter
---
To use it, decorate a class property with the ClassToDataTableConverter:

```c#
    public enum PersonGender : Int16 { Female = 1, Male = 2,  Unspecified = 3 }

    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        [ClassToDataTableConverter(typeof(ChangeTypeCtodTypeConverter), TargetPropertyType = typeof(int))]
        public PersonGender? Gender { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? Birthday { get; set; }
    }
}
```

Notes
- **Warning!**  ChangeTypeCtodTypeConverter requires that you specify a TargetPropertyType!!