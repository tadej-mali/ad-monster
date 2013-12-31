directory.Folder = Backbone.Model.extend({

    urlRoot : "http://www.advertising.loc/api/directory/",

    initialize:function () {
        this.title = "";
        this.description = "";
    }
});

directory.FolderCollection = Backbone.Collection.extend({

    url : "http://www.advertising.loc/api/directory",

    model: directory.Folder
});