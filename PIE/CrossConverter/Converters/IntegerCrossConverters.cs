using System;
using System.Globalization;

namespace PIE.CrossConverter
{

    //Converts an Unsigned integer between its binary and string representations
    public class UIntCrossConverter : ICrossConverter
    {
        private UInt64 MaxValue; //the maximum value the integer can be
        private int Length; //the length of the integer in bytes
        //the following are for converting from string to integer
        private NumberStyles Representation; //is either HexNumber or Integer
        private NumberFormatInfo NumberFormat; //stored for efficiency
        private String FormatString; //either "G" or "X"

        public UIntCrossConverter(int length, bool hex)
        {
            //Internally, the int is processed as a UInt64, so an actual value of that length needs to be treated differently
            MaxValue = length == sizeof(ulong) << 3 ? UInt64.MaxValue : (UInt64)(1 << length) - 1;
            this.Length = length >> 3; //as length comes in as the number of bits, it's divided by 8
            if (hex)
            {
                Representation = NumberStyles.HexNumber;
                FormatString = "X" + (this.Length * 2).ToString();
            }
            else
            {
                Representation = NumberStyles.Integer;
                FormatString = "G";
            }
            NumberFormat = new NumberFormatInfo();
        }

        public byte[] ToBytes(string Source)
        {
            byte[] longBytes = BitConverter.GetBytes(UInt64.Parse(Source, Representation));
            //longBytes will have 8 bytes, whereas the specified integer length could be less
            //therefore, only length bytes will be returned
            //as of now, bytes is little-endian
            Byte[] uintBytes = new Byte[Length];
            Array.Copy(longBytes, uintBytes, Length);
            return uintBytes;
        }

        public bool TryToBytes(string Source, out byte[] result)
        {
            UInt64 test;
            if (UInt64.TryParse(Source, Representation, NumberFormat, out test) && test <= MaxValue)
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
            UInt64 longValue;
            Array.Copy(Source, Index, longBytes, 0, Length);
            longValue = BitConverter.ToUInt64(longBytes, 0);
            return longValue.ToString(FormatString);
        }

        public bool TryToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= Length)
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
        private long MaxValue; //the max value the int can be
        private long MinValue; //the min value the int can be
        private long SignBit; //the sign bit of the int
        private long SignExtension; //when OR'd with the value, sign-extends the value
        private int Length; //the length of the int in bytes

        public IntCrossConverter(int length)
        {
            SignBit = 1 << (length - 1);
            //because the int is process internally as an Int64, 64 bit integers must be treated differently
            if (length == sizeof(long) << 3)
            {
                MaxValue = long.MaxValue;
                SignExtension = 0;
                MinValue = long.MinValue;
            }
            else
            {
                MaxValue = SignBit - 1;
                SignExtension = ~((1 << length) - 1);
                MinValue = SignBit | SignExtension;
            }
            this.Length = length >> 3;
        }

        public byte[] ToBytes(string Source)
        {
            byte[] longBytes = BitConverter.GetBytes(long.Parse(Source));
            //longBytes will have 8 bytes, whereas the specified integer length could be less
            //therefore, only length bytes will be returned
            //as of now, bytes is little-endian
            byte[] intBytes = new byte[Length];
            Array.Copy(longBytes, intBytes, Length);
            return intBytes;
        }

        public bool TryToBytes(string Source, out byte[] result)
        {
            long test;
            if (long.TryParse(Source, out test) && test <= MaxValue && test >= MinValue)
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
            long longValue;
            Array.Copy(Source, Index, longBytes, 0, Length);
            longValue = BitConverter.ToInt64(longBytes, 0);
            //if result is positive when it should be negative, make it negative
			if (longValue > 0 && (longValue & SignBit) != 0)
			{
				longValue |= SignExtension;
			}
            return longValue.ToString();
        }

        public bool TryToString(byte[] Source, int Index, out string result)
        {
            if (Source.Length + -1 + -Index >= Length)
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
        private NumberStyles Representation;
        private NumberFormatInfo NumberFormat;
        private String FormatString;

        public ByteCrossConverter(bool hex)
        {
            if (hex)
            {
                FormatString = "X2";
                Representation = NumberStyles.HexNumber;
            }
            else
            {
                FormatString = "G";
                Representation = NumberStyles.Integer;
            }
            NumberFormat = new NumberFormatInfo();
        }

        public byte[] ToBytes(string Source)
        {
            return new Byte[] { Byte.Parse(Source, Representation) };
        }

        public bool TryToBytes(string Source, out byte[] result)
        {
            Byte test;
            if (Byte.TryParse(Source, Representation, NumberFormat, out test))
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
            return Source[Index].ToString(FormatString);
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
