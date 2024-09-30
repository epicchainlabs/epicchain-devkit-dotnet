using Chain.SmartContract.Framework.Attributes;

namespace Chain.Compiler.CSharp.TestContracts
{
    public class Contract_ABISafe : SmartContract.Framework.SmartContract
    {
        public static int UnitTest_001()
        {
            return 1;
        }

        [Safe]
        public static int UnitTest_002()
        {
            return 2;
        }

        public static int UnitTest_003()
        {
            return 3;
        }
    }
}
