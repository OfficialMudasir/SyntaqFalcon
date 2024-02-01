﻿(function () {
    $(function () {

        var _projectsService = abp.services.app.projects;
        var _recordMatterContributorsService = abp.services.app.recordMatterContributors;

        var _$projectInformationForm = $('form[name=ProjectInformationsForm]');
        _$projectInformationForm.validate();

        var _ProjectrecordLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Projects/RecordLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Projects/_ProjectRecordLookupTableModal.js',
            modalClass: 'RecordLookupTableModal'
        });

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        $('#OpenRecordLookupTableButton').click(function () {

            var project = _$projectInformationForm.serializeFormToObject();

            _ProjectrecordLookupTableModal.open({ id: project.recordId, displayName: project.recordRecordName }, function (data) {
                _$projectInformationForm.find('input[name=recordRecordName]').val(data.displayName);
                _$projectInformationForm.find('input[name=recordId]').val(data.id);
            });
        });

        $('#ClearRecordRecordNameButton').click(function () {
            _$projectInformationForm.find('input[name=recordRecordName]').val('');
            _$projectInformationForm.find('input[name=recordId]').val('');
        });

        function save(successCallback) {
            if (!_$projectInformationForm.valid()) {
                return;
            }
            if ($('#Project_RecordId').prop('required') && $('#Project_RecordId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('Record')));
                return;
            }

            var project = _$projectInformationForm.serializeFormToObject();
            abp.ui.setBusy();
            _projectsService.createOrEdit(
                project
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                abp.event.trigger('app.createOrEditProjectModalSaved');

                if (typeof (successCallback) === 'function') {
                    successCallback();
                }
            }).always(function () {
                abp.ui.clearBusy();
            });
        };

        function clearForm() {
            _$projectInformationForm[0].reset();
        }

        $('.save-button').click(function () {
            save(function () {
                window.location = "/Falcon/Projects";
            });
        });

        $('#saveAndNewBtn').click(function () {
            save(function () {
                if (!$('input[name=id]').val()) {//if it is create page
                    clearForm();
                }
            });
        });

        //abp.event.on('app.createOrEditRecordMatterContributorModalSaved', function () {
        //    getRecordMatterContributors();
        //});

        $('#btnUpgrade').click(function () {

            var releaseId = $('#latestReleaseId').val();
            var projectId = '00000000-0000-0000-0000-000000000000';
            var projectName = $('#ProjectName').val();
            var projectDesc = $('#ProjectDescription').val();
            var recordId = $('#ProjectRecordId').val();

            _projectsService.startProject(
                releaseId, projectId, projectName, projectDesc, recordId
            ).done(function (returnurl) {
                location.replace(returnurl);
            }).always(function () {
                _modalManager.setBusy(false);
            });

        });

    });
})();