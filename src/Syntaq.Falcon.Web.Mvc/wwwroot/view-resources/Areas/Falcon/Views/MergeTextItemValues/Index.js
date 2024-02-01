(function () {
    $(function () {

        var _$mergeTextItemValuesTable = $('#MergeTextItemValuesTable');
        var _mergeTextItemValuesService = abp.services.app.mergeTextItemValues;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.MergeTextItemValues.Create'),
            edit: abp.auth.hasPermission('Pages.MergeTextItemValues.Edit'),
            'delete': abp.auth.hasPermission('Pages.MergeTextItemValues.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/MergeTextItemValues/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/MergeTextItemValues/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditMergeTextItemValueModal'
        });


		
		

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }

        var dataTable = _$mergeTextItemValuesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _mergeTextItemValuesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#MergeTextItemValuesTableFilter').val(),
					keyFilter: $('#KeyFilterId').val(),
					valueFilter: $('#ValueFilterId').val()
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
                                _createOrEditModal.open({ id: data.record.mergeTextItemValue.id });
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteMergeTextItemValue(data.record.mergeTextItemValue);
                            }
                        }]
                    }
                },
					{
						targets: 1,
						 data: "mergeTextItemValue.key"   
					},
					{
						targets: 2,
						 data: "mergeTextItemValue.value"   
					}
            ]
        });


        function getMergeTextItemValues() {
            dataTable.ajax.reload();
        }

        function deleteMergeTextItemValue(mergeTextItemValue) {
            abp.message.confirm(
                '',
                '',
                function (isConfirmed) {
                    if (isConfirmed) {
                        _mergeTextItemValuesService.delete({
                            id: mergeTextItemValue.id
                        }).done(function () {
                            getMergeTextItemValues(true);
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

        $('#CreateNewMergeTextItemValueButton').click(function () {
            _createOrEditModal.open();
        });

		

        abp.event.on('app.createOrEditMergeTextItemValueModalSaved', function () {
            getMergeTextItemValues();
        });

		$('#GetMergeTextItemValuesButton').click(function (e) {
            e.preventDefault();
            getMergeTextItemValues();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getMergeTextItemValues();
		  }
		});

    });
})();