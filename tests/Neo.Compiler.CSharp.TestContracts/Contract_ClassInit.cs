using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;
using System;
using System.Numerics;
using EpicChain.SmartContract.Framework;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public struct IntInit
    {
        public int A;
        public BigInteger B;
    }

    public class Contract_ClassInit : SmartContract.Framework.SmartContract
    {
        public static IntInit testInitInt()
        {
            return new IntInit();
        }
    }
}
