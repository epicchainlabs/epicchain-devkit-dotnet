using EpicChain.SmartContract.Framework;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_ConcatByteStringAddAssign : SmartContract.Framework.SmartContract
    {
        public static ByteString ByteStringAddAssign(ByteString a, ByteString b, string c)
        {
            ByteString result = ByteString.Empty;
            result += a;
            result += b;
            result += c;
            return result;
        }
    }
}
