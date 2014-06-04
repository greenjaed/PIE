using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PIE
{
    //Defines an interface for converting between a value's binary and string form
    public interface ICrossConverter
    {
        //Converts a value's string representation to its binary one and returns the byte array
        Byte[] ToBytes(String Source);
        //Checks to see if the source string can be safely converted.
        //If it can, it returns true and result is the byte array
        //It it can not, it returns false and result is null
        Boolean ToBytes(String Source, out Byte[] result);
        //Converts a value's binary representation to its string one and returns the string
        String ToString(Byte[] Source, Int32 Index);
        //Checks to see if the byte array is valid (i.e. if it's long enough)
        //if it is, it returns true and result is the string
        //if not, it returns false and result is null
        Boolean ToString(Byte[] Source, Int32 Index, out String result);
    }

    //Converts a Double precision floating point between its binary and string representations
    public class DoubleCrossConverter : ICrossConverter
    {
        public Byte[] ToBytes(String Source)
        {
            return BitConverter.GetBytes(Double.Parse(Source));
        }

        public Boolean ToBytes(String Source, out Byte[] result)
        {
            Double test;
            if (Double.TryParse(Source, out test))
            {
                result = BitConverter.GetBytes(test);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public String ToString(Byte[] Source, Int32 Index)
        {
            return BitConverter.ToDouble(Source, Index).ToString();
        }

        public Boolean ToString(Byte[] Source, Int32 Index, out String result)
        {
            if (Source.Length + -1 + -Index >= 8)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts a Single precision floating point between its binary and string representations
    public class SingleCrossConverter : ICrossConverter
    {
        public byte[] ToBytes(string Source)
        {
            return BitConverter.GetBytes(Single.Parse(Source));
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            Single test;
            if (Single.TryParse(Source, out test))
            {
                result = BitConverter.GetBytes(test);
                return true;
            }
            else
            {
                result = null;
                return false;
            }

        }

        public string ToString(byte[] Source, int Index)
        {
            return BitConverter.ToSingle(Source, Index).ToString();
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= 4)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts a Half precision floating point between its binary and string representations
    public class HalfCrossConverter : ICrossConverter
    {
        public byte[] ToBytes(string Source)
        {
            return Half.GetBytes(Half.Parse(Source));
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            Half test;
            if (Half.TryParse(Source, out test))
            {
                result = Half.GetBytes(test);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public string ToString(byte[] Source, int Index)
        {
            return Half.ToHalf(BitConverter.ToUInt16(Source, Index)).ToString();
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= 2)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts an Unsigned integer of any length between its binary and string representations
    public class UIntCrossConverter : ICrossConverter
    {
        private UInt64 maxValue; //the maximum value the integer can be
        private int length; //the length of the integer in bytes
        //the following are for converting from string to integer
        private NumberStyles representation; //is either HexNumber or Integer
        private NumberFormatInfo numFormat; //stored for efficiency
        private String formatString; //either "G" or "X"

        public UIntCrossConverter(int length, bool hex)
        {
            //Internally, the int is processed as a UInt64, so an actual value of that length needs to be treated differently
            maxValue = length == 64 ? UInt64.MaxValue : (UInt64)(1 << length) - 1;
            this.length = length >> 3; //as length comes in as the number of bits, it's divided by 8
            if (hex)
            {
                representation = NumberStyles.HexNumber;
                formatString = "X" + (this.length * 2).ToString();
            }
            else
            {
                representation = NumberStyles.Integer;
                formatString = "G";
            }
            numFormat = new NumberFormatInfo();
        }

        public byte[] ToBytes(string Source)
        {
            byte[] longBytes = BitConverter.GetBytes(UInt64.Parse(Source, representation));
            //longBytes will have 8 bytes, whereas the specified integer length could be less
            //therefore, only length bytes will be returned
            //as of now, bytes is little-endian
            Byte[] bytes = new Byte[length];
            Array.Copy(longBytes, bytes, length);
            return bytes;
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            UInt64 test;
            if (UInt64.TryParse(Source, representation, numFormat, out test) && test <= maxValue)
            {
                result = ToBytes(Source);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public string ToString(byte[] Source, int Index)
        {
            //because source could be less than 8 bytes, it's copied into an array that is 8 bytes
            //as of now, source is assumed to be little-endian
            byte[] longBytes = new byte[8];
            UInt64 result;
            Array.Copy(Source, Index, longBytes, 0, length);
            result = BitConverter.ToUInt64(longBytes, 0);
            return result.ToString(formatString);
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= length)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts a Signed integer between its binary and string representations
    public class IntCrossConverter : ICrossConverter
    {
        private long maxValue; //the max value the int can be
        private long minValue; //the min value the int can be
        private long signBit; //the sign bit of the int
        private long signExtension; //when OR'd with the value, sign-extends the value
        private int length; //the length of the int in bytes

        public IntCrossConverter(int length)
        {
            signBit = 1 << (length - 1);
            //because the int is process internally as an Int64, 64 bit integers must be treated differently
            if (length == 64)
            {
                maxValue = long.MaxValue;
                signExtension = 0;
                minValue = long.MinValue;
            }
            else
            {
                maxValue = signBit - 1;
                signExtension = ~((1 << length) - 1);
                minValue = signBit | signExtension;
            }
            this.length = length >> 3;
        }

        public byte[] ToBytes(string Source)
        {
            byte[] longBytes = BitConverter.GetBytes(long.Parse(Source));
            //longBytes will have 8 bytes, whereas the specified integer length could be less
            //therefore, only length bytes will be returned
            //as of now, bytes is little-endian
            byte[] bytes = new byte[length];
            Array.Copy(longBytes, bytes, length);
            return bytes;
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            long test;
            if (long.TryParse(Source, out test) && test <= maxValue && test >= minValue)
            {
                result = ToBytes(Source);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public string ToString(byte[] Source, int Index)
        {
            //because source could be less than 8 bytes, it's copied into an array that is 8 bytes
            //as of now, source is assumed to be little-endian
            byte[] longBytes = new byte[8];
            long result;
            Array.Copy(Source, Index, longBytes, 0, length);
            result = BitConverter.ToInt64(longBytes, 0);
            //if result is positive when it should be negative, make it negative
            if (result > 0 && (result & signBit) != 0)
                result |= signExtension;
            return result.ToString();
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= length)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts a Byte between its binary and string representations
    public class ByteCrossConverter : ICrossConverter
    {
        //the following are for string processing only
        private NumberStyles representation;
        private NumberFormatInfo numFormat;
        private String formatString;

        public ByteCrossConverter(bool hex)
        {
            if (hex)
            {
                formatString = "X2";
                representation = NumberStyles.HexNumber;
            }
            else
            {
                formatString = "G";
                representation = NumberStyles.Integer;
            }
            numFormat = new NumberFormatInfo();
        }

        public byte[] ToBytes(string Source)
        {
            return new Byte[] { Byte.Parse(Source, representation) };
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            Byte test;
            if (Byte.TryParse(Source, representation, numFormat, out test))
            {
                result = new Byte[] { test };
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public string ToString(byte[] Source, int Index)
        {
            return Source[Index].ToString(formatString);
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length > Index)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts a Fixed point value between its binary and string representations
    public class FixedCrossConverter : ICrossConverter
    {
        private int length; //the fixed length in bytes
        private long signBit; //the sign bit
        private long signExtension; //the sign extension
        private Decimal scaleFactor; //the value used to scale the integer
        private Decimal maxValue; //the max value of the fixed value
        private Decimal minValue; //the min value of the float

        public FixedCrossConverter(int length, int fraction)
        {
            this.length = length >> 3; //length comes in as bits
            signBit = (long)1 << (length - 1);
            //fixed values are partially processed as longs; if it is 64 bits, no need for a sign extension
            signExtension = length == 64 ? 0 : ~((1 << length) - 1);
            scaleFactor = (ulong)1 << fraction;
            long tempMax = signBit - 1;
            long tempMin = signBit | signExtension;
            maxValue = (Decimal)tempMax / scaleFactor;
            minValue = (Decimal)tempMin / scaleFactor;
        }

        public byte[] ToBytes(string Source)
        {
            Decimal tempDecimal = Decimal.Round(Decimal.Parse(Source) * scaleFactor);
            long temp = (long)tempDecimal;
            Byte[] result = BitConverter.GetBytes(temp);
            Byte[] bytes = new Byte[length];
            Array.Copy(result, bytes, length);
            return bytes;
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            Decimal test;
            if (Decimal.TryParse(Source, out test) && test <= maxValue && test >= minValue)
            {
                result = ToBytes(Source);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public string ToString(byte[] Source, int Index)
        {
            Byte[] bytes = new Byte[8];
            Array.Copy(Source, Index, bytes, 0, length);
            Int64 temp = BitConverter.ToInt64(bytes, 0);
            if ((temp & signBit) != 0)
                temp |= signExtension;
            Decimal result = (Decimal)temp / scaleFactor;
            return result.ToString();
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= length)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts an Unsigned fixed point value between its binary and string representations
    public class UFixedCrossConverter : ICrossConverter
    {
        private int length; //the length in bytes
        private Decimal scaleFactor; //the scale factor
        private Decimal maxValue; //the max value

        public UFixedCrossConverter(int length, int fraction)
        {
            //you know the drill
            this.length = length >> 3;
            scaleFactor = (ulong)1 << fraction;
            ulong tempMax = length == 64 ? ulong.MaxValue : (ulong)(1 << length) - 1;
            maxValue = (Decimal)tempMax / scaleFactor;
        }

        public byte[] ToBytes(string Source)
        {
            Decimal tempDecimal = Decimal.Round(Decimal.Parse(Source) * scaleFactor);
            ulong temp = (ulong)tempDecimal;
            Byte[] result = BitConverter.GetBytes(temp);
            Byte[] bytes = new byte[length];
            Array.Copy(bytes, result, length);
            return bytes;
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            Decimal test;
            if (Decimal.TryParse(Source, out test) && test <= maxValue && test >= 0)
            {
                result = ToBytes(Source);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public string ToString(byte[] Source, int Index)
        {
            Byte[] bytes = new Byte[8];
            Array.Copy(Source, Index, bytes, 0, length);
            UInt64 temp = BitConverter.ToUInt64(bytes, 0);
            Decimal result = (Decimal)temp / scaleFactor;
            return result.ToString();
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= length)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //Converts a string between its binary and string representations
    //Currently, only ASCII strings supported
    public class StringCrossConverter : ICrossConverter
    {
        private int length;

        public StringCrossConverter(int length)
        {
            this.length = length;
        }

        public byte[] ToBytes(string Source)
        {
            return Encoding.ASCII.GetBytes(Source.ToCharArray());
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            result = ToBytes(Source);
            return true;
        }

        public string ToString(byte[] Source, int Index)
        {
            return Encoding.ASCII.GetString(Source, Index, length / 8);
        }

        public bool ToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= length)
            {
                result = ToString(Source, Index);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }

    //A group of methods that returns a class that implements the ICrossConverter interface
    public class CrossConverterSelector
    {
        //takes paramaters and will automatically generate the appropriate class
        public static ICrossConverter SelectConverter(DataInfo parameters)
        {
            ICrossConverter converter;
            switch (parameters.dataType)
            {
                case "Floating Point":
                    converter = SelectFloatCC(parameters.size);
                    break;
                case "Fixed Point":
                    converter = SelectFixedCC(parameters.size, parameters.fraction, parameters.intFormat);
                    break;
                case "Integer":
                    converter = SelectIntCC(parameters.size, parameters.intFormat);
                    break;
                case "String":
                    converter = new StringCrossConverter(parameters.size);
                    break;
                default:
                    converter = null;
                    break;
            }
            return converter;
        }

        //generates a FixedCrossConverter
        public static ICrossConverter SelectFixedCC(int length, int fraction, IntFormat format)
        {
            if (format == IntFormat.Signed)
                return new FixedCrossConverter(length, fraction);
            else
                return new UFixedCrossConverter(length, fraction);
        }

        //Generates a FloatCrossConverter
        public static ICrossConverter SelectFloatCC(int length)
        {
            switch (length)
            {
                case 16:
                    return new HalfCrossConverter();
                case 32:
                    return new SingleCrossConverter();
                case 64:
                    return new DoubleCrossConverter();
                default:
                    return null;
            }
        }

        //Generates an IntCrossConverter
        public static ICrossConverter SelectIntCC(int length, IntFormat format)
        {
            bool hex = format == IntFormat.Hex;
            if (length == 8 && format != IntFormat.Signed)
                return new ByteCrossConverter(hex);
            else if (format == IntFormat.Signed)
                return new IntCrossConverter(length);
            else
                return new UIntCrossConverter(length, hex);
        }
    }

}
