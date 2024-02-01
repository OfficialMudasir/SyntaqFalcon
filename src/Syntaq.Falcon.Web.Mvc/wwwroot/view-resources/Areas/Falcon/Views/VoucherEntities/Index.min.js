(function () {
    $(function () {

        var _$voucherEntitiesTable = $('#VoucherEntitiesTable');
        var _voucherEntitiesService = abp.services.app.voucherEntities;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.VoucherEntities.Create'),
            edit: abp.auth.hasPermission('Pages.VoucherEntities.Edit'),
            'delete': abp.auth.hasPermission('Pages.VoucherEntities.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/VoucherEntities/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/VoucherEntities/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditVoucherEntityModal'
        });

		 var _viewVoucherEntityModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/VoucherEntities/ViewvoucherEntityModal',
            modalClass: 'ViewVoucherEntityModal'
        });

		
		

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }

        var dataTable = _$voucherEntitiesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _voucherEntitiesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#VoucherEntitiesTableFilter').val(),
					entityKeyFilter: $('#EntityKeyFilterId').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					voucherTenantIdFilter: $('#VoucherTenantIdFilterId').val()
                    };
                }
            },
            columnDefs: [
                {
                    width: 120,
                    targets: 0,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        cssClass: 'btn btn-brand dropdown-toggle',
                        text:  app.localize('Actions') + ' <i class="fas fa-angle-down"></i>',
                        items: [
						{
                                text: app.localize('View'),
                                action: function (data) {
                                    _viewVoucherEntityModal.open({ id: data.record.voucherEntity.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.voucherEntity.id });
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteVoucherEntity(data.record.voucherEntity);
                            }
                        }]
                    }
                },
					{
						targets: 1,
						 data: "voucherEntity.entityKey",
						 name: "entityKey"   
					},
					{
						targets: 2,
						 data: "voucherEntity.entityType",
						 name: "entityType"   
					},
					{
						targets: 3,
						 data: "voucherTenantId" ,
						 name: "voucherFk.tenantId" 
					}
            ]
        });


        function getVoucherEntities() {
            dataTable.ajax.reload();
        }

        function deleteVoucherEntity(voucherEntity) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _voucherEntitiesService.delete({
                            id: voucherEntity.id
                        }).done(function () {
                            getVoucherEntities(true);
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

        $('#CreateNewVoucherEntityButton').click(function () {
            _createOrEditModal.open();
        });

		$('#ExportToExcelButton').click(function () {
            _voucherEntitiesService
                .getVoucherEntitiesToExcel({
				filter : $('#VoucherEntitiesTableFilter').val(),
					entityKeyFilter: $('#EntityKeyFilterId').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					voucherTenantIdFilter: $('#VoucherTenantIdFilterId').val()
				})
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditVoucherEntityModalSaved', function () {
            getVoucherEntities();
        });

		$('#GetVoucherEntitiesButton').click(function (e) {
            e.preventDefault();
            getVoucherEntities();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getVoucherEntities();
		  }
		});

    });
})();