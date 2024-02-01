//(function ($) {
//    app.modals.CreateOrEditTeamUserRoleModal = function () {

//        var _teamUserRolesService = abp.services.app.teamUserRoles;

//        var _modalManager;
//        var _$teamUserRoleInformationForm = null;

//		        var _organizationUnitLookupTableModal = new app.ModalManager({
//            viewUrl: abp.appPath + 'Falcon/TeamUserRoles/OrganizationUnitLookupTableModal',
//            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TeamUserRoles/_OrganizationUnitLookupTableModal.js',
//            modalClass: 'OrganizationUnitLookupTableModal'
//        });        var _userLookupTableModal = new app.ModalManager({
//            viewUrl: abp.appPath + 'Falcon/TeamUserRoles/UserLookupTableModal',
//            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TeamUserRoles/_UserLookupTableModal.js',
//            modalClass: 'UserLookupTableModal'
//        });

//        this.init = function (modalManager) {
//            _modalManager = modalManager;

//			var modal = _modalManager.getModal();
//            modal.find('.date-picker').datetimepicker({
//                locale: abp.localization.currentLanguage.name,
//                format: 'L'
//            });

//            _$teamUserRoleInformationForm = _modalManager.getModal().find('form[name=TeamUserRoleInformationsForm]');
//            _$teamUserRoleInformationForm.validate();
//        };

//		          $('#OpenOrganizationUnitLookupTableButton').click(function () {

//            var teamUserRole = _$teamUserRoleInformationForm.serializeFormToObject();

//            _organizationUnitLookupTableModal.open({ id: teamUserRole.organizationUnitId, displayName: teamUserRole.organizationUnitDisplayName }, function (data) {
//                _$teamUserRoleInformationForm.find('input[name=organizationUnitDisplayName]').val(data.displayName); 
//                _$teamUserRoleInformationForm.find('input[name=organizationUnitId]').val(data.id); 
//            });
//        });
		
//		$('#ClearOrganizationUnitDisplayNameButton').click(function () {
//                _$teamUserRoleInformationForm.find('input[name=organizationUnitDisplayName]').val(''); 
//                _$teamUserRoleInformationForm.find('input[name=organizationUnitId]').val(''); 
//        });
		
//        $('#OpenUserLookupTableButton').click(function () {

//            var teamUserRole = _$teamUserRoleInformationForm.serializeFormToObject();

//            _userLookupTableModal.open({ id: teamUserRole.userId, displayName: teamUserRole.userName }, function (data) {
//                _$teamUserRoleInformationForm.find('input[name=userName]').val(data.displayName); 
//                _$teamUserRoleInformationForm.find('input[name=userId]').val(data.id); 
//            });
//        });
		
//		$('#ClearUserNameButton').click(function () {
//                _$teamUserRoleInformationForm.find('input[name=userName]').val(''); 
//                _$teamUserRoleInformationForm.find('input[name=userId]').val(''); 
//        });
		


//        this.save = function () {
//            if (!_$teamUserRoleInformationForm.valid()) {
//                return;
//            }

//            var teamUserRole = _$teamUserRoleInformationForm.serializeFormToObject();
			
//			 _modalManager.setBusy(true);
//			 _teamUserRolesService.createOrEdit(
//				teamUserRole
//			 ).done(function () {
//               abp.notify.info(app.localize('SavedSuccessfully'));
//               _modalManager.close();
//               abp.event.trigger('app.createOrEditTeamUserRoleModalSaved');
//			 }).always(function () {
//               _modalManager.setBusy(false);
//			});
//        };
//    };
//})(jQuery);