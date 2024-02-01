(function () {
    $(function () {

        var _$recordMatterItemsTable = $('#RecordMatterItemsTable');
        var _recordMatterItemsService = abp.services.app.recordMatterItems;

        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.RecordMatterItems.Create'),
            edit: abp.auth.hasPermission('Pages.RecordMatterItems.Edit'),
            'delete': abp.auth.hasPermission('Pages.RecordMatterItems.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItems/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItems/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditRecordMatterItemModal'
        });


        var dataTable = _$recordMatterItemsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterItemsService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#RecordMatterItemsTableFilter').val(),
					documentFilter: $('#DocumentFilterId').val(),
					documentNameFilter: $('#DocumentNameFilterId').val(),
					recordMatterTenantIdFilter: $('#RecordMatterTenantIdFilterId').val()
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
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.recordMatterItem.id });
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteRecordMatterItem(data.record.recordMatterItem);
                            }
                        }]
                    }
                },
					{
						targets: 1,
						 data: "recordMatterItem.document"   
					},
					{
						targets: 2,
						 data: "recordMatterItem.documentName"   
					},
					{
						targets: 3,
						 data: "recordMatterTenantId" 
					}
            ]
        });


        function getRecordMatterItems() {
            dataTable.ajax.reload();
        }

        function deleteRecordMatterItem(recordMatterItem) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _recordMatterItemsService.delete({
                            id: recordMatterItem.id
                        }).done(function () {
                            getRecordMatterItems(true);
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

        $('#CreateNewRecordMatterItemButton').click(function () {
            _createOrEditModal.open();
        });

		

        abp.event.on('app.createOrEditRecordMatterItemModalSaved', function () {
            getRecordMatterItems();
        });

		$('#GetRecordMatterItemsButton').click(function (e) {
            e.preventDefault();
            getRecordMatterItems();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getRecordMatterItems();
		  }
		});

    });
})();