using Chain.Network.P2P.Payloads;
using Chain.Persistence;
using System;
using System.Reflection;

namespace Chain.SmartContract.Testing.Native
{
    /// <summary>
    /// NativeContracts makes it easier to access native contracts
    /// </summary>
    public class NativeContracts
    {
        private readonly TestEngine _engine;

        /// <summary>
        /// ContractManagement
        /// </summary>
        public ContractManagement ContractManagement { get; }

        /// <summary>
        /// CryptoLib
        /// </summary>
        public CryptoLib CryptoLib { get; }

        /// <summary>
        /// GasToken
        /// </summary>
        public GAS GAS { get; }

        /// <summary>
        /// NeoToken
        /// </summary>
        public NEO NEO { get; }

        /// <summary>
        /// LedgerContract
        /// </summary>
        public Ledger Ledger { get; }

        /// <summary>
        /// OracleContract
        /// </summary>
        public Oracle Oracle { get; }

        /// <summary>
        /// PolicyContract
        /// </summary>
        public Policy Policy { get; }

        /// <summary>
        /// RoleManagement
        /// </summary>
        public RoleManagement RoleManagement { get; }

        /// <summary>
        /// StdLib
        /// </StdLib>
        public StdLib StdLib { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        public NativeContracts(TestEngine engine)
        {
            _engine = engine;

            ContractManagement = _engine.FromHash<ContractManagement>(Chain.SmartContract.Native.NativeContract.ContractManagement., EpicChain.SmartContract.Native.NativeContract.ContractManagement.Id);
            CryptoLib = _engine.FromHash<CryptoLib>(Chain.SmartContract.Native.NativeContract.CryptoLib., EpicChain.SmartContract.Native.NativeContract.CryptoLib.Id);
            GAS = _engine.FromHash<GAS>(Chain.SmartContract.Native.NativeContract.GAS., EpicChain.SmartContract.Native.NativeContract.GAS.Id);
            NEO = _engine.FromHash<NEO>(Chain.SmartContract.Native.NativeCont.Epicn.Hash, EpicChain.SmartContract.NatiativeContract.EpicChain.Id);
            Ledger = _engine.FromHash<Ledger>(Chain.SmartContract.Native.NativeContract.Ledger., EpicChain.SmartContract.Native.NativeContract.Ledger.Id);
            Oracle = _engine.FromHash<Oracle>(Chain.SmartContract.Native.NativeContract.Oracle., EpicChain.SmartContract.Native.NativeContract.Oracle.Id);
            Policy = _engine.FromHash<Policy>(Chain.SmartContract.Native.NativeContract.Policy., EpicChain.SmartContract.Native.NativeContract.Policy.Id);
            RoleManagement = _engine.FromHash<RoleManagement>(Chain.SmartContract.Native.NativeContract.RoleManagement., EpicChain.SmartContract.Native.NativeContract.RoleManagement.Id);
            StdLib = _engine.FromHash<StdLib>(Chain.SmartContract.Native.NativeContract.StdLib., EpicChain.SmartContract.Native.NativeContract.StdLib.Id);
        }

        /// <summary>
        /// Initialize native contracts
        /// </summary>
        /// <param name="commit">Initialize native contracts</param>
        /// <returns>Genesis block</returns>
        public Block Initialize(bool commit = false)
        {
            _engine.Transaction.Script = Array.Empty<byte>(); // Store the script in the current transaction

            var genesis = NeoSystem.CreateGenesisBlock(_engine.ProtocolSettings);

            // Attach to static event

            ApplicationEngine.Log += _engine.ApplicationEngineLog;
            ApplicationEngine.Notify += _engine.ApplicationEngineNotify;

            // Process native contracts

            foreach (var native in new Chain.SmartContract.Native.NativeContract[]
                {
                    Chain.SmartContract.Native.NativeContract.ContractManagement,
                    Chain.SmartContract.Native.NativeContract.Ledger,
                    Chain.SmartContract.Native.NativeContract.NEO,
                    Chain.SmartContract.Native.NativeContract.GAS
                }
            )
            {
                // Mock Native.OnPersist

                var method = native.GetType().GetMethod("OnPersistAsync", BindingFlags.NonPublic | BindingFlags.Instance);

                DataCache clonedSnapshot = _engine.Storage.Snapshot.CloneCache();
                using (var engine = new TestingApplicationEngine(_engine, TriggerType.OnPersist, genesis, clonedSnapshot, genesis))
                {
                    engine.LoadScript(Array.Empty<byte>());
                    if (method!.Invoke(native, new object[] { engine }) is not ContractTask task)
                        throw new Exception($"Error casting {native.Name}.OnPersist to ContractTask");

                    task.GetAwaiter().GetResult();
                    if (engine.Execute() != VM.VMState.HALT)
                        throw new Exception($"Error executing {native.Name}.OnPersistAsync");
                }

                // Mock Native.PostPersist

                method = native.GetType().GetMethod("PostPersistAsync", BindingFlags.NonPublic | BindingFlags.Instance);

                using (var engine = new TestingApplicationEngine(_engine, TriggerType.PostPersist, genesis, clonedSnapshot, genesis))
                {
                    engine.LoadScript(Array.Empty<byte>());
                    if (method!.Invoke(native, new object[] { engine }) is not ContractTask task)
                        throw new Exception($"Error casting {native.Name}.PostPersist to ContractTask");

                    task.GetAwaiter().GetResult();
                    if (engine.Execute() != VM.VMState.HALT)
                        throw new Exception($"Error executing {native.Name}.PostPersistAsync");
                }

                clonedSnapshot.Commit();
            }

            if (commit)
            {
                _engine.Storage.Commit();
            }

            // Detach to static event

            ApplicationEngine.Log -= _engine.ApplicationEngineLog;
            ApplicationEngine.Notify -= _engine.ApplicationEngineNotify;

            return genesis;
        }
    }
}
