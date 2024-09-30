// Copyright (C) 2015-2022 The Neo Project.
//
// The EpicChain.SmartContract.Framework is open-source software made available under the MIT License.
// This permissive license allows anyone to freely use, modify, and distribute both the source code
// and binary forms of the software, either with or without modifications, provided that the conditions
// specified in the license are met. For more information about the MIT License, you can refer to the
// LICENSE file located in the main directory of the project, or visit the following link:
// http://www.opensource.org/licenses/mit-license.php.
//
// The key permissions granted by the MIT License include the following:
// 1. The right to use the software for any purpose, including commercial applications.
// 2. The right to modify the source code to suit individual needs.
// 3. The right to distribute copies of the original or modified versions of the software.
//
// Redistribution and use in both source and binary forms are permitted, provided that the following
// conditions are met:
//
// 1. The original copyright notice and permission notice must be included in all copies or substantial
//    portions of the software, whether the distribution is of the unmodified source code or modified
//    versions.
// 2. The software is provided "as is," without any warranty of any kind, express or implied, including
//    but not limited to the warranties of merchantability, fitness for a particular purpose, or
//    non-infringement. In no event shall the authors or copyright holders be liable for any claim,
//    damages, or other liabilities, whether in an action of contract, tort, or otherwise, arising from,
//    out of, or in connection with the software or the use or other dealings in the software.
//
// The MIT License is widely used for open-source projects because of its flexibility, encouraging both
// individual and corporate use of the licensed software without restrictive obligations. If you wish to
// learn more about this license or its implications for the project, please consult the official page
// provided above.


using System;
using System.Runtime.CompilerServices;
using EpicChain.SmartContract.Framework.Services;

namespace EpicChain.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Global no Reentrancy protection. This no reentrant attribute by default take as a key the method name
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NoReentrantMethodAttribute : ModifierAttribute
    {
        private readonly StorageMap _context;
        private readonly string _key;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">Storage prefix</param>
        /// <param name="key">Storage key (the method name as default)</param>
        public NoReentrantMethodAttribute(byte prefix = 0xFF, [CallerMemberName] string key = "noReentrant")
        {
            _key = key;
            _context = new(Storage.CurrentContext, prefix);
        }

        public override void Enter()
        {
            var data = _context.Get(_key);
            ExecutionEngine.Assert(data == null, "Already entered");
            _context.Put(_key, 1);
        }

        public override void Exit()
        {
            _context.Delete(_key);
        }
    }
}
