using Chain.Cryptography.ECC;
using Chain.SmartContract.Testing.Attributes;
using System.Numerics;

namespace Chain.SmartContract.Testing.Native.Models
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
