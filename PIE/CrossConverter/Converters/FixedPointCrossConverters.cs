using System;

namespace PIE
{

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

        public bool TryToBytes(string Source, out byte[] result)
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

    //Converts an Unsigned fixed point value between its binary and string representations
    public class UFixedCrossConverter : ICrossConverter
    {
        private int length; //the length in bytes
        private Decimal scaleFactor; //the scale factor
        private Decimal maxValue; //the max value

        public UFixedCrossConverter(int length, int fraction)
        {
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

        public bool TryToBytes(string Source, out byte[] result)
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

}
