using Chain.SmartContract.Framework.Native;
using Chain.SmartContract.Framework.Services;
using System;
using System.Numerics;
using Chain.SmartContract.Framework;

namespace Chain.Compiler.CSharp.TestContracts
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
