(function () {
    $(function () {

        var _$mergeTextItemsTable = $('#MergeTextItemsTable');
        var _mergeTextItemsService = abp.services.app.mergeTextItems;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.MergeTextItems.Create'),
            edit: abp.auth.hasPermission('Pages.MergeTextItems.Edit'),
            'delete': abp.auth.hasPermission('Pages.MergeTextItems.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/MergeTextItems/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/MergeTextItems/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditMergeTextItemModal'
        });


		
		

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }

        var dataTable = _$mergeTextItemsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _mergeTextItemsService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#MergeTextItemsTableFilter').val(),
					nameFilter: $('#NameFilterId').val(),
					mergeTextItemValueKeyFilter: $('#MergeTextItemValueKeyFilterId').val()
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
                                _createOrEditModal.open({ id: data.record.mergeTextItem.id });
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteMergeTextItem(data.record.mergeTextItem);
                            }
                        }]
                    }
                },
					{
						targets: 1,
						 data: "mergeTextItem.name"   
					},
					{
						targets: 2,
						 data: "mergeTextItemValueKey" 
					}
            ]
        });


        function getMergeTextItems() {
            dataTable.ajax.reload();
        }

        function deleteMergeTextItem(mergeTextItem) {
            abp.message.confirm(
                '',
                '',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _mergeTextItemsService.delete({
                            id: mergeTextItem.id
                        }).done(function () {
                            getMergeTextItems(true);
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

        $('#CreateNewMergeTextItemButton').click(function () {
            _createOrEditModal.open();
        });

		

        abp.event.on('app.createOrEditMergeTextItemModalSaved', function () {
            getMergeTextItems();
        });

		$('#GetMergeTextItemsButton').click(function (e) {
            e.preventDefault();
            getMergeTextItems();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getMergeTextItems();
		  }
		});

    });
})();