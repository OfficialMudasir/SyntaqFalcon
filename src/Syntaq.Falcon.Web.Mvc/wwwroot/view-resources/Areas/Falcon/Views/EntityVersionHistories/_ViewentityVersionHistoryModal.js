(function ($) {
    
    app.modals.ViewEntityVersionHistoryModal = function () {
        this.init = function (modalManager) {
            
            if ($('#entityType').text().trim() === 'Template') {
                var options = {
                    collapsed: false,
                    withQuotes: false
                };
                $('#json-renderer').jsonViewer(JSONObj, options);
            }
            else {
                var options = {
                    collapsed: false,
                    withQuotes: false
                };

                $('#json-renderer').jsonViewer(JSONObj, options);
                var dmp = new diff_match_patch();

                var d = dmp.diff_main(JSON.stringify(previousData, null, 4), JSON.stringify(newData, null, 4), true);
                var ds = dmp.diff_prettyHtml(d);
                document.getElementById('outputdiv').innerHTML = ds;
            }
        };
    };

})(jQuery);