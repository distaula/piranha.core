/*global
    piranha
*/

piranha.postpicker = new Vue({
    el: "#postpicker",
    data: {
        loading: true,
        search: '',
        sites: [],
        archives: [],
        posts: [],
        currentSiteId: null,
        currentArchiveId: null,
        currentSiteTitle: null,
        currentArchiveTitle: null,
        filter: null,
        callback: null,
    },
    computed: {
        filteredPosts: function () {
            var self = this;

            return this.posts.filter(function (post) {
                if (self.search.length > 0) {
                    return post.title.toLowerCase().indexOf(self.search.toLowerCase()) > -1
                }
                return true;
            });
        }
    },
    methods: {
        load: function (siteId, archiveId) {
            var self = this;
            var url = piranha.baseUrl + "manager/api/post/modal";

            if (siteId) {
                url += "?siteId=" + siteId;
                if (archiveId) {
                    url += "&archiveId=" + archiveId;
                }
            }

            fetch(url)
                .then(function (response) { return response.json(); })
                .then(function (result) {
                    self.sites = result.sites;
                    self.archives = result.archives;
                    self.posts = result.posts;

                    self.currentSiteId = result.siteId;
                    self.currentArchiveId = result.archiveId;

                    self.currentSiteTitle = result.siteTitle;
                    self.currentArchiveTitle = result.archiveTitle;

                    Vue.nextTick(function () {
                        $("#postpicker-site").select2({
                            selectOnClose: true,
                            dropdownParent: $("#postpicker"),
                            templateSelection: self.formatSiteState
                        });
                        $("#postpicker-archive").select2({
                            selectOnClose: true,
                            dropdownParent: $("#postpicker"),
                            templateSelection: self.formatArchiveState
                        });
                    });
                })
                .catch(function (error) { console.log("error:", error ); });
        },
        refresh: function () {
            this.load(this.currentSiteId, this.currentArchiveId);
        },
        open: function (callback, siteId, archiveId, currentPostId) {
            this.search = '';
            this.callback = callback;

            this.load(siteId, archiveId);

            $("#postpicker").modal("show");
        },
        onEnter: function () {
            if (this.filteredPosts.length == 1) {
                this.select(this.filteredPosts[0]);
            }
        },
        select: function (item) {
            this.callback(JSON.parse(JSON.stringify(item)));
            this.callback = null;
            this.search = "";

            $("#postpicker").modal("hide");
        },
        formatSiteState: function (state) {
            if (!state.id) {
                return state.text;
            }
            var $state = $("<span><i class='fas fa-globe mr-2'></i><span></span></span>");
            $state.find("span").text(state.text);

            return $state;
        },
        formatArchiveState: function (state) {
            if (!state.id) {
                return state.text;
            }
            var $state = $("<span><i class='fas fa-font mr-2'></i><span></span></span>");
            $state.find("span").text(state.text);

            return $state;
        }
    },
    updated: function () {
        var self = this;

        if (this.loading)
        {
            // Initialize select 2
            $("#postpicker-site").select2({
                selectOnClose: true,
                dropdownParent: $("#postpicker"),
                templateSelection: self.formatSiteState
            });
            $("#postpicker-archive").select2({
                selectOnClose: true,
                dropdownParent: $("#postpicker"),
                templateSelection: self.formatArchiveState
            });

            // We need to manually bind select2 to the Vue model variable
            $("#postpicker-site").on("change", function() {
                self.load($(this).find("option:selected").val());
            });
            $("#postpicker-archive").on("change", function() {
                self.load(self.currentSiteId, $(this).find("option:selected").val());
            });
        }
        this.loading = false;
    }
});

$(document).ready(function() {
    $("#postpicker").on("shown.bs.modal", function() {
        $("#postpickerSearch").trigger("focus");
    });
});