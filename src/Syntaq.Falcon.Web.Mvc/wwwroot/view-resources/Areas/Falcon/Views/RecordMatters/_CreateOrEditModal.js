(function ($) {
	app.modals.CreateOrEditRecordMatterModal = function () {
		var _recordMattersService = abp.services.app.recordMatters;
		var _recordMatterItemsService = abp.services.app.recordMatterItems;

		var _modalManager;
		var _$recordMatterInformationForm = null;

		var _recordLookupTableModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/RecordMatters/RecordLookupTableModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/RecordMatters/_RecordLookupTableModal.js',
			modalClass: 'RecordLookupTableModal'
		});

		var _permissions = {
			view: abp.auth.hasPermission('Pages.RecordMatterItems'),
			create: abp.auth.hasPermission('Pages.RecordMatterItems.Create'),
			edit: abp.auth.hasPermission('Pages.RecordMatterItems.Edit'),
			delete: abp.auth.hasPermission('Pages.RecordMatterItems.Delete')
		};

		var matterItems = {
			MatterId: $('[name="id"]').val(),
			dataTable: null,
			init: function () {
				this.dataTable = $('#RecordMatterItemsTable').DataTable({
					paging: true,
					serverSide: true,
					processing: true,
					deferLoading: 0, //prevents table for ajax request on initialize
					listAction: {
						ajaxFunction: _recordMatterItemsService.getAllByRecordMatter,
						inputFilter: function () {
							return { id: matterItems.MatterId };
								//filter: $('#RecordMatterItemsTableFilter').val(),
								//documentFilter: $('#DocumentFilterId').val(),
								//documentNameFilter: $('#DocumentNameFilterId').val(),
								//recordMatterTenantIdFilter: $('#RecordMatterTenantIdFilterId').val()
						}
					},
					columnDefs: [
						{
							targets: 0,
							data: "recordMatterItem.documentName",
							render: function (data, type, row) {
								data = '<a href="/Falcon/RecordMatterItems/GetDocumentAsPDF?Id=' + row.recordMatterItem.id + '&version=1"><img src="/Common/Images/Entities/pdf.png" width=32 height=32/></a>'; 
								data += '<a href = "/Falcon/RecordMatterItems/GetDocumentAsWord?Id=' + row.recordMatterItem.id + '&version=1"><img src="~/Common/Images/Entities/word.png" width=32 height=32/></a>';
								data += '<span class="font-weight-bold">' + row.recordMatterItem.documentName + "</span>";
								return data;
							}
						},
						{
							width: 120,
							targets: 1,
							data: null,
							orderable: false,
							autoWidth: false,
							defaultContent: '',
							rowAction: {
								cssClass: 'btn btn-secondary pull-right',
								text: '' + app.localize('Actions') + '',
								items: [
									{
										text: app.localize('Download'),
										visible: function () {
											return _permissions.view;
										},
										action: function (data) {
											// _createOrEditModal.open({ id: data.record.recordMatterItem.id });
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
									}
								]
							}
						}
					]
				});
			}
		}

		this.init = function (modalManager) {
			_modalManager = modalManager;
			var modal = _modalManager.getModal();
			modal.find('.date-picker').datetimepicker({
				locale: abp.localization.currentLanguage.name,
				format: 'L'
			});
			_$recordMatterInformationForm = _modalManager.getModal().find('form[name=RecordMatterInformationsForm]');
			_$recordMatterInformationForm.validate();
		};

		$('#OpenRecordLookupTableButton').click(function () {
			var recordMatter = _$recordMatterInformationForm.serializeFormToObject();
			_recordLookupTableModal.open({ id: recordMatter.recordId, displayName: recordMatter.recordRecordName }, function (data) {
				_$recordMatterInformationForm.find('input[name=recordRecordName]').val(data.displayName); 
				_$recordMatterInformationForm.find('input[name=recordId]').val(data.id); 
			});
		});
		
		$('#ClearRecordRecordNameButton').click(function () {
				_$recordMatterInformationForm.find('input[name=recordRecordName]').val(''); 
				_$recordMatterInformationForm.find('input[name=recordId]').val(''); 
		});

		this.save = function () {
			if (!_$recordMatterInformationForm.valid()) {
				return;
			}
			var recordMatter = _$recordMatterInformationForm.serializeFormToObject();			
			_modalManager.setBusy(true);
			_recordMattersService.createOrEdit(
				recordMatter
			).done(function () {
			   abp.notify.info(app.localize('SavedSuccessfully'));
				 _modalManager.close();
			   abp.event.trigger('app.createOrEditRecordMatterModalSaved');
			}).always(function () {
			   _modalManager.setBusy(false);
			});
		};
		//console.log('About to init matterItems');
		//matterItems.init();
		//console.log('About to reload datatable');
		//matterItems.dataTable.ajax.reload();
		//console.log('Should Have Reloaded Datatable');
	};
})(jQuery);