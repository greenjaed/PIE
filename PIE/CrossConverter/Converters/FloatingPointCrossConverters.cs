using System;

namespace PIE
{

    //Converts a Double precision floating point between its binary and string representations
    public class DoubleCrossConverter : ICrossConverter
    {
        public Byte[] ToBytes(String Source)
        {
            return BitConverter.GetBytes(Double.Parse(Source));
        }

        public Boolean TryToBytes(String Source, out Byte[] result)
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

        public Boolean TryToString(Byte[] Source, Int32 Index, out String result)
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

        public bool TryToBytes(string Source, out byte[] result)
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

        public bool TryToString(byte[] Source, int Index, out string result)
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

        public bool TryToBytes(string Source, out byte[] result)
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

        public bool TryToString(byte[] Source, int Index, out string result)
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
}
