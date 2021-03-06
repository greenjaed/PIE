﻿namespace PIE.CrossConverter
{
    //A group of methods that returns a class that implements the ICrossConverter interface
    public class CrossConverterFactory
    {
        //takes paramaters and will automatically generate the appropriate class
        public static ICrossConverter SelectConverter(ColumnDescriptor parameters)
        {
            ICrossConverter converter;
            switch (parameters.TypeOfData)
            {
                case DataType.FloatingPoint:
                    converter = SelectFloatCC(parameters.Size);
                    break;
                case DataType.FixedPoint:
                    converter = SelectFixedCC(parameters.Size, parameters.Fraction, parameters.IntFormat);
                    break;
                case DataType.Integer:
                    converter = SelectIntCC(parameters.Size, parameters.IntFormat);
                    break;
                case DataType.CharString:
                    converter = new StringCrossConverter(parameters.Size);
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
