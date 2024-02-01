(function ($) {
    app.modals.CreateOrEditAppJobModal = function () {

        var _appJobsService = abp.services.app.appJobs;
        var _userService = abp.services.app.user;
        var _organizationUnitService = abp.services.app.organizationUnit;

        var _assignees;
        var _modalManager;
        var _$appJobInformationForm = null;

        var _$recordLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatters/RecordLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatters/_RecordLookupTableModal.js',
            modalClass: 'RecordLookupTableModal'
        });

        var _$recordMatterLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItems/RecordMatterLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItems/_RecordMatterLookupTableModal.js',
            modalClass: 'RecordMatterLookupTableModal'
        });

        var _$foldersLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Folders/FoldersLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Folders/_FoldersLookupTableModal.js',
            modalClass: 'FoldersLookupTableModal'
        });

        var _$templateLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Templates/TemplateLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Templates/_TemplateLookupTableModal.js',
            modalClass: 'TemplateLookupTableModal'
        });

        function initWizard() {
            // Initialize form wizard
            wizard = new KTWizard('kt_wizard_v2', {
                startStep: 1, // initial active step number
                clickableSteps: true  // allow step clicking
            });

            // Change event
            wizard.on('change', function (wizard) {
                KTUtil.scrollTop();
            });

            //$('#btn-add-DocumentTemplate').click(function () {
            $('#btn-add-DocumentTemplate').off('click.cloneDocTemp');
            $('#btn-add-DocumentTemplate').on('click.cloneDocTemp', function () {

                var clone = $('#kt_wizard_form_step_1_formblock').clone();
                clone.removeAttr("id");

                $("input", clone).each(function (index) {
                    //$(this).val('');
                });

                var IdElements = $("[name='Document[][DocumentId]']");
                var IdElementsCopy = IdElements.clone();
                IdElementsCopy.splice(-1, 1);
                var IdElementsCount = IdElementsCopy.length;
                $(clone).find("[name='Document[][DocumentId]']").val(IdElementsCount + 1);

                $('#kt_wizard_form_step_1_form').append(clone);


                var elt = $('.allow-word-assignees, .allow-pdf-assignees, .allow-html-assignees', clone);
                $(elt).each(function (index) {
                    var ctrl = this;
                    $(this).attr('data-role', 'tagsinput');
                    $(this).addClass('tagsinput-typeahead');
                });

                $(elt).tagsinput({
                    itemValue: 'value',
                    itemText: 'value',
                    typeaheadjs: {
                        name: 'assignees',
                        displayKey: 'value',
                        source: _assignees.ttAdapter()
                    }
                });
 
            });

            //$('#btn-add-RecordMatter').click(function () {
            $('#btn-add-RecordMatter').off('click.cloneRecMat');
            $('#btn-add-RecordMatter').on('click.cloneRecMat', function () {

                var clone = $('#kt_wizard_form_step_2_formblock').clone(false);
                clone.removeAttr("id");

                $("input", clone).each(function (index) {

                });

                setLookupModalEvents(clone);

                $('#kt_wizard_form_step_2_form').append(clone);

                var elt = $('#Assignees', clone);
                $(elt).each(function (index) {
                    var ctrl = this;
                    $(this).attr('data-role', 'tagsinput');
                    $(this).addClass('tagsinput-typeahead');
                });
                elt.tagsinput({
                    itemValue: 'value',
                    itemText: 'value',
                    typeaheadjs: {
                        name: 'assignees',
                        displayKey: 'value',
                        source: _assignees.ttAdapter()
                    }
                });
            });

            //$('#btn-add-BeforeAssembly').click(function () {
            $('#btn-add-BeforeAssembly').off('click.cloneBeforeAss');
            $('#btn-add-BeforeAssembly').on('click.cloneBeforeAss', function () {

                var clone = $('#kt_wizard_form_step_3_BeforeAssembly_formblock').clone();
                clone.removeAttr("id");

                $("input", clone).each(function (index) {
                    // $(this).val('');
                });

                $('#kt_wizard_form_step_3_BeforeAssembly').append(clone);

            });

            //$('#btn-add-AfterAssembly').click(function () {
            $('#btn-add-AfterAssembly').off('click.cloneAfterAss');
            $('#btn-add-AfterAssembly').on('click.cloneAfterAss', function () {

                var clone = $('#kt_wizard_form_step_3_AfterAssembly_formblock').clone();
                clone.removeAttr("id");

                $("input", clone).each(function (index) {
                    // $(this).val('');
                });

                $('#kt_wizard_form_step_3_AfterAssembly').append(clone);

            });

            //$('#btn-add-Email').click(function () {
            $('#btn-add-Email').off('click.cloneEmail');

            $('#btn-add-Email').on('click.cloneEmail', function () {

                var clone = $('#kt_wizard_form_step_3_Email_formblock').clone();
                clone.removeAttr("id");

                $("input", clone).each(function (index) {
                    // $(this).val('');
                });

                setLookupModalEvents(clone);

                $('#kt_wizard_form_step_3_Email').append(clone);

                // ZZZ
                var detl = $('[name*=DocumentAttachmentIds]', clone);
                $(detl).each(function (index) {
                    var ctrl = this;
                    $(this).attr('data-role', 'tagsinput');
                });

                detl.tagsinput({
                    itemValue: 'value',
                    itemText: 'text'
                });

                var fetl = $('[name*=EmailBodyDocumentIds]', clone);
                $(fetl).each(function (index) {
                    var ctrl = this;
                    $(this).attr('data-role', 'tagsinput');
                });

                fetl.tagsinput({
                    itemValue: 'value',
                    itemText: 'text'
                });

                $("[name='WorkFlow[]Email[][EmailType]']").change(function () {
                    var EmailBodyType = this.value;
                    switch (EmailBodyType) {
                        case "From HTML source":
                            $(this).closest(".formblock-email").find("[id='HTMLEmailSource']").show();
                            $(this).closest(".formblock-email").find("[id='HTMLEmailPart']").show();
                            $(this).closest(".formblock-email").find("[id='PlainEmail']").hide();
                            $(this).closest(".formblock-email").find("#DocumentEmail").hide();
                            break;
                        case "Plain Email Body":
                            $(this).closest(".formblock-email").find("[id='HTMLEmailSource']").hide();
                            $(this).closest(".formblock-email").find("[id='HTMLEmailPart']").hide();
                            $(this).closest(".formblock-email").find("[id='PlainEmail']").show();
                            $(this).closest(".formblock-email").find("#DocumentEmail").hide();
                            break;
                        case "From Assembled Document":
                            $(this).closest(".formblock-email").find("[id='HTMLEmailSource']").hide();
                            $(this).closest(".formblock-email").find("[id='HTMLEmailPart']").hide();
                            $(this).closest(".formblock-email").find("[id='PlainEmail']").hide();
                            $(this).closest(".formblock-email").find("#DocumentEmail").show();
                            break;
                    }
                });

                $("[name='WorkFlow[]Email[][EmailType]']").trigger("change");

            });

            $('.close-button').click(function () {
                $('#appJobModal').modal('show');
            });

            $('[data-ktwizard-type="action-save"]').click(function () {

                _modalManager.setBusy(true);

                var user = $('#kt_wizard_form_step_user_form').serializeJSON({ useIntKeysAsArrayIndex: false });
                var documents = $('#kt_wizard_form_step_1_form').serializeJSON({ useIntKeysAsArrayIndex: false });
                var recordmatters = $('#kt_wizard_form_step_2_form').serializeJSON({ useIntKeysAsArrayIndex: false });

                if (JSON.stringify(recordmatters) !== "{}") {

                    var step2 = $('#kt_wizard_form_step_2_form');

                    if (recordmatters.RecordMatter.length === 1) {
                        recordmatters.RecordMatter[0].Assignees = $(".tagsinput-typeahead", step2).tagsinput('items');
                    } else if (recordmatters.RecordMatter.length > 1) {
                        var AssigneeList = $(".tagsinput-typeahead", step2).tagsinput('items');
                        $.each(AssigneeList, function (j, value) {
                            recordmatters.RecordMatter[j].Assignees = value;
                        });
                    }
                }

                if (JSON.stringify(documents) !== "{}") {

                    var docStep = $('#kt_wizard_form_step_1_form');

                    if (documents.Document.length === 1) {

                        documents.Document[0].AllowWordAssignees = $(".allow-word-assignees", docStep).tagsinput('items');
                        documents.Document[0].AllowPdfAssignees = $(".allow-pdf-assignees", docStep).tagsinput('items');
                        documents.Document[0].AllowHtmlAssignees = $(".allow-html-assignees", docStep).tagsinput('items');

                    } else if (documents.Document.length > 1) {
                        var AssigneeList1 = $(".allow-word-assignees", docStep).tagsinput('items');
                        $.each(AssigneeList1, function (j, value) {
                            documents.Document[j].AllowWordAssignees = value;
                        });

                        AssigneeList1 = $(".allow-pdf-assignees", docStep).tagsinput('items');
                        $.each(AssigneeList1, function (j, value) {
                            documents.Document[j].AllowPdfAssignees = value;
                        });

                        AssigneeList1 = $(".allow-html-assignees", docStep).tagsinput('items');
                        $.each(AssigneeList1, function (j, value) {
                            documents.Document[j].AllowHtmlAssignees = value;
                        });

                    }
                }

                var workflow = $('#kt_wizard_form_step_workflow_form').serializeJSON({ useIntKeysAsArrayIndex: false });
                //var redirect = $('#kt_wizard_form_step_7_form').serializeJSON({ useIntKeysAsArrayIndex: false });
                 
                var deleteRecordsAfterAssembly = $('#DeleteRecordsAfterAssembly')[0].checked;
 

                _appJobsService.createOrEdit({
                    id: $('[name=JobId]').val(),
                    appid: $('[name=AppId]').val(),
                    entityid: $('[name=EntityId]').val(),
                    user: user,
                    name: $('[name=JobName]').val(),
                    deleteRecordsAfterAssembly: deleteRecordsAfterAssembly,
                    document: documents.Document,
                    recordmatter: recordmatters.RecordMatter,
                    workflow: workflow.WorkFlow,
                    sendWebhooksAfterAssembly: $("#SendWebhooksAfterAssembly:checked").length
                    /*,
					redirecturl: $('[name=RedirectURL]').val()*/
                }).done(function () {
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    _modalManager.close();

                    $('#appJobModal').modal('show');

                    abp.event.trigger('app.createOrEditAppJobModal');
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
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$appJobInformationForm = _modalManager.getModal().find('form[name=AppJobInformationsForm]');
            _$appJobInformationForm.validate();

            initWizard();

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

            var uelt = $('[data-role="tagsinput"]');
            uelt.tagsinput({
                itemValue: 'value',
                itemText: 'text'
            });

            $(uelt).each(function (index) {
                var ctrl = this;
                var value = $(this).attr('data-value');
                if (value) {
                    var obj = JSON.parse(value);
                    jQuery.each(obj, function () {
                        $(ctrl).tagsinput('add', this);
                    });
                }
            });
        };

        var _selectedrecord;
        function setLookupModalEvents(context) {
            $('.OpenUserLookupTableButton', context).click(function () {
                var elt = $(this).closest('.formblock-record').find('.uidtagsinput');
                _$userLookupTableModal.open({ id: '' },
                    function (data) {
                        var json = '{"value": ' + data.id + ', "text" : "' + data.userName + '"}';
                        var obj = JSON.parse(json);
                        $(elt).tagsinput('add', obj);
                    });
            });

            $('.OpenTeamsLookupTableButton', context).click(function () {
                var elt = $(this).closest('.formblock-record').find('.tidtagsinput');
                _$teamLookupTableModal.open({ id: '' },
                    function (data) {
                        var json = '{"value": ' + data.id + ', "text" : "' + data.displayName + '"}';
                        var obj = JSON.parse(json);
                        $(elt).tagsinput('add', obj);
                    });
            });


            // HERE
            $('.OpenRecordLookupTableButton', context).click(function () {
                _selectedrecord = $(this).closest('.formblock');
                _$recordLookupTableModal.open({ id: '9E372214-B1D3-47CC-48F6-08D62F2A232A', displayName: '' },
                    function (data) {
                        $('[name="RecordMatter[][RecordName]"]', _selectedrecord).val(data.displayName);
                        $('[name="RecordMatter[][RecordId]"]', _selectedrecord).val(data.id);
                    });

            });

            $('.OpenRecordMatterLookupTableButton', context).click(function () {
                _selectedrecord = $(this).closest('.formblock');
                _$recordMatterLookupTableModal.open({ id: '9E372214-B1D3-47CC-48F6-08D62F2A232A', displayName: '' },
                    function (data) {
                        $('[name="RecordMatter[][RecordMatterName]"]', _selectedrecord).val(data.displayName);
                        $('[name="RecordMatter[][RecordMatterId]"]', _selectedrecord).val(data.id);
                        $('[name="RecordMatter[][RecordMatterGroupId]"]', _selectedrecord).val(data.id);
                    });
            });

            $('.OpenFormsFolderLookupTableButton', context).click(function () {
                _selectedrecord = $(this).closest('.formblock');
                _$foldersLookupTableModal.open({ id: '9E372214-B1D3-47CC-48F6-08D62F2A232A', displayName: '' }, function (data) {
                    $('[name="RecordMatter[][FolderName]"]', _selectedrecord).val(data.displayName);
                    $('[name="RecordMatter[][FolderId]"]', _selectedrecord).val(data.id);
                });
            });

            $('.OpenDocumentAttachmentLookupTableButton', context).click(function () {
                var elt = $(this).closest('.formblock-email').find(".form-group").find(".input-group").find('.didtagsinput');
                _$templateLookupTableModal.open({},
                    function (data) {
                        var json = '{"value": "' + data.DocumentId + "|" + data.DocumentName + '", "text" : "' + data.DocumentName +  '"}';
                        var obj = JSON.parse(json);
                        $(elt).tagsinput('add', obj);
                    });
            });

            $('.OpenEmailBodyDocumentLookupTableButton', context).click(function () {
                var elt = $(this).closest('.formblock-email').find(".form-group").find(".input-group").find('.ebidtagsinput');
                _$templateLookupTableModal.open({},
                    function (data) {
                        var json = '{"value": ' + data.DocumentId + ', "text" : "' + data.DocumentName + '"}';
                        var obj = JSON.parse(json);
                        $(elt).tagsinput('add', obj);
                    });
            });
        }
        setLookupModalEvents(null);

        $("[name='WorkFlow[]Email[][EmailType]']").change(function () {
            var EmailBodyType = this.value;
            switch (EmailBodyType) {
                case "From HTML source":
                    $(this).closest(".formblock-email").find("[id='HTMLEmailSource']").show();
                    $(this).closest(".formblock-email").find("[id='HTMLEmailPart']").show();
                    $(this).closest(".formblock-email").find("[id='PlainEmail']").hide();
                    $(this).closest(".formblock-email").find("#DocumentEmail").hide();
                    break;
                case "Plain Email Body":
                    $(this).closest(".formblock-email").find("[id='HTMLEmailSource']").hide();
                    $(this).closest(".formblock-email").find("[id='HTMLEmailPart']").hide();
                    $(this).closest(".formblock-email").find("[id='PlainEmail']").show();
                    $(this).closest(".formblock-email").find("#DocumentEmail").hide();
                    break;
                case "From Assembled Document":
                    $(this).closest(".formblock-email").find("[id='HTMLEmailSource']").hide();
                    $(this).closest(".formblock-email").find("[id='HTMLEmailPart']").hide();
                    $(this).closest(".formblock-email").find("[id='PlainEmail']").hide();
                    $(this).closest(".formblock-email").find("#DocumentEmail").show();
                    break;
            }
        });

        $("[name='WorkFlow[]Email[][EmailType]']").trigger("change");
    };
})(jQuery);