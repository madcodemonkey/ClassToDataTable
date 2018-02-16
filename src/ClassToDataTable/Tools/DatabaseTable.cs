namespace ClassToDataTable.Tools
{
    public class DatabaseTable
    {
        public string Schema { get; set; }
        public string TableName { get; set; }
        public override string ToString()
        {
            return $"{Schema}.{TableName}";
        }
    }
}
