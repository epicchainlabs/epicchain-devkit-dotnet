using System;

namespace EpicChain.SmartContract.Testing.Interpreters
{
    public interface IStringInterpreter
    {
        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Value</returns>
        public string GetString(ReadOnlySpan<byte> bytes);
    }
}
