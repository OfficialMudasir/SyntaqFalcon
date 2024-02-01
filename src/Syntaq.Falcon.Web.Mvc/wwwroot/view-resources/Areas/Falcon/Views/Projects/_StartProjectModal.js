(function () {
    app.modals.StartProjectModal = function () {

        $('#releaseContent').hide();

        var _projectsService = abp.services.app.projects;
        var _tagValuesService = abp.services.app.tagValues;

        var _$formTable = $('#PtTable');
        var _modalManager;

        $('[data-bs-toggle="tooltip"]').tooltip();

        $('#clearbtnModal').click(function (e) {
            $('#searchTable').val('');
            getUserProjects();
        });
        var tags;
        var dataTable = _$formTable.DataTable({
            paging: false,
            serverSide: true,
            processing: true,
            "searching": true,
            ordering: false,
            createdRow: function (row, data, dataIndex) {
                $(row).find("#startProjectButton").on("click", function () {
                    setInput(data.project.releaseId, data.project.id, data.project.releaseNotes, data.project.releaseDate, data.project.versionMajor, data.project.versionMinor, data.project.versionRevision);
                    $(row).siblings().removeClass('focusedRow');
                    row.setAttribute('class', 'focusedRow');
                });
            },
            listAction: {
                ajaxFunction: _projectsService.getForUser,
                inputFilter: function () {
                    tags = $('#tagValues').serializeArray();

                    return {
                        tags: JSON.stringify(tags),
                        filter: $('#searchTable').val()
                    };
                }
            },
            columnDefs: [
                {
                    targets: 0,
                    data: "name",
                    render: function (data, type, row) {
         
                        data = `<div><b>${row.project.name}</b></div><div>${decode(row.project.description)}</div>`;

                        if (row.project.projectEnvironmentType !== 3) {

                            var badge = row.project.projectEnvironmentType == 0 ? 'danger' :
                                        row.project.projectEnvironmentType == 1 ? 'warning' :
                                        row.project.projectEnvironmentType == 2 ? 'primary' : 'success';

                            data = `${data} <span class="badge badge-${badge}"> ${row.project.projectEnvironmentName}</span>`
                        }

                        return data;
                    }
                },//,
                {
                    targets: 1,
                    data: null,
                    render: function (data, type, row) {
                        data = "<div class='pull-right'><button id='startProjectButton'  style='pointer:default' class='btn btn-secondary btn-sm' type='button'>" + app.localize('Select') + "</button ></div>";
                        return data;
                    }
                }
            ]
        });

        function decode(str) {
            let txt = document.createElement("textarea");
            txt.innerHTML = str;
            return txt.value;
        }

        function checkInputValidation(wizard) {

            if ($('#ProjectNameInput').val() === "") {
                $('#ProjectNameInput')[0].style.borderColor = 'red';
                abp.notify.error(app.localize('Project Name Required'));
                return false;
            } else {
                $('#ProjectNameInput')[0].style.borderColor = '';
                return true;
            }
        }

        function htmlDecode(input) {
            var e = document.createElement('div');
            e.innerHTML = input;
            return e.childNodes[0].nodeValue;
        }

        //data.project.releaseId, data.project.id, data.project.notes, data.project.versionMajor, data.project.versionMinor, data.project.versionRevision
        function setInput(rId, pId, rnotes, rdate, rvmajor, rvminor, rvrevision) {
     
            $('[name="selectProjectId"]').val(pId);
            $('[name="selectReleaseId"]').val(rId);

            let notes = document.getElementById("releaseNotes");
            notes.innerHTML = htmlDecode(rnotes);
 
            var dt = new Date(rdate);
            var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            var tmoptions = { hour: 'numeric', minute: 'numeric' };
 

            $('#releaseDate').text(dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions) );
            $('#releaseVersionMajor').text(rvmajor);
            $('#releaseVersionMinor').text(rvminor);
            $('#releaseVersionRevision').text(rvrevision);
            $('#btn-next').click();
        }

        $('[data-ktwizard-type="action-save"]').click(() => {
            if (checkInputValidation()) {

                var pId = $('[name="selectProjectId"]').val(); //ptid
                var rId = $('[name="selectReleaseId"]').val(); //ptid
                var _projectsService = abp.services.app.projects;
                var projectname = $('#ProjectNameInput').val(); //p name
                var projDescript = $('[name="TemplateDescription"]').val();

                abp.notify.info(app.localize('StartingProject'));
                abp.ui.setBusy($('.modal-content'));

                _projectsService.startProject(
                    rId, pId, projectname, projDescript
                ).done(function (returnurl) {

                    location.replace(returnurl);
                }).always(function () {
                    _modalManager.setBusy(false);
                });

            }
        });
 
        function _initWizard() {
            // Initialize form wizard
            _wizardObj = new KTWizard('kt_wizard_v4', {
                startStep: 1, // initial active step number
                clickableSteps: false  // allow step clicking
            });

            _wizardObj.on('change', function (wizard) {
                KTUtil.scrollTop();
                SubmitbuttonShowHide(wizard.isLastStep());
            });
            $('[data-ktwizard-type="action-save"]').hide();
        }

        function SubmitbuttonShowHide(isLastStep) {
            if (isLastStep) {
                $('[data-ktwizard-type="action-save"]').show();
            } else {
                $('[data-ktwizard-type="action-save"]').hide();
            }
        }


        function getUserProjects() {
            dataTable.ajax.reload();
        }
        $('#searchTableModalButton').click(function (e) {
            getUserProjects();

        });

        $('.kt-checkbox').on('change', function (e) {
            getUserProjects();
        });
 
        $(document).on('keyup', '#searchTable', function (e) {
            if (e.which == 13) {  
                getUserProjects();
                return false;
            }
        });


        this.init = async function (modalManager) {
            _modalManager = modalManager;

            _$modalDialog = _modalManager.getModal().find('.modal-dialog');
            //  _$modalDialog.css("max-width", "90em");

            var modal = _modalManager.getModal();
            /*            modal.find('.date-picker').datetimepicker({
                            locale: abp.localization.currentLanguage.name,
                            format: 'L'
                        });*/

            _initWizard();

        }

        $("#PtTable").keydown(function (e) {
            if (e.which == 13) {
                if (e.target.nodeName == "BUTTON") {
                    $(e.target).trigger("click");
                }
            }
        });

        $("button[data-ktwizard-type='action-prev']").keydown(function (e) {
            if (e.which == 13) {
                $(e.target).trigger("click");
            }
        });

        $("button[data-ktwizard-type='action-save']").keydown(function (e) {
            if (e.which == 13) {
                $(e.target).trigger("click");
            }
        });

        $(".kt-checkbox.mb-2").keydown(function (e) {
            if (e.which == 13) {
                $(this).trigger("click");
            }
        });
    }
})(jQuery);