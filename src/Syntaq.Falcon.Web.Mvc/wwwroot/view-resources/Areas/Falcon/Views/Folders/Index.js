(function () {
	$(function () {

		var _$foldersTable = $('#FoldersTable');
		var _foldersService = abp.services.app.folders;

		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.Folders.Create'),
			edit: abp.auth.hasPermission('Pages.Folders.Edit'),
			'delete': abp.auth.hasPermission('Pages.Folders.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/Folders/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Folders/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditFolderModal'
		});


		var getDateFilter = function (element) {
			if (element.data("DateTimePicker").date() == null) {
				return null;
			}
			return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
		}

		var dataTable = _$foldersTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,
			listAction: {
				ajaxFunction: _foldersService.getAll,
				inputFilter: function () {
					return {
						filter: $('#FoldersTableFilter').val()
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
								_createOrEditModal.open({ id: data.record.folder.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteFolder(data.record.folder);
							}
						}]
					}
				},
					{
						targets: 1,
						 data: "folder.name"   
					},
					{
						targets: 2,
						 data: "folder.description"   
					}
			]
		});


		function getFolders() {
			dataTable.ajax.reload();
		}

		function deleteFolder(folder) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_foldersService.delete({
							id: folder.id
						}).done(function (result) {
							//getFolders(true);
							//abp.notify.success(app.localize('SuccessfullyDeleted'));
							if (result.success == true) {
								getFolders(true);
								abp.notify.success(app.localize('SuccessfullyDeletedFolder'));
							} else {
								abp.notify.warn(app.localize('FailedToDeleteFolder'));
							}
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

		$('#CreateNewFolderButton').click(function () {
			_createOrEditModal.open();
		});

		

		abp.event.on('app.createOrEditFolderModalSaved', function () {
			getFolders();
		});

		$('#GetFoldersButton').click(function (e) {
			e.preventDefault();
			getFolders();
		});

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getFolders();
		  }
		});

	});
})();