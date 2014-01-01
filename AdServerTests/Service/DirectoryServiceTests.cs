using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AdServer.Data;
using AdServer.Models;
using AdServer.Service;
using AdServer.Utility;
using NUnit.Framework;

namespace AdServerTests.Service
{
    [TestFixture]
    class DirectoryServiceTests
    {
        private Directory[] rawDirectories;

        private Directory DirectoryWithMultilineDescription
        {
            get { return this.rawDirectories[5]; }
        }

        private static DirectoryService DirectoryService(AdvertisingContext dbContext)
        {
            var toTest = new DirectoryService(new DbContextProxy<AdvertisingContext>(dbContext));
            return toTest;
        }

        [SetUp]
        protected void SetUp()
        {
            this.rawDirectories = InMemoryDirectoryService.CreateTestDirectoriesData();
        }

        [Test]
        public void Load_NonExistingDirectory_ReturnNull()
        {
            using (var dbContext = new AdvertisingContext())
            {
                var toTest = DirectoryService(dbContext);
                var dir = toTest.GetDirectory(new DirectoryQuery(-1));

                Assert.IsNull(dir);
            }
        }

        [Test]
        public void Save_NewDirectoryNoAdvertisements_NewKeyIsAssigned([Values(false)] bool shallCommit)
        {
            using (var dbContext = new AdvertisingContext())
            {
                var toTest = DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy(shallCommit))
                {
                    var newDirectory = DirectoryWithMultilineDescription;
                    toTest.SaveDirectory(newDirectory);
                    transaction.Commit();

                    Assert.AreNotEqual(0, newDirectory.Id);
                }
            }
        }

        [Test]
        public void Save_NewDirectoryWithAdvertisements_ForeignKeyCreated([Values(false)] bool shallCommit)
        {
            using (var dbContext = new AdvertisingContext())
            {
                var toTest = DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy(shallCommit))
                {
                    var newDirectory = DirectoryWithMultilineDescription;
                    newDirectory.Advertisements = new List<Advertisement>();
                    newDirectory.Advertisements.AddRange(InMemoryDirectoryService.CreateTestAdvertisements(newDirectory));

                    toTest.SaveDirectory(newDirectory);
                    transaction.Commit();

                    newDirectory.Advertisements.ForEach(x => Assert.AreEqual(newDirectory.Id, x.DirectoryId));
                }
            }
        }

        [Test]
        public void Load_SavedDirectoryNoAdvertisements_ReturnsData([Values(true)]bool shallCleanup)
        {
            using (var dbContext = new AdvertisingContext())
            {
                var toTest = DirectoryService(dbContext);

                var newDirectory = DirectoryWithMultilineDescription;

                using (var transaction = new TransactionScopeProxy())
                {
                    toTest.SaveDirectory(newDirectory);
                    transaction.Commit();
                }
                dbContext.Entry(newDirectory).State = EntityState.Detached;

                using (var transaction = new TransactionScopeProxy())
                {
                    var dir = toTest.GetDirectory(newDirectory.Id);
                    Assert.IsNotNull(dir);
                    Assert.AreEqual(newDirectory.Id, dir.Id);

                    if (shallCleanup)
                    {
                        toTest.DeleteDirectory(dir);
                    }
                    transaction.Commit();
                }

                if (shallCleanup)
                {
                    var thisShouldBeNull = toTest.GetDirectory(newDirectory.Id);
                    Assert.IsNull(thisShouldBeNull);
                }
            }
        }

        [Test]
        public void LoadDirectory_WithAdvertisements_AdvertisementsCounted([Values(true)] bool shallCleanup)
        {
            int adCount;
            int testDirId;

            using (var dbContext = new AdvertisingContext())
            {
                dbContext.Database.Log = Console.Write;
                var toTest = DirectoryService(dbContext);
                var testDir = DirectoryWithMultilineDescription;

                using (var transaction = new TransactionScopeProxy())
                {
                    testDir.Advertisements = new List<Advertisement>();
                    testDir.Advertisements.AddRange(InMemoryDirectoryService.CreateTestAdvertisements(testDir));

                    toTest.SaveDirectory(testDir);
                    transaction.Commit();
                }
                adCount = testDir.Advertisements.Count;
                testDirId = testDir.Id;

                dbContext.Entry(testDir).State = EntityState.Detached;
            }

            using (var dbContext = new AdvertisingContext())
            {
                dbContext.Database.Log = Console.Write;
                var toTest = DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy())
                {
                    var dir = toTest.GetDirectory(new DirectoryQuery(testDirId) { WithCount = true });

                    Assert.AreEqual(adCount, dir.ItemsCount);

                    if (shallCleanup) { toTest.DeleteDirectory(dir); }
                    transaction.Commit();
                }
            }
        }

        [Test]
        public void LoadDirectoryList_SavedDirectory_ReturnsData([Values(true)]bool shallCleanup)
        {
            const int listSize = 3;
            var adListSize = 0;
            var testEntries = new List<int>();

            using (var dbContext = new AdvertisingContext())
            {
                dbContext.Database.Log = Console.Write;
                var toTest = DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy())
                {
                    for (var i = 0; i < listSize; i++)
                    {
                        var newDirectory = this.rawDirectories[i];

                        newDirectory.Advertisements = new List<Advertisement>();
                        newDirectory.Advertisements.AddRange(InMemoryDirectoryService.CreateTestAdvertisements(newDirectory));
                        toTest.SaveDirectory(newDirectory);

                        adListSize = newDirectory.Advertisements.Count;
                        testEntries.Add(newDirectory.Id);
                    }

                    transaction.Commit();
                }
            }

            using (var dbContext = new AdvertisingContext())
            {
                dbContext.Database.Log = Console.Write;
                var toTest = DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy())
                {
                    var dir = toTest.GetDirectoryList();

                    var myDirs = dir.Where(x => testEntries.Contains(x.Id)).ToList();
                    Assert.AreEqual(listSize, myDirs.Count);

                    myDirs.ForEach(x => Assert.AreEqual(adListSize, x.ItemsCount));

                    if (shallCleanup) { myDirs.ForEach(toTest.DeleteDirectory); }
                    transaction.Commit();
                }
            }
        }

        [Test]
        [Ignore("Used only for data loading")]
        public void LoadTestData()
        {
            using (var dbContext = new AdvertisingContext())
            {
                var toTest = DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy())
                {
                    foreach (var newDirectory in rawDirectories)
                    {
                        newDirectory.Advertisements = new List<Advertisement>();
                        newDirectory.Advertisements.AddRange(InMemoryDirectoryService.CreateTestAdvertisements(newDirectory));
                        toTest.SaveDirectory(newDirectory);

                        newDirectory.Advertisements.ForEach(x => Assert.AreEqual(newDirectory.Id, x.DirectoryId));
                    }

                    transaction.Commit();
                }
            }
        }
    }
}

