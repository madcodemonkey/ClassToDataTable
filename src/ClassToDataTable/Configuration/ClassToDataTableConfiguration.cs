namespace ClassToDataTable
{
    public class ClassToDataTableConfiguration
    {
        /// <summary>If a property does NOT have a converter and the type is not a valid type for a DataTable, just ignore it rather than  throwing an exception.</summary>
        public bool IgnoreInvalidTypes { get; set; }
    }
}
