(function () {
    $(function () {

        var _$voucherUsagesTable = $('#VoucherUsagesTable');
        var _voucherUsagesService = abp.services.app.voucherUsages;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.VoucherUsages.Create'),
            edit: abp.auth.hasPermission('Pages.VoucherUsages.Edit'),
            'delete': abp.auth.hasPermission('Pages.VoucherUsages.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/VoucherUsages/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/VoucherUsages/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditVoucherUsageModal'
        });

		 var _viewVoucherUsageModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/VoucherUsages/ViewvoucherUsageModal',
            modalClass: 'ViewVoucherUsageModal'
        });

		
		

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }

        var dataTable = _$voucherUsagesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _voucherUsagesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#VoucherUsagesTableFilter').val(),
					minDateRedeemedFilter:  getDateFilter($('#MinDateRedeemedFilterId')),
					maxDateRedeemedFilter:  getDateFilter($('#MaxDateRedeemedFilterId')),
					entityKeyFilter: $('#EntityKeyFilterId').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val()
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
                        text:  app.localize('Actions') + ' <span class="caret"></span>',
                        items: [
						{
                                text: app.localize('View'),
                                action: function (data) {
                                    _viewVoucherUsageModal.open({ id: data.record.voucherUsage.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.voucherUsage.id });
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteVoucherUsage(data.record.voucherUsage);
                            }
                        }]
                    }
                },
					{
						targets: 1,
						 data: "voucherUsage.dateRedeemed",
						 name: "dateRedeemed" ,
					render: function (dateRedeemed) {
						if (dateRedeemed) {
							return moment(dateRedeemed).format('L');
						}
						return "";
					}
			  
					},
					{
						targets: 2,
						 data: "voucherUsage.entityKey",
						 name: "entityKey"   
					},
					{
						targets: 3,
						 data: "voucherUsage.entityType",
						 name: "entityType"   
					},
					{
						targets: 4,
						 data: "userName" ,
						 name: "userFk.name" 
					}
            ]
        });


        function getVoucherUsages() {
            dataTable.ajax.reload();
        }

        function deleteVoucherUsage(voucherUsage) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _voucherUsagesService.delete({
                            id: voucherUsage.id
                        }).done(function () {
                            getVoucherUsages(true);
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

        $('#CreateNewVoucherUsageButton').click(function () {
            _createOrEditModal.open();
        });

		$('#ExportToExcelButton').click(function () {
            _voucherUsagesService
                .getVoucherUsagesToExcel({
				filter : $('#VoucherUsagesTableFilter').val(),
					minDateRedeemedFilter:  getDateFilter($('#MinDateRedeemedFilterId')),
					maxDateRedeemedFilter:  getDateFilter($('#MaxDateRedeemedFilterId')),
					entityKeyFilter: $('#EntityKeyFilterId').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val()
				})
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditVoucherUsageModalSaved', function () {
            getVoucherUsages();
        });

		$('#GetVoucherUsagesButton').click(function (e) {
            e.preventDefault();
            getVoucherUsages();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getVoucherUsages();
		  }
		});

    });
})();