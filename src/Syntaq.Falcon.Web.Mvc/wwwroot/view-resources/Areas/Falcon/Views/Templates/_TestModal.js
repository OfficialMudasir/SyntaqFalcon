﻿//(function ($) {
//    app.modals.TestModal = function () {
//        var _aclService = abp.services.app.aCLs;
//        var _userService = abp.services.app.user;
//        var _organizationUnitService = abp.services.app.organizationUnit;

//        var _modalManager;
//        var _$form = null;

//        this.init = async function (modalManager) {

//            _modalManager = modalManager;
//            _$modalDialog = _modalManager.getModal().find('.modal-dialog');
//            _$modalDialog.css("max-width", "43em");
//            _$form = _modalManager.getModal().find('form');
//            _$form.validate();

//            var _localSource = [];
            
//            var users = await _userService.getUsers({ filter: null, permission: null, role: null, onlyLockedUsers: false });

//            $(users.items).each(function () {
//                var user = { display: "" + this.userName + "", type: "User", id: this.id };
//                _localSource.push(user);
//            });

//            var teams = await _organizationUnitService.getOrganizationUnits();

//            $(teams.items).each(function () {
//                var team = { display: "" + this.displayName + "", type: "Team", id: this.id };
//                _localSource.push(team);
//            });

//            //var _localSource = [
//            //    { text: "admin", value: "User" },
//            //    { text: "Bobby", value: "User" },
//            //    { text: "BobBob", value: "User" },
//            //    { text: "DavyJones", value: "User" },
//            //    { text: "IHaveACL", value: "User" },
//            //    { text: "IHaveDefaultFolders", value: "User" },
//            //    { text: "IHaveDefaultFoldersToo", value: "User" },
//            //    { text: "IHaveRecordsFolder", value: "User" },
//            //    { text: "IHaveRecordsFolderToo", value: "User" },
//            //    { text: "JoeLeech", value: "User" },
//            //    { text: "Team Z", value: "Team" },
//            //    { text: "Team X", value: "Team" },
//            //    { text: "Team Y", value: "Team" },
//            //    { text: "Teams", value: "Team" }
//            //];

//            console.log(_localSource);

//            var assignees = new Bloodhound({
//                datumTokenizer: Bloodhound.tokenizers.obj.whitespace('display'),
//                //datumTokenizer: function (datum) {
//                //    var textTokens = Bloodhound.tokenizers.whitespace(datum.text);
//                //    var valueTokens = Bloodhound.tokenizers.whitespace(datum.value);
//                //    var idTokens = Bloodhound.tokenizers.whitespace(datum.id);
//                //    //return numberTokens.concat(idTokens).concat(frenchTokens);
//                //    return textTokens.concat(valueTokens).concat(idTokens);
//                //},
//                queryTokenizer: Bloodhound.tokenizers.whitespace,
//                local: _localSource
//            });
//            assignees.initialize();

//            //var cities = new Bloodhound({
//            //    datumTokenizer: Bloodhound.tokenizers.obj.whitespace('text'),
//            //    queryTokenizer: Bloodhound.tokenizers.whitespace,
//            //    prefetch: 'https://bootstrap-tagsinput.github.io/bootstrap-tagsinput/examples/assets/cities.json'
//            //});
//            //cities.initialize();

//            var elt = $('.tagsinput-typeahead');
//            elt.tagsinput({
//                itemValue: 'id',
//                itemText: 'display',
//                typeaheadjs: {
//                    name: 'assignees',
//                    displayKey: 'display',
//                    source: assignees.ttAdapter()
//                }
//            });

//           // elt.tagsinput('add', { "value": 1, "text": "Amsterdam", "continent": "Europe" });

//            //var assignables = new Bloodhound({
//            //    datumTokenizer: Bloodhound.tokenizers.obj.whitespace('text'),
//            //    queryTokenizer: Bloodhound.tokenizers.whitespace,
//            //    local: _localSource
//            //});
//            //assignables.initialize();

//            //var data = new Bloodhound({
//            //    datumTokenizer: Bloodhound.tokenizers.obj.whitespace('text'),
//            //    queryTokenizer: Bloodhound.tokenizers.whitespace,
//            //    local: _localSource
//            //});
//            //data.initialize();


//            //$('.tagsinput-typeahead').tagsinput({
//            //    //itemValue: 'value',
//            //    //itemText: 'text',
//            //    typeahead: {
//            //        source: _localSource.map(function (item) { return item.text }),
//            //        afterSelect: function () {
//            //            this.$element[0].value = '';
//            //        }
//            //    }
//            //    //typeaheadjs: {
//            //    //    name: 'assignables',
//            //    //    displayKey: 'text',
//            //    //    source: assignables.ttAdapter()
//            //    //}
//            //}); 




//        };

//        async function grantACL() {
//            var ACLForm = _$form.serializeFormToObject();
//            ACLForm.Assignees = $(".tagsinput-typeahead").tagsinput('items');

//            await _aclService.grantACL(ACLForm)
//        };

//        $('#GrantACL').click(async function () {
//            await grantACL();
//            _modalManager.close();
//        });
//    }
//})(jQuery);