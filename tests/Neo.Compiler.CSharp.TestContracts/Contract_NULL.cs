using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_NULL : SmartContract.Framework.SmartContract
    {
        public static bool IsNull(object? value)
        {
            return value is null;
        }

        public static bool EqualNullA(object? value)
        {
            return null == value;
        }

        public static bool EqualNullB(object? value)
        {
            return value == null;
        }

        public static bool EqualNotNullA(object? value)
        {
            return null != value;
        }

        public static bool EqualNotNullB(object? value)
        {
            return value != null;
        }

        public static string? NullCoalescing(string code)
        {
            string? myname = code?.Substring(1, 2);
            return myname;
        }

        public static string NullCollation(string code)
        {
            string myname = code ?? "linux";
            return myname;
        }

        public static bool NullPropertyGT(string a)
        {
            return a?.Length > 0;
        }

        public static bool NullPropertyLT(string a)
        {
            return a?.Length < 0;
        }

        public static bool NullPropertyGE(string a)
        {
            return a?.Length >= 0;
        }

        public static bool NullPropertyLE(string a)
        {
            return a?.Length <= 0;
        }

        public static bool NullProperty(string a)
        {
            return a?.Length != 0;
        }

        public static bool IfNull(object obj)
        {
            if ((bool)obj)
            {
                return true;
            }

            return false;
        }

        public static object NullCollationAndCollation(string code)
        {
            var context = Storage.CurrentContext;
            return Storage.Get(context, code) ?? (ByteString)new byte[] { 123 };
        }

        public static object NullCollationAndCollation2(string code)
        {
            var context = Storage.CurrentContext;
            Storage.Put(context, code, "111");
            return Storage.Get(context, code) ?? (ByteString)new byte[] { 123 };
        }

        class TestClass
        {
            public void VoidMethod()
            {
            }
            public int IntProperty => 42;
        }

        public static void NullType()
        {
            TestClass? obj1 = null;
            obj1?.VoidMethod();
        }
    }
}
