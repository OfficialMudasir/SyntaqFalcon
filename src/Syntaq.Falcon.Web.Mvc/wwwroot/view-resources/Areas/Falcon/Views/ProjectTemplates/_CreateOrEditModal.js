﻿(function () {
    app.modals.CreateOrEditProjectTemplatesModal = function () {
 
        var _projectsService = abp.services.app.projects;

        // ACL----
        var _userService = abp.services.app.user;
        var _organizationUnitService = abp.services.app.organizationUnit;
        var _assignees;
        // ACL----

        var _modalManager;
 
        function initWizard() {
            // Initialize form wizard
            wizard = new KTWizard('kt_wizard_v2', {
                startStep: 1, // initial active step number
                clickableSteps: true  // allow step clicking
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
                switch (wizard.currentStep) {
                    case 1:
                        {
                            // step 1, check project template name
                            if ($('[name=ProjectTemplateName]').val() === "") {
                                $('[name=ProjectTemplateName]')[0].style.borderColor = 'red';
                                wizard.stop();
                            } else {
                                $('[name=ProjectTemplateName]')[0].style.borderColor = '';
                            }
                            break;
                        }
                    case 2:
                        {
                            var flag = true;
                            // step 2 check steps Name 
                            if ($('[name="Steps[][StepName]"]').length <= 1) {
                                abp.message.warn('', 'Please add a step for this Template');
                                wizard.stop();
                            } else {
                                $('[name="Steps[][StepName]"]').each(function (index) {
                                    if (($('[name="Steps[][StepName]"]').length - 1) !== index && $(this).val() == "") {
                                        $(this)[0].style.borderColor = 'red';
                                        flag = false;
                                    } else {
                                        $(this)[0].style.borderColor = '';
                                    }
                                });
                            }
                            //and Form
                            if ($('[name="Steps[][FormId]"]').length <= 1) {
                                wizard.stop();
                            } else {
                                $('[name="Steps[][FormId]"]').each(function (index) {
                                    if (($('[name="Steps[][FormId]"]').length - 1) !== index && $(this).val() == "") {
                                        $(this)[0].style.borderColor = 'red';
                                        flag = false;
                                    } else {
                                        $(this)[0].style.borderColor = '';
                                    }
                                });
                            }
                            if (!flag) { wizard.stop(); }
                            break;
                        }
                    case 3:
                        {

                        }
                    default:
                }
            }

            $('[name=ProjectTemplateName]').change(() => {
                if ($('[name=ProjectTemplateName]').val() === "") {
                    $('[name=ProjectTemplateName]')[0].style.borderColor = 'red';
                } else {
                    $('[name=ProjectTemplateName]')[0].style.borderColor = '';
                }
            });

            //$('[name="Steps[][StepName]"]').change(() => {
            $('#kt_wizard_form_step_2_form').change(() => {
                $('[name="Steps[][StepName]"]').each(function (index) {
                    if (($('[name="Steps[][StepName]"]').length - 1) !== index && $(this).val() == "") {
                        $(this)[0].style.borderColor = 'red';
                    } else {
                        $(this)[0].style.borderColor = '';
                    }
                });

                $('[name="Steps[][FormId]"]').each(function (index) {
                    if (($('[name="Steps[][FormId]"]').length - 1) !== index && $(this).val() == "") {
                        $(this)[0].style.borderColor = 'red';
                    } else {
                        $(this)[0].style.borderColor = '';
                    }
                });
            });


            // initial hide the submission button
            //$('[data-ktwizard-type="action-save"]').hide();
            function SubmitbuttonShowHide(isLastStep) {
                //if (isLastStep) {
                    $('[data-ktwizard-type="action-save"]').show();
              //  } else {
               //     $('[data-ktwizard-type="action-save"]').hide();
               // }
            }

            //var index = 0;
            // Add Step funcion
            var counter = 1
            $('#btn-add-ProjectStep').off('click.cloneStep');
            $('#btn-add-ProjectStep').on('click.cloneStep', function (wizard) {

                //kt-form__section kt-form__section--first
                var clone = $('#kt_wizard_form_step_2_formblock').clone();
                clone.removeAttr("id");
                
                clone.find('[name="Steps[][Order]"]').each(function () {
                    //console.log("the step is: " + counter);
                    $(this).val(counter);
                    $(this).attr('placeholder', counter);
                    counter++;
                });

                $('#kt_wizard_form_step_2_form').append(clone);
              
            });


            $('[data-ktwizard-type="action-save"]').click(() => {
                //TODO check all input data validated.

                _modalManager.setBusy(true);               

                var setpsJSON = $('#kt_wizard_form_step_2_form').serializeJSON({ useIntKeysAsArrayIndex: false });

                var tags = $('#kt_wizard_form_step_3_tags').serializeArray();

                _projectsService.createOrEditProjectTemplate({
                    id: $('[name=ProjectTemplateId]').val(),
                    name : $('[name=ProjectTemplateName]').val(),
                    description : $('[name=TemplateDescription]').val(),
                    stepsSchema: setpsJSON.Steps,
                    enabled: Boolean($('[name=TemplateEnable]').is(':checked')),
                    assignees: $('#Assignees').tagsinput('items').length == 0 ? null : $('#Assignees').tagsinput('items'),
                    tags: tags
                }).done(function () {
                    _modalManager.close();
				    $('#savedTemplateName').text($('[name=ProjectTemplateName]').val());
                    $('#SetAliveHeaderMessage').text("Project template: " + $('[name=ProjectTemplateName]').val() +' is successfully saved.');
                    $("#setLiveToast").addClass("show");
                    abp.event.trigger('app.createOrEditProjectTemplatesModalSaved');
                }).always(function () {
                    _modalManager.setBusy(false);
                });

            });
        }

        this.init = async function (modalManager) {

            _modalManager = modalManager;

            _$modalDialog = _modalManager.getModal().find('.modal-dialog');
            _$modalDialog.css("max-width", "90em");

            var modal = _modalManager.getModal();
            //modal.find('.date-picker').datetimepicker({
            //    locale: abp.localization.currentLanguage.name,
            //    format: 'L'
            //});

            initWizard();

            // ACL
            var _localSource = [];
            var users = await _userService.getUsers({ filter: null, permission: null, role: null, onlyLockedUsers: false });
            //var users = await _userService.getUsersForSharing({ filter: null, permission: null, role: null, onlyLockedUsers: false });

            $(users.items).each(function () {
                var user = { value: "" + this.userName + "", type: "User", id: this.id };
                _localSource.push(user);
            });

            var teams = await _organizationUnitService.getOrganizationUnits();

            $(teams.items).each(function () {
                var team = { value: "" + this.displayName + "", type: "Team", id: this.id };
                _localSource.push(team);
            });

            //console.log(_localSource);

            _assignees = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: _localSource
            });
            _assignees.initialize();

            var elt = $('.tagsinput-typeahead');
            elt.tagsinput({
                itemValue: 'value',
                itemText: 'value',
                typeaheadjs: {
                    name: 'assignees',
                    displayKey: 'value',
                    source: _assignees.ttAdapter()
                }
            });

            $(elt).each(function (index) {
                var ctrl = this;
                var value = $(this).attr('data-value');
                if (value) {
                    var obj = JSON.parse(value);
                    jQuery.each(obj, function () {
                        $(ctrl).tagsinput('add', this);
                    });
                }
            });
        }
    }
})(jQuery);