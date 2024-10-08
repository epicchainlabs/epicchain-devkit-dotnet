using System;
using EpicChain.SmartContract.Framework;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_Instance : SmartContract.Framework.SmartContract
    {
        public int init = 0;

        public Contract_Instance()
        {
            init++;
        }

        public int sum(int a)
        {
            return a + init;
        }

        public int sum2(int a)
        {
            return sum(a) + sum(a);
        }
    }
}
