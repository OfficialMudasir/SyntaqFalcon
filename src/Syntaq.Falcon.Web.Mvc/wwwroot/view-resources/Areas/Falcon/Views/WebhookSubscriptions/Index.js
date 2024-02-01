﻿(function () {
    $(function () {
        var _table = $('#SubscriptionTable');
        var _webhookSubscriptionService = abp.services.app.webhookSubscription;

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.WebhookSubscription.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.WebhookSubscription.Edit'),
            changeActivity: abp.auth.hasPermission('Pages.Administration.WebhookSubscription.ChangeActivity'),
            detail: abp.auth.hasPermission('Pages.Administration.WebhookSubscription.Detail'),
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/WebhookSubscription/CreateModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/WebhookSubscriptions/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditWebhookSubscriptionModal',
            cssClass: 'scrollable-modal',
        });

        var dataTable = _table.DataTable({
            paging: false,
            serverSide: false,
            processing: false,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td").on("click", function () {
                    goToDetail(data.id);
                });
            },
            listAction: {
                ajaxFunction: _webhookSubscriptionService.getAllSubscriptions,
            },
            columnDefs: [
                {
                    className: 'control responsive',
                    orderable: false,
                    render: function () {
                        return '';
                    },
                    targets: 0,
                },
                {
                    targets: 1,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    visible: _permissions.detail,
                    rowAction: {
                        element: $('<button/>')

                            .addClass('btn btn-icon btn-secondary btn-active-color-primary btn-sm')
                            .attr('title', app.localize('Detail'))
                            .append($('<i/>').addClass('fa fa-search'))
                            .click(function () {
                                goToDetail($(this).data().id);
                            }),
                    },
                },
                {
                    targets: 2,
                    data: 'webhookUri',
                },
                {
                    targets: 3,
                    data: 'webhooks',
                    render: function (webhooks) {
                        var result = '';
                        if (webhooks && webhooks.length > 0) {
                            for (var i = 0; i < webhooks.length; i++) {
                                if (i > 2) {
                                    result += '. . .';
                                    return result;
                                }
                                var webhook = webhooks[i];
                                result += webhook + '<br/>';
                            }
                        }
                        return result;
                    },
                },
                {
                    targets: 4,
                    data: 'isActive',
                    render: function (isActive) {
                        var $span = $('<span/>').addClass('label');
                        if (isActive) {
                            $span.addClass('badge badge-success').text(app.localize('Yes'));
                        } else {
                            $span.addClass('badge badge-dark').text(app.localize('No'));
                        }

                        return $span[0].outerHTML;
                    },
                },
            ],
        });

        function goToDetail(id) {
            if (id) {
                window.location = '/Falcon/WebhookSubscription/Detail/' + id;
            }
        }

        $('#CreateNewWebhookSubscription').click(function () {
            _createOrEditModal.open();
        });

        $('#GetSubscriptionsButton').click(function (e) {
            e.preventDefault();
            getWebhooks();
        });

        function getWebhooks() {
            dataTable.ajax.reload();
        }

        abp.event.on('app.createOrEditWebhookSubscriptionModalSaved', function () {
            getWebhooks();
        });
    });
})();
