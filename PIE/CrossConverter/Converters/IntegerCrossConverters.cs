using System;
using System.Globalization;

namespace PIE
{

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

        public bool TryToBytes(string Source, out byte[] result)
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

        public bool TryToString(byte[] Source, int Index, out string result)
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

        public bool TryToBytes(string Source, out byte[] result)
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

        public bool TryToString(byte[] Source, int Index, out string result)
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

        public bool TryToBytes(string Source, out byte[] result)
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

        public bool TryToString(byte[] Source, int Index, out string result)
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
}
