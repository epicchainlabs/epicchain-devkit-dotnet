using EpicChain.SmartContract.Framework.Native;
using EpicChain.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_PostfixUnary : SmartContract.Framework.SmartContract
    {
        public class Person
        {
            public string Name { get; set; }

            public int Age;

            public int[] BWH = { 80, 60, 80 };

            public Person(string name) { Name = name; }
        }

        public static string? Test()
        {
            Person? p = new("John");
            if (IsValid(p))
            {
                p.Age++;
                p.BWH[1]++;

                return p!.Name;
            }

            return null;
        }

        public static bool IsValid(Person? person) => person is not null && person.Name is not null;
    }
}
