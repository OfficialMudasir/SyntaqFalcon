(function () {
    $(function () {

        var _$recordMatterContributorsTable = $('#RecordMatterContributorsTable');
        var _recordMatterContributorsService = abp.services.app.recordMatterContributors;
		var _entityTypeFullName = 'Syntaq.Falcon.Records.RecordMatterContributor';
        
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.RecordMatterContributors.Create'),
            edit: abp.auth.hasPermission('Pages.RecordMatterContributors.Edit'),
            'delete': abp.auth.hasPermission('Pages.RecordMatterContributors.Delete')
        };

         var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterContributors/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditRecordMatterContributorModal'
        });       

		 var _viewRecordMatterContributorModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterContributors/ViewrecordMatterContributorModal',
            modalClass: 'ViewRecordMatterContributorModal'
        });

		        var _entityTypeHistoryModal = app.modals.EntityTypeHistoryModal.create();
		        function entityHistoryIsEnabled() {
            return abp.auth.hasPermission('Pages.Administration.AuditLogs') &&
                abp.custom.EntityHistory &&
                abp.custom.EntityHistory.IsEnabled &&
                _.filter(abp.custom.EntityHistory.EnabledEntities, entityType => entityType === _entityTypeFullName).length === 1;
        }

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }

        var dataTable = _$recordMatterContributorsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterContributorsService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#RecordMatterContributorsTableFilter').val(),
					organizationNameFilter: $('#OrganizationNameFilterId').val(),
					nameFilter: $('#NameFilterId').val(),
					accessTokenFilter: $('#AccessTokenFilterId').val(),
					minTimeFilter:  getDateFilter($('#MinTimeFilterId')),
					maxTimeFilter:  getDateFilter($('#MaxTimeFilterId')),
					stepStatusFilter: $('#StepStatusFilterId').val(),
					stepRoleFilter: $('#StepRoleFilterId').val(),
					stepActionFilter: $('#StepActionFilterId').val(),
					completeFilter: $('#CompleteFilterId').val(),
					emailFilter: $('#EmailFilterId').val(),
					formSchemaFilter: $('#FormSchemaFilterId').val(),
					formScriptFilter: $('#FormScriptFilterId').val(),
					formRulesFilter: $('#FormRulesFilterId').val(),
					formPagesFilter: $('#FormPagesFilterId').val(),
					recordMatterRecordMatterNameFilter: $('#RecordMatterRecordMatterNameFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val(),
					formNameFilter: $('#FormNameFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    className: 'control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0
                },
                {
                    width: 120,
                    targets: 1,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text:  app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
						{
                                text: app.localize('View'),
                                action: function (data) {
                                    _viewRecordMatterContributorModal.open({ id: data.record.recordMatterContributor.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            _createOrEditModal.open({ id: data.record.recordMatterContributor.id });                                
                            }
                        },
                        {
                            text: app.localize('History'),
                            visible: function () {
                                return entityHistoryIsEnabled();
                            },
                            action: function (data) {
                                _entityTypeHistoryModal.open({
                                    entityTypeFullName: _entityTypeFullName,
                                    entityId: data.record.recordMatterContributor.id
                                });
                            }
						}, 
						{
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteRecordMatterContributor(data.record.recordMatterContributor);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "recordMatterContributor.organizationName",
						 name: "organizationName"   
					},
					{
						targets: 3,
						 data: "recordMatterContributor.name",
						 name: "name"   
					},
					{
						targets: 4,
						 data: "recordMatterContributor.accessToken",
						 name: "accessToken"   
					},
					{
						targets: 5,
						 data: "recordMatterContributor.time",
						 name: "time" ,
					render: function (time) {
						if (time) {
							return moment(time).format('L');
						}
						return "";
					}
			  
					},
					{
						targets: 6,
						 data: "recordMatterContributor.stepStatus",
						 name: "stepStatus"   ,
						render: function (stepStatus) {
							return app.localize('Enum_ProjectStepStatus_' + stepStatus);
						}
			
					},
					{
						targets: 7,
						 data: "recordMatterContributor.stepRole",
						 name: "stepRole"   ,
						render: function (stepRole) {
							return app.localize('Enum_ProjectStepRole_' + stepRole);
						}
			
					},
					{
						targets: 8,
						 data: "recordMatterContributor.stepAction",
						 name: "stepAction"   ,
						render: function (stepAction) {
							return app.localize('Enum_ProjectStepAction_' + stepAction);
						}
			
					},
					{
						targets: 9,
						 data: "recordMatterContributor.complete",
						 name: "complete"  ,
						render: function (complete) {
							if (complete) {
								return '<div class="text-center"><i class="fa fa-check kt--font-success" title="True"></i></div>';
							}
							return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
					}
			 
					},
					{
						targets: 10,
						 data: "recordMatterContributor.email",
						 name: "email"   
					},
					{
						targets: 11,
						 data: "recordMatterContributor.formSchema",
						 name: "formSchema"   
					},
					{
						targets: 12,
						 data: "recordMatterContributor.formScript",
						 name: "formScript"   
					},
					{
						targets: 13,
						 data: "recordMatterContributor.formRules",
						 name: "formRules"   
					},
					{
						targets: 14,
						 data: "recordMatterContributor.formPages",
						 name: "formPages"   
					},
					{
						targets: 15,
						 data: "recordMatterRecordMatterName" ,
						 name: "recordMatterFk.recordMatterName" 
					},
					{
						targets: 16,
						 data: "userName" ,
						 name: "userFk.name" 
					},
					{
						targets: 17,
						 data: "formName" ,
						 name: "formFk.name" 
					}
            ]
        });

        function getRecordMatterContributors() {
            dataTable.ajax.reload();
        }

        function deleteRecordMatterContributor(recordMatterContributor) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _recordMatterContributorsService.delete({
                            id: recordMatterContributor.id
                        }).done(function () {
                            getRecordMatterContributors(true);
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        }

		$('#ShowAdvancedFiltersSpan').click(function () {
            $('#ShowAdvancedFiltersSpan').hide();
            $('#HideAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideDown();
        });

        $('#HideAdvancedFiltersSpan').click(function () {
            $('#HideAdvancedFiltersSpan').hide();
            $('#ShowAdvancedFiltersSpan').show();
            $('#AdvacedAuditFiltersArea').slideUp();
        });

        $('#CreateNewRecordMatterContributorButton').click(function () {
            _createOrEditModal.open();
        });        

		$('#ExportToExcelButton').click(function () {
            _recordMatterContributorsService
                .getRecordMatterContributorsToExcel({
				filter : $('#RecordMatterContributorsTableFilter').val(),
					organizationNameFilter: $('#OrganizationNameFilterId').val(),
					nameFilter: $('#NameFilterId').val(),
					accessTokenFilter: $('#AccessTokenFilterId').val(),
					minTimeFilter:  getDateFilter($('#MinTimeFilterId')),
					maxTimeFilter:  getDateFilter($('#MaxTimeFilterId')),
					stepStatusFilter: $('#StepStatusFilterId').val(),
					stepRoleFilter: $('#StepRoleFilterId').val(),
					stepActionFilter: $('#StepActionFilterId').val(),
					completeFilter: $('#CompleteFilterId').val(),
					emailFilter: $('#EmailFilterId').val(),
					formSchemaFilter: $('#FormSchemaFilterId').val(),
					formScriptFilter: $('#FormScriptFilterId').val(),
					formRulesFilter: $('#FormRulesFilterId').val(),
					formPagesFilter: $('#FormPagesFilterId').val(),
					recordMatterRecordMatterNameFilter: $('#RecordMatterRecordMatterNameFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val(),
					formNameFilter: $('#FormNameFilterId').val()
				})
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditRecordMatterContributorModalSaved', function () {
            getRecordMatterContributors();
        });

		$('#GetRecordMatterContributorsButton').click(function (e) {
            e.preventDefault();
            getRecordMatterContributors();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getRecordMatterContributors();
		  }
		});
    });
})();