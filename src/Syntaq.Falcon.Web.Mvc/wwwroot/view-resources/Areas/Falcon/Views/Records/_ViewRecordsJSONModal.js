(function ($) {
    app.modals.ViewRecordsJSONModal = function () {
        this.init = function (modalManager) {
            var options = {
                collapsed: $('#IsCollapsed').is(':checked'),
                withQuotes: $('#With-quotes').is(':checked')
            };
            $('#json-renderer').jsonViewer(JSONObj, options);
        };

        $("#IsCollapsed").change(function () {
            var options = {
                collapsed: $('#IsCollapsed').is(':checked'),
                withQuotes: $('#With-quotes').is(':checked')
            };
            $('#json-renderer').jsonViewer(JSONObj, options);
        });

        $("#With-quotes").change(function () {
            var options = {
                collapsed: $('#IsCollapsed').is(':checked'),
                withQuotes: $('#With-quotes').is(':checked')
            };
            $('#json-renderer').jsonViewer(JSONObj, options);
        });
    };
})(jQuery);