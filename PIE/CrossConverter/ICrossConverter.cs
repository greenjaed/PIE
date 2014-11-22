using System;
using System.Globalization;
using System.Text;

namespace PIE.CrossConverter
{
    //Defines an interface for converting between a value's binary and string form
    public interface ICrossConverter
    {
        //Converts a value's string representation to its binary one and returns the byte array
        Byte[] ToBytes(String Source);
        //Checks to see if the source string can be safely converted.
        //If it can, it returns true and result is the byte array
        //It it can not, it returns false and result is null
        Boolean TryToBytes(String Source, out Byte[] result);
        //Converts a value's binary representation to its string one and returns the string
        String ToString(Byte[] Source, Int32 Index);
        //Checks to see if the byte array is valid (i.e. if it's long enough)
        //if it is, it returns true and result is the string
        //if not, it returns false and result is null
        Boolean TryToString(Byte[] Source, Int32 Index, out String result);
    }

}
