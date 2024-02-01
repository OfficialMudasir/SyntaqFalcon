(function () {
    app.modals.ViewProjectTemplateModal = function () {
        function formatDateTime(timeString) {
            var dt = new Date(timeString);
            var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            var tmoptions = { hour: 'numeric', minute: 'numeric' };
            var formatedTime = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
            return formatedTime;
        }

        this.init = function (modalManager) {
            _modalManager = modalManager;
            $('#creationTime').text(formatDateTime(creatationTime));
            $('#lastModificationTime').text(formatDateTime(lastModificationTime));
        }
    };
})(jQuery);