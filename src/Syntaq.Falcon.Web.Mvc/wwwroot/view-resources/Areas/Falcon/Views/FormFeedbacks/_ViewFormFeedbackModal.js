(function () {
    app.modals.ViewFormFeedbackModal = function () {

        this.init = function (modalManager) {
            _modalManager = modalManager;

            Formio.createForm(document.getElementById("viewFeedbackForm"), FeedbackFormSchema, { readOnly: true })
                .then(function (viewForm) {
                    viewForm.submission = {
                        data: FeedbackData
                    };

                });

            if (JSON.stringify(FeedbackFormSchema).includes("Comment_txt")) {

                $("#commentdiv").hide();
            }
        }
    };
})(jQuery);