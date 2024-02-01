

var feedbackFormSchema;

function toggleVisibility() {
    var e = document.getElementById('feedback-main');
    if (e.style.display == 'block') {
        e.style.display = 'none';
    }
    else {
        loadFeedbackForm();
        e.style.display = 'block';
    }
};

function loadFeedbackButton() {
    var buttonE = document.createElement("button");
    buttonE.id = "popupFeedbackForm";
    buttonE.classList.add("feedback-button");
    buttonE.classList.add("text-info");
    buttonE.textContent = "Feedback";
    buttonE.onclick = () => {
        toggleVisibility();
    };
    document.body.appendChild(buttonE);
    var btclose = document.getElementById("feedbackClose");
    btclose.onclick = () => {
        toggleVisibility();
    };
}

function loadFeedbackForm() {
    //var template = '{"type":"form","display":"form","components":[{"key":"Rating","label":"Rating","showSummary":false,"defaultValue":"5","DoNotLoadFromRecord":false,"widthslider":"12","offsetslider":"0","minValue":1,"type":"slider","input":true,"tableView":true,"logic":[]},{"key":"description","label":"Description","showSummary":false,"defaultValue":"","DoNotLoadFromRecord":false,"showWordCount":false,"showCharCount":false,"widthslider":"12","offsetslider":"0","rows":5,"type":"sfatextarea","input":true,"tableView":true,"logic":[]}],"buttons":{"save_label":"Save","save_disable":"true","save_hide":"true","submit_label":"Submit","clear_label":"Clear","next_label":"Next","previous_label":"Previous"},"autoSaving":false}';
    //Formio.createForm(document.getElementById("feedback-content"), JSON.parse(template), {})
    Formio.createForm(document.getElementById("feedback-content"), feedbackFormSchema, {})
        .then((feedbackform) => {
            feedbackform.on('submit', function (submission) {
                var data = {
                    FormId: Syntaq.Form.FormId,
                    FeedbackFormId: Syntaq.Form.FeedbackFormId,
                    FeedbackFormData: JSON.stringify(submission.data),
                    TenantId: Syntaq.TenantId
                };
                if (Syntaq.Form.Anonymous) {
                    data.UserName = submission.data.senderUserName;
                    data.Email = submission.data.senderEmail;
                }
                feedbackSubmit(data);
            });


            function feedbackSubmit(input) {

                abp.ajax({
                    type: "POST",
                    contentType: 'application/json',
                    url: _SyntaqBaseURI + "/api/services/app/FormFeedbacks/SendFeedback",
                    data: JSON.stringify(input)
                }).done(function (result) {
                    if (result) {
                        toggleVisibility();
                        toastr.success('Feedback submitted');
                    } else {
                        toastr.error('Feedback not submitted');
                    }
                }).fail(function () {
                    toastr.error('Feedback not submitted');
                });

            }

        });
}