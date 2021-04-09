namespace ClassToDataTable
{
    /// <summary>Configuration</summary>
    public interface IClassToDataTableConfiguration
    {
        /// <summary>Should we ignore invalid types or throw an exception.</summary>
        bool IgnoreInvalidTypes { get; set; }
    }
}