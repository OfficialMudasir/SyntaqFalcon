﻿(function () {
    $(function () {

        var _$recordMatterItemHistoriesTable = $('#RecordMatterItemHistoriesTable');
        var _recordMatterItemHistoriesService = abp.services.app.recordMatterItemHistories;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.RecordMatterItemHistories.Create'),
            edit: abp.auth.hasPermission('Pages.RecordMatterItemHistories.Edit'),
            'delete': abp.auth.hasPermission('Pages.RecordMatterItemHistories.Delete')
        };

         var _createOrEditModal = new app.ModalManager({
                    viewUrl: abp.appPath + 'Falcon/RecordMatterItemHistories/CreateOrEditModal',
                    scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatterItemHistories/_CreateOrEditModal.js',
                    modalClass: 'CreateOrEditRecordMatterItemHistoryModal'
                });
                   

		 var _viewRecordMatterItemHistoryModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterItemHistories/ViewrecordMatterItemHistoryModal',
            modalClass: 'ViewRecordMatterItemHistoryModal'
        });

        var getDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
        }
        
        var getMaxDateFilter = function (element) {
            if (element.data("DateTimePicker").date() == null) {
                return null;
            }
            return element.data("DateTimePicker").date().format("YYYY-MM-DDT23:59:59Z"); 
        }

        var dataTable = _$recordMatterItemHistoriesTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterItemHistoriesService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#RecordMatterItemHistoriesTableFilter').val(),
					documentNameFilter: $('#DocumentNameFilterId').val(),
					allowedFormatsFilter: $('#AllowedFormatsFilterId').val(),
					statusFilter: $('#StatusFilterId').val(),
					recordMatterItemDocumentNameFilter: $('#RecordMatterItemDocumentNameFilterId').val(),
					formNameFilter: $('#FormNameFilterId').val(),
					submissionSubmissionStatusFilter: $('#SubmissionSubmissionStatusFilterId').val()
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
                                iconStyle: 'far fa-eye mr-2',
                                action: function (data) {
                                    _viewRecordMatterItemHistoryModal.open({ id: data.record.recordMatterItemHistory.id });
                                }
                        },
						{
                            text: app.localize('Edit'),
                            iconStyle: 'far fa-edit mr-2',
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            _createOrEditModal.open({ id: data.record.recordMatterItemHistory.id });                                
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            iconStyle: 'far fa-trash-alt mr-2',
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteRecordMatterItemHistory(data.record.recordMatterItemHistory);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "recordMatterItemHistory.documentName",
						 name: "documentName"   
					},
					{
						targets: 3,
						 data: "recordMatterItemHistory.allowedFormats",
						 name: "allowedFormats"   
					},
					{
						targets: 4,
						 data: "recordMatterItemHistory.status",
						 name: "status"   
					},
					{
						targets: 5,
						 data: "recordMatterItemDocumentName" ,
						 name: "recordMatterItemFk.documentName" 
					},
					{
						targets: 6,
						 data: "formName" ,
						 name: "formFk.name" 
					},
					{
						targets: 7,
						 data: "submissionSubmissionStatus" ,
						 name: "submissionFk.submissionStatus" 
					}
            ]
        });

        function getRecordMatterItemHistories() {
            dataTable.ajax.reload();
        }

        function deleteRecordMatterItemHistory(recordMatterItemHistory) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _recordMatterItemHistoriesService.delete({
                            id: recordMatterItemHistory.id
                        }).done(function () {
                            getRecordMatterItemHistories(true);
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

        $('#CreateNewRecordMatterItemHistoryButton').click(function () {
            _createOrEditModal.open();
        });        

		

        abp.event.on('app.createOrEditRecordMatterItemHistoryModalSaved', function () {
            getRecordMatterItemHistories();
        });

		$('#GetRecordMatterItemHistoriesButton').click(function (e) {
            e.preventDefault();
            getRecordMatterItemHistories();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getRecordMatterItemHistories();
		  }
		});
		
		
		
    });
})();
