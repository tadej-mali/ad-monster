using System;
using System.Collections.Generic;
using System.Linq;
using AdServer.Models;

namespace AdServer.Service
{
    public class InMemoryDirectoryService : IDirectoryService
    {
        public void SaveDirectory(Directory dir)
        {
            throw new NotImplementedException();
        }

        public void DeleteDirectory(Directory dir)
        {
            throw new NotImplementedException();
        }

        public Directory GetDirectory(int id)
        {
            return this.GetDirectory(new DirectoryQuery(id));
        }

        public Directory GetDirectory(DirectoryQuery q)
        {
            var testDirs = CreateTestDirectoriesEntities();

            var theDirectory = testDirs.SingleOrDefault(x => x.Id == q.Id);
            if (theDirectory == null) { return null; }

            theDirectory.Advertisements = CreateTestAdvertisements(theDirectory);

            return theDirectory;
        }

        public IList<Directory> GetDirectoryList()
        {
            var testDirs = CreateTestDirectoriesEntities();

            var testAds = CreateTestAdvertisements(testDirs);
            foreach (var dir in testDirs)
            {
                dir.ItemsCount = testAds.Count(x => x.DirectoryId == dir.Id);
            }
            return testDirs;
        }

        private static Directory[] CreateTestDirectoriesEntities()
        {
            var testDirs = CreateTestDirectoriesData();
            for (int i = 0; i < testDirs.Length; i++)
            {
                testDirs[i].Id = i + 1;
            }

            return testDirs;
        }

        public static Directory[] CreateTestDirectoriesData()
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

        public static Advertisement[] CreateTestAdvertisements(params Directory[] directories)
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
    }
}