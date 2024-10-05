namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_Optimize : SmartContract.Framework.SmartContract
    {
        private static string privateMethod()
        {
            return "EpicChain";
        }

        public static byte[] unitTest_001()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
            return nb;
        }

        public static void testVoid()
        {
            var nb = new byte[] { 1, 2, 3, 4 };
        }

        public static byte[] testArgs1(byte a)
        {
            var nb = new byte[] { 1, 2, 3, 3 };
            nb[3] = a;
            return nb;
        }

        public static object testArgs2(byte[] a)
        {
            return a;
        }
    }
}
