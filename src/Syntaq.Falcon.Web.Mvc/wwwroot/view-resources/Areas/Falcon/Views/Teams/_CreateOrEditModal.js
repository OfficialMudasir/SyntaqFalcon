//(function ($) {
//    app.modals.CreateOrEditTeamModal = function () {

//        var _teamsService = abp.services.app.teams;

//        var _modalManager;
//        var _$teamInformationForm = null;

		

//        this.init = function (modalManager) {
//            _modalManager = modalManager;

//			var modal = _modalManager.getModal();
//            modal.find('.date-picker').datetimepicker({
//                locale: abp.localization.currentLanguage.name,
//                format: 'L'
//            });

//            _$teamInformationForm = _modalManager.getModal().find('form[name=TeamInformationsForm]');
//            _$teamInformationForm.validate();
//        };

		  

//        this.save = function () {
//            if (!_$teamInformationForm.valid()) {
//                return;
//            }

//            var team = _$teamInformationForm.serializeFormToObject();
			
//			 _modalManager.setBusy(true);
//			 _teamsService.createOrEdit(
//				team
//			 ).done(function () {
//               abp.notify.info(app.localize('SavedSuccessfully'));
//               _modalManager.close();
//               abp.event.trigger('app.createOrEditTeamModalSaved');
//			 }).always(function () {
//               _modalManager.setBusy(false);
//			});
//        };
//    };
//})(jQuery);