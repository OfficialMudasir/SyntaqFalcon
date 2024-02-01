(function ($) {

    const params = new URLSearchParams(window.location.search)
    let returnurl = params.get('ReturnUrl')

    //@if (Model.Count == 0) {
    //    @Html.Raw("window.location.replace(returnurl);");
    //}

    var userAcceptanceDialogWarning = '@L("UserAcceptanceDialogWarning")';

    function save() {

        var checked = $('#userAcceptanceChk').is(':checked');

        if (checked === false) {
            alert(userAcceptanceDialogWarning);
            return false;
        };

        // manually build JSON
        var data = [];
        $(".acceptance-type").each(function (index) {
            var uatId = $(this).find('[name="UserAcceptanceTypeId"]').val();
            var rmcId = $(this).find('[name="RecordMatterContributorId"]').val();
            data[index] = { "userAcceptanceTypeId": uatId, "recordMatterContributorId": rmcId };
        });

        var _userAcceptancesService = abp.services.app.userAcceptances;
        _userAcceptancesService.accept(
            data
        ).done(function () {
            window.location.replace(returnurl);
            abp.event.trigger('app.UserAcceptanceCompleted');
        }).always(function () {
            _modalManager.setBusy(false);
        });
    };

    $("#acceptAllBtn").click(function () {
        save();
    });

    $('#acceptanceModal').modal('show');

    function showModal(id) {
        var modal = $('#dialog_' + id);
        $(modal).modal('show');
    }

})(jQuery);