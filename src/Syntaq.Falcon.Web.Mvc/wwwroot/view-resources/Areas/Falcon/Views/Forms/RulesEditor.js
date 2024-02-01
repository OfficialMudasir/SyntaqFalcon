(function () {
	$(function () {

		var _$rulesTable = $('#FormRulesTable');
		var _formsService = abp.services.app.forms;
		var _formRulesService = abp.services.app.formRules;

		var fieldoptions = '';
		var filterx = { "filters": [] };
		var url_string = window.location.href;
		var url = new URL(url_string);

		var _permissions = {
			create: abp.auth.hasPermission('Pages.FormRules.Create'),
			edit: abp.auth.hasPermission('Pages.FormRules.Edit'),
			'delete': abp.auth.hasPermission('Pages.FormRules.Delete')
		};

		var _importRulesModal = new app.ModalManager({
			viewUrl: abp.appPath + 'Falcon/FormRules/ImportRulesModal',
			scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/FormRules/_ImportRulesModal.js',
			modalClass: 'ImportRulesModal'
		});

		// Allow Page URL to activate a tab's ID
		var taburl = document.location.toString();
		if (taburl.match('#')) {
			$('.nav-tabs a[href="#' + taburl.split('#')[1] + '"]').tabs('show');
		}

		loadFullSchema(JSONFormObj.Id);

		async function loadFullSchema(fid) {
			return await _formsService.getSchema({ id: fid }, "Load").then(response => JSON.parse(response))
				.then(data => {
					loadform(JSON.stringify(data));
				});
		}

		loadform(JSONFormObj.Schema);

		function loadform(form) {
			if (form === '' || !form) {
				form = '{"type": "form","display": "form","components": [{"label": "Save","action": "event","theme": "primary","shortcut": "","customClass": "d-inline mr-1","mask": false,"tableView": true,"type": "sfabutton","input": true,"key": "save","showValidations": false,"event": "save","leftIcon": ""},{"type": "sfabutton","label": "Submit","key": "submit","disableOnInvalid": true,"inline": true,"theme": "primary","customClass": "d-inline","input": true,"tableView": true}]}';
			}

			var formJSON = JSON.parse(form);

			if (formJSON.hasOwnProperty('components')) {
				loadcomponents(formJSON.components);
			}

			function loadcomponents(components) {
				var filter;
				var key;
				var cnt;
				if (components.length !== 0) {
					for (var component in components) {
						if (components[component].type !== 'button') {
							//fieldoptions += '<option value="' + components[component].key + '">' + components[component].key + '</option>';

							// We dont have a class type 
							// Push into the filter for the condition editor
							if (//Default Components
								components[component].type === 'textfield' ||
								components[component].type === 'number' ||
								components[component].type === 'textarea' ||
								components[component].type === 'day' ||
								components[component].type === 'datetime' ||
								components[component].type === 'selectboxes' ||
								components[component].type === 'checkbox' ||
								components[component].type === 'phoneNumber' ||
								components[component].type === 'radio' ||
								components[component].type === 'email' ||
								components[component].type === 'select' ||
								components[component].type === 'panel' ||
								//Custom Components
								components[component].type === 'sfatextfield' ||
								components[component].type === 'sfanumber' ||
								components[component].type === 'sfaradioyn' ||
								components[component].type === 'sfaselect' ||
								components[component].type === 'sfadatetime' ||
								components[component].type === 'sfacheckbox' ||
								components[component].type === 'checkboxesgroup' ||
								components[component].type === 'radiogroup' ||
								components[component].type === 'person' ||
								components[component].type === 'addressgroup' ||
								components[component].type === 'section' ||
								components[component].type === 'sfapanel' ||
								components[component].type === 'datagrid' ||
								components[component].type === 'heading' ||
								components[component].type === 'label' ||
								components[component].type === 'divider' ||
								components[component].type === 'helpnotes' ||
								components[component].type === 'sfaemail' ||
								components[component].type === 'sfatextarea' ||
								components[component].type === 'slider' ||
								components[component].type === 'image' ||
								components[component].type === 'imageupload' ||
								components[component].type === 'country' ||
								components[component].type === 'link' ||
								components[component].type === 'sfahtmlelement' ||
								components[component].type === 'youtube' ||
								components[component].type === 'sfafile' ||
								components[component].type === 'popupform' ||
								components[component].type === 'sfabutton'
							) {
								filter = { id: components[component].key, label: components[component].key, type: 'string' };

								key = components[component].key;
								formFieldNames.indexOf(key) === -1 ? formFieldNames.push(key) : '';
								cnt = filterx.filters.filter(i => i.id === key);
								if (filterx.filters.length === 0 || cnt.length === 0) {
									if (filter.id !== '') {
										filterx.filters.push(filter);
									}
								}
							}
						}
						if (components[component].hasOwnProperty('components')) {
							loadcomponents(components[component].components);
						}
					}
				} else {
					filter = { id: 'placeholder', label: 'Your form has no fields - add at least one before creating rules', type: 'string' };
					key = 'placeholder';
					cnt = filterx.filters.filter(i => i.id === key);
					if (filterx.filters.length === 0 || cnt.length === 0) {
						filterx.filters.push(filter);
					}
				}
				filterx.filters.sort((a, b) => a.label.localeCompare(b.label));
				fieldoptions = '';
				for (i = 0; i < filterx.filters.length; ++i) {
					fieldoptions += '<option value="' + filterx.filters[i].id + '">' + filterx.filters[i].label + '</option>';
				}
			}
			panzoomLoad();
		}

		function panzoomLoad() {
			panzoom(document.querySelector('.querybuilder'), {
				maxZoom: 1,
				minZoom: 0.3,
				bounds: true,
				boundsPadding: 0.15
			}).zoomAbs(
				300, // initial x position
				500, // initial y position
				1  // initial zoom 
			);
		}
		// validation function ----Begin
		function validateExistingFields(ruleData) {
			var ruleset = ruleData.set[0];
			var errorFields = [];
			try {
				// condition
				if (ruleset.condition === null) {
					errorFields.push('HasErrorCondition');
				} else {
					ruleset.condition.rules.forEach(e => {
						if (formFieldNames.indexOf(e.field) == -1) {
							errorFields.indexOf(e.field) == -1 ? errorFields.push(e.field) : '';
						}
					})
				}
				// if_yes
				ruleset.if_yes.forEach(e => {
					var feildK = e.action.field;
					if (formFieldNames.indexOf(feildK) == -1) {

						errorFields.indexOf(feildK) == -1 ? errorFields.push(feildK) : '';
					}
				})
				// if_no
				ruleset.if_no.forEach(e => {
					var feildK = e.action.field;
					if (formFieldNames.indexOf(feildK) == -1) {
						errorFields.indexOf(feildK) == -1 ? errorFields.push(feildK) : '';
					}
				})
			} catch (e) { }

			return errorFields;
		}
		// validation function ----End

		var dataTable = _$rulesTable.DataTable({
			paging: true,
			serverSide: true,
			scrollX: true,
			responsive: {
				details: false
			},
			createdRow: function (row, data, dataIndex) {
				$(row).find("[name='DisableRuleLink']").on("click", async function () {
					await _formRulesService.toggleRule(data.formRule.id, false);
					getFormRules();
					UpdateFormScript()
				});

				$(row).find("[name='EnableRuleLink']").on("click", async function () {
					await _formRulesService.toggleRule(data.formRule.id, true);
					getFormRules();
					UpdateFormScript()
				});

				$(row).find("[name='EditRuleLink']").on("click", function () {
					var tabs = $('ul.nav').find('li');
					tabs.each(function () {
						$(this).removeClass('active');
						$(this).removeClass('show');
					});
					$('#rulebuildtab').closest('li').addClass('active');
					loadRule(data.formRule, validateExistingFields);
				});

				$(row).find("[name='DeleteRuleLink']").on("click", function () {
					swal({
						title: "Are you sure?",
						text: "Once deleted, you will not be able to recover this rule!",
						icon: "warning",
						buttons: true,
						dangerMode: true,
					}).then(async function (willDelete) {
						var RuleResult = await _formRulesService.delete({ id: data.formRule.id });
						if (willDelete) {
							if (RuleResult) {
								abp.notify.info(app.localize('DeletedSuccessfully'));
								UpdateFormScript();
								getFormRules();
								loadBlankRule();
							} else {
								abp.notify.warn(app.localize('DeletedUnsuccessfully'));
							}
						}
					});
				});
			},
			listAction: {
				ajaxFunction: _formRulesService.getRulesByFormId,
				inputFilter: function () {
					return { id: JSONFormObj.Id, isEnabledFilter: false };
				}
			},
			columnDefs: [
				{
					targets: 0,
					data: "formRule.rule",
					render: function (data, type, row) {
						var ruleData = JSON.parse(data);
						data = ruleData.name
						return data;
					}
				},
				{
					targets: 1,
					orderable: false,
					data: "formRule.lastModificationTime",
					render: function (data, type, row) {
						var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
						var tmoptions = { hour: 'numeric', minute: 'numeric' };

						if (row.formRule.lastModificationTime === null) {
							dt = new Date(row.formRule.creationTime);
							dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
							return dt;
							//return data.creationTime.toString();
						} else {
							dt = new Date(row.formRule.lastModificationTime);
							dt = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
							return dt;
							//return data.lastModificationTime.toString();
						}
					}
				},
				{
					targets: 2,
					orderable: false,
					data: "formRule.rule",
					render: function (data, type, row) {
						var ruleData = JSON.parse(data);
						var errorFields = validateExistingFields(ruleData);
						if (errorFields.length > 0) {
							data = '<span style="color: red;font-weight: bold;" title="Error field: ';
							errorFields.forEach(e => {
								data += '\n--' + e;
							})
							data += '">&#9747;</span>';
						} else {
							data = '<span style="color: chartreuse;font-weight: bold;">&#10003;</span>';

						}
						//data = errorFields.length > 0 ? '<span style="color: red;font-weight: bold;" title="test \ntest \ntest">&#9747;</span>' : '<span style="color: chartreuse;font-weight: bold;">&#10003;</span>';
						return data;
					}
				},
				{
					targets: 3,
					responsivePriority: 1,
					orderable: false,
					data: "FormName",
					render: function (data, type, row) {
						data = '<div class="pull-right">';
						data += '<div class="ml-1">';
						if (row.formRule.isEnabled == true) {
							data += '<a class="OnClickLink" name="DisableRuleLink" href="#">  Disable</a> | ';
						} else {
							data += '<a class="OnClickLink" name="EnableRuleLink" href="#">  Enable&nbsp;</a> | ';
						}
						data += '<a class="OnClickLink" href="' + abp.appPath + 'Falcon/FormRules/GetRulesForDownload?FormId=' + JSONFormObj.Id + '&RuleId=' + row.formRule.id + '" name="ExportRuleLink">  Export</a> | ';
						data += '<a class="OnClickLink" name="EditRuleLink" href="#rulebuild" data-toggle="tab" data-bs-toggle="tab"><i class=""></i> Edit</a> | ';
						///Falcon/FormRules / GetRulesForDownload ? FormId = data.formRule.id & version=@Html.Raw(Model.Form.Version.ToString())
						data += '<a class="OnClickLink" name="DeleteRuleLink" href="#"><i class="  text-danger"></i> Delete</a>';
						data += '</div>';
						data += '</div>';
						return data;
					}
				}
			]
		});

		$('#rulelisttab').click(function () {
			setTimeout(function () {
				getFormRules();;
			}, 500);
			//dataTable.fixedHeader.adjust();
		});

		function getFormRules() {
			dataTable.ajax.reload();
		}

		function loadBlankRule() {
			$('.querybuilder').empty();
			$('#Rule_Name').val('');
			$('#RuleId').val('');
			var elem_id = Math.round(new Date().getTime() + (Math.random() * 100));
			createConditionalSection(elem_id, 'main', '.querybuilder', null)
			queryRulesBuilder(elem_id);
			$('#btn-create-rules').addClass('hidden');
		}

		loadBlankRule();

		async function loadRule(Rule, valiF) {
			if ($('#RuleId').val() === "") {
				$('.querybuilder').empty();
			}
			$('.querybuilder').empty();
			try {
				var json_object = JSON.parse(Rule.rule);
				$('#Rule_Name').val('' + json_object.name + '');
				$('#RuleId').val('' + Rule.id + '');
				createLogicTree(json_object.set, '.querybuilder');
			} catch {
				// continue
			}
			$('#btn-create-rules').addClass("hidden");
		}

		async function saveRules() {
			var ruleSet = {};
			if ($('#Rule_Name').val() === null || $('#Rule_Name').val() === "") {
				ruleSet['name'] = 'Rule_' + Math.round(new Date().getTime() + (Math.random() * 100));
			} else {
				ruleSet['name'] = $('#Rule_Name').val();
			}

			ruleSet['set'] = dfs($('.querybuilder'));

			var FormId = $('#FormId').val();
			var RuleId = $('#RuleId').val() === (null || "") ? null : $('#RuleId').val();
			var rules = JSON.stringify(ruleSet, undefined, 2);
			var errorFields = validateExistingFields(ruleSet);
			if (errorFields.length > 0) {
				return false;
			}
			if (rules.indexOf('ErrorFieldName') !== -1) return false;
			if (FormId !== null && FormId !== "") {
				var RuleResult = await _formRulesService.createOrEdit({
					Id: RuleId,
					Rule: rules,
					FormId: FormId
				});

				if (RuleResult) {
					$('#RuleId').val(RuleResult);
					return UpdateFormScript();
				} else {
					return false;
				}
			}
			//    .done(function () {
			//    abp.notify.info(app.localize('SavedSuccessfully'));
			//    abp.event.trigger('app.rulesEditorModalSaved');
			//}).always(function () {

			//});
		}

		async function UpdateFormScript() {
			var JointRules = '';
			var FormId = $('#FormId').val();
			var AllRules = await _formRulesService.getRulesByFormId({ id: FormId, isEnabledFilter: true, maxResultCount: 1000 });
			var i;
			for (i = 0; i < AllRules.items.length; i++) {
				JointRules += AllRules.items[i].formRule.rule;
				if (i !== AllRules.items.length - 1) {
					JointRules += ',';
				}
			}

			rules = '{ "rules" : { "rule" : [' + JointRules + ']}}';

			var rulesscript = RunRulesGenerateScript(rules);
			RuleResult = _formsService.saveRules({
				Id: FormId,
				RulesScript: rulesscript
			});

			if (RuleResult) {
				return true;
			} else {
				return false;
			}
		}

		/* Recursive function to create logic app object tree including all the nodes */
		function dfs(element) {
			var set = [];
			element.children('.logics').each(function () {
				var sectionId = $(this).attr('id');
				var commonID = sectionId.split('_')[1];
				var child_yes = $('#sub-yes_' + commonID);
				var child_no = $('#sub-no_' + commonID);
				var builderElement = $('#builder_' + commonID);
				var item = {};
				item.description = $('#input-description_' + commonID).val();
				item.section_id = commonID;
				item.section_type = ($(this).hasClass('sub-sections')) ? 'sub' : 'main';
				if (builderElement.length) {
					if (builderElement.hasClass('condition')) {
						item.type = 'condition';
						item.condition = $(builderElement).queryBuilder('getRules');
						item.if_yes = dfs($(child_yes));
						item.if_no = dfs($(child_no));
					} else {
						item.type = 'action';
						item.parent_elem_id = $(this).data('parent-id')
						item.action_condtion = $(this).parent().hasClass('sub-yes') ? 'yes' : 'no';
						item.action = getActionSetJson(commonID)
					}
				}
				set.push(item);
			});
			return set;
		}

		//function generateScript() {
		//    var ruleSet = {};
		//    if ($('#Rule_Name').val() === null || $('#Rule_Name').val() === "") {
		//        ruleSet['name'] = 'Rule_' + Math.round(new Date().getTime() + (Math.random() * 100));
		//    } else {
		//        ruleSet['name'] = $('#Rule_Name').val();
		//    }
		//    ruleSet['set'] = dfs($('.querybuilder'));

		//    var rules = JSON.stringify(ruleSet, undefined, 2);
		//    rules = '{ "rules" : { "rule" : [' + rules + ']}}';

		//    var result = RunRulesGenerateScript(rules);
		//    return result;
		//}

		/* Create an action set json object */
		function getActionSetJson(parent_elem_id) {
			var action_type = $('#action_builder_rule_operator_' + parent_elem_id).val();
			var apply_to = $('#action_builder_rule_apply_to_' + parent_elem_id).val();
			var field = $('#action_builder_rule_filter_' + parent_elem_id).val();
			var action_value = $('#action_builder_rule_value_' + parent_elem_id).val();
			var action_object = {
				'action_type': action_type,
				'apply_to': apply_to,
				'field': field,
				'action_value': action_value
			};
			return action_object;
		}

		/* Show or hide the action fields */
		function showHideActionValues(e, type = 'create') {
			var commonID = $(e).attr('id').split('_')[4];
			var value = type === 'edit' ? e.val() : e.value;
			var ctrl = $(e).closest('.rule-container').find('.rule-value-container');
			var fieldselect = $('#action_builder_rule_filter_' + commonID);
			var applyto = $('#action_builder_rule_apply_to_' + commonID);

			if (value === 'Set Value' || value === 'Show Message Box' || value === 'Set Document Template' || value === 'Append Document Template') {
				$('#action_builder_rule_value_' + commonID).prop('disabled', false);
			}
			else {
				if (value === "Show Form" || value === "Hide Form") {
					$(applyto).first().prop('disabled', true);
				} else {
					$(applyto).first().prop('disabled', false);
				}
				$('#action_builder_rule_value_' + commonID).val('');
				$('#action_builder_rule_value_' + commonID).prop('disabled', true);
			}

			if (value === 'Show Message Box') {
				$(fieldselect).first().prop('disabled', true);
				$(fieldselect).first().prop('disabled', true);
				$(applyto).first().prop('disabled', true);
				$(applyto).first().prop('disabled', true);
			}
			else {
				$(fieldselect).first().prop('disabled', false);
				$(fieldselect).first().prop('disabled', false);
				if (value === "Show Form" || value === "Hide Form") {
					$(applyto).first().prop('disabled', true);
					$(applyto).first().prop('disabled', true);
				} else {
					$(applyto).first().prop('disabled', false);
					$(applyto).first().prop('disabled', false);
				}
			}

			if (value === 'Set Document Template' || value === 'Append Document Template') {
				$(applyto).first().prop('disabled', true);
				$(fieldselect).first().prop('disabled', true);
				$(ctrl).first().find('input').prop('disabled', false);
			}
		}

		/* Create an action section */
		function queryActionBuilder(elem_id) {

			var html = '<div class="rules-group-body action-container" id="action_container_' + elem_id + '">' +
				'<div class="rules-list">' +
				'<div id="action_builder_' + elem_id + '_rule" class="rule-container">' +
				'<div class="rule-header">' +
				'<div class="btn-group pull-right rule-actions" > <button type="button" class="btn btn-xs btn-danger remove-action" data-bs-delete="rule" data-delete="rule"  data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '">' +
				'<i class="fa fa-times"></i>' +
				'</button> </div >' +
				'</div >' +
				'<div class="rule-operator-container">' +
				'<select class="form-control action-ctrl" id="action_builder_rule_operator_' + elem_id + '">' +
				'<option value = "-1">Select action</option>' +
				'<option>Set Value</option>' +
				'<option>Show</option>' +
				//'<option>Show (don"t toggle)</option>' +
				//'<option>Show (and content)</option>' +
				'<option>Hide</option>' +
				//'<option>Hide (don"t toggle)</option > ' +
				//'<option>Hide and clear values</option>' +
				'<option>Enable</option>' +
				//'<option>Enable (don"t toggle)</option>' +
				'<option>Disable</option>' +
				//'<option>Disable (don"t toggle)</option>' +
				'<option>Show Message Box</option>' +
				//'<option>Show Form</option>' +
				//'<option>Hide Form </option>' +
				//'<option>Set Document Template</option>' +
				//'<option>Append Document Template</option>' +
				'</select ></div >' +
				'<div class="rule-operator-container">' +
				'<select class="form-control " id="action_builder_rule_apply_to_' + elem_id + '">' +
				'<option value = "-1"> Apply to</option>' +
				'<option>Current Repeat</option>' +
				'<option>Entire Form</option>' +
				'</select ></div >' +
				'<div class="rule-filter-container"><select class="form-control" id="action_builder_rule_filter_' + elem_id + '">' +
				'<option value="-1">Select field</option>' +
				fieldoptions +
				'</select></div>' +
				'<div class="rule-value-container"><input class="form-control" type="text" id="action_builder_rule_value_' + elem_id + '"></div>' +
				'</div>' +
				'</div>';

			$('#builder_' + elem_id + ' .rules-group-container').append(html)
		}

		/* Create an action section on load */
		function queryActionBuilderOnLoad(elem_id, rule) {

			var html = '<div class="rules-group-body action-container" id="action_container_' + elem_id + '">' +
				'<div class="rules-list">' +
				'<div id="action_builder_' + elem_id + '_rule" class="rule-container">' +
				'<div class="rule-header">' +
				'<div class="btn-group pull-right rule-actions" > <button type="button" class="btn btn-xs btn-danger remove-action" data-bs-delete="rule" data-delete="rule"  data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '">' +
				'<i class="fa fa-times"></i>' +
				'</button> </div >' +
				'</div >' +
				'<div class="rule-operator-container">' +
				'<select class="form-control action-ctrl" id="action_builder_rule_operator_' + elem_id + '">' +
				'<option value = "-1">Select action</option>' +
				'<option>Set Value</option>' +
				'<option>Show</option>' +
				//'<option>Show (don"t toggle)</option>' +
				//'<option>Show (and content)</option>' +
				'<option>Hide</option>' +
				//'<option>Hide (don"t toggle)</option > ' +
				//'<option>Hide and clear values</option>' +
				'<option>Enable</option>' +
				//'<option>Enable (don"t toggle)</option>' +
				'<option>Disable</option>' +
				//'<option>Disable (don"t toggle)</option>' +
				'<option>Show Message Box</option>' +
				//'<option>Show Form</option>' +
				//'<option>Hide Form </option>' +
				//'<option>Set Document Template</option>' +
				//'<option>Append Document Template</option>' +
				'</select ></div >' +
				'<div class="rule-operator-container">' +
				'<select class="form-control " id="action_builder_rule_apply_to_' + elem_id + '">' +
				'<option value = "-1"> Apply to</option>' +
				'<option>Current Repeat</option>' +
				'<option>Entire Form</option>' +
				'</select ></div >' +
				'<div class="rule-filter-container"><select class="form-control" id="action_builder_rule_filter_' + elem_id + '">' +
				'<option value="-1">Select field</option>' +
				fieldoptions +
				'</select></div>' +
				'<div class="rule-value-container"><input class="form-control" type="text" id="action_builder_rule_value_' + elem_id + '"></div>' +
				'</div>' +
				'</div>';

			$('#builder_' + elem_id + ' .rules-group-container').append(html);
			$('#action_builder_rule_operator_' + elem_id).val(rule.action_type);
			$('#action_builder_rule_filter_' + elem_id).val(rule.field);
			$('#action_builder_rule_value_' + elem_id).val(rule.action_value);
			$('#action_builder_rule_apply_to_' + elem_id).val(rule.apply_to);
			showHideActionValues($('#action_builder_rule_operator_' + elem_id), 'edit');
		}

		/* Build an action section wrapper on load */
		function createActionSectionLoad(elem_id, section_type, selector, parent_elem_id, description) {
			var html = '<div id="section_' + elem_id + '" data-parent-id="' + parent_elem_id + '" class="logics ';
			html += (section_type === 'main') ? 'sections"' : ' sub-sections"';
			if (section_type !== 'main') {
				html += '<input  class="form-control form-control-description" id="input-description_' + elem_id + '" type="text" placeholder="Enter description">';
			}
			html += ' <div id = "builder_' + elem_id + '" class= "builder actions form-inline query-builder"';
			html += ' ><div class="rules-group-container"> </div></div></div>';

			$(selector).append(html);
			$('#input-description_' + elem_id).val(description);
		}

		/* Jquery query builder on load */
		function queryBuilderOnLoad(item) {
			var hasCondition = item.condition !== null ? true : false;

			var errorF = [];
			if (hasCondition) {
				item.condition.rules.forEach(e => {
					if (formFieldNames.indexOf(e.field) == -1) {
						errorF.indexOf(e.field) == -1 ? errorF.push(e.field) : '';
					}
				})
			}
			filterx.filters.forEach((filter, index) => {
				if (filter.id === 'ErrorFieldName') {
					filterx.filters.pop(index + 1);
				}
			})
			if (!hasCondition || errorF.length !== 0) {
				var tFilters = filterx.filters;
				tFilters.push({ id: "ErrorFieldName", label: "ErrorFieldName", type: "string" });
				var tItem = item.condition === null ? { condition: "AND", rules: [], allow_empty: true } : item.condition;
				tItem.rules = [{ id: "ErrorFieldName", field: "ErrorFieldName", type: "string", input: "text", operator: "equal", type: "string", value: "Please update this rule." }]
				var itemWithNoGroupRule = getItemWithNoGroupRule(tItem.condition);
				try {
					jQuery('#builder_' + item.section_id).queryBuilder({
						plugins: ['bt-tooltip-errors'],
						filters: tFilters,
						rules: tItem,
						allow_empty: true
					});
				} catch (e) {
					jQuery('#builder_' + item.section_id).queryBuilder("destroy");
				}

			} else {
				var itemWithNoGroupRule = getItemWithNoGroupRule(item.condition);
				jQuery('#builder_' + item.section_id).queryBuilder({
					plugins: ['bt-tooltip-errors'],
					filters: filterx.filters,
					rules: item.condition
				});
			}
			//var itemWithNoGroupRule = getItemWithNoGroupRule(item.condition);
			//jQuery('#builder_' + item.section_id).queryBuilder({
			//	plugins: ['bt-tooltip-errors'],
			//	filters: filterx.filters,
			//	rules: item.condition
			//});
		}

		/* Build an ACTION section on object load */
		function buildActionSectionsOnLoad(item) {
			var wrapper = item.action_condtion === 'yes' ? '#sub-yes_' + item.parent_elem_id : '#sub-no_' + item.parent_elem_id;
			createActionSectionLoad(item.section_id, 'sub', wrapper, item.parent_elem_id, item.description);
			queryActionBuilderOnLoad(item.section_id, item.action);
		}

		/* Remove group rule from the condtions */
		function getItemWithNoGroupRule(queryCondition) {
			var flags = {
				no_add_group: true
			};
			queryCondition.flags = flags;
			return queryCondition;
		}

		/* Build condition section on object load */
		function buildConditionSectionsOnLoad(item, wrapper) {
			var html = '<div id="section_' + item.section_id + '" class="logics ';
			html += item.section_type === 'main' ? 'sections"' : ' sub-sections"';
			html += '<div class="add-more-section" data-id="' + item.section_id + '" data-bs-id="' + item.section_id + '"> ' +
				'<input class="form-control" id="input-description_' + item.section_id + '" type="text" placeholder="Enter description">' +
				'<div id="builder_' + item.section_id + '" class="condition builder"></div >' +
				'<h1 class="dashed-down-arrow">&#x21e3</h1>' +
				'<div class="container conditions-section" id="conditions-section_' + item.section_id + '">' +
				'<div class="row-fluid">' +
				'<div class="yes-block" id="yes-block_' + item.section_id + '">' +
				'<p class="p-yes"><i class="glyphicon glyphicon-ok"></i> If condition is true</p>' +
				'<div class="row-fluid">' +
				'<div class="col-12  add_more_actions_container" id="add_more_actions_container_yes_' + item.section_id + '">';
			//hidden due to nested condition issues - awaitng resolution
			//if (item.if_yes[0]) {data-elem-id="' + item.s
			//    if (item.if_yes[0].action) {
			//        html += '<button type="button" class="btn btn-xs btn-success btn-add-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-action_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>'+
			//            '<button type="button" class="btn btn-xs btn-success btn-add-condition hidden" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-condition_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
			//            '<button type="button" class="btn btn-xs btn-danger btn-clear-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-clear-action_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-times adjust"></i> Clear All</button>';
			//    } else {
			//        html += '<button type="button" class="btn btn-xs btn-success btn-add-action hidden" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-action_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>'+
			//            '<button type="button" class="btn btn-xs btn-success btn-add-condition hidden" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-condition_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
			//            '<button type="button" class="btn btn-xs btn-danger btn-clear-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-clear-action_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-times adjust"></i> Remove Condition</button>';
			//    }
			//} else {
			html += '<button type="button" class="btn btn-xs btn-success btn-add-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-action_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
				//        '<button type="button" class="btn btn-xs btn-success btn-add-condition" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-condition_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
				'<button type="button" class="btn btn-xs btn-danger btn-clear-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="yes" data-condition="yes" id="btn-clear-action_yes_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-times adjust"></i> Clear All</button>';
			//}
			html += '<div style="clear:both"></div>' +
				'</div></div>' +
				'<div class="row-fluid sub-yes sub-logic" id="sub-yes_' + item.section_id + '"></div>' +
				'</div>' +
				'<div class="no-block" id="no-block_' + item.section_id + '">' +
				'<p class="p-no"><i class="fa fa-times"></i> If condition is false</p>' +
				'<div class="row-fluid">' +
				'<div class="col-12 add_more_actions_container" id="add_more_actions_container_no_' + item.section_id + '">';
			//hidden due to nested condition issues - awaitng resolution
			//if (item.if_no[0]) {
			//    if (item.if_no[0].action) {
			//        html += '<button type="button" class="btn btn-xs btn-success btn-add-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '"  data-bs-condition="no" data-condition="no" id="btn-add-action_no_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>'+
			//            '<button type="button" class="btn btn-xs btn-success btn-add-condition hidden" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="no" data-condition="no" id="btn-add-condition_no_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
			//            '<button type="button" class="btn btn-xs btn-danger btn-clear-action"  data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="no" data-condition="no" id="btn-clear-action_no_' + item.section_id + '"  data-bs-add="rule" data-add="rule"> <i class="fa fa-times adjust"></i> Clear All</button>';
			//    } else {
			//        html += '<button type="button" class="btn btn-xs btn-success btn-add-action hidden" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '"  data-bs-condition="no" data-condition="no" id="btn-add-action_no_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>'+
			//            '<button type="button" class="btn btn-xs btn-success btn-add-condition hidden" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="no" data-condition="no" id="btn-add-condition_no_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
			//            '<button type="button" class="btn btn-xs btn-danger btn-clear-action"  data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="no" data-condition="no" id="btn-clear-action_no_' + item.section_id + '"  data-bs-add="rule" data-add="rule"> <i class="fa fa-times adjust"></i> Remove Condition</button>';
			//    }
			//} else {
			html += '<button type="button" class="btn btn-xs btn-success btn-add-action" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '"  data-bs-condition="no" data-condition="no" id="btn-add-action_no_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
				//        '<button type="button" class="btn btn-xs btn-success btn-add-condition" data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="no" data-condition="no" id="btn-add-condition_no_' + item.section_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
				'<button type="button" class="btn btn-xs btn-danger btn-clear-action"  data-bs-elem-id="' + item.section_id + '" data-elem-id="' + item.section_id + '" data-bs-condition="no" data-condition="no" id="btn-clear-action_no_' + item.section_id + '"  data-bs-add="rule" data-add="rule"> <i class="fa fa-times adjust"></i> Clear All</button>';
			//}
			html += '<div style="clear:both"></div>' +
				'</div></div>' +
				'<div class="row-fluid sub-no sub-logic" id="sub-no_' + item.section_id + '"></div>' +
				'</div>' +
				'</div>' +
				'</div>' +
				'</div><hr>';

			$(wrapper).append(html);
			$('#input-description_' + item.section_id).val(item.description);
			queryBuilderOnLoad(item);

			item.if_yes && createLogicTree(item.if_yes, '#sub-yes_' + item.section_id);
			item.if_no && createLogicTree(item.if_no, '#sub-no_' + item.section_id);
		}

		/* Create Logic tree */
		function createLogicTree(json_object, wrapper) {
			$.each(json_object, function (i, item) {
				item.type === 'condition' && buildConditionSectionsOnLoad(item, wrapper);
				item.type === 'action' && buildActionSectionsOnLoad(item);
			});

			//showHideActionOrConditionButtons();
		}

		//function showHideActionOrConditionButtons() {
		//    var parent = $('.action-section').parent().parent();
		//    parent.children('.action-container').find('.action_select').addClass('hidden');
		//    parent.children('.action-container').find('.add_more_actions_container').removeClass('hidden');

		//    var no_of_sections = $('#container').children('.sections').length;
		//    no_of_sections > 0 && $('#btn-create-rules-main').addClass('hidden')
		//    no_of_sections > 0 && $('.btn-clear-sections, .btn-save-rules').removeClass('hidden');
		//}

		/* Create an action section wrapper */
		function createActionSection(elem_id, section_type, selector, parent_elem_id) {
			var html = '<div id="section_' + elem_id + '" data-parent-id="' + parent_elem_id + '" class="logics ';
			html += section_type === 'main' ? 'sections"' : ' sub-sections"';
			if (section_type !== 'main') {
				html += '<input class="form-control" id="input-description_' + elem_id + '" type="text" placeholder="Enter description">'
			}
			html += ' <div id = "builder_' + elem_id + '" class= "builder actions form-inline query-builder"';
			html += ' ><div class="rules-group-container"> </div></div></div>';

			$(selector).append(html);
		}

		/* Create a conditional section */
		function createConditionalSection(elem_id, section_type, selector, super_parent_elem_id = '', type = 'create') {
			var html = '<div id="section_' + elem_id + '" class="logics ';
			html += (section_type === 'main') ? 'sections"' : ' sub-sections" style="padding: 0px 2.5px 0px 5px;">';
			html += '<div class="add-more-section" data-bs-id="' + elem_id + '" data-id="' + elem_id + '" > ';
			//html += '<button type="button" data-bs-id="' + elem_id + '" data-id="' + elem_id + '" id="btn-add-more-section_' + elem_id + '" class="btn btn-default btn-add-more-section">';
			//html += '<span class="fa fa-plus-sign"></span></button><h1 class="down-arrow">&#x2193</h1></div>';
			if (section_type !== 'main') {

			};
			html += '<input class="form-control" id="input-description_' + elem_id + '" type="text" placeholder="Enter description">';
			html += '<div id = "builder_' + elem_id + '" class= "condition builder"';
			html += '></div ><h1 class="dashed-down-arrow">&#x21e3</h1>' +
				'<div class="container conditions-section" id="conditions-section_' + elem_id + '">' +
				'<div class="row-fluid">' +
				'<div class="yes-block" id="yes-block_' + elem_id + '">' +
				'<p class="p-yes"><i class="glyphicon glyphicon-ok"></i> If condition is true</p>' +
				'<div class="row-fluid">' +
				//'<div class="action_select form-inline"  id = "action_select_yes_' + elem_id + '" > <select class="select_action form-control" id="btn-sub-yes-select-action_' + elem_id + '"> <option value="action">Take No Action</option> <option value="action">Action</option>  <option value="condition">Condition</option></select> & nbsp; ' +
				//'<button class="btn btn-outline-primary btn-sub-condtion form-control" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '" data-super-parent-elem-id="' + super_parent_elem_id + '" data-condition = "yes" id="btn-sub-yes-condition_' + elem_id + '">select</button>' +
				//'</div>' +
				'<div class="col-12  add_more_actions_container" id="add_more_actions_container_yes_' + elem_id + '">' +
				'<button type="button" class="btn btn-xs btn-success btn-add-action" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-action_yes_' + elem_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
				//hidden due to nested condition issues - awaitng resolution
				//'<button type="button" class="btn btn-xs btn-success btn-add-condition" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '" data-bs-condition="yes" data-condition="yes" id="btn-add-condition_yes_' + elem_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
				'<button type="button" class="btn btn-xs btn-danger btn-clear-action" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '" data-bs-condition="yes" data-condition="yes" id="btn-clear-action_yes_' + elem_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-times adjust"></i> Clear All</button>' +
				'<div style="clear:both"></div>' +
				'</div></div>' +
				'<div class="row-fluid sub-yes sub-logic" id="sub-yes_' + elem_id + '"></div>' +
				'</div>' +
				'<div class="no-block" id="no-block_' + elem_id + '">' +
				'<p class="p-no"><i class="fa fa-times"></i> If condition is false</p>' +
				'<div class="row-fluid">' +
				//'<div class="action_select form-inline"  id = "action_select_no_' + elem_id + '" > <select class="select_action form-control" id="btn-sub-no-select-action_' + elem_id + '"> <option value="action">Take No Action</option> <option value="action">Action</option>  <option value="condition">Condition</option></select> & nbsp; <button class="btn btn-outline-primary btn-sub-condtion form-control" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '" data-bs-condition="no" data-condition="no" id="btn-sub-no-condition_' + elem_id + '">select</button>' +
				//'</div>' +
				'<div class="col-12 add_more_actions_container" id="add_more_actions_container_no_' + elem_id + '">' +
				'<button type="button" class="btn btn-xs btn-success btn-add-action" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '"  data-bs-super-parent-elem-id="' + super_parent_elem_id + '" data-super-parent-elem-id="' + super_parent_elem_id + '" data-bs-condition="no" data-condition="no" id="btn-add-action_no_' + elem_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
				//hidden due to nested condition issues - awaitng resolution
				//'<button type="button" class="btn btn-xs btn-success btn-add-condition" data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '"  data-super-parent-elem-id="' + super_parent_elem_id + '" data-bs-condition="no" data-condition="no" id="btn-add-condition_no_' + elem_id + '" data-bs-add="rule" data-add="rule"><i class="fa fa-plus"></i> Add condition</button>' +
				'<button type="button" class="btn btn-xs btn-danger btn-clear-action"  data-bs-elem-id="' + elem_id + '" data-elem-id="' + elem_id + '" data-bs-condition="no" data-condition="no" id="btn-clear-action_no_' + elem_id + '"  data-bs-add="rule" data-add="rule"><i class="fa fa-times adjust"></i> Clear All</button>' +
				'<div style="clear:both"></div>' +
				'</div></div>' +
				'<div class="row-fluid sub-no sub-logic" id="sub-no_' + elem_id + '"></div>' +
				'</div>' +
				'</div>' +
				'</div>' +
				'</div>';

			(type === 'more') ? $(html).insertBefore(selector) : $(selector).append(html);
			if ((section_type === 'main')) {
				$('#yes-block_' + elem_id).css('width', '50%');
				$('#no-block_' + elem_id).css('width', '50%');
			}
		}

		/* JQuery query builder for conditional section */
		function queryRulesBuilder(elem_id) {
			// build plugin in filter
			jQuery('#builder_' + elem_id).queryBuilder({
				plugins: ['bt-tooltip-errors'],

				filters: filterx.filters//,

				//rules: rules_basic,
			});
		}

		$('#CreateRuleButton').click(function () {
			var tabs = $('ul.nav').find('li');
			tabs.each(function () {
				$(this).removeClass('active');
				$(this).removeClass('show');
			});
			$('#rulebuildtab').closest('li').addClass('active');

			loadBlankRule();
		});

		$('#ImportRulesButton').click(function () {
			//_formRulesService.getRulesForExport(JSONFormObj.Id);
			_importRulesModal.open({ FormId: JSONFormObj.Id });
		});

		$('#btn-create-rules').click(function () {
			if ($('.querybuilder').children().length > 0) {
				abp.notify.info(app.localize('SaveCurrentRule'));
			} else {
				loadBlankRule();
				//var elem_id = Math.round(new Date().getTime() + (Math.random() * 100));
				//createConditionalSection(elem_id, 'main', '.querybuilder', null)
				//queryRulesBuilder(elem_id);
				//$('#btn-create-rules').addClass('hidden');
				//$(".btn-danger").contents().filter(function () { return this.nodeType == 3; }).remove();
			}
		});

		$('#btn-save-rules').click(async function () {
			var ruleSet = {};
			if ($('#Rule_Name').val() === null || $('#Rule_Name').val() === "") {
				ruleSet['name'] = 'Rule_' + Math.round(new Date().getTime() + Math.random() * 100);
			} else {
				ruleSet['name'] = $('#Rule_Name').val();
			}
			ruleSet['set'] = dfs($('.querybuilder'));
			//console.log(JSON.stringify(ruleSet, undefined, 2));

			var SaveSuccessful;
			SaveSuccessful = await saveRules();
			if (SaveSuccessful) {
				abp.notify.info(app.localize('SavedSuccessfully'));
				getFormRules();
			} else {
				abp.notify.warn(app.localize('SavedUnsuccessfully'));
			}
		});

		$('#btn-clear-rules').click(function () {
			swal({
				title: "Have you saved this rule?",
				text: "Once deleted, you will be unable to recover an unsaved rule!",
				icon: "warning",
				buttons: true,
				dangerMode: true
			}).then((willDelete) => {
				if (willDelete) {
					$('.querybuilder').empty();
					$('#btn-create-rules').removeClass('hidden');
					$('#Rule_Name').val('');
					$('#RuleId').val('');
				}
			});
		});

		//hidden due to nested condition issues - awaitng resolution
		//$(document).on('click', '.btn-add-condition', function () {
		//    var parent_elem_id = $(this).data('elem-id');
		//    var is_condition = $(this).data('condition');
		//    elem_id = Math.round(new Date().getTime() + Math.random() * 100);
		//    var wrapper = is_condition === 'yes' ? '#sub-yes_' + parent_elem_id : '#sub-no_' + parent_elem_id;
		//    createConditionalSection(elem_id, 'sub', wrapper, parent_elem_id);
		//    queryRulesBuilder(elem_id);
		//    $(this).addClass('hidden');
		//    $(this).siblings('.btn-add-action').addClass('hidden');
		//    $(this).siblings('.btn-clear-action').text('');
		//    $(this).siblings('.btn-clear-action').append('<i class="fa fa-times adjust"></i> Remove Condition');
		//    $(this).siblings('.btn-clear-action').css('margin-left', '-5px');
		//});

		$(document).on('click', '.btn-add-action', function () {
			var parent_elem_id = $(this).data('elem-id');
			var is_condition = $(this).data('condition');
			elem_id = Math.round(new Date().getTime() + Math.random() * 100);
			var wrapper = is_condition === 'yes' ? '#sub-yes_' + parent_elem_id : '#sub-no_' + parent_elem_id;
			createActionSection(elem_id, 'sub', wrapper, parent_elem_id);
			queryActionBuilder(elem_id);
			//hidden due to nested condition issues - awaitng resolution
			//$(this).siblings('.btn-add-condition').addClass('hidden');
		});

		$(document).on('click', '.btn-clear-action', function () {
			swal({
				title: "Are you sure?",
				text: "Once deleted, you will not be able to recover this imaginary file!",
				icon: "warning",
				buttons: true,
				dangerMode: true
			})
				.then((willDelete) => {
					if (willDelete) {
						var parent_elem_id = $(this).data('elem-id');
						var is_condition = $(this).data('condition');
						if (is_condition === 'yes') {
							var wrapper = '#sub-yes_' + parent_elem_id;
						} else {
							wrapper = '#sub-no_' + parent_elem_id;
						}
						$(wrapper).html('');
						//hidden due to nested condition issues - awaitng resolution
						//$(this).siblings('.btn-add-condition').removeClass('hidden');
						$(this).siblings('.btn-add-action').removeClass('hidden');
						$(this).text('');
						$(this).append('<i class="fa fa-times adjust"></i> Clear All');
						$(this).css('margin-left', '5px');
					}
				});

		});

		$(document).on('click', '.remove-action', function () {
			$(this).closest('.sub-sections').remove();
		});

		//Not used anywhere?
		$(document).on('click', '.btn-sub-condtion', function () {
			var parent_elem_id = $(this).data('elem-id');
			var is_condition = $(this).data('condition');
			var elem_id = Math.round(new Date().getTime() + Math.random() * 100);
			var select_yes_no = is_condition === 'yes' ? $('#btn-sub-yes-select-action_' + parent_elem_id).val() : $('#btn-sub-no-select-action_' + parent_elem_id).val();
			var action_type = select_yes_no === 'condition' ? 'condition' : select_yes_no === 'action' ? 'action' : '';
			var wrapper = is_condition === 'yes' ? '#sub-yes_' + parent_elem_id : '#sub-no_' + parent_elem_id;

			if ($('#section_' + parent_elem_id).hasClass('sections')) {
				if (is_condition === 'yes') {
					$('#section_' + parent_elem_id + ' .yes-block').removeAttr('style');
				} else {
					$('#section_' + parent_elem_id + ' .no-block').removeAttr('style');
				}

			}
			if (action_type === 'condition') {
				createConditionalSection(elem_id, 'sub', wrapper, parent_elem_id);
				queryRulesBuilder(elem_id);
			} else if (action_type === 'action') {
				$(this).parent().addClass('hidden');
				$(this).closest('.row-fluid').find('.add_more_actions_container').removeClass('hidden');
				$(wrapper).html('');
				createActionSection(elem_id, 'sub', wrapper, parent_elem_id);
				queryActionBuilder(elem_id, 1);
			}

			$(".btn-danger").contents().filter(function () { return this.nodeType === 3; }).remove();
		});

		$(document).on('change', '.action-ctrl', function () {
			showHideActionValues(this);
		});

	});
})();