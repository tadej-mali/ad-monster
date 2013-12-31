var directory = {

    loadTemplates: function(views, callback) {

        var deferreds = [];

        $.each(views, function (index, view) {
            if (directory[view]) {
                deferreds.push($.get('templates/' + view + '.html', function(data) {
                    directory[view].prototype.template = _.template(data);
                }, 'html'));
            } else {
                alert(view + " not found");
            }
        });

        $.when.apply(null, deferreds).done(callback);
    },

    // This is the endpoint of service api site
    siteUrl: "http://192.168.0.14"
};

directory.Router = Backbone.Router.extend({

    routes: {
        "":                 "home",
        "directory/:id":    "directoryContent"
    },

    initialize: function () {
        this.$content = $("#content");
    },

    home: function () {
        var self = this;
        var dir = new directory.FolderCollection();
        dir.fetch({
            success: function () {
                directory.homeView = new directory.HomeView({model: dir});
                directory.homeView.render();
                self.$content.html(directory.homeView.el);
            }
        });
    },
    
    directoryContent: function (id) {
        var self = this;
        var folder = new directory.Folder({id: id});
        folder.fetch({
            success: function(data) {
                self.$content.html(new directory.DirectoryDetailsView({ model: data }).render().el);
            }
        });
    }
});

_.mixin({
  nl2br: function (str, isXhtml) {
    if (!str) return str;
    var breakTag = (isXhtml || typeof isXhtml === 'undefined') ? '<br />' : '<br>';
    return (str + '').replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, '$1'+ breakTag +'$2');
  }
});

Backbone.ajax = function (options) {
    options.contentType = "application/json; charset=utf-8";
    options.url = directory.siteUrl + options.url;
    return $.ajax(options);
};

$(document).on("ready", function () {

    directory.loadTemplates(["HomeView", "DirectoryListView", "DirectoryListItemView", "DirectoryDetailsView"],
        function () {
            directory.router = new directory.Router();
            Backbone.history.start();
        });
});
