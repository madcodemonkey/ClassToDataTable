using System.Collections.Generic;
using System.Data;

namespace ClassToDataTable
{
    public interface IClassToDataTableService<T>
    {
        ClassToDataTableConfiguration Configuration { get; }
        int Count { get; }
        DataTable Table { get; set; }

        void AddRow(T source);
        void AddRows(IEnumerable<T> list);
        void Clear();
    }
}