(function () {
	$(function () {

		var _$documentsTable = $('#DocumentsTable');
		var _documentsService = abp.services.app.documents;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.Documents.Create'),
			edit: abp.auth.hasPermission('Pages.Documents.Edit'),
			'delete': abp.auth.hasPermission('Pages.Documents.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Documents/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Documents/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditDocumentModal'
		});


		var dataTable = _$documentsTable.DataTable({
			paging: true,
            serverSide: true,
            pageLength: 25,
			processing: true,
			listAction: {
				ajaxFunction: _documentsService.getAll,
				inputFilter: function () {
					return {
						filter: $('#DocumentsTableFilter').val()
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
							text: app.localize('Edit'),
							visible: function () {
								return _permissions.edit;
							},
							action: function (data) {
								_createOrEditModal.open({ id: data.record.document.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteDocument(data.record.document);
							}
						}]
					}
				}
			]
		});


		function getDocuments() {
			dataTable.ajax.reload();
		}

		function deleteDocument(document) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_documentsService.delete({
							id: document.id
						}).done(function () {
							getDocuments(true);
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

		$('#CreateNewDocumentButton').click(function () {
			_createOrEditModal.open();
		});

		

		abp.event.on('app.createOrEditDocumentModalSaved', function () {
			getDocuments();
		});

		$('#GetDocumentsButton').click(function (e) {
			e.preventDefault();
			getDocuments();
		});
		$('#clearbtn').click(function () {
			$('#DocumentsTableFilter').val('');
			getDocuments();
		});
		$(document).keypress(function(e) {
			if(e.which === 13) {
			getDocuments();
			}
		});

	});
})();