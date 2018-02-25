# Creating a DataTable

To create a DataTable from a List of T, there are 7 basic steps
1. Add the ClassToDataTable NuGet package to your project
2. Create a class
3. Optional: Attribute the class with the [ClassToDataTable] attribute if the DataTable column name should be different from the class property name.  You can also control the order the columns are written by using the ONE based Order property.
4. Instantiate the ClassToDataTableService class
5. Add ClassToDataTable namespace to your using statements.
6. Use the AddRow or AddRows methods to add class instances as rows to your data table.
7. Use the Table property on the ClassToDataTableService class to gain access to your DataTable 

## Examples
- [Simple Example 1](./Examples/Simple1.md) - In this simple example to show basic usage

## Advanced Topics
- [Type Converters](./TypeConverters/TypeConverters-Main.md) - Convert C# types into new types to stuff into the DataTable.  


## Helpful links
- [DataTable supported data types](https://msdn.microsoft.com/en-us/library/system.data.datacolumn.datatype(v=vs.110).aspx) - See Remarks section for the list of supported .NET data types.