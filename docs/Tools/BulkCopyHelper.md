# Bulk Copy Helper

This is basically a wrapper around [Microsoft's SqlBulkCopy](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlbulkcopy(v=vs.110).aspx.), which internally uses ClassToDataTableService to convert the class into a DataTable.  If your intent is to use the ClassToDataTable library to bulk import data into a database, BulkCopyHelper may be a tool that you can use to simplify your work. 

There are two classes used with Bulk Copy Helper
- BulkCopyHelper<T> - The class that does all the work
- BulkCopyHelper - A class with a few static method used to dynamically create a BulkCopyHelper<T> when the type can only be determined at runtime.  I've imported a CSV file with type, namespace and assembly information in it and instantiated BulkCopyHelper<T> with BulkCopyHelper.  

Here is a basic example of using BulkCopyHelper<T> directly:

```c#
const int batchSize = 10000;
const int bulkCopyTimeoutInSeconds = 120;
string myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;

using (SqlConnection destinationConnection = new SqlConnection(myConnectionString))
using (IBulkCopyHelper copyHelper = new BulkCopyHelper<Person>())
{  
   destinationConnection.Open(); 
   copyHelper.Initialize(destinationConnection, "dbo", "Person", batchSize, bulkCopyTimeoutInSeconds);

   for(int i =0; i< 10500; i++)
   {
      // After 10K are added, which is the batch size, it will automatically insert the records 
      copyHelper.AddRow(new Person());
   }

   // Insert the remaining 500 to the database
   copyHelper.Flush();
}
```     

Warnings
- SQL Bulk Copy is VERY picky about field names.  They must not only match in type, but also in CASE.  Yes, it is case sensitive.  

