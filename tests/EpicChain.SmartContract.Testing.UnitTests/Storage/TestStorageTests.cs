using Microsoft.VisualStudio.TestTools.UnitTesting;
using EpicChain.Json;
using EpicChain.Persistence;
using EpicChain.SmartContract.Testing.Storage;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EpicChain.SmartContract.Testing.UnitTests.Storage
{
    [TestClass]
    public class TestStorageTests
    {
        [TestMethod]
        public void TestCheckpoint()
        {
            // Create a new test engine with native contracts already initialized

            var engine = new TestEngine(true);

            // Check that all it works

            Assert.IsTrue(engine.Native.EpicChain.Storage.Contains(1)); // Prefix_VotersCount
            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);

            // Create checkpoint

            var checkpoint = engine.Storage.Checkpoint();

            // Create new storage, and restore the checkpoint on it

            var storage = new EngineStorage(new MemoryStore());
            checkpoint.Restore(storage.Snapshot);

            // Create new test engine without initialize
            // and set the storage to the restored one

            engine = new TestEngine(storage, false);

            // Ensure that all works

            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);

            // Test restoring in raw

            storage = new EngineStorage(new MemoryStore());
            new EngineCheckpoint(new MemoryStream(checkpoint.ToArray())).Restore(storage.Snapshot);

            engine = new TestEngine(storage, false);
            Assert.AreEqual(1_000_000_000, engine.Native.EpicChain.TotalSupply);
        }

        [TestMethod]
        public void LoadExportImport()
        {
            EngineStorage store = new(new MemoryStore());

            // empty

            var entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 0);

            // simple object

            var json = @"{""bXlSYXdLZXk="":""dmFsdWU=""}";

            store.Import(((JObject)JToken.Parse(json)!)!);
            store.Commit();

            entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 1);

            Assert.AreEqual("myRawKey", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            // prefix object

            json = @"{""bXk="":{""UmF3S2V5LTI="":""dmFsdWUtMg==""}}";

            store.Import(((JObject)JToken.Parse(json)!)!);
            store.Commit();

            entries = store.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 2);

            Assert.AreEqual("myRawKey", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            Assert.AreEqual("myRawKey-2", Encoding.ASCII.GetString(entries[1].Key));
            Assert.AreEqual("value-2", Encoding.ASCII.GetString(entries[1].Value));

            // Test import

            EngineStorage storeCopy = new(new MemoryStore());

            store.Commit();
            storeCopy.Import(store.Export());
            storeCopy.Commit();

            entries = storeCopy.Store.Seek(Array.Empty<byte>(), SeekDirection.Forward).ToArray();
            Assert.AreEqual(entries.Length, 2);

            Assert.AreEqual("myRawKey", Encoding.ASCII.GetString(entries[0].Key));
            Assert.AreEqual("value", Encoding.ASCII.GetString(entries[0].Value));

            Assert.AreEqual("myRawKey-2", Encoding.ASCII.GetString(entries[1].Key));
            Assert.AreEqual("value-2", Encoding.ASCII.GetString(entries[1].Value));
        }
    }
}
