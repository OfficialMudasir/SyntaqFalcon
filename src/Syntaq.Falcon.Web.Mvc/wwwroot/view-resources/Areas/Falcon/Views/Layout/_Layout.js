﻿(function ($) {
    $(function () {

        // TODO REFACTOR REMOVE FROM BASE CLASS
        var _userAcceptancesTypeService = abp.services.app.userAcceptanceTypes;

        var _userAcceptanceDocumentModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/UserAcceptances/UserAcceptanceDocumentModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/UserAcceptances/_UserAcceptanceDocumentModal.js',
            modalClass: 'UserAcceptanceDocumentModal'
        });

        $('p[id*="footerUserAcceptanceDocument"]').click(function () {
            _userAcceptancesTypeService.getUserAcceptanceTypeForView({
                id: $("input." + this.id).val()
            }).done(function (data) {
                _userAcceptanceDocumentModal.open({
                    id: data.userAcceptanceType.templateId
                });
            });
        });

        function scrollToCurrentMenuElement() {
            if (!$('#kt_aside_menu').length) {
                return;
            }

      var path = location.pathname;
      var menuItem = $("a[href='" + path + "']");
      if (menuItem && menuItem.length) {
        menuItem[0].scrollIntoView({ block: 'center' });
      }
    }

    scrollToCurrentMenuElement();
  });
})(jQuery);
