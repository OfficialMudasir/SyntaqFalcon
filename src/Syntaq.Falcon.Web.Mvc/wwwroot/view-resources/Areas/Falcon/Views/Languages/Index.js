jQuery,
    (app.modals.EditLanguageTextModal = function () {
        var a,
            e = abp.services.app.language,
            t = null;
        (this.init = function (e) {
            (t = (a = e).getModal().find("form[name=EditLanguageTextForm]")).validate();
        }),
            (this.save = function () {
                t.valid() &&
                    (a.setBusy(!0),
                        e
                            .updateLanguageText(t.serializeFormToObject())
                            .done(function () {
                                abp.notify.info(app.localize("SavedSuccessfully")), a.close(), abp.event.trigger("app.editLanguageTextModalSaved");
                            })
                            .always(function () {
                                a.setBusy(!1);
                            }));
            });
    });
$(function () {
    var e = $("#TextsTable"),
        t = abp.services.app.language,
        a = new app.ModalManager({ viewUrl: abp.appPath + "Falcon/Languages/EditTextModal", scriptUrl: abp.appPath + "view-resources/Areas/Falcon/Views/Languages/_EditTextModal.js", modalClass: "EditLanguageTextModal" }),
        n = e.DataTable({
            paging: !0,
            serverSide: !0,
            processing: !0,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(1),td:nth-child(2),td:nth-child(3),td:nth-child(4)").on("click", function () {

                    var e = data;
                    a.open({ sourceName: $("#TextSourceSelectionCombobox").val(), baseLanguageName: $("#TextBaseLanguageSelectionCombobox").val(), languageName: $("#TextTargetLanguageSelectionCombobox").val(), key: e.key });

                });
            },
            listAction: {
                ajaxFunction: t.getLanguageTexts,
                inputFilter: function () {
                    return {
                        targetLanguageName: $("#TextTargetLanguageSelectionCombobox").val(),
                        sourceName: $("#TextSourceSelectionCombobox").val(),
                        baseLanguageName: $("#TextBaseLanguageSelectionCombobox").val(),
                        targetValueFilter: $("#TargetValueFilterSelectionCombobox").val(),
                        filterText: $("#TextFilter").val(),
                    };
                },
            },
            columnDefs: [
                {
                    className: "control responsive",
                    orderable: !1,
                    render: function () {
                        return "";
                    },
                    targets: 0,
                },
                {
                    targets: 2,
                    data: "key",
                    render: function (e) {
                        return '<span title="' + e + '">' + app.utils.string.truncate(e, 32) + "</span>";
                    },
                },
                {
                    targets: 3,
                    data: "baseValue",
                    render: function (e) {
                        return $("<span/>")
                            .attr("title", e || "")
                            .html(app.utils.string.truncate(e, 32) || "")[0].outerHTML;
                    },
                },
                {
                    targets: 4,
                    data: "targetValue",
                    render: function (e) {
                        return $("<span/>")
                            .attr("title", e || "")
                            .html(app.utils.string.truncate(e, 32) || "")[0].outerHTML;
                    },
                },
                {
                    targets: 1,
                    data: null,
                    orderable: !1,
                    defaultContent: "",
                    rowAction: {
                        element: $('<button class="btn btn-icon btn-bg-secondary btn-active-color-primary btn-sm" title="' + app.localize("Edit") + '"><i class="fa fa-edit"></i></button>').click(function () {
                            var e = $(this).data();
                            a.open({ sourceName: $("#TextSourceSelectionCombobox").val(), baseLanguageName: $("#TextBaseLanguageSelectionCombobox").val(), languageName: $("#TextTargetLanguageSelectionCombobox").val(), key: e.key });
                        }),
                    },
                },
            ],
        });
    $("#TextBaseLanguageSelectionCombobox,#TextTargetLanguageSelectionCombobox").select2({ theme: "bootstrap5", selectionCssClass: "form-select", language: abp.localization.currentCulture.name, width: "100%" }),
        $("#TextSourceSelectionCombobox,#TargetValueFilterSelectionCombobox").select2({ theme: "bootstrap5", selectionCssClass: "form-select", language: abp.localization.currentCulture.name, width: "100%" }),
        $("#RefreshTextsButton").click(function (e) {
            e.preventDefault(), o();
        }),
        $("#TextsFilterForm select").change(function () {
            o();
        }),
        $("#TextFilter").focus();
    var o = function () {
        n.ajax.reload();
    };
    abp.event.on("app.editLanguageTextModalSaved", function () {
        n.ajax.reloadPage();
    });
});
