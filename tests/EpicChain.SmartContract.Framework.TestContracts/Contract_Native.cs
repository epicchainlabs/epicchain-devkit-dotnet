using EpicChain.SmartContract.Framework.Native;
using System.ComponentModel;
using System.Numerics;

namespace EpicChain.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_Native : SmartContract
    {
        [DisplayName("EpicChain_Decimals")]
        public static int EpicChain_Decimals()
        {
            return EpicChain.Decimals;
        }

        [DisplayName("EpicChain_Transfer")]
        public static bool EpicChain_Transfer(UInt160 from, UInt160 to, BigInteger amount)
        {
            return EpicChain.Transfer(from, to, amount, null);
        }

        [DisplayName("EpicChain_BalanceOf")]
        public static BigInteger EpicChain_BalanceOf(UInt160 account)
        {
            return EpicChain.BalanceOf(account);
        }

        [DisplayName("EpicChain_GetAccountState")]
        public static object EpicChain_GetAccountState(UInt160 account)
        {
            return EpicChain.GetAccountState(account);
        }

        [DisplayName("EpicChain_GetEpicPulsePerBlock")]
        public static BigInteger EpicChain_GetEpicPulsePerBlock()
        {
            return EpicChain.GetEpicPulsePerBlock();
        }

        [DisplayName("EpicChain_UnclaimedEpicPulse")]
        public static BigInteger EpicChain_UnclaimedEpicPulse(UInt160 account, uint end)
        {
            return EpicChain.UnclaimedEpicPulse(account, end);
        }

        [DisplayName("EpicChain_RegisterCandidate")]
        public static bool EpicChain_RegisterCandidate(ECPoint pubkey)
        {
            return EpicChain.RegisterCandidate(pubkey);
        }

        [DisplayName("EpicChain_GetCandidates")]
        public static (ECPoint, BigInteger)[] EpicChain_GetCandidates()
        {
            return EpicChain.GetCandidates();
        }

        [DisplayName("EpicPulse_Decimals")]
        public static int EpicPulse_Decimals()
        {
            return EpicPulse.Decimals;
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
