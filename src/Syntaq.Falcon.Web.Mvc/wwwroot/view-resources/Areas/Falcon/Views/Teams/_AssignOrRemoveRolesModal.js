//(function () {
//    $(function () {
//        app.modals.AssignOrRemoveRolesModal = function () {

//            var _userService = abp.services.app.user;
//            var _teamUserRoleService = abp.services.app.teamUserRoles;

//            var _modalManager;
//            var _$userInformationForm = null;

//            function _findAssignedRoleNames() {
//                //var assignedRoleNames = [];

//                var assignedRoleNames = [];
//                //assignedRoleNames['Role'] = [];
//                var i = 0;

//                //var assignedRoleNames = {};
//                //assignedRoleNames['Role'] = [];
//                //var i = 0;

//                //_modalManager.getModal()
//                //    .find('.team-user-role-checkbox-list input[type=checkbox]')
//                //    .each(function () {
//                //        if ($(this).is(':checked')) {
//                //            assignedRoleNames.push($(this).attr('name'));
//                //        }
//                //    });

//                //_modalManager.getModal()
//                //    .find('.team-user-role-checkbox-list input[type=checkbox]')
//                //    .each(function () {
//                //        assignedRoleNames.Role[i] = {};
//                //        if ($(this).is(':checked')) {
//                //            assignedRoleNames.Role[i]['Id'] = $(this).attr('id');
//                //            assignedRoleNames.Role[i]['Assigned'] = 'true';
//                //        } else {
//                //            assignedRoleNames.Role[i]['Id'] = $(this).attr('id');
//                //            assignedRoleNames.Role[i]['Assigned'] = 'false';
//                //        }
//                //        i++;
//                //    });

//                _modalManager.getModal()
//                    .find('.team-user-role-checkbox-list input[type=checkbox]')
//                    .each(function () {
//                        assignedRoleNames[i] = {};
//                        if ($(this).is(':checked')) {
//                            assignedRoleNames[i]['Id'] = $(this).attr('id');
//                            assignedRoleNames[i]['Assigned'] = 'true';
//                        } else {
//                            assignedRoleNames[i]['Id'] = $(this).attr('id');
//                            assignedRoleNames[i]['Assigned'] = 'false';
//                        }
//                        i++;
//                    });

//                return assignedRoleNames;
//            }

//            function GetCurrentTeam() {
//                var TeamId = $("#TeamSelectionCombo", parent.document).val();
//                return TeamId;
//            }

//            function GetUserForEdit() {
//                var UserId = _modalManager.getModal().find('.team-user-role-checkbox-list input[type=text]').val();
//                return UserId;
//            }

//            this.init = function (modalManager) {
//                _modalManager = modalManager;
//                _$userInformationForm = _modalManager.getModal().find('form[name=TeamRoleInformationsForm]');
//                _$userInformationForm.validate();

//                _modalManager.getModal()
//                    .find('.user-role-checkbox-list input[type=checkbox]')
//                    .change(function () {
//                        $('#assigned-role-count').text(_findAssignedRoleNames().length);
//                    });

//                _modalManager.getModal().find('[data-toggle=tooltip]').tooltip();
//            };

//            this.save = function () {
//                if (!_$userInformationForm.valid()) {
//                    return;
//                }

//                var assignedRoleNames = _findAssignedRoleNames();
//                var user = _$userInformationForm.serializeFormToObject();

//                var teamUserRole = {};
//                teamUserRole.OrganizationUnitId = GetCurrentTeam();
//                teamUserRole.UserId = GetUserForEdit();

//                _modalManager.setBusy(true);
//                _teamUserRoleService.assignOrUpdateRoles({
//                    TeamUser: teamUserRole,
//                    AssignedTeamUserRoles: assignedRoleNames
//                }).done(function () {
//                    abp.notify.info(app.localize('SavedSuccessfully'));
//                    _modalManager.close();
//                    abp.event.trigger('app.createOrEditUserModalSaved');
//                }).always(function () {
//                    _modalManager.setBusy(false);
//                });
//            };
//        };
//    });
//})();