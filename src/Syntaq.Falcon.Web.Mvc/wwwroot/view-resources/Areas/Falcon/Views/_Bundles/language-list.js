(function($) {
    app.modals.CreateOrEditLanguageModal = function () {

        var _modalManager;
        var _languageService = abp.services.app.language;
        var _$languageInformationForm = null;

        this.init = function(modalManager) {
            _modalManager = modalManager;

            _modalManager.getModal()
                .find('#LanguageNameEdit')
                .selectpicker({
                    iconBase: "fa",
                    tickIcon: "fa fa-check"
                });

            _modalManager.getModal()
                .find('#LanguageIconEdit')
                .selectpicker({
                    iconBase: "famfamfam-flag",
                    tickIcon: "fa fa-check"
                });

            _$languageInformationForm = _modalManager.getModal().find('form[name=LanguageInformationsForm]');
        };

        this.save = function () {
            var language = _$languageInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            _languageService.createOrUpdateLanguage({
                language: language
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditLanguageModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})(jQuery);
(function () {
    $(function () {

        var _$languagesTable = $('#LanguagesTable');
        var _languageService = abp.services.app.language;
        var _defaultLanguageName = null;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.Languages.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.Languages.Edit'),
            changeTexts: abp.auth.hasPermission('Pages.Administration.Languages.ChangeTexts'),
            'delete': abp.auth.hasPermission('Pages.Administration.Languages.Delete')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Languages/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Languages/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditLanguageModal'
        });

        var dataTable = _$languagesTable.DataTable({
            paging: false,
            serverSide: false,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3),td:nth-child(4)").on("click", function () {
                     document.location.href = abp.appPath + "Falcon/Languages/Texts?languageName=" + data.name;
                });
            },
            listAction: {
                ajaxFunction: _languageService.getLanguages
            },
            columnDefs: [
                {
                    className: 'control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0
                },
                {
                    targets: 6,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        text:  app.localize('Actions'),
                        items: [{
                            text: app.localize('Edit'),
                            visible: function (data) {
                                return _permissions.edit && data.record.tenantId === abp.session.tenantId;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.id });
                            }
                        }, {
                            text: app.localize('ChangeTexts'),
                            visible: function () {
                                return _permissions.changeTexts;
                            },
                            action: function (data) {
                                document.location.href = abp.appPath + "Falcon/Languages/Texts?languageName=" + data.record.name;
                            }
                        }, {
                            text: app.localize('SetAsDefaultLanguage'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                setAsDefaultLanguage(data.record);
                            }
                        }, {
                            text: app.localize('Delete'),
                            visible: function (data) {
                                return _permissions.delete && data.record.tenantId === abp.session.tenantId;
                            },
                            action: function (data) {
                                deleteLanguage(data.record);
                            }
                        }]
                    }
                },
                {
                    targets: 1,
                    data: "displayName",
                    render: function (displayName, type, row, meta) {
                        var $span = $('<span/>')
                            .append($("<i/>").addClass(row.icon).css("margin-right", "5px"))
                            .append($("<span/>").attr("data-language-name", row.name).text(row.displayName));
                        
                        if (meta.settings.rawServerResponse.defaultLanguageName === row.name) {
                            $span.addClass("text-bold").append(" (" + app.localize("Default") + ")");
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: "name"
                },
                {
                    targets: 3,
                    data: "tenantId",
                    visible: abp.session.tenantId ? true : false, //this field is visible only for tenants
                    render: function (tenantId, type, row, meta) {
                        var $span = $('<span/>').addClass("label");

                        if (tenantId !== abp.session.tenantId) {
                            $span.addClass("kt-badge kt-badge--success kt-badge--inline").text(app.localize('Yes'));
                        } else {
                            $span.addClass("kt-badge kt-badge--dark kt-badge--inline").text(app.localize('No'));
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 4,
                    data: "isDisabled",
                    render: function (isDisabled) {
                        var isEnabled = !isDisabled;
                        var $span = $("<span/>").addClass("label");
                        if (isEnabled) {
                            $span.addClass("kt-badge kt-badge--success kt-badge--inline").text(app.localize('Yes'));
                        } else {
                            $span.addClass("kt-badge kt-badge--dark kt-badge--inline").text(app.localize('No'));
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 5,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    }
                }
            ]
        });


        function setAsDefaultLanguage(language) {
            _languageService.setDefaultLanguage({
                name: language.name
            }).done(function () {
                getLanguages();
                abp.notify.success(app.localize('SuccessfullySaved'));
            });
        };

        function deleteLanguage(language) {
            abp.message.confirm(
                app.localize('LanguageDeleteWarningMessage', language.displayName),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _languageService.deleteLanguage({
                            id: language.id
                        }).done(function () {
                            getLanguages();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        };

        $('#CreateNewLanguageButton').click(function () {
            _createOrEditModal.open();
        });

        function getLanguages() {
            dataTable.ajax.reload();
        }

        abp.event.on('app.createOrEditLanguageModalSaved', function () {
            getLanguages();
        });

    });
})();