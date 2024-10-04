using EpicChain.SmartContract.Framework.Native;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Json : SmartContract
    {
        public static string Serialize(object obj)
        {
            return EssentialLib.JsonSerialize(obj);
        }

        public static object Deserialize(string json)
        {
            return EssentialLib.JsonDeserialize(json);
        }
    }
}
