using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_Debug : SmartContract.Framework.SmartContract
    {
        public static int TestElse()
        {
#if DEBUG
            Runtime.Debug("Debug compilation");
            return 1;
#else
            return 2;
#endif
        }

        public static int TestIf()
        {
            int ret = 2;
#if DEBUG
            ret = 1;
#endif
            return ret;
        }
    }
}
