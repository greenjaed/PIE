using System;

namespace PIE.CrossConverter
{

    //Converts a Fixed point value between its binary and string representations
    public class FixedCrossConverter : ICrossConverter
    {
        private int Length; //the fixed length in bytes
        private long SignBit; //the sign bit
        private long SignExtension; //the sign extension
        private Decimal ScaleFactor; //the value used to scale the integer
        private Decimal MaxValue; //the max value of the fixed value
        private Decimal MinValue; //the min value of the fixed value

        public FixedCrossConverter(int length, int fraction)
        {
            this.Length = length >> 3; //length comes in as bits
            SignBit = (long)1 << (length - 1);
            //fixed values are partially processed as longs; if it is 64 bits, no need for a sign extension
            SignExtension = length == sizeof(long) << 3 ? 0 : ~((1 << length) - 1);
            ScaleFactor = (ulong)1 << fraction;
            long maxValue = SignBit - 1;
            long minValue = SignBit | SignExtension;
            MaxValue = (Decimal)maxValue / ScaleFactor;
            MinValue = (Decimal)minValue / ScaleFactor;
        }

        public byte[] ToBytes(string Source)
        {
            Decimal decimalValue = Decimal.Round(Decimal.Parse(Source) * ScaleFactor);
            long longValue = (long)decimalValue;
            Byte[] longBytes = BitConverter.GetBytes(longValue);
            Byte[] fixedBytes = new Byte[Length];
            Array.Copy(longBytes, fixedBytes, Length);
            return fixedBytes;
        }

        public bool TryToBytes(string Source, out byte[] result)
        {
            Decimal test;
            if (Decimal.TryParse(Source, out test) && test <= MaxValue && test >= MinValue)
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
            Array.Copy(Source, Index, bytes, 0, Length);
            Int64 longValue = BitConverter.ToInt64(bytes, 0);
			if ((longValue & SignBit) != 0)
			{
				longValue |= SignExtension;
			}
            Decimal decimalValue = (Decimal)longValue / ScaleFactor;
            return decimalValue.ToString();
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

    //Converts an Unsigned fixed point value between its binary and string representations
    public class UFixedCrossConverter : ICrossConverter
    {
        private int Length; //the length in bytes
        private Decimal ScaleFactor; //the scale factor
        private Decimal MaxValue; //the max value

        public UFixedCrossConverter(int length, int fraction)
        {
            this.Length = length >> 3;
            ScaleFactor = (ulong)1 << fraction;
            ulong maxValue = length == sizeof(ulong) << 3 ? ulong.MaxValue : (ulong)(1 << length) - 1;
            MaxValue = (Decimal)maxValue / ScaleFactor;
        }

        public byte[] ToBytes(string Source)
        {
            Decimal decimalValue = Decimal.Round(Decimal.Parse(Source) * ScaleFactor);
            ulong ulongValue = (ulong)decimalValue;
            Byte[] ulongBytes = BitConverter.GetBytes(ulongValue);
            Byte[] fixedBytes = new byte[Length];
            Array.Copy(fixedBytes, ulongBytes, Length);
            return fixedBytes;
        }

        public bool TryToBytes(string Source, out byte[] result)
        {
            Decimal test;
            if (Decimal.TryParse(Source, out test) && test <= MaxValue && test >= 0)
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
            Array.Copy(Source, Index, bytes, 0, Length);
            UInt64 ulongValue = BitConverter.ToUInt64(bytes, 0);
            Decimal decimalValue = (Decimal)ulongValue / ScaleFactor;
            return decimalValue.ToString();
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

}
