using System;
using System.Collections.Generic;

namespace ClassToDataTable.Mapper
{
    internal class ValidDataTableDataTypes
    {
        private Dictionary<Type, bool> _validTypes = new Dictionary<Type, bool>();
        public ValidDataTableDataTypes()
        {
            AddValidTypes();
        }

    
        public bool IsValidType(Type theType)
        {
            if (_validTypes.ContainsKey(theType) == false)
            {
                return false;
            }

            return _validTypes[theType];
        }

        private void AddValidTypes()
        {
            _validTypes.Add(typeof(Boolean), true);
            _validTypes.Add(typeof(Boolean?), true);
            _validTypes.Add(typeof(Byte), true);
            _validTypes.Add(typeof(Byte?), true);
            _validTypes.Add(typeof(Char), true);
            _validTypes.Add(typeof(Char?), true);
            _validTypes.Add(typeof(DateTime), true);
            _validTypes.Add(typeof(DateTime?), true);
            _validTypes.Add(typeof(Decimal), true);
            _validTypes.Add(typeof(Decimal?), true);
            _validTypes.Add(typeof(Double), true);
            _validTypes.Add(typeof(Double?), true);
            _validTypes.Add(typeof(Guid), true);
            _validTypes.Add(typeof(Guid?), true);
            _validTypes.Add(typeof(Int16), true);
            _validTypes.Add(typeof(Int16?), true);
            _validTypes.Add(typeof(Int32), true);
            _validTypes.Add(typeof(Int32?), true);
            _validTypes.Add(typeof(Int64), true);
            _validTypes.Add(typeof(Int64?), true);
            _validTypes.Add(typeof(SByte), true);
            _validTypes.Add(typeof(SByte?), true);
            _validTypes.Add(typeof(Single), true);
            _validTypes.Add(typeof(Single?), true);
            _validTypes.Add(typeof(String), true);
            _validTypes.Add(typeof(TimeSpan), true);
            _validTypes.Add(typeof(TimeSpan?), true);
            _validTypes.Add(typeof(UInt16), true);
            _validTypes.Add(typeof(UInt16?), true);
            _validTypes.Add(typeof(UInt32), true);
            _validTypes.Add(typeof(UInt32?), true);
            _validTypes.Add(typeof(UInt64), true);
            _validTypes.Add(typeof(UInt64?), true);
            _validTypes.Add(typeof(byte[]), true);
        }

    }
}
