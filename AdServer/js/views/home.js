directory.HomeView = Backbone.View.extend({

    initialize: function () {
        this.foldersView = new directory.DirectoryListView({model: this.model});
    },

    render: function() {
        this.$el.html(this.template());

        this.foldersView.render();
        this.$el.append(this.foldersView.el);

        return this;
    }
});

directory.DirectoryListView = Backbone.View.extend({

    initialize:function () {

        var self = this;
        this.model.on("reset", this.render, this);
        this.model.on("add", function (dir) {
            self.$el.find(".list-group").append(new directory.DirectoryListItemView({model:dir}).render().el);
        });
    },

    render:function () {
        this.$el.html(this.template());

        _.each(this.model.models, function (dir) {
            this.$el.append(new directory.DirectoryListItemView({model:dir}).render().el);
        }, this);
        return this;
    }
});

directory.DirectoryListItemView = Backbone.View.extend({
    className: "list-group-item",

    initialize: function() {
        this.model.on("change", this.render, this);
        this.model.on("destroy", this.close, this);
    },

    render: function() {
        this.$el.html(this.template(this.model.attributes));
        return this;
    }
});
