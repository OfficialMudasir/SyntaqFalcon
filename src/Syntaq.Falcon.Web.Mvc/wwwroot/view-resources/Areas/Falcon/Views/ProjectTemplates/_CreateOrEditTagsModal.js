debugger;

(function () {    
    app.modals.CreateOrEditProjectTemplatesTagModal = function () {

        debugger;
 
        var _modalManager;
        var _projectsService = abp.services.app.projects;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            var modal = _modalManager.getModal();
            //modal.find('.date-picker').datetimepicker({
            //    locale: abp.localization.currentLanguage.name,
            //    format: 'L'
            //});
 
        };
 
        this.save = function () {

            _modalManager.setBusy(true);
            
            var tags = $('[name="TagForm"]').serializeArray();

            _projectsService.createOrEditProjectTemplateTags({
                id: $('[name=ProjectTemplateId]').val(),
                name: $('[name=ProjectTemplateName]').val(),
                stepsSchema: [],
                tags: tags
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
            }).always(function () {
                _modalManager.setBusy(false);
            });

        };
  
    }
})(jQuery);