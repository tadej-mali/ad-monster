var directory = {

    views: {},

    models: {},

    loadTemplates: function(views, callback) {

        var deferreds = [];

        $.each(views, function(index, view) {
            if (directory[view]) {
                deferreds.push($.get('templates/' + view + '.html', function(data) {
                    directory[view].prototype.template = _.template(data);
                }, 'html'));
            } else {
                alert(view + " not found");
            }
        });

        $.when.apply(null, deferreds).done(callback);
    }

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
            success: function (data) {
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
  nl2br: function (str, is_xhtml) {
    if (!str) return str;
    var breakTag = (is_xhtml || typeof is_xhtml === 'undefined') ? '<br />' : '<br>';
    return (str + '').replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, '$1'+ breakTag +'$2');
  }
});

$(document).on("ready", function () {
    
    directory.loadTemplates(["HomeView", "DirectoryListView", "DirectoryListItemView", "DirectoryDetailsView"],
        function () {
            directory.router = new directory.Router();
            Backbone.history.start();
        });
});
