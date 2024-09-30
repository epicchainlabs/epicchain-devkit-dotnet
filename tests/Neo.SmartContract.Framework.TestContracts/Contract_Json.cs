using EpicChain.SmartContract.Framework.Native;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Json : SmartContract
    {
        public static string Serialize(object obj)
        {
            return StdLib.JsonSerialize(obj);
        }

        public static object Deserialize(string json)
        {
            return StdLib.JsonDeserialize(json);
        }
    }
}
