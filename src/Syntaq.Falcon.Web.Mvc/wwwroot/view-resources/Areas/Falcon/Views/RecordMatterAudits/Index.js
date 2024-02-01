(function () {
    $(function () {

        var _$recordMatterAuditsTable = $('#RecordMatterAuditsTable');
        var _recordMatterAuditsService = abp.services.app.recordMatterAudits;
		
        //$('.date-picker').datetimepicker({
        //    locale: abp.localization.currentLanguage.name,
        //    format: 'L'
        //});

        var _permissions = {
            create: abp.auth.hasPermission('Pages.RecordMatterAudits.Create'),
            edit: abp.auth.hasPermission('Pages.RecordMatterAudits.Edit'),
            'delete': abp.auth.hasPermission('Pages.RecordMatterAudits.Delete')
        };

               

		 var _viewRecordMatterAuditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/RecordMatterAudits/ViewrecordMatterAuditModal',
            modalClass: 'ViewRecordMatterAuditModal'
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

        var dataTable = _$recordMatterAuditsTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _recordMatterAuditsService.getAll,
                inputFilter: function () {
                    return {
					filter: $('#RecordMatterAuditsTableFilter').val(),
					statusFilter: $('#StatusFilterId').val(),
					dataFilter: $('#DataFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val(),
					recordMatterRecordMatterNameFilter: $('#RecordMatterRecordMatterNameFilterId').val()
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
                                    window.location="/Falcon/RecordMatterAudits/ViewRecordMatterAudit/" + data.record.recordMatterAudit.id;
                                }
                        },
						{
                            text: app.localize('Edit'),
                            iconStyle: 'far fa-edit mr-2',
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                            window.location="/Falcon/RecordMatterAudits/CreateOrEdit/" + data.record.recordMatterAudit.id;                                
                            }
                        }, 
						{
                            text: app.localize('Delete'),
                            iconStyle: 'far fa-trash-alt mr-2',
                            visible: function () {
                                return _permissions.delete;
                            },
                            action: function (data) {
                                deleteRecordMatterAudit(data.record.recordMatterAudit);
                            }
                        }]
                    }
                },
					{
						targets: 2,
						 data: "recordMatterAudit.status",
						 name: "status"   ,
						render: function (status) {
							return app.localize('Enum_RecordMatterStatus_' + status);
						}
			
					},
					{
						targets: 3,
						 data: "recordMatterAudit.data",
						 name: "data"   
					},
					{
						targets: 4,
						 data: "userName" ,
						 name: "userFk.name" 
					},
					{
						targets: 5,
						 data: "recordMatterRecordMatterName" ,
						 name: "recordMatterFk.recordMatterName" 
					}
            ]
        });

        function getRecordMatterAudits() {
            dataTable.ajax.reload();
        }

        function deleteRecordMatterAudit(recordMatterAudit) {
            abp.message.confirm(
                '',
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _recordMatterAuditsService.delete({
                            id: recordMatterAudit.id
                        }).done(function () {
                            getRecordMatterAudits(true);
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

                

		$('#ExportToExcelButton').click(function () {
            _recordMatterAuditsService
                .getRecordMatterAuditsToExcel({
				filter : $('#RecordMatterAuditsTableFilter').val(),
					statusFilter: $('#StatusFilterId').val(),
					dataFilter: $('#DataFilterId').val(),
					userNameFilter: $('#UserNameFilterId').val(),
					recordMatterRecordMatterNameFilter: $('#RecordMatterRecordMatterNameFilterId').val()
				})
                .done(function (result) {
                    app.downloadTempFile(result);
                });
        });

        abp.event.on('app.createOrEditRecordMatterAuditModalSaved', function () {
            getRecordMatterAudits();
        });

		$('#GetRecordMatterAuditsButton').click(function (e) {
            e.preventDefault();
            getRecordMatterAudits();
        });

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getRecordMatterAudits();
		  }
		});
		
		
		
    });
})();
