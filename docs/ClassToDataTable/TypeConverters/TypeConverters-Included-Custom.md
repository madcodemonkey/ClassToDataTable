 # Creating a DataTable: Custom type converters included

There are one custom type converter included with this project that you can use:
- **PercentCtodTypeConverter** - Converts a string with a percentage sign in it into a decimal and then divides it by 100.  If no percentage symbol is found, it still tries to convert the string into a decimal and divide by 100.  It it cannot parse the string, it throws an exception.  Nulls are ignored.</summary>

To use them, decorate the class property with the CsvConverterCustomAttribute:

```c#
public class PercentageTestData
{
    [ClassToDataTableConverter(typeof(PercentCtodTypeConverter))]
    public string SomeTestProperty { get; set; }
    
    public int SomeIntProperty { get; set; }
}
```
