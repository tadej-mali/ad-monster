using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AdServer.Data;
using AdServer.Models;
using AdServer.Service;

namespace AdServer.Controllers.Api
{
    static class ModelExtensions
    {
        public static DirectoryController.DirectoryDescription ToDescription(this Directory target)
        {
            if (target == null) { return null; }

            return new DirectoryController.DirectoryDescription
            {
                id = target.Id,
                title = target.Title,
                itemsCount = target.ItemsCount,
                description = target.Description
            };
        }

        public static DirectoryController.AdvertisementDescription ToDescription(this Advertisement target)
        {
            if (target == null) { return null; }

            return new DirectoryController.AdvertisementDescription
            {
                id = target.Id,
                title = target.Title,
                description = target.Description,
                url = target.Url,
                parentId = target.DirectoryId
            };
        }
    }

    public class DirectoryController : ApiController
    {
        public class DirectoryDescription
        {
            public int id { get; set; }
            public string title { get; set; }
            public int itemsCount { get; set; }
            public string description { get; set; }
            public AdvertisementDescription[] advertisements { get; set; }

            public DirectoryDescription Copy()
            {
                return new DirectoryDescription
                {
                    id = this.id,
                    title = this.title,
                    itemsCount = this.itemsCount,
                    description = this.description
                };
            }
        }


        public class AdvertisementDescription
        {
            public int parentId { get; set; }
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string url { get; set; }
        }

        private static readonly DirectoryDescription[] Directories = new []
        {
            new DirectoryDescription {id = 1,  title = "James",  itemsCount = 0, description = "In a sense, modernism holds that society, somewhat ironically, has objective value. The primary theme of the works of Tarantino is the bridge between class and sexual identity."},
            new DirectoryDescription {id = 2,  title = "Julie",  itemsCount = 0, description = "But the subject is contextualised into a neocapitalist paradigm of expression that includes culture as a reality. Derrida's analysis of modernism states that narrativity is used to entrench class divisions."},
            new DirectoryDescription {id = 3,  title = "Eugene", itemsCount = 0, description = "However, the collapse of the neocapitalist paradigm of expression intrinsic to Tarantino's Four Rooms is also evident in Reservoir Dogs. Bailey holds that we have to choose between cultural discourse and textual discourse."},
            new DirectoryDescription {id = 4,  title = "John",   itemsCount = 0, description = "It could be said that Sontag promotes the use of the neocapitalist paradigm of expression to read sexual identity. Lyotard uses the term 'cultural discourse' to denote the role of the poet as observer"
                                                                                             + "\nThus, a number of theories concerning subdeconstructivist desituationism exist. The primary theme of the works of Gibson is a self-supporting reality."
                                                                                             + "\nBut if modernism holds, we have to choose between the neocapitalist paradigm of expression and capitalist presemantic theory. Debord suggests the use of modernism to attack the status quo. "},
            new DirectoryDescription {id = 5,  title = "Ray",    itemsCount = 0, description = "In a sense, several narratives concerning modernism exist. The subject is contextualised into a neocapitalist paradigm of expression that includes narrativity as a reality."
                                                                                             + "\nTherefore, Sartre suggests the use of postcultural textual theory to deconstruct capitalism. The subject is interpolated into a modernism that includes consciousness as a whole."
                                                                                             + "\nBut a number of theories concerning the role of the reader as participant may be found. Foucault promotes the use of subsemantic nationalism to modify class. "},
            new DirectoryDescription {id = 6,  title = "Paul",   itemsCount = 0, description = "Until about midday the Pool of London was an astonishing scene. Steamboats and shipping of all sorts lay there, tempted by the enormous sums of money offered by fugitives, and it is said that many who swam out to these vessels were thrust off with boathooks and drowned."
                                                                                             + "\nAbout one o'clock in the afternoon the thinning remnant of a cloud of the black vapour appeared between the arches of Blackfriars Bridge. At that the Pool became a scene of mad confusion, fighting, and collision, and for some time a multitude of boats and barges jammed in the northern arch of the Tower Bridge, and the sailors and lightermen had to fight savagely against the people who swarmed upon them from the riverfront."
                                                                                             + "\nPeople were actually clambering down the piers of the bridge from above."},
            new DirectoryDescription {id = 7,  title = "Paula",  itemsCount = 0, description = "When, an hour later, a Martian appeared beyond the Clock Tower and waded down the river, nothing but wreckage floated above Limehouse."
                                                                                             + "\nOf the falling of the fifth cylinder I have presently to tell. The sixth star fell at Wimbledon. My brother, keeping watch beside the women in the chaise in a meadow, saw the green flash of it far beyond the hills. On Tuesday the little party, still set upon getting across the sea, made its way through the swarming country towards Colchester. The news that the Martians were now in possession of the whole of London was confirmed. They had been seen at Highgate, and even, it was said, at Neasden. But they did not come into my brother's view until the morrow."},
            new DirectoryDescription {id = 8,  title = "Lisa",   itemsCount = 0, description = "People were watching for Martians here from the church towers. My brother, very luckily for him as it chanced, preferred to push on at once to the coast rather than wait for food, although all three of them were very hungry. By midday they passed through Tillingham, which, strangely enough, seemed to be quite silent and deserted, save for a few furtive plunderers hunting for food."},
            new DirectoryDescription {id = 9,  title = "Gary",   itemsCount = 0, description = "Near Tillingham they suddenly came in sight of the sea, and the most amazing crowd of shipping of all sorts that it is possible to imagine."},
            new DirectoryDescription {id = 10, title = "Kathy",  itemsCount = 0, description = "'The first Chauffeur was Bill, a common fellow, as I said before,' the old man expounded; 'but his wife was a lady, a great lady. Before the Scarlet Death she was the wife of Van Worden. He was President of the Board of Industrial Magnates, and was one of the dozen men who ruled America. He was worth one billion, eight hundred millions of dollars-coins like you have there in your pouch, Edwin."}
        };


        private static AdvertisementDescription[] CreateAdvertisements(IEnumerable<DirectoryDescription> directories)
        {
            var nextId = 1;

            return directories
                .SelectMany(item => new []
                {
                    new AdvertisementDescription { parentId = item.id, id = nextId, title = "Title " + nextId, description = "Description of advertisement " + nextId++, url = "http://www.google.com"},
                    new AdvertisementDescription { parentId = item.id, id = nextId, title = "Title " + nextId, description = "Description of advertisement " + nextId++, url = "http://www.yahoo.com"},
                    new AdvertisementDescription { parentId = item.id, id = nextId, title = "Title " + nextId, description = "Description of advertisement " + nextId++, url = "http://www.twitter.com"}
                })
                .ToArray();
        }

        private static readonly AdvertisementDescription[] Advertisements = CreateAdvertisements(Directories);

        // GET api/directory
        public object Get()
        {
            /*
            foreach (var description in Directories)
            {
                description.itemsCount = Advertisements.Count(x => x.parentId == description.id);
            }
            return Directories;
            */

            using (var dbContext = new AdvertisingContext())
            {
                var dirService = new DirectoryService(new DbContextProxy<AdvertisingContext>(dbContext));
                var dir = dirService.GetDirectoryList();

                return dir.Select(x => x.ToDescription()).ToArray();
            }
        }

        // GET api/directory/5
        public object Get(int id)
        {
            /*
            var theDirectory = Directories.SingleOrDefault(x => x.id == id);
            if (theDirectory == null) { return null; }

            var result = theDirectory.Copy();
            result.advertisements = Advertisements.Where(x => x.parentId == id).ToArray();
            */

            using (var dbContext = new AdvertisingContext())
            {
                var dirService = new DirectoryService(new DbContextProxy<AdvertisingContext>(dbContext));
                var theDirectory = dirService.GetDirectory(id);

                if (theDirectory == null) { return null; }

                var result = theDirectory.ToDescription();
                result.advertisements = theDirectory.Advertisements.Select(x => x.ToDescription()).ToArray();
                return result;
            }
        }

        // POST api/directory
        public void Post([FromBody]string value)
        {
        }

        // PUT api/directory/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/directory/5
        public void Delete(int id)
        {
        }
    }
}
