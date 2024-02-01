(function ($) {
    app.modals.ScriptEditorModal = function () {

        var _formsService = abp.services.app.forms;

        var _modalManager;
        var editor;
        var undoManager;

        this.init = async function (modalManager) {
            _modalManager = modalManager;
            
            var Script = await _formsService.getScript({ id: JSONObj.Id });

            editor = ace.edit("editor");
            editor.setTheme("ace/theme/chrome");
            editor.getSession().setMode("ace/mode/javascript");
            editor.focus();

            if (Script !== null && Script !== '') {
                editor.session.insert({row: 0, column: 0}, Script);
            }
        };

        $('#UndoBtn').click(function () {
            editor.undo();
        });

        $('#RedoBtn').click(function () {
            editor.redo();
        });

        $('.dropdown-item').click(function () {
            $('#SearchType').text($(this).text());
        });

        $('#SearchEditor').keyup(function (event) {
            if (event.keyCode === 13) {
                var searchNeedle = $("#SearchEditor")[0].value;
                var searchType = $("#SearchType").text();
                if (searchType === "Dropdown") { searchType = "Find" }
                editor.find(searchNeedle, {}, true);
            }
        });

        $('#PrevBtn').click(function () {
            editor.findPrevious();
        });

        $('#NextBtn').click(function () {
            editor.findNext();
        });

        this.save = function () {

            var Id = JSONObj.Id;

            //var Script = JSON.stringify($('.ace_content')[0].text);
            var Script = editor.getValue();

            _modalManager.setBusy(true);
            _formsService.saveScript({
                Id: Id, script: Script
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.scriptEditorModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };

})(jQuery);