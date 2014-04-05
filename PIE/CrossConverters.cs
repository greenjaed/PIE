using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Half;

namespace PIE
{
    public interface ICrossConverter
    {
        public Byte[] ToBytes(String Source);
        public Boolean ToBytes(String Source, out Byte[] result);
        public String ToString(Byte[] Source, Int32 Index);
        public Boolean ToString(Byte[] Source, Int32 Index, out String result);
    }

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

    public class UIntCrossconverter : ICrossConverter
    {
        private UInt64 maxValue;
        private int length;
        private Byte[] bytes;
        private NumberStyles representation;
        private NumberFormatInfo numFormat;
        private String formatString;

        public UIntCrossconverter(int length, bool hex)
        {
            maxValue = length == 64 ? UInt64.MaxValue : (UInt64)(1 << length) - 1;
            this.length = length >> 3;
            bytes = new Byte[this.length];
            representation = hex ? NumberStyles.HexNumber : NumberStyles.Integer;
            numFormat = new NumberFormatInfo();
            formatString = hex ? "X" : "G";
        }

        public byte[] ToBytes(string Source)
        {
            byte[] longBytes = BitConverter.GetBytes(UInt64.Parse(Source, representation));
            Array.Copy(longBytes, 0, bytes, 0, length);
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

    public class IntCrossconverter : ICrossConverter
    {
        private long maxValue;
        private long minValue;
        private long signBit;
        private int length;
        private Byte[] bytes;
        private NumberStyles representation;
        private NumberFormatInfo numFormat;
        private String formatString;

        public IntCrossconverter(int length, bool hex)
        {
            signBit = 1 << (length - 1);
            maxValue = length == 64 ? long.MaxValue : signBit - 1;
            minValue = length == 64 ? long.MinValue : signBit;
            this.length = length >> 3;
            bytes = new Byte[this.length];
            representation = hex ? NumberStyles.HexNumber : NumberStyles.Integer;
            numFormat = new NumberFormatInfo();
            formatString = hex ? "X" : "G";
        }

        public byte[] ToBytes(string Source)
        {
            byte[] longBytes = BitConverter.GetBytes(long.Parse(Source, representation));
            Array.Copy(longBytes, 0, bytes, 0, length);
            return bytes;
        }

        public bool ToBytes(string Source, out byte[] result)
        {
            long test;
            if (long.TryParse(Source, representation, numFormat, out test) && Math.Abs(test) <= maxValue || test == minValue)
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
            byte[] longBytes = new byte[8];
            long result;
            Array.Copy(Source, Index, longBytes, 0, length);
            result = BitConverter.ToInt64(longBytes, 0);
            if (result > 0 && (result & signBit) != 0)
                result *= -1;
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

    public class ByteCrossConverter : ICrossConverter
    {
        private NumberStyles representation;
        private NumberFormatInfo numFormat;
        private String formatString;

        public ByteCrossConverter(bool hex)
        {
            formatString = hex ? "X" : "G";
            representation = hex ? NumberStyles.HexNumber : NumberStyles.Integer;
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
}
