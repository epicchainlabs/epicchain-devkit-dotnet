using EpicChain.SmartContract.Framework.Attributes;
using EpicChain.SmartContract.Framework.Services;
using EpicChain.SmartContract.Framework.Native;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class OwnerOnlyAttribute : ModifierAttribute
    {
        UInt160 owner;

        public OwnerOnlyAttribute(string hex)
        {
            owner = (UInt160)(byte[])EssentialLib.Base64Decode(hex);
        }

        public override void Enter()
        {
            if (!Runtime.CheckWitness(owner)) throw new System.Exception();
        }

        public override void Exit() { }
    }

    public class Contract_Attribute : SmartContract
    {
        [OwnerOnly("AAAAAAAAAAAAAAAAAAAAAAAAAAA=")]
        public static bool test()
        {
            return true;
        }

        [NoReentrant]
        public void reentrantB()
        {
            // do nothing
        }

        [NoReentrant]
        public void reentrantA()
        {
            reentrantB();
        }

        [NoReentrantMethod]
        public void reentrantTest(int value)
        {
            if (value == 0) return;
            if (value == 123)
            {
                reentrantTest(0);
            }
        }
    }
}
