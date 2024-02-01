/**
 * version 0.0.2: Refactor the code framework; 
 *                provides call RunRulesGenerateScript(rulesstr);
 * 
 * Version 0.0.3: Fixed the "generator()" processiong null action problem.
 *                Fixed the 'Current Repeat' judgement: 
 *                  ActionShow(),ActionShowAndContent(),ActionHide(),
 *                  ActionHideAndClearContent(),ActionEnable(),ActionDisable(),
 *                  ActionSetValue().
 * 
 * Version 0.0.4: Fixed the Input Json, from {name:**, set:[]} to {rules:{rule[{name:**, set[{}]]}}
 *                Added condition "is_empty".
 * 
 * Version 0.0.5: Modified the case to insensitivity. .toLowerCase()
 *                Determine if the value is a number => RunRulesGetConditionalEvaluator()
 * 
 * Version 0.1.0: * Reconstructed data structure and Output Templates
 *                  Added multiple "$(repeat).find('[name$="FIELD_6"],').on('change', function () {});"
 *                  Fixed Triggers conflict.(multiple rules conflict) 
 *
 * Version 0.1.1: * Reconstructing to use FormIO form data and native methods
 *   */

var rulesjson;
var ConditionAndAction = new Array();
var triggerselectors = '';
var triggerselectorsOneSet = '';
var TotalConditionAndActionWithTrigger = new Array();
var subConditionAndAction = new Array();
var setSchema = new Array();

function RunRulesGenerateScript(rulesstr) {

    ConditionAndAction = new Array();
    triggerselectors = '';
    triggerselectorsOneSet = '';
    TotalConditionAndActionWithTrigger = new Array();
    subConditionAndAction = new Array();
    setSchema = new Array();

    if (typeof rulesstr !== "undefined" && rulesstr !== null && rulesstr !== "undefined") {
        rulesjson = JSON.parse(rulesstr); //Takes a well-formed JSON string and returns the resulting JavaScript value.
    }

    var rulesscript = '';
    if (rulesjson) {
        if (rulesjson.rules) {
            var rule = rulesjson.rules.rule;
            for (var i = 0; i < rule.length; i++) {

                for (var n = 0; n < rule[i].set.length; n++) {

                    set = rule[i].set[n];
                    var setContision = InitialCondition(set);
                    //console.log(setContision);
                    generator(set.if_yes, set.if_no, setContision);

                    var combineConditionAndAction = {};
                    combineConditionAndAction.set = set;
                    combineConditionAndAction.triggers = triggerselectorsOneSet;
                    combineConditionAndAction.ConditionAndAction = subConditionAndAction;
                    TotalConditionAndActionWithTrigger.push(combineConditionAndAction);
                    subConditionAndAction = [];
                    triggerselectorsOneSet = '';

                }
            }
        }
    }

    var frmnamescript = ""; // $("#FormName").val().trim().replace(/ /g, '').replace(/[.,\/#!$%\^&\*;:{}=\-_`~()]/g, "");

    // Only generate 4th part of script now using formio logic blocks
    //rulesscript += FirstPartOfTemplate(frmnamescript, triggerselectors) + SecondPartOfTemplate(TotalConditionAndActionWithTrigger) + ThirdPartOfTemplate();  

    //rulesscript += SecondPartOfTemplate(TotalConditionAndActionWithTrigger);
    SecondPartOfTemplate(TotalConditionAndActionWithTrigger);
    rulesscript += ThirdPartOfTemplate();
    rulesscript += FourthPartOfTemplate();

    return rulesscript;

}

//1------------------
function InitialCondition(set) {
    if (set.condition === null) {
        set.condition = { condition: "AND", rules: [] };
    }
    var condition = set.condition.condition;
    var rules = [];
    rules = set.condition.rules;
    setConditionRules = CombineSetRules(condition, rules, set);
    return setConditionRules;
}
//2------------generate ConditionAndAction[]
function generator(if_y, if_n, setC) {
    var condition;
    var rules;
    var setCadd;
    var actionRule;
    if (if_y.length === 0) { // do nothings, and aviod nullpoint error.

    } else if (if_y[0].hasOwnProperty("condition")) {
        condition = if_y[0].condition.condition;
        rules = [];
        rules = if_y[0].condition.rules;
        setCadd = CombineSubRules(condition, rules, setC);
        if_y_next = if_y[0].if_yes;
        if_n_next = if_y[0].if_no;
        generator(if_y_next, if_n_next, setCadd);
    } else {
        actionRule = {};
        actionRule.condition = setC;
        //     actionRule.action =if_y.action.field +" "+ if_y.action.action_type+ " "+ if_y.action.action_value;
        actionRule.actions = CombineAction(if_y);
        ConditionAndAction.push(actionRule);
        subConditionAndAction.push(actionRule); //
    }
    if (if_n.length === 0) { // do nothings

    } else if (if_n[0].hasOwnProperty("condition")) {
        condition = if_n[0].condition.condition;
        rules = [];
        rules = if_n[0].condition.rules;
        setC = InsertNegation(setC);
        setCadd = CombineSubRules(condition, rules, setC);
        if_y_next = if_n[0].if_yes;
        if_n_next = if_n[0].if_no;
        generator(if_y_next, if_n_next, setCadd);
    } else { // no condition, must be action,OR could also change to ' else if(if_n[0].hasOwnProperty("action"))'
        actionRule = {};
        // actionRule.condition = "" + setC;
        actionRule.condition = InsertNegation(setC);
        //    actionRule.action =if_n.action.field +" "+ if_n.action.action_type+ " "+ if_n.action.action_value;
        actionRule.actions = CombineAction(if_n);
        ConditionAndAction.push(actionRule);
        subConditionAndAction.push(actionRule); //
    }

}
//3----------
//-------------combine rules togeter
function CombineSetRules(con, rul, set) {

    con = con === 'AND' ? '&&' : '||';
    var comrules = "";
    for (var i = 0; i < rul.length; i++) {

        //var triggerName = ' data.' + rul[i].field.trim();
        var triggerName = ' getDataValue(instance, \'' + rul[i].field.trim() + '\')';

        //var triggerRowName = ' row.' + rul[i].field.trim();
        var operation = RunRulesGetConditionalEvaluator(rul[i].operator, rul[i].value);

        comrules += triggerName + operation + " " + con + " ";

        if (triggerselectors.indexOf(triggerName) === -1) {
            triggerselectors += triggerName;

            if (i < rul.length - 1) {
                triggerselectors += ",";
            }

        }
        if (triggerselectorsOneSet.indexOf(triggerName) === -1) {
            triggerselectorsOneSet += triggerName;

            if (i < rul.length - 1) {
                triggerselectorsOneSet += ",";
            }
        }

    }

    //Add parent type
    var parent = 'var data = event.changed.instance.data;\n';
    parent += 'var parent = event.changed.instance.parent;\n\n';

    var result = " if((" + comrules.substr(0, comrules.length - 4) + ")){";
    return result;

}

function CombineSubRules(con, rul, comrules) {
    con = con === 'AND' ? '&&' : '||';
    comrules = "";

    for (var i = 0; i < rul.length; i++) {

        //var triggerName = '.formio-component-' + rul[i].field + ' input, .formio-component-' + rul[i].field + ' select   ' ; 
        //var triggerName = ' data.' + rul[i].field.trim();
        var triggerName = ' getDataValue(instance, \'' + rul[i].field.trim() + '\')';

        if (i < rul.lenth - 1) triggerName += ", ";

        var operation = RunRulesGetConditionalEvaluator(rul[i].operator, rul[i].value);

        //comrules += "$('" + triggerName + "', parent).getval()" + operation + " " + con + " ";
        comrules += triggerName + operation + " " + con + " ";

        if (triggerselectors.indexOf(triggerName) === -1) {
            triggerselectors += triggerName;
            if (i < rul.length - 1) {
                triggerselectors += ",";
            }

        }
        if (triggerselectorsOneSet.indexOf(triggerName) === -1) {
            triggerselectorsOneSet += triggerName;
            if (i < rul.length - 1) {
                triggerselectorsOneSet += ",";
            }

        }

        //Add parent type
        var parent = 'var data = event.changed.instance.data;\n';
        parent += 'var parent = event.changed.instance.parent;\n\n';
    }

    var result = " if((" + comrules.substr(0, comrules.length - 4) + ")){";
    return result;

}
//--negation
function InsertNegation(str) {
    str += "";
    var lastIfIndex = str.lastIndexOf("if((");
    var temp1 = str.substring(0, lastIfIndex + 3); //======
    var temp2 = str.substring(lastIfIndex + 3);
    // console.log(temp1 + "!" + temp2);
    return temp1 + "!" + temp2;
}
//4----------Refere from RunRulesGetConditionalEvaluator
// operators: 'changes value', 'equal', 'not_equal', 'begins_with'
//              'contains', 'not_contains', 'ends_with', 
//           'greater than','greater than or equal to',
//          'less than','less than or equal to'
function RunRulesGetConditionalEvaluator(evaluator, value) {


    if (isDate(value)) {
        value = ' new Date(\'' + value + '\') ';
    }
    else if (value === null || value === undefined) {
        value = '';
    }
    else if (isNaN(value)) {
        value = '\'' + value.toLowerCase() + '\'';
    }

    switch (evaluator) {
        case 'changes value':
            return ' ';
        case 'equal':
            return ' == ' + value + '';
        case 'not_equal':
            return ' != ' + value + '';
        case 'begins_with':
            return '.startsWith(' + value + ')';
        case 'contains':
            return ".indexOf(" + value + ") > -1";
        case 'not_contains':
            return ".indexOf(" + value + ") == -1";
        case 'ends_with':
            return ".endsWith(" + value + ")";
        case 'greater':
            return ' > ' + value + '';
        case 'greater_or_equal':
            return ' >= ' + value + '';
        case 'less':
            return ' < ' + value + '';
        case 'less_or_equal':
            return ' <= ' + value + '';
        case 'is_empty':
            return ' == \'\'';
        case 'is_not_empty':
            return ' != \'\'';
        case 'is_null':
            return ' == \'\' ';
    }
}

function isDate(dateVal) {


    if (!isNaN(dateVal)) {
        return false;
    }

    var d = new Date(dateVal);
    return d.toString() === 'Invalid Date' ? false : true;

    //var d = new Date(dateVal);
    //return d.toString() === 'Invalid Date' ? false : true;
}

// action={field: "FIELD_5", action_type: "Set Value", action_value: "YYY", apply_to: "Current Repeat"}
//All these action don't toggle
function RunRulesGetAction(condition, action, set) {
    var actionScript = "";

    switch (action.action_type) {

        case "Show":
            actionScript = ActionShow(condition, action, set);
            break;
        case "Show (and content)":
            actionScript = ActionShowAndContent(action, set);
            break;
        case "Hide":
            actionScript = ActionHide(condition, action, set);
            break;
        case "Hide and clear values":
            actionScript = ActionHideAndClearContent(action, set);
            break;
        case "Enable":
            actionScript = ActionEnable(condition, action, set);
            break;
        case "Disable":
            actionScript = ActionDisable(condition, action, set);
            break;
        case "Set Value":
            actionScript = ActionSetValue(condition, action, set);
            break;
        case "Show Message Box":

            // Need the field name here
            actionScript = ActionShowMessageBox(condition, action, set);
            break;
        // case ("Show Form"):

        //     actions += toggle ? RunRulesGetActionHideForm(action) : RunRulesGetActionShowForm(action);
        //     break;
        // case ("Hide Form"):

        //     actions += toggle ? RunRulesGetActionShowForm(action) : RunRulesGetActionHideForm(action);
        //     break;
        case "Set Document Template":
            actionScript = ActionSetDocumentTemplate(action);
            break;
        case "Append Document Template":
            actionScript = ActionAppendDocumentTemplate(action);
            break;
    }

    return actionScript;

}
//------------------------------------------------------
// generate action script functions

// Show
// Example  $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").show() : $("[id='FIELD_6']").show();
//          $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").removeClass('hidden') : $("[id='FIELD_6']").removeClass('hidden');
function ActionShow(condition, action, set) {

    var javascriptlogic = buildshowjavascriptlogic(condition, set);

    var script = 'formJSON = UpdateSchemaLogic ( formJSON, "' + action.field + '" , "Hidden", "' + javascriptlogic + '", false); ';
    setSchema.push(script);

    return '';

}


function buildmsgboxjavascriptlogic(condition, set) {
    var result = '';

    var cond = condition.condition;
    cond = cond + 'result = true;';
    cond = cond + 'abp.message.info(\'Hi Bob\', \'\');';
    cond = cond + '}';
    cond = cond + 'else {';
    cond = cond + 'result = false;';
    cond = cond + '}';

    result = cond;
    return result;
}

function buildenablejavascriptlogic(condition, set) {
    var result = '';

    var cond = condition.condition;
    cond = cond + 'result = true;';
    cond = cond + '}';
    cond = cond + 'else {';
    cond = cond + 'result = false;';
    cond = cond + '}';

    result = cond;
    return result;
}

function builddisabledjavascriptlogic(condition, set) {
    var result = '';

    var cond = condition.condition;
    cond = cond + 'result = true;';
    cond = cond + '}';
    cond = cond + 'else {';
    cond = cond + 'result = false;';
    cond = cond + '}';

    result = cond;
    return result;
}

function buildshowjavascriptlogic(condition, set) {
    var result = '';

    var cond = condition.condition;
    cond = cond + 'show = true;';
    cond = cond + '}';
    cond = cond + 'else {';
    cond = cond + 'show = false;';
    cond = cond + '}';

    result = cond;
    return result;
}

function buildhidejavascriptlogic(condition, set) {
    var result = '';

    var cond = condition.condition;
    cond = cond + 'show = false;';
    cond = cond + '}';
    cond = cond + 'else {';
    cond = cond + 'show = true;';
    cond = cond + '}';

    result = cond;
    return result;
}

function buildjavascriptlogic(condition, set) {
    var result = '';
    var cond = condition.condition;
    cond = cond + 'result = true;';
    cond = cond + '}';
    cond = cond + 'else {';
    cond = cond + 'result = false;';
    cond = cond + '}';

    result = cond;
    return result;
}

// Show (and content) [contains ActionShow]
// Example  $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_4']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_4']").find('.droppedField, .droppedGroup').show() : $("[id='FIELD_4']").find('.droppedField, .droppedGroup').show();
//          $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_4']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_4']").find('.droppedField, .droppedGroup').removeClass('hidden') : $("[id='FIELD_4']").find('.droppedField, .droppedGroup').removeClass('hidden');
function ActionShowAndContent(action) {
    var dropshow;
    var dropremoveHidden;
    var actionShow = ActionShow(action);
    //var traigN = "[id$='" + action.field + "']";
    var traigN = ".formio-component-" + action.field + " ";
    if (action.apply_to === 'Current Repeat') {
        //var dropshow = '$(this).closest(\'div.repeat[data-isrepeat="true"]\').find("' + traigN + '").length > 0 ? $(this).closest(\'div.repeat[data-isrepeat="true"]\').find("' + traigN + '").find(\'.droppedField, .droppedGroup\').show() : $("' + traigN + '").find(\'.droppedField, .droppedGroup\').show();';
        //var dropremoveHidden = '$(this).closest(\'div.repeat[data-isrepeat="true"]\').find("' + traigN + '").length > 0 ? $(this).closest(\'div.repeat[data-isrepeat="true"]\').find("' + traigN + '").find(\'.droppedField, .droppedGroup\').removeClass(\'hidden\') : $("' + traigN + '").find(\'.droppedField, .droppedGroup\').removeClass(\'hidden\');';
        dropshow = '$(this).closest(\'tr\').find("' + traigN + '").length > 0 ? $(this).closest(\'tr\').find("' + traigN + '").show() : $("' + traigN + '").find(\'.droppedField, .droppedGroup\').show();';
        dropremoveHidden = '$(this).closest(\'tr\').find("' + traigN + '").length > 0 ? $(this).closest(\'tr\').find("' + traigN + '").prop(\'hidden\', null) : $("' + traigN + '").find(\'.droppedField, .droppedGroup\').removeClass(\'hidden\');';
        return actionShow + "\n" + dropshow + "\n" + dropremoveHidden;
    }
    dropshow = '$("' + traigN + '").show();';
    dropremoveHidden = '$("' + traigN + '").removeProp(\'hidden\');';
    return actionShow + "\n" + dropshow + "\n" + dropremoveHidden;
}

// Hide
// Example  $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").hide() : $("[id='FIELD_6']").hide();
//          $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_6']").addClass('hidden') : $("[id='FIELD_6']").addClass('hidden');
function ActionHide(condition, action) {

    var javascriptlogic = buildhidejavascriptlogic(condition, set);
    var script = 'formJSON = UpdateSchemaLogic ( formJSON, "' + action.field + '" , "Hidden", "' + javascriptlogic + '", true); ';
    setSchema.push(script);

    return '';

}

// Hide and clear values
// Example 
function ActionHideAndClearContent(action) {

    var traigN = "[id$='" + action.field + "']";
    var actionctrl = "$(this).closest('div.repeat[data-isrepeat=\"true\"]').find(\"" + traigN + "\").length > 0 ? $(this).closest('div.repeat[data-isrepeat=\"true\"]').find(\"" + traigN + "\")";
    var actionHide = ActionHide(action);

    var findHide;
    var findAddClass;
    var typeText;
    var typeHidden;
    var typeRadio;
    var typeCheckbox;
    var select;
    var textarea;
    var input;
    var inputTypeRadio;
    var inputTypeCheckbox;
    var selectTrigger;
    var textareaTrigger;

    if (action.apply_to === 'Current Repeat') {
        findHide = actionctrl + ".find('.droppedField, .droppedGroup').hide() : $(\"" + traigN + "\").find('.droppedField, .droppedGroup').hide();\n";
        findAddClass = actionctrl + ".find('.droppedField, .droppedGroup').addClass('hidden') : $(\"" + traigN + "\").find('.droppedField, .droppedGroup').addClass('hidden');\n";
        typeText = actionctrl + ".find('input[type=text]').val('') : $(\"" + traigN + "\").find('input[type=text]').val('');\n";
        typeHidden = actionctrl + ".find('input[type=hidden]').val('') : $(\"" + traigN + "\").find('input[type=hidden]').val('');\n";
        typeRadio = actionctrl + ".find('input[type=radio]').prop('checked', '') : $(\"" + traigN + "\").find('input[type=radio]').prop('checked', '');\n";
        typeCheckbox = actionctrl + ".find('input[type=checkbox]').prop('checked', '') : $(\"" + traigN + "\").find('input[type=checkbox]').prop('checked', '');\n";
        select = actionctrl + ".find('select').val('') : $(\"" + traigN + "\").find('select').val('');\n";
        textarea = actionctrl + ".find('textarea').val('') : $(\"" + traigN + "\").find('textarea').val('');\n";
        input = actionctrl + ".find('input').trigger('onchange') : $(\"" + traigN + "\").find('input').trigger('onchange');\n";
        inputTypeRadio = actionctrl + ".find('input[type=radio]').trigger('onchange') : $(\"" + traigN + "\").find('input[type=radio]').trigger('onchange');\n";
        inputTypeCheckbox = actionctrl + ".find('input[type=checkbox]').trigger('onchange') : $(\"" + traigN + "\").find('input[type=checkbox]').trigger('onchange');\n";
        selectTrigger = actionctrl + ".find('select').trigger('onchange') : $(\"" + traigN + "\").find('select').trigger('onchange');\n";
        textareaTrigger = actionctrl + ".find('textarea').trigger('onchange') : $(\"" + traigN + "\").find('textarea').trigger('onchange');\n";

        return actionHide + findHide + findAddClass + typeText + typeHidden + typeRadio + typeCheckbox + select + textarea + input + inputTypeRadio + inputTypeCheckbox + selectTrigger + textareaTrigger;
    }
    findHide = "$(\"" + traigN + "\").find('.droppedField, .droppedGroup').hide();\n";
    findAddClass = "$(\"" + traigN + "\").find('.droppedField, .droppedGroup').addClass('hidden');\n";
    typeText = "$(\"" + traigN + "\").find('input[type=text]').val('');\n";
    typeHidden = "$(\"" + traigN + "\").find('input[type=hidden]').val('');\n";
    typeRadio = "$(\"" + traigN + "\").find('input[type=radio]').prop('checked', '');\n";
    typeCheckbox = "$(\"" + traigN + "\").find('input[type=checkbox]').prop('checked', '');\n";
    select = "$(\"" + traigN + "\").find('select').val('');\n";
    textarea = "$(\"" + traigN + "\").find('textarea').val('');\n";
    input = "$(\"" + traigN + "\").find('input').trigger('onchange');\n";
    inputTypeRadio = "$(\"" + traigN + "\").find('input[type=radio]').trigger('onchange');\n";
    inputTypeCheckbox = "$(\"" + traigN + "\").find('input[type=checkbox]').trigger('onchange');\n";
    selectTrigger = "$(\"" + traigN + "\").find('select').trigger('onchange');\n";
    textareaTrigger = "$(\"" + traigN + "\").find('textarea').trigger('onchange');\n";

    return actionHide + findHide + findAddClass + typeText + typeHidden + typeRadio + typeCheckbox + select + textarea + input + inputTypeRadio + inputTypeCheckbox + selectTrigger + textareaTrigger;

}

// Enable
// Example: $("input, select, textarea", $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']") : $("[id='FIELD_5']")).attr("disabled", false);
//          $("input, select, textarea", $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']") : $("[id='FIELD_5']")).prop('disabled', false);
function ActionEnable(condition, action, set) {

    var javascriptlogic = buildenablejavascriptlogic(condition, set);
    var script = 'formJSON = UpdateSchemaLogic (formJSON, "' + action.field + '" , "Disabled", "' + javascriptlogic + '", false); ';
    setSchema.push(script);

    return '';
}

// Disable
// Example  $("input, select, textarea", $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']") : $("[id='FIELD_5']")).attr("disabled", true);
//          $("input, select, textarea", $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']") : $("[id='FIELD_5']")).prop('disabled', true);
function ActionDisable(condition, action) {

    var javascriptlogic = builddisabledjavascriptlogic(condition, set);
    var script = 'formJSON = UpdateSchemaLogic (formJSON, "' + action.field + '" , "Disabled", "' + javascriptlogic + '", true); ';
    setSchema.push(script);

    return '';

}

// Set Value
// Example: $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']").length > 0 ? $(this).closest('div.repeat[data-isrepeat="true"]').find("[id$='FIELD_5']").setvalue('Yes,No,No') : $("[id='FIELD_5']").setvalue('Yes,No,No');
function ActionSetValue(condition, action, set) {

    var javascriptlogic = buildjavascriptlogic(condition, set);
    var script = 'formJSON = UpdateSchemaLogic (formJSON, "' + action.field + '" , "SetValue", "' + javascriptlogic + '", "' + action.action_value + '"); ';

    setSchema.push(script);

    return '';
}

//Show Message Box
// Example  $('#dialog-user-message').text('test123test123'); $('#dialog-user').modal('show');
function ActionShowMessageBox(condition, action, set) {


    MessageBox = 'if(';

    $(set.set.condition.rules).each(function (index) {
        MessageBox += 'instance.changed.component.key == \'' + this.field + '\' ';
        if (index < set.set.condition.rules.length - 1) MessageBox += ' || ';
    });

    MessageBox += ') {\n';

    MessageBox += 'abp.message.info(\'' + action.action_value + '\', \'\');';

    MessageBox += '}\n';
    //MessageBox += '}\n';

    return MessageBox;

    //var javascriptlogic = buildmsgboxjavascriptlogic(condition, set);
    //$(set.set.condition.rules).each(function (index) {
    //    var script = 'formJSON = UpdateSchemaLogic ( formJSON, "' + this.field + '" , "", "' + javascriptlogic + '", false); ';
    //    setSchema.push(script);
    //});

    return '';

}

//Set Document Template
//Example   $('#DocumentTemplateURL').val('url_url_url_url');
function ActionSetDocumentTemplate(action) {
    var SetDocumentTemplate = '$(\'#DocumentTemplateURL\').val(\'' + action.action_value + '\');\n';
    return SetDocumentTemplate;
}

// Append Document Template
// Example 
function ActionAppendDocumentTemplate(action) {
    var AppendDocumentTemplate = '';
    AppendDocumentTemplate += 'if ($(\'#DocumentTemplateURL\').val().indexOf(\'' + action.action_value + '\') < 0) {\n\ var docurl = $(\'#DocumentTemplateURL\').val(); \n $(\'#DocumentTemplateURL\').val( docurl + (docurl.length > 1? \',\': \'\') + \'' + action.action_value + '\' ); \n\}';
    return AppendDocumentTemplate;
}

//==========================================
//Todo  templates 
// First part    try {  ...   if (repeat.length == 0) repeat = $('#SelectedForm');
// parameters 1. var frmnamescript = $("#FormName").val().trim().replace(/ /g, '').replace(/[.,\/#!$%\^&\*;:{}=\-_`~()]/g, "");
// parameters 2. triggerselectors
console.log(FirstPartOfTemplate("Test", triggerselectors) + SecondPartOfTemplate(TotalConditionAndActionWithTrigger) + ThirdPartOfTemplate());

function FirstPartOfTemplate(frmnamescript = "Test", triggerselectors) {

    var Script = '';
    //var Script = "try { \n  " +
    //    "function " + frmnamescript + "_TriggerRulesScript(formio_form) {\n   " +
    //    " $(formio_form).find('" +
    //    triggerselectors + "').trigger('change'); \n }\n";

    //Script += 'function ' + frmnamescript + '_InitRulesScript(formio_form) { \n   ' +

    //    'formio_form = $(".formio-form"); \n     ' +

    //    '$(formio_form).each(function (index) { \n      ' +

    //    'var repeat = this;\n      ';

    /** Next begin IF sentences part */
    return Script;
}

// Second part is logic sentence
// Parameters 1. TotalConditionAndActionWithTrigger
function SecondPartOfTemplate(TotalConditionAndActionWithTrigger) {
    var Script = "";
    for (var t = 0; t < TotalConditionAndActionWithTrigger.length; t++) {

        var triggers = TotalConditionAndActionWithTrigger[t].triggers;
        var ConditionAndAction = TotalConditionAndActionWithTrigger[t].ConditionAndAction;

        for (var i = 0; i < ConditionAndAction.length; i++) {
            var ifScript = ConditionAndAction[i].condition + "\n    ";
            var ifcount = (ifScript.match(/\)\)\{/g) || []).length; // calculate the number of " { ",

            for (var j = 0; j < ConditionAndAction[i].actions.length; j++) {
                ifScript += RunRulesGetAction(ConditionAndAction[i], ConditionAndAction[i].actions[j], TotalConditionAndActionWithTrigger[t]) + "\n   ";
            }
            Script += ifScript + "}\n".repeat(ifcount);
        }

    }
    //Do not return script we are just building
    return Script;
}

// Third part is some brackets and "catch (err)", there is not parameter
function ThirdPartOfTemplate() {
    var Script = "\n           \n         })\n     }\n} catch (err) {\n }";
    Script = '';
    return Script;
}

// Fourth part of template builds the formio schema function calls
function FourthPartOfTemplate() {

    //var Script = "\n           \n         })\n     }\n} catch (err) {\n }";
    //Script = '';

    var result = '\n\n';
    // result += 'function UpdateSchema() {\n';
    for (i in setSchema) {
        result += "\n " + setSchema[i] + '\n';
    }

    // result += "};\n";

    return result;

}
//-----------------combine action----------------
function CombineAction(if_yes_no) {
    var actions = new Array();
    for (var i = 0; i < if_yes_no.length; i++) {
        var action = {};
        action.field = if_yes_no[i].action.field;
        action.action_type = if_yes_no[i].action.action_type;
        action.action_value = if_yes_no[i].action.action_value;
        action.apply_to = if_yes_no[i].action.apply_to;
        actions.push(action);
    }
    return actions;
}
//=================================