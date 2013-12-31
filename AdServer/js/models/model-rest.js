directory.Folder = Backbone.Model.extend({

    urlRoot : "/api/directory/",

    initialize:function () {
        this.title = "";
        this.description = "";
    }
});

directory.FolderCollection = Backbone.Collection.extend({

    url : "/api/directory",

    model: directory.Folder
});