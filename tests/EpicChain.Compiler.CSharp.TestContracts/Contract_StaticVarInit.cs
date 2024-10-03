using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_StaticVarInit : SmartContract.Framework.SmartContract
    {
        //define and static var and init it with a runtime code.
        static UInt160 callscript = Runtime.ExecutingScriptHash;

        public static UInt160 StaticInit()
        {
            return TestStaticInit();
        }

        public static UInt160 DirectGet()
        {
            return Runtime.ExecutingScriptHash;
        }

        static UInt160 TestStaticInit()
        {
            return callscript;
        }
    }
}
