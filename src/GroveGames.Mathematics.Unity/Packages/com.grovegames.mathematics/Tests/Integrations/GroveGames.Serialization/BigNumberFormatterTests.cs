using System;
using System.Collections.Generic;
using System.IO;
using GroveGames.Database;
using GroveGames.Serialization;
using NUnit.Framework;

namespace GroveGames.Mathematics.Unity.Serialization.Editor.Tests
{
    public sealed class BigNumberFormatterTests
    {
        private string _directory = null!;

        [SetUp]
        public void SetUp()
        {
            _directory = Path.Combine(Path.GetTempPath(), "BigNumberFormatterTests", Guid.NewGuid().ToString("N"));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_directory))
            {
                Directory.Delete(_directory, true);
            }
        }

        [Test]
        public void Serialize_MessagePack_RoundTripsBeyondDoubleRange()
        {
            var serializer = new MessagePackSerializer();
            var value = new BigNumber(1.5d, 400L);

            var result = serializer.Deserialize<BigNumber>(serializer.Serialize(value));

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void Serialize_SchemaMember_RoundTrips()
        {
            var serializer = new MessagePackSerializer();
            var record = new BigNumberRecord { Balance = new BigNumber(2.25d, 90L), History = new List<BigNumber> { 1, new BigNumber(3d, 12L) } };

            var result = serializer.Deserialize<BigNumberRecord>(serializer.Serialize(record))!;

            Assert.That(result.Balance, Is.EqualTo(record.Balance));
            Assert.That(result.History, Is.EqualTo(record.History));
        }

        [Test]
        public void SetDocument_Reopened_ReadsStoredValue()
        {
            var balance = new BigNumber(7.5d, 123L);

            using (var database = new FileDatabase(_directory))
            {
                database.GetDocument<BigNumberRecord>("record").Set(new BigNumberRecord { Balance = balance });
            }

            using var reopened = new FileDatabase(_directory);
            var result = reopened.GetDocument<BigNumberRecord>("record").Value;

            Assert.That(result.Balance, Is.EqualTo(balance));
        }
    }
}
