using System;
using System.Text;

namespace PIE
{
    //Converts a string between its binary and string representations
    //Currently, only ASCII strings supported
    public class StringCrossConverter : ICrossConverter
    {
        private int length;

        public StringCrossConverter(int length)
        {
            this.length = length / 8;
        }

        public byte[] ToBytes(string Source)
        {
            return Encoding.ASCII.GetBytes(Source.ToCharArray());
        }

        public bool TryToBytes(string Source, out byte[] result)
        {
			if (Source == null)
			{
				result = new byte[length];
			}
            else
            {
                result = ToBytes(Source);
                if (Source.Length != length)
                {
                    byte[] temp = new byte[length];
                    Array.Copy(result, temp, Math.Min(result.Length, temp.Length));
                    result = temp;
                }
            }
            return true;
        }

        public string ToString(byte[] Source, int Index)
        {
            return Encoding.ASCII.GetString(Source, Index, length);
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
