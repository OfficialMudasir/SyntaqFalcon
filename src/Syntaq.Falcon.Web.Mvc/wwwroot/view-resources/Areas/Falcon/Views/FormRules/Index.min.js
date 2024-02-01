(function () {
	$(function () {

		var _$formRulesTable = $('#FormRulesTable');
		var _formRulesService = abp.services.app.formRules;
		
		//$('.date-picker').datetimepicker({
		//    locale: abp.localization.currentLanguage.name,
		//    format: 'L'
		//});

		var _permissions = {
			create: abp.auth.hasPermission('Pages.FormRules.Create'),
			edit: abp.auth.hasPermission('Pages.FormRules.Edit'),
			'delete': abp.auth.hasPermission('Pages.FormRules.Delete')
		};

		var _createOrEditModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/FormRules/CreateOrEditModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/FormRules/_CreateOrEditModal.js',
			modalClass: 'CreateOrEditFormRuleModal'
		});

		 var _viewFormRuleModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/FormRules/ViewformRuleModal',
			modalClass: 'ViewFormRuleModal'
		});

		
		

		var getDateFilter = function (element) {
			if (element.data("DateTimePicker").date() == null) {
				return null;
			}
			return element.data("DateTimePicker").date().format("YYYY-MM-DDT00:00:00Z"); 
		}

		var dataTable = _$formRulesTable.DataTable({
			paging: true,
			serverSide: true,
			processing: true,
			listAction: {
				ajaxFunction: _formRulesService.getAll,
				inputFilter: function () {
					return {
						filter: $('#FormRulesTableFilter').val(),
						ruleFilter: $('#RuleFilterId').val(),
						ruleScriptFilter: $('#RuleScriptFilterId').val(),
						formNameFilter: $('#FormNameFilterId').val()
					};
				}
			},
			columnDefs: [
				{
					width: 20,
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
									_viewFormRuleModal.open({ id: data.record.formRule.id });
								}
						},
						{
							text: app.localize('Edit'),
							visible: function () {
								return _permissions.edit;
							},
							action: function (data) {
								_createOrEditModal.open({ id: data.record.formRule.id });
							}
						}, 
						{
							text: app.localize('Delete'),
							visible: function () {
								return _permissions.delete;
							},
							action: function (data) {
								deleteFormRule(data.record.formRule);
							}
						}]
					}
				},
				{
					width: 20,
						targets: 1,
						 data: "formRule.rule"   
					},
				{
					width: 20,
						targets: 2,
						 data: "formRule.ruleScript"   
					},
				{
					width: 20,
						targets: 3,
						 data: "formName" 
					}
			]
		});


		function getFormRules() {
			dataTable.ajax.reload();
		}

		function deleteFormRule(formRule) {
			abp.message.confirm(
				'',
				'',
				function (isConfirmed) {
					if (isConfirmed) {
						_formRulesService.delete({
							id: formRule.id
						}).done(function () {
							getFormRules(true);
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

		$('#CreateNewFormRuleButton').click(function () {
			_createOrEditModal.open();
		});

		

		abp.event.on('app.createOrEditFormRuleModalSaved', function () {
			getFormRules();
		});

		$('#GetFormRulesButton').click(function (e) {
			e.preventDefault();
			getFormRules();
		});

		$(document).keypress(function(e) {
		  if(e.which === 13) {
			getFormRules();
		  }
		});

	});
})();