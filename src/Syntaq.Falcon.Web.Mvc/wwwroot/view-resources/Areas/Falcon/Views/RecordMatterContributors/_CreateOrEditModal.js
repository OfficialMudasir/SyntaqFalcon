﻿(function ($) {
    app.modals.CreateOrEditRecordMatterContributorModal = function () {

        var _recordMatterContributorsService = abp.services.app.recordMatterContributors;

        var _modalManager;
        var _$recordMatterContributorInformationForm = null;

        var _RecordMatterContributorrecordMatterLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/RecordMatterLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterContributors/_RecordMatterContributorRecordMatterLookupTableModal.js',
            modalClass: 'RecordMatterLookupTableModal'
        });

        var _RecordMatterContributoruserLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/UserLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterContributors/_RecordMatterContributorUserLookupTableModal.js',
            modalClass: 'UserLookupTableModal'
        });

        var _RecordMatterContributorformLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/FormLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterContributors/_RecordMatterContributorFormLookupTableModal.js',
            modalClass: 'FormLookupTableModal'
        });

        function initWizard() {
            // Initialize form wizard
            wizard = new KTWizard('kt_wizard_v4', {
                startStep: 1, // initial active step number
                clickableSteps: false  // allow step clicking
            });
            // Change event
            wizard.on('change', function (wizard) {
                KTUtil.scrollTop();
                SubmitbuttonShowHide(wizard.isLastStep());
            });

            wizard.on('beforeNext', function (wizard) {
                //Check current requrie input fiels.
                checkInputValidation(wizard);

            });


            function checkInputValidation(wizard) {
                debugger;
                switch (wizard.currentStep) {
                    case 1:
                        {
                            if ($("[name*=inviteType]")[0].checked) {
                                //invite User
                                if ($('#UserName').val().trim() == "") {
                                    $('#UserName')[0].style.borderColor = 'red';
                                    //$('#UserNameEmail')[0].style.borderColor = 'red';
                                    wizard.stop();
                                } else {
                                    $('#UserName')[0].style.borderColor = '';
                                    //$('#UserNameEmail')[0].style.borderColor = '';
                                }
                            }
                            else {
                                if ($('#RecordMatterContributor_Name').val().trim() == "") {
                                    $('#RecordMatterContributor_Name')[0].style.borderColor = 'red';
                                    wizard.stop();
                                } else {
                                    $('#RecordMatterContributor_Name')[0].style.borderColor = '';
                                }

                            }
                            if (!checkEmailValidation($('#RecordMatterContributor_Email').val())) {
                                $('#RecordMatterContributor_Email')[0].style.borderColor = 'red';
                                wizard.stop();
                            } else {
                                $('#RecordMatterContributor_Email')[0].style.borderColor = '';
                            }
                            break;
                        }
                    case 2:
                        {
                        }
                    case 3:
                        {

                        }
                    default:
                }
            }

            function checkEmailValidation(email) {
                return /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/.test(email);
            }

            $('#RecordMatterContributor_Name').change(() => {
                if ($('#RecordMatterContributor_Name').val().trim() == "") {
                    $('#RecordMatterContributor_Name')[0].style.borderColor = 'red';
                } else {
                    $('#RecordMatterContributor_Name')[0].style.borderColor = '';
                }
            });

            $('#RecordMatterContributor_Email').change(() => {
                if ($('#RecordMatterContributor_Email').val() == "") {
                    $('#RecordMatterContributor_Email')[0].style.borderColor = 'red';
                } else {
                    $('#RecordMatterContributor_Email')[0].style.borderColor = '';
                }
            });


            // initial hide the submission button
            $('[data-ktwizard-type="action-save"]').hide();
            function SubmitbuttonShowHide(isLastStep) {
                if (isLastStep) {
                    $('[data-ktwizard-type="action-save"]').show();
                } else {
                    $('[data-ktwizard-type="action-save"]').hide();
                }
            }
        }


        this.init = function (modalManager) {
            _modalManager = modalManager;
            _$modalDialog = _modalManager.getModal().find('.modal-dialog');
            // _$modalDialog.css("max-width", "90em");

            var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$recordMatterContributorInformationForm = _modalManager.getModal().find('form[name=RecordMatterContributorInformationsForm]');
            _$recordMatterContributorInformationForm.validate();

            // initWizard();
        };

        $('#OpenRecordMatterLookupTableButton').click(function () {

            var recordMatterContributor = _$recordMatterContributorInformationForm.serializeFormToObject();

            _RecordMatterContributorrecordMatterLookupTableModal.open({ id: recordMatterContributor.recordMatterId, displayName: recordMatterContributor.recordMatterRecordMatterName }, function (data) {
                _$recordMatterContributorInformationForm.find('input[name=recordMatterRecordMatterName]').val(data.displayName);
                _$recordMatterContributorInformationForm.find('input[name=recordMatterId]').val(data.id);
            });
        });

        $('#ClearRecordMatterRecordMatterNameButton').click(function () {
            _$recordMatterContributorInformationForm.find('input[name=recordMatterRecordMatterName]').val('');
            _$recordMatterContributorInformationForm.find('input[name=recordMatterId]').val('');
        });

        $('#OpenUserLookupTableButton').click(function () {
            //$("div[role=modal]")[0].style.display = "none";
            var recordMatterContributor = _$recordMatterContributorInformationForm.serializeFormToObject();

            _RecordMatterContributoruserLookupTableModal.open({ id: recordMatterContributor.userId, displayName: recordMatterContributor.userName }, function (data) {
                //$("div[role=modal]")[0].style.display = "block";
                _$recordMatterContributorInformationForm.find('input[name=userName]').val(data.displayName === '' ? data.email : data.displayName);
                //_$recordMatterContributorInformationForm.find('input[name="userNameEmail"]').val(data.email);
                $('#UserName')[0].style.borderColor = '';
                _$recordMatterContributorInformationForm.find('input[name=userId]').val(data.id);
                _$recordMatterContributorInformationForm.find('input[name=email]').val(data.email);
                $('#RecordMatterContributor_Email')[0].style.borderColor = '';
                _$recordMatterContributorInformationForm.find('input[name=organizationName]').val(data.entity);
            });
        });

        $('#ClearUserNameButton').click(function () {
            _$recordMatterContributorInformationForm.find('input[name=userName]').val('');
            _$recordMatterContributorInformationForm.find('input[name=userId]').val('');
            _$recordMatterContributorInformationForm.find('input[name=email]').val('');
            _$recordMatterContributorInformationForm.find('input[name=organizationName]').val('');
        });

        $('#OpenFormLookupTableButton').click(function () {

            var recordMatterContributor = _$recordMatterContributorInformationForm.serializeFormToObject();

            _RecordMatterContributorformLookupTableModal.open({ id: recordMatterContributor.formId, displayName: recordMatterContributor.formName }, function (data) {
                _$recordMatterContributorInformationForm.find('input[name=formName]').val(data.displayName);
                _$recordMatterContributorInformationForm.find('input[name=formId]').val(data.id);
            });
        });

        $('#ClearFormNameButton').click(function () {
            _$recordMatterContributorInformationForm.find('input[name=formName]').val('');
            _$recordMatterContributorInformationForm.find('input[name=formId]').val('');
        });

        this.save = function () {

            if (!_$recordMatterContributorInformationForm.valid()) {
                return;
            }
            if ($('#RecordMatterContributor_RecordMatterId').prop('required') && $('#RecordMatterContributor_RecordMatterId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('RecordMatter')));
                return;
            }
            if ($('#RecordMatterContributor_UserId').prop('required') && $('#RecordMatterContributor_UserId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('User')));
                return;
            }
            if ($('#RecordMatterContributor_FormId').prop('required') && $('#RecordMatterContributor_FormId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('Form')));
                return;
            }

            var recordMatterContributor = _$recordMatterContributorInformationForm.serializeFormToObject();
            recordMatterContributor.FormPages = $('[name=FormPages]:checked').serialize();

            recordMatterContributor.Enabled = true;
            recordMatterContributor.name = $('[name$="inviteType"]')[0].checked ? $('#UserName').val() : recordMatterContributor.name;

            var recordMatterId = $('#RecordMatterContributor_RecordMatterId').val();
            console.log("new record matter Id");
            console.log(recordMatterId);

            abp.ui.setBusy('.modal');
            _modalManager.setBusy(true);
            abp.ui.block();

            recordMatterContributor.time = new Date();

            _recordMatterContributorsService.createOrEdit(
                recordMatterContributor
            ).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                abp.event.trigger('app.createOrEditRecordMatterContributorSaved', recordMatterId);
                $('#btn-contributor-publish').trigger('click');
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
                abp.ui.unblock();
                //abp.ui.clearBusy();
            });
        };


        $('[data-ktwizard-type="action-save"]').click(() => {

            this.save();
        });

        $('[name$="inviteType"]').on("click", function () {

            if ($('[name$="inviteType"]')[0].checked) {
                $('#inviteByEmail').hide();
                // $('#UserEmailShowHide').show();
                $('#inviteUser').show();
            }
            else {
                //$('#UserEmailShowHide').show();
                $('#inviteByEmail').show();
                $('#inviteUser').hide();
            }
            $('#UserName').val('');
            $('#RecordMatterContributor_UserId').val('');
            $('#RecordMatterContributor_OrganizationName').val('');
            $('#RecordMatterContributor_Email').val('');
            $('#RecordMatterContributor_Email')[0].style.borderColor = '';
            $('#RecordMatterContributor_Name')[0].style.borderColor = '';
        });

        $('#btn-selete-pages').click(() => {

            if ($('#btn-selete-pages')[0].text == "Select All") {
                $("[name=FormPages]").each((i) => {
                    $("[name=FormPages]")[i].checked = true;
                });
                $('#btn-selete-pages')[0].text = "Unselect All";
            } else {
                $("[name=FormPages]").each((i) => {
                    $("[name=FormPages]")[i].checked = false;
                });
                $('#btn-selete-pages')[0].text = "Select All";
            }
        });
        $("[name=FormPages]").on("change", () => {
            var checkedAll = true;
            $("[name=FormPages]").each((i) => {
                if (!$("[name=FormPages]")[i].checked) {
                    checkedAll = false;
                    return false;
                }
            });
            if (checkedAll) {
                $('#btn-selete-pages')[0].text = "Unselect All";
            } else {
                $('#btn-selete-pages')[0].text = "Select All";
            }
        });

        this.setInvite = function () {

            $('#setInvite').hide();

        };

    };
})(jQuery); 