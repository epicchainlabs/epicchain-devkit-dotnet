using EpicChain.Cryptography.ECC;
using EpicChain.SmartContract.Testing.Attributes;
using System.Numerics;

namespace EpicChain.SmartContract.Testing.Native.Models
{
    public class Candidate
    {
        /// <summary>
        /// Public key
        /// </summary>
        [FieldOrder(0)]
        public ECPoint? PublicKey { get; set; }

        /// <summary>
        /// Votes
        /// </summary>
        [FieldOrder(1)]
        public BigInteger Votes { get; set; }
    }
}
