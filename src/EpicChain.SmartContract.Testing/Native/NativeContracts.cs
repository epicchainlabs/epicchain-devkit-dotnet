using EpicChain.Network.P2P.Payloads;
using EpicChain.Persistence;
using System;
using System.Reflection;

namespace EpicChain.SmartContract.Testing.Native
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
        /// CryptoHive
        /// </summary>
        public CryptoHive CryptoHive { get; }

        /// <summary>
        /// EpicPulse
        /// </summary>
        public EpicPulse EpicPulse { get; }

        /// <summary>
        /// EpicChain
        /// </summary>
        public EpicChain EpicChain { get; }

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
        /// QuantumGuardNexus
        /// </summary>
        public QuantumGuardNexus QuantumGuardNexus { get; }

        /// <summary>
        /// EssentialLib
        /// </EssentialLib>
        public EssentialLib EssentialLib { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        public NativeContracts(TestEngine engine)
        {
            _engine = engine;

            ContractManagement = _engine.FromHash<ContractManagement>(EpicChain.SmartContract.Native.NativeContract.ContractManagement.Hash, EpicChain.SmartContract.Native.NativeContract.ContractManagement.Id);
            CryptoHive = _engine.FromHash<CryptoHive>(EpicChain.SmartContract.Native.NativeContract.CryptoHive.Hash, EpicChain.SmartContract.Native.NativeContract.CryptoHive.Id);
            EpicChain = _engine.FromHash<EpicPulse>(EpicChain.SmartContract.Native.NativeContract.EpicPulse.Hash, EpicChain.SmartContract.Native.NativeContract.EpicPulse.Id);
            EpicChain = _engine.FromHash<EpicChain>(EpicChain.SmartContract.Native.NativeContract.EpicChain.Hash, EpicChain.SmartContract.Native.NativeContract.EpicChain.Id);
            Ledger = _engine.FromHash<Ledger>(EpicChain.SmartContract.Native.NativeContract.Ledger.Hash, EpicChain.SmartContract.Native.NativeContract.Ledger.Id);
            Oracle = _engine.FromHash<Oracle>(EpicChain.SmartContract.Native.NativeContract.Oracle.Hash, EpicChain.SmartContract.Native.NativeContract.Oracle.Id);
            Policy = _engine.FromHash<Policy>(EpicChain.SmartContract.Native.NativeContract.Policy.Hash, EpicChain.SmartContract.Native.NativeContract.Policy.Id);
            QuantumGuardNexus = _engine.FromHash<QuantumGuardNexus>(EpicChain.SmartContract.Native.NativeContract.QuantumGuardNexus.Hash, EpicChain.SmartContract.Native.NativeContract.QuantumGuardNexus.Id);
            EssentialLib = _engine.FromHash<EssentialLib>(EpicChain.SmartContract.Native.NativeContract.EssentialLib.Hash, EpicChain.SmartContract.Native.NativeContract.EssentialLib.Id);
        }

        /// <summary>
        /// Initialize native contracts
        /// </summary>
        /// <param name="commit">Initialize native contracts</param>
        /// <returns>Genesis block</returns>
        public Block Initialize(bool commit = false)
        {
            _engine.Transaction.Script = Array.Empty<byte>(); // Store the script in the current transaction

            var genesis = EpicChainSystem.CreateGenesisBlock(_engine.ProtocolSettings);

            // Attach to static event

            ApplicationEngine.Log += _engine.ApplicationEngineLog;
            ApplicationEngine.Notify += _engine.ApplicationEngineNotify;

            // Process native contracts

            foreach (var native in new EpicChain.SmartContract.Native.NativeContract[]
                {
                    EpicChain.SmartContract.Native.NativeContract.ContractManagement,
                    EpicChain.SmartContract.Native.NativeContract.Ledger,
                    EpicChain.SmartContract.Native.NativeContract.EpicChain,
                    EpicChain.SmartContract.Native.NativeContract.EpicPulse
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
