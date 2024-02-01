(function () {
	$(function () {

		var _$filesTable = $('#FilesTable');
		var _filesService = abp.services.app.files;
		
		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.Files.Create'),
			edit: abp.auth.hasPermission('Pages.Files.Edit'),
			'delete': abp.auth.hasPermission('Pages.Files.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Files/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Files/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditFileModal'
		});

		var getDateFilter = function (element) {
			if (element.data("DateTimePicker").date() == null) {
				return null;
			}
			return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
		}

		var dataTable = _$filesTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,
			listAction: {
				ajaxFunction: _filesService.getAll,
				inputFilter: function () {
					return {
						filter: $('#FilesTableFilter').val(),
						recordRecordNameFilter: $('#RecordRecordNameFilterId').val(),
						recordMatterRecordMatterNameFilter: $('#RecordMatterRecordMatterNameFilterId').val()
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
								_createOrEditModal.open({ id: data.record.file.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteFile(data.record.file);
							}
						}]
					}
				},
					{
						targets: 1,
						 data: "file.fileUploadId"   
					},
					{
						targets: 2,
						 data: "file.fileUploads"   
					},
					{
						targets: 3,
						 data: "recordRecordName" 
					},
					{
						targets: 4,
						 data: "recordMatterRecordMatterName" 
					}
			]
		});


		function getFiles() {
			dataTable.ajax.reload();
		}

		function deleteFile(file) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_filesService.delete({
							id: file.id
						}).done(function () {
							getFiles(true);
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

		$('#CreateNewFileButton').click(function () {
			_createOrEditModal.open();
		});

		

		abp.event.on('app.createOrEditFileModalSaved', function () {
			getFiles();
		});

		$('#GetFilesButton').click(function (e) {
			e.preventDefault();
			getFiles();
		});

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getFiles();
		  }
		});

	});
})();