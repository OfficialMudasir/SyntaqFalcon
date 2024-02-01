//(function () {
//    $(function () {

//        var _$teamUserRolesTable = $('#TeamUserRolesTable');
//        var _teamUserRolesService = abp.services.app.teamUserRoles;

//        $('.date-picker').datetimepicker({
//            locale: abp.localization.currentLanguage.name,
//            format: 'L'
//        });

//        var _permissions = {
//            create: abp.auth.hasPermission('Pages.TeamUserRoles.Create'),
//            edit: abp.auth.hasPermission('Pages.TeamUserRoles.Edit'),
//            'delete': abp.auth.hasPermission('Pages.TeamUserRoles.Delete')
//        };

//        var _createOrEditModal = new app.ModalManager({
//            viewUrl: abp.appPath + 'Falcon/TeamUserRoles/CreateOrEditModal',
//            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TeamUserRoles/_CreateOrEditModal.js',
//            modalClass: 'CreateOrEditTeamUserRoleModal'
//        });


//        var dataTable = _$teamUserRolesTable.DataTable({
//            paging: true,
//            serverSide: true,
//            processing: true,
//            listAction: {
//                ajaxFunction: _teamUserRolesService.getAll,
//                inputFilter: function () {
//                    return {
//					filter: $('#TeamUserRolesTableFilter').val(),
//					organizationUnitDisplayNameFilter: $('#OrganizationUnitDisplayNameFilterId').val(),
//					userNameFilter: $('#UserNameFilterId').val()
//                    };
//                }
//            },
//            columnDefs: [
//                {
//                    width: 120,
//                    targets: 0,
//                    data: null,
//                    orderable: false,
//                    autoWidth: false,
//                    defaultContent: '',
//                    rowAction: {
//                        cssClass: 'btn btn-brand dropdown-toggle',
//                        text:  app.localize('Actions') + ' <span class="caret"></span>',
//                        items: [
//						{
//                            text: app.localize('Edit'),
//                            visible: function () {
//                                return _permissions.edit;
//                            },
//                            action: function (data) {
//                                _createOrEditModal.open({ id: data.record.teamUserRole.id });
//                            }
//                        }, 
//						{
//                            text: app.localize('Delete'),
//                            visible: function () {
//                                return _permissions.delete;
//                            },
//                            action: function (data) {
//                                deleteTeamUserRole(data.record.teamUserRole);
//                            }
//                        }]
//                    }
//                },
//					{
//						targets: 1,
//						 data: "organizationUnitDisplayName" 
//					},
//					{
//						targets: 2,
//						 data: "userName" 
//					}
//            ]
//        });


//        function getTeamUserRoles() {
//            dataTable.ajax.reload();
//        }

//        function deleteTeamUserRole(teamUserRole) {
//            abp.message.confirm(
//                '',
//                function (isConfirmed) {
//                    if (isConfirmed) {
//                        _teamUserRolesService.delete({
//                            id: teamUserRole.id
//                        }).done(function () {
//                            getTeamUserRoles(true);
//                            abp.notify.success(app.localize('SuccessfullyDeleted'));
//                        });
//                    }
//                }
//            );
//        }

//		$('#ShowAdvancedFiltersSpan').click(function () {
//            $('#ShowAdvancedFiltersSpan').hide();
//            $('#HideAdvancedFiltersSpan').show();
//            $('#AdvacedAuditFiltersArea').slideDown();
//        });

//        $('#HideAdvancedFiltersSpan').click(function () {
//            $('#HideAdvancedFiltersSpan').hide();
//            $('#ShowAdvancedFiltersSpan').show();
//            $('#AdvacedAuditFiltersArea').slideUp();
//        });

//        $('#CreateNewTeamUserRoleButton').click(function () {
//            _createOrEditModal.open();
//        });

		

//        abp.event.on('app.createOrEditTeamUserRoleModalSaved', function () {
//            getTeamUserRoles();
//        });

//		$('#GetTeamUserRolesButton').click(function (e) {
//            e.preventDefault();
//            getTeamUserRoles();
//        });

//		$(document).keypress(function(e) {
//		  if(e.which === 13) {
//			getTeamUserRoles();
//		  }
//		});

//    });
//})();