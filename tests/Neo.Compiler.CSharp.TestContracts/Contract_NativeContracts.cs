using Chain.SmartContract.Framework;
using Chain.SmartContract.Framework.Native;

namespace Chain.Compiler.CSharp.TestContracts
{
    public class Contract_NativeContracts : SmartContract.Framework.SmartContract
    {
        public static uint OracleMinimumResponseFee()
        {
            return Oracle.MinimumResponseFee;
        }

        public static string NEOSymbol()
        {
            return Chain.Symbol;
        }

        public static string GASSymbol()
        {
            return GAS.Symbol;
        }

        public static ECPoint[] getOracleNodes()
        {
            return RoleManagement.GetDesignatedByRole(Role.Oracle, 0);
        }

        public static UInt160 NEOHash()
        {
            return Chain.Hash;
        }


        public static UInt160 LedgerHash()
        {
            return Ledger.Hash;
        }


        public static UInt256 LedgerCurrentHash()
        {
            return Ledger.CurrentHash;
        }

        public static uint LedgerCurrentIndex()
        {
            return Ledger.CurrentIndex;
        }

    }
}
