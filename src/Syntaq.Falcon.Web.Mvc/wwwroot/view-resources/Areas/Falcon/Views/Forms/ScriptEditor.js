
(function () {
	$(function () {


		$('#UndoBtn').click(function () {
			editor.undo();
		});

		$('#RedoBtn').click(function () {
			editor.redo();
		});

		$('.dropdown-item').click(function () {
			$('#SearchType').text($(this).text());
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

		$('#SaveBtn').click(function (event, Id) {
			saveScript();
		});

		function saveScript() {
		 
			var Id = JSONFormObj.Id;
			var Script = editor.getValue();

			//_modalManager.setBusy(true);
			_formsService.saveScript({
				Id: Id, script: Script
			}).done(function () {
				abp.notify.info(app.localize('SavedSuccessfully'));
				//_modalManager.close();
				abp.event.trigger('app.scriptEditorModalSaved');
			}).always(function () {
				//_modalManager.setBusy(false);
			});

		}

		var _formsService = abp.services.app.forms;

		loadScript(JSONFormObj.Script);

		// Get Form Schema from app service
		//_formsService.getScript({
		//    id: JSONFormObj.Id
		//})
		//.done(function (result) {
		//    loadScript(result);
		//});

		function loadScript(result) {
			 
			var script = result;

			editor = ace.edit("editor");
			//editor.setTheme("ace/theme/chrome");
			editor.getSession().setMode("ace/mode/javascript");
			editor.focus();

			if (script !== null && script !== '') {
				editor.session.insert({ row: 0, column: 0 }, script);
			}
		}
 

	});
})();