using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PIE;

namespace PIETestProject
{
    [TestClass]
    public class IntConverterUnitTest
    {
        //simple test to make sure conversion from byte to string and back works
        [TestMethod]
        public void TestByteCC()
        {
            Byte[] test = new Byte[] { 0x7f };
            Byte[] result;
            String temp;
            ICrossConverter byteConverter = CrossConverterFactory.SelectIntCC(8, IntFormat.Hex);
            temp = byteConverter.ToString(test, 0);
            Assert.AreEqual("7F", temp);
            result = byteConverter.ToBytes(temp);
            Assert.AreEqual(test[0], result[0]);
        }

        //test if a Byte is correctly converted to an SByte
        [TestMethod]
        public void TestSByte1()
        {
            Byte[] test = new Byte[] { 0x9c };
            String temp;
            ICrossConverter sbyteConverter = CrossConverterFactory.SelectIntCC(8, IntFormat.Signed);
            temp = sbyteConverter.ToString(test, 0);
            Assert.AreEqual("-100", temp);
        }

        //tests if the converter rejects values out of bounds
        [TestMethod]
        public void TestSByte2()
        {
            String invalid = "128";
            Byte[] result;
            ICrossConverter sbyteConverter = CrossConverterFactory.SelectIntCC(8, IntFormat.Signed);
            Assert.AreNotEqual(true, sbyteConverter.TryToBytes(invalid, out result));
            invalid = "-129";
            Assert.AreNotEqual(true, sbyteConverter.TryToBytes(invalid, out result));
        }

        //tests if the min and max values are correctly set
        [TestMethod]
        public void TestSByte3()
        {
            String valid = "127";
            Byte[] result;
            ICrossConverter sbyteConverter = CrossConverterFactory.SelectIntCC(8, IntFormat.Signed);
            Assert.AreEqual(true, sbyteConverter.TryToBytes(valid, out result));
            valid = "-128";
            sbyteConverter.TryToBytes(valid, out result);
            Assert.AreEqual(0x80, result[0]);
        }

        //general test to see if the converter works properly
        [TestMethod]
        public void TestFixed1()
        {
            Byte[] testVal = new Byte[] { 13 };
            Byte[] resultFinal;
            String result;
            ICrossConverter fixedConverter = CrossConverterFactory.SelectFixedCC(8, 1, IntFormat.Signed);
            result = fixedConverter.ToString(testVal, 0);
            Assert.AreEqual("6.5", result);
            resultFinal = fixedConverter.ToBytes(result);
            Assert.AreEqual(testVal[0], resultFinal[0]);
        }

        //test to make sure max and min values are properly set
        [TestMethod]
        public void TestFixed2()
        {
            Byte[] testVal = new byte[] { 8 };
            Byte[] endResult;
            String result;
            ICrossConverter fixedConverter = CrossConverterFactory.SelectFixedCC(8, 4, IntFormat.Signed);
            Assert.AreNotEqual(true, fixedConverter.TryToString(testVal, 0, out result));
            Assert.AreEqual(true, fixedConverter.TryToBytes("-8", out endResult));
        }

        //tests for proper detection and conversion of negative numbers
        [TestMethod]
        public void TestFixed3()
        {
            Byte[] test = new byte[] { 0xc0 };
            ICrossConverter fixedConverter = CrossConverterFactory.SelectFixedCC(8, 7, IntFormat.Signed);
            Assert.AreEqual("-0.5", fixedConverter.ToString(test, 0));
        }
    }
}
