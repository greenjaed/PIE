using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace PIE
{
    //stores information about the data in  a table column
    [DataContract]
    public class ColumnDescriptor
    {
        //The column name
        [DataMember]
        public string Name { get; set; }
        //The data type
        [DataMember]
        public DataType TypeOfData { get; set; }
        //The size of the data type, in bytes
        [DataMember]
        public int Size { get; set; }
        //for ints: how the int is formatted
        [DataMember]
        public IntFormat IntFormat { get; set; }
        //for fixed: the size of the fraction, in bits
        [DataMember]
        public int Fraction { get; set; }
        [DataMember]
        public string DefaultValue { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum IntFormat
    {
        None,
        Signed,
        Hex
    }

    public enum DataType
    {
        Integer,
        FloatingPoint,
        FixedPoint,
        CharString
    }
    
}
