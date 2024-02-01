(function () {
	$(function () {

		var _$mergeTextsTable = $('#MergeTextsTable');
		var _mergeTextsService = abp.services.app.mergeTexts;
		
		//$('.date-picker').datetimepicker({
		//	locale: abp.localization.currentLanguage.name,
		//	format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.MergeTexts.Create'),
			edit: abp.auth.hasPermission('Pages.MergeTexts.Edit'),
			'delete': abp.auth.hasPermission('Pages.MergeTexts.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/MergeTexts/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/MergeTexts/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditMergeTextModal'
		});

		 var _viewMergeTextModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/MergeTexts/ViewmergeTextModal',
			modalClass: 'ViewMergeTextModal'
		});

		
		

		var getDateFilter = function (element) {
			if (element.data("DateTimePicker").date() == null) {
				return null;
			}
			return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
		}

		var dataTable = _$mergeTextsTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,
			listAction: {
				ajaxFunction: _mergeTextsService.getAll,
				inputFilter: function () {
					return {
					filter: $('#MergeTextsTableFilter').val(),
					entityTypeFilter: $('#EntityTypeFilterId').val(),
					entityKeyFilter: $('#EntityKeyFilterId').val(),
					mergeTextItemNameFilter: $('#MergeTextItemNameFilterId').val()
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
									_viewMergeTextModal.open({ id: data.record.mergeText.id });
								}
						},
						{
							text: app.localize('Edit'),
							visible: function () {
								return _permissions.edit;
							},
							action: function (data) {
								_createOrEditModal.open({ id: data.record.mergeText.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteMergeText(data.record.mergeText);
							}
						}]
					}
				},
					{
						targets: 1,
						 data: "mergeText.entityType"   
					},
					{
						targets: 2,
						 data: "mergeText.entityKey"   
					},
					{
						targets: 3,
						 data: "mergeTextItemName" 
					}
			]
		});


		function getMergeTexts() {
			dataTable.ajax.reload();
		}

		function deleteMergeText(mergeText) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_mergeTextsService.delete({
							id: mergeText.id
						}).done(function () {
							getMergeTexts(true);
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

		$('#CreateNewMergeTextButton').click(function () {
			_createOrEditModal.open();
		});

		

		abp.event.on('app.createOrEditMergeTextModalSaved', function () {
			getMergeTexts();
		});

		$('#GetMergeTextsButton').click(function (e) {
			e.preventDefault();
			getMergeTexts();
		});

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getMergeTexts();
		  }
		});

	});
})();