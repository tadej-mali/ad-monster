directory.Folder = Backbone.Model.extend({

    initialize:function () {
        this.title = "";
        this.description = "";
    },

    sync: function(method, model, options) {
        if (method === "read") {
            directory.store.getDirectory(parseInt(this.id), function (data) {
                options.success(data);
            });
        }
    }
});

directory.FolderCollection = Backbone.Collection.extend({

    model: directory.Folder,

    sync: function(method, model, options) {
        if (method === "read") {
            directory.store.getAllDirectories(function (data) {
                options.success(data);
            });
        }
    }
});

directory.MemoryStore = function(successCallback, errorCallback) {

    var self = this;

    function getAdvertisementsByDirectory(id) {
        return _(self.advertisements).filter(function(x) { return x.parentId == id; });
    }

    ;

    this.getAllDirectories = function(callback) {
        _(self.directories).each(function(item) {
            var allAds = getAdvertisementsByDirectory(item.id);
            item.itemsCount = allAds.length;
        });

        callLater(callback, self.directories);
    };

    this.getDirectory = function(id, callback) {
        var found = _(this.directories).find(function(x) { return x.id == id; });

        if (found) {
            var ads = getAdvertisementsByDirectory(found.id);
            found.advertisements = ads;
        }

        callLater(callback, found);
    };

    // Used to simulate async calls. This is done to provide a consistent interface with stores that use async data access APIs
    var callLater = function(callback, data) {
        if (callback) {
            setTimeout(function() {
                callback(data);
            });
        }
    };

    this.directories = [
        { "id": 1, "title": "James", itemsCount: 0, description: "In a sense, modernism holds that society, somewhat ironically, has objective value. The primary theme of the works of Tarantino is the bridge between class and sexual identity." },
        { "id": 2, "title": "Julie", itemsCount: 0, description: "But the subject is contextualised into a neocapitalist paradigm of expression that includes culture as a reality. Derrida's analysis of modernism states that narrativity is used to entrench class divisions." },
        { "id": 3, "title": "Eugene", itemsCount: 0, description: "However, the collapse of the neocapitalist paradigm of expression intrinsic to Tarantino's Four Rooms is also evident in Reservoir Dogs. Bailey holds that we have to choose between cultural discourse and textual discourse." },
        {
            "id": 4,
            "title": "John",
            itemsCount: 0,
            description: "It could be said that Sontag promotes the use of the neocapitalist paradigm of expression to read sexual identity. Lyotard uses the term 'cultural discourse' to denote the role of the poet as observer."
                + "\nThus, a number of theories concerning subdeconstructivist desituationism exist. The primary theme of the works of Gibson is a self-supporting reality."
                + "\nBut if modernism holds, we have to choose between the neocapitalist paradigm of expression and capitalist presemantic theory. Debord suggests the use of modernism to attack the status quo. "
        },
        {
            "id": 5,
            "title": "Ray",
            itemsCount: 0,
            description: "In a sense, several narratives concerning modernism exist. The subject is contextualised into a neocapitalist paradigm of expression that includes narrativity as a reality."
                + "\nTherefore, Sartre suggests the use of postcultural textual theory to deconstruct capitalism. The subject is interpolated into a modernism that includes consciousness as a whole."
                + "\nBut a number of theories concerning the role of the reader as participant may be found. Foucault promotes the use of subsemantic nationalism to modify class. "
        },
        {
            "id": 6,
            "title": "Paul",
            itemsCount: 0,
            description: "Until about midday the Pool of London was an astonishing scene. Steamboats and shipping of all sorts lay there, tempted by the enormous sums of money offered by fugitives, and it is said that many who swam out to these vessels were thrust off with boathooks and drowned."
                + "\nAbout one o'clock in the afternoon the thinning remnant of a cloud of the black vapour appeared between the arches of Blackfriars Bridge. At that the Pool became a scene of mad confusion, fighting, and collision, and for some time a multitude of boats and barges jammed in the northern arch of the Tower Bridge, and the sailors and lightermen had to fight savagely against the people who swarmed upon them from the riverfront."
                + "\nPeople were actually clambering down the piers of the bridge from above."
        },
        {
            "id": 7,
            "title": "Paula",
            itemsCount: 0,
            description: "When, an hour later, a Martian appeared beyond the Clock Tower and waded down the river, nothing but wreckage floated above Limehouse."
                + "\nOf the falling of the fifth cylinder I have presently to tell. The sixth star fell at Wimbledon. My brother, keeping watch beside the women in the chaise in a meadow, saw the green flash of it far beyond the hills. On Tuesday the little party, still set upon getting across the sea, made its way through the swarming country towards Colchester. The news that the Martians were now in possession of the whole of London was confirmed. They had been seen at Highgate, and even, it was said, at Neasden. But they did not come into my brother's view until the morrow."
        },
        { "id": 8, "title": "Lisa", itemsCount: 0, description: "People were watching for Martians here from the church towers. My brother, very luckily for him as it chanced, preferred to push on at once to the coast rather than wait for food, although all three of them were very hungry. By midday they passed through Tillingham, which, strangely enough, seemed to be quite silent and deserted, save for a few furtive plunderers hunting for food." },
        { "id": 9, "title": "Gary", itemsCount: 0, description: "Near Tillingham they suddenly came in sight of the sea, and the most amazing crowd of shipping of all sorts that it is possible to imagine." },
        { "id": 10, "title": "Kathy", itemsCount: 0, description: "'The first Chauffeur was Bill, a common fellow, as I said before,' the old man expounded; 'but his wife was a lady, a great lady. Before the Scarlet Death she was the wife of Van Worden. He was President of the Board of Industrial Magnates, and was one of the dozen men who ruled America. He was worth one billion, eight hundred millions of dollars-coins like you have there in your pouch, Edwin." }
    ];


    this.advertisements = (function() {
        var data = [];
        var nextId = 1;

        _(self.directories).each(function(item) {

            Array.prototype.push.apply(data, [
                { parentId: item.id, id: nextId, title: "Title " + nextId, description: "Description of advertisement " + nextId++, url: "http://www.google.com" },
                { parentId: item.id, id: nextId, title: "Title " + nextId, description: "Description of advertisement " + nextId++, url: "http://www.yahoo.com" },
                { parentId: item.id, id: nextId, title: "Title " + nextId, description: "Description of advertisement " + nextId++, url: "http://www.twitter.com" }
            ]);

            /*
                data.push({ parentId: item.id, id: nextId, title: "Title " + nextId, description: "Description of advertisement " + nextId++, url: "http://www.google.com"});
                data.push({ parentId: item.id, id: nextId, title: "Title " + nextId, description: "Description of advertisement " + nextId++, url: "http://www.yahoo.com"});
                data.push({ parentId: item.id, id: nextId, title: "Title " + nextId, description: "Description of advertisement " + nextId++, url: "http://www.twitter.com"});		
                */
        });

        return data;
    })();

    callLater(successCallback);
};

directory.store = new directory.MemoryStore();