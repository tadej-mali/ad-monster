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

        private static Directory[] CreateTestDirectories()
        {
            return new[]
            {
                new Directory {Title = "James",  Description = "In a sense, modernism holds that society, somewhat ironically, has objective value. The primary theme of the works of Tarantino is the bridge between class and sexual identity."},
                new Directory {Title = "Julie",  Description = "But the subject is contextualised into a neocapitalist paradigm of expression that includes culture as a reality. Derrida's analysis of modernism states that narrativity is used to entrench class divisions."},
                new Directory {Title = "Eugene", Description = "However, the collapse of the neocapitalist paradigm of expression intrinsic to Tarantino's Four Rooms is also evident in Reservoir Dogs. Bailey holds that we have to choose between cultural discourse and textual discourse."},
                new Directory {Title = "John",   Description = "It could be said that Sontag promotes the use of the neocapitalist paradigm of expression to read sexual identity. Lyotard uses the term 'cultural discourse' to denote the role of the poet as observer"
                                                             + "\nThus, a number of theories concerning subdeconstructivist desituationism exist. The primary theme of the works of Gibson is a self-supporting reality."
                                                             + "\nBut if modernism holds, we have to choose between the neocapitalist paradigm of expression and capitalist presemantic theory. Debord suggests the use of modernism to attack the status quo. "},
                new Directory {Title = "Ray",    Description = "In a sense, several narratives concerning modernism exist. The subject is contextualised into a neocapitalist paradigm of expression that includes narrativity as a reality."
                                                             + "\nTherefore, Sartre suggests the use of postcultural textual theory to deconstruct capitalism. The subject is interpolated into a modernism that includes consciousness as a whole."
                                                             + "\nBut a number of theories concerning the role of the reader as participant may be found. Foucault promotes the use of subsemantic nationalism to modify class. "},
                new Directory {Title = "Paul",   Description = "Until about midday the Pool of London was an astonishing scene. Steamboats and shipping of all sorts lay there, tempted by the enormous sums of money offered by fugitives, and it is said that many who swam out to these vessels were thrust off with boathooks and drowned."
                                                             + "\nAbout one o'clock in the afternoon the thinning remnant of a cloud of the black vapour appeared between the arches of Blackfriars Bridge. At that the Pool became a scene of mad confusion, fighting, and collision, and for some time a multitude of boats and barges jammed in the northern arch of the Tower Bridge, and the sailors and lightermen had to fight savagely against the people who swarmed upon them from the riverfront."
                                                             + "\nPeople were actually clambering down the piers of the bridge from above."},
                new Directory {Title = "Paula",  Description = "When, an hour later, a Martian appeared beyond the Clock Tower and waded down the river, nothing but wreckage floated above Limehouse."
                                                             + "\nOf the falling of the fifth cylinder I have presently to tell. The sixth star fell at Wimbledon. My brother, keeping watch beside the women in the chaise in a meadow, saw the green flash of it far beyond the hills. On Tuesday the little party, still set upon getting across the sea, made its way through the swarming country towards Colchester. The news that the Martians were now in possession of the whole of London was confirmed. They had been seen at Highgate, and even, it was said, at Neasden. But they did not come into my brother's view until the morrow."},
                new Directory {Title = "Lisa",   Description = "People were watching for Martians here from the church towers. My brother, very luckily for him as it chanced, preferred to push on at once to the coast rather than wait for food, although all three of them were very hungry. By midday they passed through Tillingham, which, strangely enough, seemed to be quite silent and deserted, save for a few furtive plunderers hunting for food."},
                new Directory {Title = "Gary",   Description = "Near Tillingham they suddenly came in sight of the sea, and the most amazing crowd of shipping of all sorts that it is possible to imagine."},
                new Directory {Title = "Kathy",  Description = "'The first Chauffeur was Bill, a common fellow, as I said before,' the old man expounded; 'but his wife was a lady, a great lady. Before the Scarlet Death she was the wife of Van Worden. He was President of the Board of Industrial Magnates, and was one of the dozen men who ruled America. He was worth one billion, eight hundred millions of dollars-coins like you have there in your pouch, Edwin."}
            };
        }

        private static Advertisement[] CreateTestAdvertisements(params Directory[] directories)
        {
            var nextId = 1;

            return directories
                .SelectMany(item => new[]
                {
                    new Advertisement { DirectoryId = item.Id, Title = item.Title + ", Advertisement - " + nextId, Description = "Description of advertisement " + nextId++ + " on google.com",   Url = "http://www.google.com"},
                    new Advertisement { DirectoryId = item.Id, Title = item.Title + ", Advertisement - " + nextId, Description = "Description of advertisement " + nextId++ + " on yahoo.com",    Url = "http://www.yahoo.com"},
                    new Advertisement { DirectoryId = item.Id, Title = item.Title + ", Advertisement - " + nextId, Description = "Description of advertisement " + nextId++ + " on twitter.com",  Url = "http://www.twitter.com"},
                    new Advertisement { DirectoryId = item.Id, Title = item.Title + ", Advertisement - " + nextId, Description = "Description of advertisement " + nextId++ + " on facebook.com", Url = "http://www.facebook.com"}
                })
                .ToArray();
        }

        private Directory DirectoryWithMultilineDescription
        {
            get { return this.rawDirectories[5]; }
        }

        [SetUp]
        protected void SetUp()
        {
            this.rawDirectories = CreateTestDirectories();
        }

        [Test]
        public void Load_NonExistingDirectory_ReturnNull()
        {
            var toTest = new DirectoryService();

            var dir = toTest.GetDirectory(new DirectoryQuery(-1));

            Assert.IsNull(dir);
        }

        [Test]
        public void Save_NewDirectoryNoAdvertisements_NewKeyIsAssigned([Values(false)]bool shallCommit)
        {
            var toTest = new DirectoryService();

            using (var transaction = new TransactionScopeProxy(shallCommit))
            {
                var newDirectory = DirectoryWithMultilineDescription;
                toTest.SaveDirectory(newDirectory);
                transaction.Commit();

                Assert.AreNotEqual(0, newDirectory.Id);
            }
        }

        [Test]
        public void Save_NewDirectoryWithAdvertisements_ForeignKeyCreated([Values(false)]bool shallCommit)
        {
            var toTest = new DirectoryService();

            using (var transaction = new TransactionScopeProxy(shallCommit))
            {
                var newDirectory = DirectoryWithMultilineDescription;
                newDirectory.Advertisements = new List<Advertisement>();
                newDirectory.Advertisements.AddRange(CreateTestAdvertisements(newDirectory));

                toTest.SaveDirectory(newDirectory);
                transaction.Commit();

                newDirectory.Advertisements.ForEach(x => Assert.AreEqual(newDirectory.Id, x.DirectoryId));
            }
        }

        [Test]
        public void Load_SavedDirectoryNoAdvertisements_ReturnsData([Values(true)]bool shallCleanup)
        {
            var dbContext = new AdvertisingContext();
            var toTest = new DirectoryService(dbContext);
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

                if (shallCleanup) { toTest.DeleteDirectory(dir); }
                transaction.Commit();
            }

            if (shallCleanup)
            {
                var thisShouldBeNull = toTest.GetDirectory(newDirectory.Id);
                Assert.IsNull(thisShouldBeNull);
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
                var toTest = new DirectoryService(dbContext);
                var testDir = DirectoryWithMultilineDescription;

                using (var transaction = new TransactionScopeProxy())
                {
                    testDir.Advertisements = new List<Advertisement>();
                    testDir.Advertisements.AddRange(CreateTestAdvertisements(testDir));

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
                var toTest = new DirectoryService(dbContext);

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
                var toTest = new DirectoryService(dbContext);

                using (var transaction = new TransactionScopeProxy())
                {
                    for (var i = 0; i < listSize; i++)
                    {
                        var newDirectory = this.rawDirectories[i];

                        newDirectory.Advertisements = new List<Advertisement>();
                        newDirectory.Advertisements.AddRange(CreateTestAdvertisements(newDirectory));
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
                var toTest = new DirectoryService(dbContext);

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
            var toTest = new DirectoryService();

            using (var transaction = new TransactionScopeProxy())
            {
                foreach (var newDirectory in rawDirectories)
                {
                    newDirectory.Advertisements = new List<Advertisement>();
                    newDirectory.Advertisements.AddRange(CreateTestAdvertisements(newDirectory));
                    toTest.SaveDirectory(newDirectory);

                    newDirectory.Advertisements.ForEach(x => Assert.AreEqual(newDirectory.Id, x.DirectoryId));
                }

                transaction.Commit();
            }
        }
    }
}

