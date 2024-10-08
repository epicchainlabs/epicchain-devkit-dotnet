using System;
using System.Text;

namespace EpicChain.SmartContract.Testing.Interpreters
{
    public class StringInterpreter : IStringInterpreter
    {
        /// <summary>
        /// Encoding
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// A strict UTF8 encoding used in EpicChain system.
        /// </summary>
        public static readonly StringInterpreter StrictUTF8 = new(Encoding.UTF8, true);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strict">Strict</param>
        public StringInterpreter(Encoding encoding, bool strict)
        {
            if (strict)
            {
                Encoding = (Encoding)Encoding.UTF8.Clone();
                Encoding.DecoderFallback = DecoderFallback.ExceptionFallback;
                Encoding.EncoderFallback = EncoderFallback.ExceptionFallback;
            }
            else
            {
                Encoding = encoding;
            }
        }

        /// <summary>
        /// Get string from bytes
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Value</returns>
        public virtual string GetString(ReadOnlySpan<byte> bytes)
        {
            return Encoding.GetString(bytes);
        }
    }
}
