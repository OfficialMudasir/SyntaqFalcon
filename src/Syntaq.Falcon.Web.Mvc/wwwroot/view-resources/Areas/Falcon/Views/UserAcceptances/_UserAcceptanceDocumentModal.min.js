(function ($) {
    app.modals.UserAcceptanceDocumentModal = function () {

        var _modalManager;


        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            _modalManager.getModal().find(".modal-content").css("width", "inherit");
        };
    };
})(jQuery);