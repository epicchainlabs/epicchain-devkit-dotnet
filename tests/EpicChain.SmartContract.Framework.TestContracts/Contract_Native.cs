using EpicChain.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Native : SmartContract
    {
        [DisplayName("NEO_Decimals")]
        public static int NEO_Decimals()
        {
            return EpicChain.Decimals;
        }

        [DisplayName("NEO_Transfer")]
        public static bool NEO_Transfer(UInt160 from, UInt160 to, BigInteger amount)
        {
            return EpicChain.Transfer(from, to, amount, null);
        }

        [DisplayName("NEO_BalanceOf")]
        public static BigInteger NEO_BalanceOf(UInt160 account)
        {
            return EpicChain.BalanceOf(account);
        }

        [DisplayName("NEO_GetAccountState")]
        public static object NEO_GetAccountState(UInt160 account)
        {
            return EpicChain.GetAccountState(account);
        }

        [DisplayName("NEO_GetGasPerBlock")]
        public static BigInteger NEO_GetGasPerBlock()
        {
            return EpicChain.GetGasPerBlock();
        }

        [DisplayName("NEO_UnclaimedGas")]
        public static BigInteger NEO_UnclaimedGas(UInt160 account, uint end)
        {
            return EpicChain.UnclaimedGas(account, end);
        }

        [DisplayName("NEO_RegisterCandidate")]
        public static bool NEO_RegisterCandidate(ECPoint pubkey)
        {
            return EpicChain.RegisterCandidate(pubkey);
        }

        [DisplayName("NEO_GetCandidates")]
        public static (ECPoint, BigInteger)[] NEO_GetCandidates()
        {
            return EpicChain.GetCandidates();
        }

        [DisplayName("EpicPulse_Decimals")]
        public static int EpicPulse_Decimals()
        {
            return GAS.Decimals;
        }

        [DisplayName("Policy_GetFeePerByte")]
        public static long Policy_GetFeePerByte()
        {
            return Policy.GetFeePerByte();
        }

        [DisplayName("Policy_IsBlocked")]
        public static bool Policy_IsBlocked(UInt160 account)
        {
            return Policy.IsBlocked(account);
        }
    }
}
