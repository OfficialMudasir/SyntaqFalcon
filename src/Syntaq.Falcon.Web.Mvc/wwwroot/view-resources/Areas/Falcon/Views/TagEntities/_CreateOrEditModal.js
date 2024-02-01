﻿(function ($) {
    app.modals.CreateOrEditTagEntityModal = function () {

        var _tagEntitiesService = abp.services.app.tagEntities;

        var _modalManager;
        var _$tagEntityInformationForm = null;

		        var _TagEntitytagValueLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/TagEntities/TagValueLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/TagEntities/_TagEntityTagValueLookupTableModal.js',
            modalClass: 'TagValueLookupTableModal'
        });
		
		

        this.init = function (modalManager) {
            _modalManager = modalManager;

			var modal = _modalManager.getModal();
            modal.find('.date-picker').datetimepicker({
                locale: abp.localization.currentLanguage.name,
                format: 'L'
            });

            _$tagEntityInformationForm = _modalManager.getModal().find('form[name=TagEntityInformationsForm]');
            _$tagEntityInformationForm.validate();
        };

		          $('#OpenTagValueLookupTableButton').click(function () {

            var tagEntity = _$tagEntityInformationForm.serializeFormToObject();

            _TagEntitytagValueLookupTableModal.open({ id: tagEntity.tagValueId, displayName: tagEntity.tagValueValue }, function (data) {
                _$tagEntityInformationForm.find('input[name=tagValueValue]').val(data.displayName); 
                _$tagEntityInformationForm.find('input[name=tagValueId]').val(data.id); 
            });
        });
		
		$('#ClearTagValueValueButton').click(function () {
                _$tagEntityInformationForm.find('input[name=tagValueValue]').val(''); 
                _$tagEntityInformationForm.find('input[name=tagValueId]').val(''); 
        });
		


        this.save = function () {
            if (!_$tagEntityInformationForm.valid()) {
                return;
            }
            if ($('#TagEntity_TagValueId').prop('required') && $('#TagEntity_TagValueId').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('TagValue')));
                return;
            }

            

            var tagEntity = _$tagEntityInformationForm.serializeFormToObject();
            
            
            
			
			 _modalManager.setBusy(true);
			 _tagEntitiesService.createOrEdit(
				tagEntity
			 ).done(function () {
               abp.notify.info(app.localize('SavedSuccessfully'));
               _modalManager.close();
               abp.event.trigger('app.createOrEditTagEntityModalSaved');
			 }).always(function () {
               _modalManager.setBusy(false);
			});
        };
        
        
    };
})(jQuery);