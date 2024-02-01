$(function () {
    var _startModal = new app.ModalManager({
        viewUrl: abp.appPath + 'Falcon/Projects/StartProjectModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Projects/_StartProjectModal.js',
        modalClass: 'StartProjectModal'
    });

    $('#StartNewProjectButton').click(function (e) {
        e.preventDefault();
        _startModal.open();
    });
});