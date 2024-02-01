jQuery(document).ready(function () {

    // GET THE FORM SCHEMA AND LIST OF FIELDS
    //var _formsService = abp.services.app.forms;
    //var fieldoptions = '';
    //var filterx = {"filters" : []};

    // Get Form Schema from app service
    //_formsService.getSchema({
    //    id: JSONFormObj.Id
    //})
    //.done(function (result) {
    //    loadform(result);
    //    });

    //loadform(JSONFormObj.Schema);

    //function loadform(form) {
    //    if (form === '' || !form) {
    //        form = '{"type": "form","display": "form","components": [{"label": "Save","action": "event","theme": "primary","shortcut": "","customClass": "d-inline mr-1","mask": false,"tableView": true,"type": "sfabutton","input": true,"key": "save","showValidations": false,"event": "save","leftIcon": "fas fa-save"},{"type": "sfabutton","label": "Submit","key": "submit","disableOnInvalid": true,"inline": true,"theme": "primary","customClass": "d-inline","input": true,"tableView": true}]}';
    //    }

    //    var formJSON = JSON.parse(form);


    //    if (formJSON.hasOwnProperty('components')) {    
    //        loadcomponents(formJSON.components);
    //    }

    //    function loadcomponents(components) {
    //        for (var component in components) {
    //            if (components[component].type !== 'button') {
 
    //                fieldoptions += '<option value="' + components[component].key + '">' + components[component].key + '</option>';

    //                // We dont have a class type 
    //                // Push into the filter for the condition editor
    //                if (components[component].type === 'sfatextfield' || 
    //                    components[component].type === 'textfield' || 
    //                    components[component].type === 'number' || 
    //                    components[component].type === 'textarea' || 
    //                    components[component].type === 'day' || 
    //                    components[component].type === 'datetime' || 
    //                    components[component].type === 'selectboxes' || 
    //                    components[component].type === 'checkbox' || 
    //                    components[component].type === 'phoneNumber' || 
    //                    components[component].type === 'radio' || 
    //                    components[component].type === 'url' || 
    //                    components[component].type === 'email' ||

    //                    components[component].type === 'sfaradioyn' ||
    //                    components[component].type === 'radiogroup' ||
    //                    components[component].type === 'sfatextarea' ||
    //                    components[component].type === 'checkboxgroup' ||
    //                    components[component].type === 'sfadatetime' ||
    //                    components[component].type === 'sfanumber' ||
    //                    components[component].type === 'sfaselect' ||
    //                    components[component].type === 'select' ||
    //                    components[component].type === 'sfabutton'

    //                ) { 
                        
    //                    var filter = { id: components[component].key, label: components[component].key, type: 'string' };     

    //                    var key = components[component].key;
    //                    var cnt = filterx.filters.filter(i => i.id === key);
    //                    if (filterx.filters.length === 0 || cnt.length === 0) {
    //                        filterx.filters.push(filter); 
    //                    }

    //                }

    //            }
    //            if (components[component].hasOwnProperty('components')) {
    //                loadcomponents(components[component].components);
    //            }
    //        }
    //    }

    //    panzoomLoad();

    //    //if (load === 'true') {
    //    //    try {
    //    //        var json_object = JSON.parse(JSONFormObj.Rules);
    //    //        createLogicTree(json_object.set, '.btn-container');
    //    //    }
    //    //    catch{
    //    //        // continue
    //    //    }
    //    //}

    //}

    //var url_string = window.location.href;
    //var url = new URL(url_string);
    //var load = url.searchParams.get("load");

    //var rules_basic = {
    //    condition: 'AND',
    //    rules: [{
    //        id: 'FIELD_4',
    //        operator: 'equal',
    //        value: 10
    //    },],
    //    flags: {
    //        no_add_group: true
    //    },
    //};

    //function panzoomLoad() {
    //    panzoom(document.querySelector('.querybuilder'), {
    //        maxZoom: 1,
    //        minZoom: 0.3
    //    }).zoomAbs(
    //        300, // initial x position
    //        500, // initial y position
    //        1  // initial zoom 
    //    );
    //}

    ///**
    // *Create a conditional section
    // *
    // * @param {string} elem_id
    // * @param {string} section_type
    // * @param {object} selector
    // * @param {string} super_parent_elem_id
    // */

    //function createConditionalSection(elem_id, section_type, selector, super_parent_elem_id = '', type = 'create') {
    //    var html = '<div id="section_' + elem_id + '" class="logics ';
    //    html += (section_type == 'main') ? 'sections"' : ' sub-sections"';
    //    html += ' ><div class="add-more-section" data-id="' + elem_id + '">';
    //    html += '<button type="button" data-id="' + elem_id + '" id="btn-add-more-section_' + elem_id + '" class="btn btn-default btn-add-more-section">';
    //    html += '<span class="fa fa-plus-sign"></span></button><h1 class="down-arrow">&#x2193</h1></div>';
    //    if (section_type != 'main') {

    //    }
    //    html += '<input class="form-control" id="input-description_' + elem_id + '" type="text" placeholder="Enter description">';
    //    html += '<div id = "builder_' + elem_id + '" class= "condition builder"';
    //    html += '></div ><h1 class="dashed-down-arrow">&#x21e3</h1>' +
    //        '<div class="container conditions-section" id="conditions-section_' + elem_id + '">' +
    //        '<div class="row-fluid">' +
    //        '<div class="yes-block" id="yes-block_' + elem_id + '">' +
    //        '<p class="p-yes"><i class="glyphicon glyphicon-ok"></i> If condition is true</p>' +
    //        '<div class="row-fluid"> <div class="action_select form-inline"  id="action_select_yes_' + elem_id + '"><select class="select_action form-control" id="btn-sub-yes-select-action_' + elem_id + '"> <option value="action">Action</option>  <option value="condition">Condition</option></select>&nbsp;' +
    //        '<button class="btn btn-outline-primary btn-sub-condtion form-control" data-elem-id="' + elem_id + '" data-super-parent-elem-id="' + super_parent_elem_id + '" data-condition = "yes" id="btn-sub-yes-condition_' + elem_id + '">select</button>' +
    //        '</div>' +
    //        '<div class="col-md-5  add_more_actions_container hidden" id="add_more_actions_container_yes_' + elem_id + '">' +
    //        '<button type="button" class="btn btn-xs btn-success btn-add-action" data-elem-id="' + elem_id + '" data-condition = "yes" id="btn-add-action_yes_' + elem_id + '" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
    //        '<button type="button" class="btn btn-xs btn-success btn-clear-action" data-elem-id="' + elem_id + '" data-condition = "yes" id="btn-clear-action_yes_' + elem_id + '" data-add="rule"><i class="fa fa-plus"></i> Clear All</button>' +
    //        '<div style="clear:both"></div>' +
    //        '</div></div>' +
    //        '<div class="row-fluid sub-yes sub-logic" id="sub-yes_' + elem_id + '"></div>' +
    //        '</div>' +
    //        '<div class="no-block" id="no-block_' + elem_id + '">' +
    //        '<p class="p-no"><i class="glyphicon glyphicon-remove"></i> If condition is false</p>' +
    //        '<div class="row-fluid"> <div class="action_select form-inline"  id="action_select_no_' + elem_id + '"><select class="select_action form-control" id="btn-sub-no-select-action_' + elem_id + '"> <option value="action">Action</option>  <option value="condition">Condition</option></select>&nbsp;<button class="btn btn-outline-primary btn-sub-condtion form-control" data-elem-id="' + elem_id + '" data-condition = "no" id="btn-sub-no-condition_' + elem_id + '">select</button>' +
    //        '</div>' +
    //        '<div class="col-md-5 add_more_actions_container hidden" id="add_more_actions_container_no_' + elem_id + '">' +
    //        '<button type="button" class="btn btn-xs btn-success btn-add-action" data-elem-id="' + elem_id + '"  data-super-parent-elem-id="' + super_parent_elem_id + '" data-condition = "no" id="btn-add-action_no_' + elem_id + '" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
    //        '<button type="button" class="btn btn-xs btn-success btn-clear-action"  data-elem-id="' + elem_id + '" data-condition = "no" id="btn-clear-action_no_' + elem_id + '"  data-add="rule">         <i class="fa fa-plus"></i> Clear All</button>' +
    //        '<div style="clear:both"></div>' +
    //        '</div></div>' +
    //        '<div class="row-fluid sub-no sub-logic" id="sub-no_' + elem_id + '"></div>' +
    //        '</div>' +
    //        '</div>' +
    //        '</div>' +
    //        '</div><hr>';

    //    (type == 'more') ? $(html).insertBefore(selector) : $(selector).append(html);
    //    if ((section_type == 'main')) {
    //        $('#yes-block_' + elem_id).css('width', '750px');
    //        $('#no-block_' + elem_id).css('width', '750px');
    //    }

    //}

    ///**
    // * Create an action section wrapper
    // *
    // * @param {string} elem_id
    // * @param {string} section_type
    // * @param {object} selector
    // * @param {string} parent_elem_id
    // */
    //function createActionSection(elem_id, section_type, selector, parent_elem_id) {
    //    var html = '<div id="section_' + elem_id + '" data-parent-id="' + parent_elem_id + '" class="logics ';
    //    html += (section_type == 'main') ? 'sections"' : ' sub-sections"';
    //    html += ' >';
    //    if (section_type != 'main') {
    //        html += '<input class="form-control" id="input-description_' + elem_id + '" type="text" placeholder="Enter description">'
    //    }
    //    html += ' <div id = "builder_' + elem_id + '" class= "builder actions form-inline query-builder"';
    //    html += ' ><div class="rules-group-container"> </div></div></div>';

    //    $(selector).append(html);
    //}

    ///**
    // *Create an action section
    // *
    // * @param {string} elem_id
    // */
    //function queryActionBuilder(elem_id) {
    //    var html = '<div class="rules-group-body action-container" id="action_container_' + elem_id + '">' +
    //        '<div class="rules-list">' +
    //        '<div id="action_builder_' + elem_id + '_rule" class="rule-container">' +
    //        '<div class="rule-header">' +
    //        '<div class="btn-group pull-right rule-actions" > <button type="button" class="btn btn-xs btn-danger remove-action" data-delete="rule" data-elem-id="' + elem_id + '">' +
    //        '<i class="glyphicon glyphicon-remove"></i>' +
    //        '</button> </div >' +
    //        '</div >' +
    //        '<div class="rule-operator-container">' +
    //        '<select class="form-control action-ctrl" id="action_builder_rule_operator_' + elem_id + '">' +
    //        '<option value = "-1">Select action</option>' +
    //        '<option>Set Value</option>' +
    //        '<option>Show</option>' +
    //        //'<option>Show (don"t toggle)</option>' +
    //        //'<option>Show (and content)</option>' +
    //        '<option>Hide</option>' +
    //        //'<option>Hide (don"t toggle)</option > ' +
    //        '<option>Hide and clear values</option>' +
    //        '<option>Enable</option>' +
    //        //'<option>Enable (don"t toggle)</option>' +
    //        '<option>Disable</option>' +
    //        //'<option>Disable (don"t toggle)</option>' +
    //        '<option>Show Message Box</option>' +
    //        '<option>Show Form</option>' +
    //        '<option>Hide Form </option>' +
    //        //'<option>Set Document Template</option>' +
    //        //'<option>Append Document Template</option>' +
    //        '</select ></div >' +
    //        '<div class="rule-operator-container">' +
    //        '<select class="form-control " id="action_builder_rule_apply_to_' + elem_id + '">' +
    //        '<option value = "-1"> Apply to</option>' +
    //        '<option>Current Repeat</option>' +
    //        '<option>Entire Form</option>' +
    //        '</select ></div >' +
    //        '<div class="rule-filter-container"><select class="form-control" id="action_builder_rule_filter_' + elem_id + '">' +
    //        '<option value="-1">Select field</option>' +

    //        fieldoptions + 

    //        '</select></div>' +
    //        '<div class="rule-value-container"><input class="form-control" type="text" id="action_builder_rule_value_' + elem_id + '"></div>' +
    //        '</div>' +
    //        '</div>';

    //    $('#builder_' + elem_id + ' .rules-group-container').append(html)
    //}
    
    ///**
    // *JQuery query builder for conditional section
    // *
    // * @param {string} elem_id
    // */
    //function queryRulesBuilder(elem_id) {

    //    // build plugin in filter
    //    jQuery('#builder_' + elem_id).queryBuilder({
    //        plugins: ['bt-tooltip-errors'],

    //        filters: filterx.filters//,

    //        //rules: rules_basic,
    //    });
    //}

    ///**
    // *Create an action set json object
    // *
    // * @param {string} parent_elem_id
    // * @returns {object}
    // */
    //function getActionSetJson(parent_elem_id) {
    //    var action_type = $('#action_builder_rule_operator_' + parent_elem_id).val();
    //    var apply_to = $('#action_builder_rule_apply_to_' + parent_elem_id).val();
    //    var field = $('#action_builder_rule_filter_' + parent_elem_id).val();
    //    var action_value = $('#action_builder_rule_value_' + parent_elem_id).val();
    //    var action_object = {
    //        'action_type': action_type,
    //        'apply_to': apply_to,
    //        'field': field,
    //        'action_value': action_value
    //    }
    //    return action_object;
    //}


    ///**
    // *Recursive function to create logic app object tree including all the nodes
    // *
    // * @param {object} element
    // * @returns {Array}
    // */
    //function dfs(element) {
    //    var set = [];
    //    element.children('.logics').each(function () {
    //        var sectionId = $(this).attr('id');
    //        var commonID = sectionId.split('_')[1];
    //        var child_yes = $('#sub-yes_' + commonID);
    //        var child_no = $('#sub-no_' + commonID);
    //        var builderElement = $('#builder_' + commonID);
    //        var item = {};
    //        item.description = $('#input-description_' + commonID).val();
    //        item.section_id = commonID;
    //        item.section_type = ($(this).hasClass('sub-sections')) ? 'sub' : 'main';
    //        if (builderElement.length) {
    //            if (builderElement.hasClass('condition')) {
    //                item.type = 'condition';
    //                item.condition = $(builderElement).queryBuilder('getRules');
    //                item.if_yes = dfs($(child_yes));
    //                item.if_no = dfs($(child_no));
    //            } else {
    //                item.type = 'action';
    //                item.parent_elem_id = $(this).data('parent-id')
    //                item.action_condtion = $(this).parent().hasClass('sub-yes') ? 'yes' : 'no';
    //                item.action = getActionSetJson(commonID)
    //            }
    //        }
    //        set.push(item);
    //    });
    //    return set;

    //}


    ///**
    // *Show or hide the action fields
    // *
    // * @param {object} e
    // * @param {string} [type='create']
    // */
    //function showHideActionValues(e, type = 'create') {
    //    var commonID = $(e).attr('id').split('_')[4];
    //    var value = type == 'edit' ? e.val() : e.value;
    //    var ctrl = $(e).closest('.rule-container').find('.rule-value-container');
    //    var fieldselect = $('#action_builder_rule_filter_' + commonID);
    //    var applyto = $('#action_builder_rule_apply_to_' + commonID);

    //    if (value == 'Set Value' || value == 'Show Message Box' || value == 'Set Document Template' || value == 'Append Document Template') {
    //        $('#action_builder_rule_value_' + commonID).prop('disabled', false);
    //    }
    //    else {

    //        if (value == "Show Form" || value == "Hide Form") {
    //            $(applyto).first().prop('disabled', true);
    //        } else {
    //            $(applyto).first().prop('disabled', false);
    //        }

    //        $('#action_builder_rule_value_' + commonID).val('');
    //        $('#action_builder_rule_value_' + commonID).prop('disabled', true);
    //    }

    //    if (value == 'Show Message Box') {
    //        $(fieldselect).first().prop('disabled', true);
    //        $(fieldselect).first().prop('disabled', true);
    //        $(applyto).first().prop('disabled', true);
    //        $(applyto).first().prop('disabled', true);

    //    }
    //    else {
    //        $(fieldselect).first().prop('disabled', false);
    //        $(fieldselect).first().prop('disabled', false);

    //        if (value == "Show Form" || value == "Hide Form") {
    //            $(applyto).first().prop('disabled', true);
    //            $(applyto).first().prop('disabled', true);

    //        } else {
    //            $(applyto).first().prop('disabled', false);
    //            $(applyto).first().prop('disabled', false);
    //        }

    //    }

    //    if (value == 'Set Document Template' || value == 'Append Document Template') {
    //        $(applyto).first().prop('disabled', true)
    //        $(fieldselect).first().prop('disabled', true)
    //        $(ctrl).first().find('input').prop('disabled', false)
    //    }

    //}

    /****************************************************************
                          Logic Application loading functions
  *****************************************************************/
    ///**
    // * Remove gruop rule from the condtions
    // *
    // * @param {object} queryCondition
    // * @returns {object}
    // */
    //function getItemWithNoGroupRule(queryCondition) {
    //    var flags = {
    //        no_add_group: true
    //    }
    //    queryCondition.flags = flags
    //    return queryCondition;
    //}
    ///**
    // * Jquery query builder on load
    // *
    // * @param {*} item
    // */
    //function queryBuilderOnLoad(item) {
    //    var itemWithNoGroupRule = getItemWithNoGroupRule(item.condition)
    //    jQuery('#builder_' + item.section_id).queryBuilder({
    //        plugins: ['bt-tooltip-errors'],

    //        filters: filterx.filters,

    //        rules: item.condition,
    //    });
    //}

    ///**
    // * Create an action section on load
    // *
    // * @param {string} elem_id
    // * @param {object} rule
    // */
    //function queryActionBuilderOnLoad(elem_id, rule) {

    //    var html = '<div class="rules-group-body action-container" id="action_container_' + elem_id + '">' +
    //        '<div class="rules-list">' +
    //        '<div id="action_builder_' + elem_id + '_rule" class="rule-container">' +
    //        '<div class="rule-header">' +
    //        '<div class="btn-group pull-right rule-actions" > <button type="button" class="btn btn-xs btn-danger remove-action" data-delete="rule" data-elem-id="' + elem_id + '">' +
    //        '<i class="fa fa-times"></i>' +
    //        '</button> </div >' +
    //        '</div >' +
    //        '<div class="rule-operator-container">' +
    //        '<select class="form-control action-ctrl" id="action_builder_rule_operator_' + elem_id + '">' +
    //        '<option value = "-1">Select action</option>' +
    //        '<option>Set Value</option>' +
    //        '<option>Show</option>' +
    //        //'<option>Show (don"t toggle)</option>' +
    //        //'<option>Show (and content)</option>' +
    //        '<option>Hide</option>' +
    //        //'<option>Hide (don"t toggle)</option > ' +
    //        //'<option>Hide and clear values</option>' +
    //        '<option>Enable</option>' +
    //        //'<option>Enable (don"t toggle)</option>' +
    //        '<option>Disable</option>' +
    //        //'<option>Disable (don"t toggle)</option>' +
    //        '<option>Show Message Box</option>' +
    //        //'<option>Show Form</option>' +
    //        //'<option>Hide Form </option>' +
    //        //'<option>Set Document Template</option>' +
    //        //'<option>Append Document Template</option>' +
    //        '</select ></div >' +
    //        '<div class="rule-operator-container">' +
    //        '<select class="form-control " id="action_builder_rule_apply_to_' + elem_id + '">' +
    //        '<option value = "-1"> Apply to</option>' +
    //        '<option>Current Repeat</option>' +
    //        '<option>Entire Form</option>' +
    //        '</select ></div >' +
    //        '<div class="rule-filter-container"><select class="form-control" id="action_builder_rule_filter_' + elem_id + '">' +
    //        '<option value="-1">Select field</option>' +
 
    //        fieldoptions +
    //        '</select></div>' +
    //        '<div class="rule-value-container"><input class="form-control" type="text" id="action_builder_rule_value_' + elem_id + '"></div>' +
    //        '</div>' +
    //        '</div>';
    //    //var oo = fieldoptions;

    //    $('#builder_' + elem_id + ' .rules-group-container').append(html)
    //    $('#action_builder_rule_operator_' + elem_id).val(rule.action_type)
    //    $('#action_builder_rule_filter_' + elem_id).val(rule.field)
    //    $('#action_builder_rule_value_' + elem_id).val(rule.action_value)
    //    $('#action_builder_rule_apply_to_' + elem_id).val(rule.apply_to)
    //    showHideActionValues($('#action_builder_rule_operator_' + elem_id), 'edit')
    //}

    ///**
    // *Build an ACTION section on object load
    // *
    // * @param {Object} item
    // */
    //function buildActionSectionsOnLoad(item) {
    //    var wrapper = item.action_condtion == 'yes' ? '#sub-yes_' + item.parent_elem_id : '#sub-no_' + item.parent_elem_id;
    //    createActionSectionLoad(item.section_id, 'sub', wrapper, item.parent_elem_id, item.description)
    //    queryActionBuilderOnLoad(item.section_id, item.action);
    //}

    ///**
    // *Build condition section on object load
    // *
    // * @param {object} item
    // * @param {string} wrapper
    // */
    //function buildConditionSectionsOnLoad(item, wrapper) {
    //    var html = '<div id="section_' + item.section_id + '" class="logics ';
    //    html += (item.section_type == 'main') ? 'sections"' : ' sub-sections"';
    //    html += ' ><div class="add-more-section" data-id="' + item.section_id + '"><button type="button" data-type="' + item.section_type + '" data-id="' + item.section_id + '" id="btn-add-more-section_' + item.section_id + '" class="btn btn-default btn-add-more-section"><span class="fa fa-plus-sign"></span></button><button type="button" data-type="' + item.section_type + '" data-id="' + item.section_id + '" id="btn-add-more-section_' + item.section_id + '" class="btn btn-default btn-add-more-section"><span class="fa fa-plus-sign"></span></button><h1 class="down-arrow">&#x2193</h1></div>';
    //    if (item.section_type != 'main') {
    //        html += '<input class="form-control" id="input-description_' + item.section_id + '" type="text" placeholder="Enter description">'
    //    }
    //    html += ' <div id = "builder_' + item.section_id + '" class= "condition builder"';
    //    html += '></div ><h1 class="dashed-down-arrow">&#x21e3</h1>' +
    //        '<div class="container conditions-section" id="conditions-section_' + item.section_id + '">' +
    //        '<div class="row-fluid">' +
    //        '<div class="yes-block" id="yes-block_' + item.section_id + '">' +
    //        '<p class="p-yes"><i class="glyphicon glyphicon-ok"></i> If condition is true</p>' +
    //        '<div class="row-fluid"> <div class="action_select form-inline"  id="action_select_yes_' + item.section_id + '"><select class="select_action form-control" id="btn-sub-yes-select-action_' + item.section_id + '"> <option value="action">Action</option>  <option value="condition">Condition</option></select>&nbsp;' +
    //        '<button class="btn btn-outline-primary btn-sub-condtion form-control" data-elem-id="' + item.section_id + '" data-condition = "yes" id="btn-sub-yes-condition_' + item.section_id + '">select</button>' +
    //        '</div>' +
    //        '<div class="col-md-5  add_more_actions_container hidden" id="add_more_actions_container_yes_' + item.section_id + '">' +
    //        '<button type="button" class="btn btn-xs btn-success btn-add-action" data-elem-id="' + item.section_id + '" data-condition = "yes" id="btn-add-action_yes_' + item.section_id + '" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
    //        '<button type="button" class="btn btn-xs btn-success btn-clear-action" data-elem-id="' + item.section_id + '" data-condition = "yes" id="btn-clear-action_yes_' + item.section_id + '" data-add="rule"><i class="fa fa-plus"></i> Clear All</button>' +
    //        '<div style="clear:both"></div>' +
    //        '</div></div>' +
    //        '<div class="row-fluid sub-yes sub-logic" id="sub-yes_' + item.section_id + '"></div>' +
    //        '</div>' +
    //        '<div class="no-block" id="no-block_' + item.section_id + '">' +
    //        '<p class="p-no"><i class="fa fa-times"></i> If condition is false</p>' +
    //        '<div class="row-fluid"> <div class="action_select form-inline"  id="action_select_no_' + item.section_id + '"><select class="select_action form-control" id="btn-sub-no-select-action_' + item.section_id + '">  <option value="action">Action</option> <option value="condition">Condition</option></select>&nbsp;<button class="btn btn-outline-primary btn-sub-condtion form-control" data-elem-id="' + item.section_id + '" data-condition = "no" id="btn-sub-no-condition_' + item.section_id + '">select</button>' +
    //        '</div>' +
    //        '<div class="col-md-5 add_more_actions_container hidden" id="add_more_actions_container_no_' + item.section_id + '">' +
    //        '<button type="button" class="btn btn-xs btn-success btn-add-action" data-elem-id="' + item.section_id + '"  data-condition = "no" id="btn-add-action_no_' + item.section_id + '" data-add="rule"><i class="fa fa-plus"></i> Add action</button>' +
    //        '<button type="button" class="btn btn-xs btn-success btn-clear-action"  data-elem-id="' + item.section_id + '" data-condition = "no" id="btn-clear-action_no_' + item.section_id + '"  data-add="rule">         <i class="fa fa-plus"></i> Clear All</button>' +
    //        '<div style="clear:both"></div>' +
    //        '</div></div>' +
    //        '<div class="row-fluid sub-no sub-logic" id="sub-no_' + item.section_id + '"></div>' +
    //        '</div>' +
    //        '</div>' +
    //        '</div>' +
    //        '</div><hr>';
    //    $(wrapper).append(html);
    //    $('#input-description_' + item.section_id).val(item.description);
    //    queryBuilderOnLoad(item);


    //    item.if_yes && createLogicTree(item.if_yes, '#sub-yes_' + item.section_id);
    //    item.if_no && createLogicTree(item.if_no, '#sub-no_' + item.section_id);
    //}


    ///**
    // * Build an action section wrapper on load
    // *
    // * @param {string} elem_id
    // * @param {string} section_type
    // * @param {object} selector
    // * @param {string} parent_elem_id
    // * @param {string} description
    // */
    //function createActionSectionLoad(elem_id, section_type, selector, parent_elem_id, description) {
    //    var html = '<div id="section_' + elem_id + '" data-parent-id="' + parent_elem_id + '" class="logics ';
    //    html += (section_type == 'main') ? 'sections"' : ' sub-sections"';
    //    html += ' >';
    //    if (section_type != 'main') {
    //        html += '<input  class="form-control form-control-description" id="input-description_' + elem_id + '" type="text" placeholder="Enter description">'
    //    }
    //    html += ' <div id = "builder_' + elem_id + '" class= "builder actions form-inline query-builder"';
    //    html += ' ><div class="rules-group-container"> </div></div></div>';

    //    $(selector).append(html);
    //    $('#input-description_' + elem_id).val(description);
    //}


    ///**
    // * Create Logic tree
    // *
    // * @param {object} json_object
    // * @param {string} wrapper
    // */
    //function createLogicTree(json_object, wrapper) {
    //    $.each(json_object, function (i, item) {
    //        item.type == 'condition' && buildConditionSectionsOnLoad(item, wrapper);
    //        item.type == 'action' && buildActionSectionsOnLoad(item);
    //    });
    //}

    /****************************************************************
                            Triggers and Changers
    *****************************************************************/

    $('#btn-get').on('click', function () {
        var result = $('#builder').queryBuilder('getRules');
        if (!$.isEmptyObject(result)) {
            alert(JSON.stringify(result, null, 2));
        }
        else {
            console.log("invalid object :");
        }
        console.log(result);
    });

    $('#btn-reset').on('click', function () {
        $('#builder').queryBuilder('reset');
    });

    $('#btn-set').on('click', function () {
        var result = $('#builder').queryBuilder('getRules');
        if (!$.isEmptyObject(result)) {
            rules_basic = result;
        }
    });

    //When rules changed :
    $('#builder').on('getRules.queryBuilder.filter', function (e) {
    });

    //$(document).on('click', '.btn-sub-condtion', function () {
    //    var parent_elem_id = $(this).data('elem-id');
    //    var is_condition = $(this).data('condition');
    //    var elem_id = Math.round(new Date().getTime() + (Math.random() * 100));
    //    var select_yes_no = (is_condition === 'yes') ? $('#btn-sub-yes-select-action_' + parent_elem_id).val() : $('#btn-sub-no-select-action_' + parent_elem_id).val();
    //    var action_type = (select_yes_no == 'condition') ? 'condition' : (select_yes_no == 'action') ? 'action' : '';
    //    var wrapper = (is_condition === 'yes') ? '#sub-yes_' + parent_elem_id : '#sub-no_' + parent_elem_id;


    //    if ($('#section_' + parent_elem_id).hasClass('sections')) {
    //        if (is_condition === 'yes') {
    //            $('#section_' + parent_elem_id + ' .yes-block').removeAttr('style');
    //        } else {
    //            $('#section_' + parent_elem_id + ' .no-block').removeAttr('style');
    //        }

    //    }
    //    if (action_type == 'condition') {
    //        createConditionalSection(elem_id, 'sub', wrapper, parent_elem_id)
    //        queryRulesBuilder(elem_id)
    //    } else if (action_type == 'action') {
    //        $(this).parent().addClass('hidden');
    //        $(this).closest('.row-fluid').find('.add_more_actions_container').removeClass('hidden');
    //        $(wrapper).html('');
    //        createActionSection(elem_id, 'sub', wrapper, parent_elem_id)
    //        queryActionBuilder(elem_id, 1);
    //    }

    //    $(".btn-danger").contents().filter(function () { return this.nodeType == 3; }).remove();
    //});

    //$('#btn-create-rules').click(function () {
    //    var elem_id = Math.round(new Date().getTime() + (Math.random() * 100));
    //    createConditionalSection(elem_id, 'main', '.querybuilder', null)
    //    queryRulesBuilder(elem_id);
    //    $(".btn-danger").contents().filter(function () { return this.nodeType == 3; }).remove();
    //});
    
    //function generateScript() { 
    //    var ruleSet = {};
    //    ruleSet['name'] = 'Rule_' + Math.round(new Date().getTime() + (Math.random() * 100));
    //    ruleSet['set'] = dfs($('.querybuilder'));
 
    //    var rules = JSON.stringify(   ruleSet  , undefined, 2);
    //    rules = '{ "rules" : { "rule" : [' + rules + ']}}';

    //    var result = RunRulesGenerateScript(rules);
    //    return result;
    //}


    //$('body').on("click", '#btn-save-rules', function () {
    //    var ruleSet = {};
    //    ruleSet['name'] = 'Rule_' + Math.round(new Date().getTime() + (Math.random() * 100));
    //    ruleSet['set'] = dfs($('.btn-container'));
    //    console.log(JSON.stringify(ruleSet, undefined, 2));

    //    saveRules();
    //});

    //var _formsService = abp.services.app.forms;

    //function saveRules() { 
    //    var ruleSet = {};
    //    ruleSet['name'] = 'Rule_' + Math.round(new Date().getTime() + (Math.random() * 100));
    //    ruleSet['set'] = dfs($('.btn-container'));

    //    var rules = JSON.stringify(ruleSet, undefined, 2);    
    //    var rulesscript = generateScript();

    //    _formsService.saveRules({
    //        Id: JSONFormObj.Id,
    //        Rules: rules,
    //        RulesScript: rulesscript
    //    }).done(function () {
    //        abp.notify.info(app.localize('SavedSuccessfully'));   
    //        abp.event.trigger('app.rulesEditorModalSaved');
    //    }).always(function () {
      
    //    });
    //}

    // Get Form Schema from app service
    //_formsService.getRules({
    //    id: JSONFormObj.Id
    //})
    //.done(function (result) {
    //    loadRules(result);
    //});


    //function loadRules(result) {

    //}

    //$(document).on('click', '.btn-add-action', function () {
    //    var parent_elem_id = $(this).data('elem-id');
    //    var is_condition = $(this).data('condition');
    //    elem_id = Math.round(new Date().getTime() + (Math.random() * 100));
    //    var wrapper = (is_condition === 'yes') ? '#sub-yes_' + parent_elem_id : '#sub-no_' + parent_elem_id;
    //    createActionSection(elem_id, 'sub', wrapper, parent_elem_id)
    //    queryActionBuilder(elem_id);
    //});

    //$(document).on('click', '.btn-clear-action', function () {
    //    swal({
    //        title: "Are you sure?",
    //        text: "Once deleted, you will not be able to recover this imaginary file!",
    //        icon: "warning",
    //        buttons: true,
    //        dangerMode: true,
    //    })
    //        .then((willDelete) => {
    //            if (willDelete) {
    //                var parent_elem_id = $(this).data('elem-id');
    //                var is_condition = $(this).data('condition');
    //                if (is_condition === 'yes') {
    //                    var wrapper = '#sub-yes_' + parent_elem_id;
    //                    var action_select = '#action_select_yes_' + parent_elem_id;
    //                } else {
    //                    wrapper = '#sub-no_' + parent_elem_id;
    //                    action_select = '#action_select_no_' + parent_elem_id;
    //                }
    //                $(wrapper).html('');
    //                $(action_select).removeClass('hidden');
    //                $(this).closest('.add_more_actions_container').addClass('hidden');
    //            }
    //        });

    //});

    //$(document).on('click', '.remove-action', function () {
    //    $(this).closest('.sub-sections').remove();
    //});

    //$(document).on('change', '.action-ctrl', function () {
    //    showHideActionValues(this)
    //});

    $(document).on('click', '.btn-add-more-section', function () {
        var elem_id = Math.round(new Date().getTime() + (Math.random() * 100));
        var section = '#section_' + $(this).data('id');
        var type = $(this).data('type')
        createConditionalSection(elem_id, type, section, null, 'more');
        queryRulesBuilder(elem_id);
        $(".btn-danger").contents().filter(function () { return this.nodeType === 3; }).remove();
    });

});
