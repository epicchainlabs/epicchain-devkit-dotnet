using EpicChain.SmartContract.Framework;
using EpicChain.SmartContract.Framework.Native;

namespace EpicChain.Compiler.CSharp.TestContracts
{
    public class Contract_NativeContracts : SmartContract.Framework.SmartContract
    {
        public static uint OracleMinimumResponseFee()
        {
            return Oracle.MinimumResponseFee;
        }

        public static string NEOSymbol()
        {
            return XPR.Symbol;
        }

        public static string GASSymbol()
        {
            return EpicPulse.Symbol;
        }

        public static ECPoint[] getOracleNodes()
        {
            return QuantumGuardNexus.GetDesignatedByRole(Role.Oracle, 0);
        }

        public static UInt160 NEOHash()
        {
            return EpicChain.Hash;
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
