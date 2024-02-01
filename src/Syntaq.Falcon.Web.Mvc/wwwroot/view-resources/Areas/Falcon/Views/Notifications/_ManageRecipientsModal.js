(function ($) {
    app.modals.ManageNotificationRecipients= function () {
        var _userService = abp.services.app.user;
        var _organizationUnitService = abp.services.app.organizationUnit;
        var _notificationService = abp.services.app.notification;
        var _$form = null;
        var _modalManager;
        this.init = async function (modalManager) {

            _modalManager = modalManager;
            _$form = _modalManager.getModal().find('form');
            _$form.validate();
            var _localSource = [];

            var users = await _userService.getUsers({ filter: null, permission: null, role: null, onlyLockedUsers: false, maxResultCount: 999 });

            $(users.items).each(function () {
                var user = { display: "" + this.userName + "", type: "User", id: this.id };
                _localSource.push(user);
            });

            var HasFeature = abp.session.tenantId === null ? true : abp.features.isEnabled('App.TeamsManagement');
            if (HasFeature) {
                var teams = await _organizationUnitService.getOrganizationUnits();

                $(teams.items).each(function () {
                    var team = { display: "" + this.displayName + "", type: "Team", id: this.id };
                    _localSource.push(team);
                });
            }

            var assignees = new Bloodhound({
                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('display'),
                queryTokenizer: Bloodhound.tokenizers.whitespace,
                local: _localSource
            });
            assignees.initialize();
           
            var elt = $('.tagsinput-typeahead');
            elt.tagsinput({
                itemValue: 'display',
                itemText: 'display',
                typeaheadjs: {
                    name: 'assignees',
                    displayKey: 'display',
                    source: assignees.ttAdapter()
                }
            });
        };
        
        $("input[name=channel]").change(function () {
            if ($("#NotifyInPlatformAndEmail").is(":checked")) {
                $("#EmailBody").attr("hidden", false);
            } else {
                $("#EmailBody").attr("hidden", true);
            }
        });

        $('#notify').click(async function () {
            var notificationRecipients = _$form.serializeFormToObject();
            notificationRecipients.Assignees = $(".tagsinput-typeahead").tagsinput('items');
            var recipientsSelected = notificationRecipients.Assignees.length;

            var _message = $('#SetAliveHeader').text();
            var _emailBody = $('#notificationMessage').val();
            var entityName = $('#entityName').text();
            if (_emailBody.match(/^ *$/) !== null) {
                _emailBody = null;
            }

            var channel = "";
            if ($("#NotifyInPlatform").is(':checked'))
                channel += "Platform";
            if ($("#NotifyInPlatformAndEmail").is(':checked'))
                channel += "EmailPlatform";

            if (channel == "Platform" && recipientsSelected > 0) {
                _notificationService.formTemplateUpdateNotification(_message, _emailBody, notificationRecipients, channel, entityName);
                _modalManager.close();
            }
            if (channel == "EmailPlatform" && recipientsSelected > 0 && _emailBody != null) {
                _notificationService.formTemplateUpdateNotification(_message, _emailBody, notificationRecipients, channel, entityName);
                _modalManager.close();
            }
        });
    };
})(jQuery);