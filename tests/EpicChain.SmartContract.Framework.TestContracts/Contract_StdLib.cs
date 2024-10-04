using EpicChain.SmartContract.Framework.Native;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_EssentialLib : SmartContract
    {
        public static string base58CheckEncode(ByteString input)
        {
            return EssentialLib.Base58CheckEncode(input);
        }

        public static byte[] base58CheckDecode(string input)
        {
            return (byte[])EssentialLib.Base58CheckDecode(input);
        }

        public static byte[] base64Decode(string input)
        {
            return (byte[])EssentialLib.Base64Decode(input);
        }

        public static string base64Encode(byte[] input)
        {
            return EssentialLib.Base64Encode((ByteString)input);
        }

        public static byte[] base58Decode(string input)
        {
            return (byte[])EssentialLib.Base58Decode(input);
        }

        public static string base58Encode(byte[] input)
        {
            return EssentialLib.Base58Encode((ByteString)input);
        }

        public static BigInteger atoi(string value, int @base)
        {
            return EssentialLib.Atoi(value, @base);
        }

        public static string itoa(int value, int @base)
        {
            return EssentialLib.Itoa(value, @base);
        }

        public static int memoryCompare(ByteString str1, ByteString str2)
        {
            return EssentialLib.MemoryCompare(str1, str2);
        }

        public static int memorySearch1(ByteString mem, ByteString value)
        {
            return EssentialLib.MemorySearch(mem, value);
        }

        public static int memorySearch2(ByteString mem, ByteString value, int start)
        {
            return EssentialLib.MemorySearch(mem, value, start);
        }

        public static int memorySearch3(ByteString mem, ByteString value, int start, bool backward)
        {
            return EssentialLib.MemorySearch(mem, value, start, backward);
        }

        public static string[] stringSplit1(string str, string separator)
        {
            return EssentialLib.StringSplit(str, separator);
        }

        public static string[] stringSplit2(string str, string separator, bool removeEmptyEntries)
        {
            return EssentialLib.StringSplit(str, separator, removeEmptyEntries);
        }
    }
}
