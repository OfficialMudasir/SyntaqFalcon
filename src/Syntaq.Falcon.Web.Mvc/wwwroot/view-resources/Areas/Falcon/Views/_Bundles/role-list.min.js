var PermissionsTree = (function ($) {
    return function() {
        var $tree;

        function initFiltering() {
            var to = false;
            $('#PermissionTreeFilter').keyup(function () {
                if (to) { clearTimeout(to); }
                to = setTimeout(function () {
                    var v = $('#PermissionTreeFilter').val();
                    if ($tree.jstree(true)) {
                        $tree.jstree(true).search(v);
                    }
                }, 250);
            });
        }

        function init($treeContainer) {
            $tree = $treeContainer;
            $tree.jstree({
                "types": {
                    "default": {
                        "icon": "fa fa-folder kt--font-warning"
                    },
                    "file": {
                        "icon": "fa fa-file kt--font-warning"
                    }
                },
                'checkbox': {
                    keep_selected_style: false,
                    three_state: false,
                    cascade: ''
                },
                'search': {
                    'show_only_matches': true
                },
                plugins: ['checkbox', 'types', 'search']
            });

            $tree.on("changed.jstree", function (e, data) {
                if (!data.node) {
                    return;
                }

                var childrenNodes;

                if (data.node.state.selected) {
                    selectNodeAndAllParents($tree.jstree('get_parent', data.node));

                    childrenNodes = $.makeArray($tree.jstree('get_node', data.node).children);
                    $tree.jstree('select_node', childrenNodes);

                } else {
                    childrenNodes = $.makeArray($tree.jstree('get_node', data.node).children);
                    $tree.jstree('deselect_node', childrenNodes);
                }
            });

            initFiltering();
        };

        function selectNodeAndAllParents(node) {
            $tree.jstree('select_node', node, true);
            var parent = $tree.jstree('get_parent', node);
            if (parent) {
                selectNodeAndAllParents(parent);
            }
        };

        function getSelectedPermissionNames() {
            var permissionNames = [];

            var selectedPermissions = $tree.jstree('get_selected', true);
            for (var i = 0; i < selectedPermissions.length; i++) {
                permissionNames.push(selectedPermissions[i].id);
            }

            return permissionNames;
        };

        return {
            init: init,
            getSelectedPermissionNames: getSelectedPermissionNames
        }
    }
})(jQuery);
(function () {
    app.modals.CreateOrEditRoleModal = function () {

        var _modalManager;
        var _roleService = abp.services.app.role;
        var _$roleInformationForm = null;
        var _permissionsTree;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _permissionsTree = new PermissionsTree();
            _permissionsTree.init(_modalManager.getModal().find('.permission-tree'));

            _$roleInformationForm = _modalManager.getModal().find('form[name=RoleInformationsForm]');
            _$roleInformationForm.validate({ ignore: "" });
        };

        this.save = function() {
            if (!_$roleInformationForm.valid()) {
                return;
            }

            var role = _$roleInformationForm.serializeFormToObject();

            _modalManager.setBusy(true);
            _roleService.createOrUpdateRole({
                role: role,
                grantedPermissionNames: _permissionsTree.getSelectedPermissionNames()
            }).done(function () {
                abp.notify.info(app.localize('SavedSuccessfully'));
                _modalManager.close();
                abp.event.trigger('app.createOrEditRoleModalSaved');
            }).always(function () {
                _modalManager.setBusy(false);
            });
        };
    };
})();
(function () {
    $(function () {

        var _$rolesTable = $('#RolesTable');
        var _roleService = abp.services.app.role;
        var _entityTypeFullName = 'Syntaq.Falcon.Authorization.Roles.Role';

        var _permissions = {
            create: abp.auth.hasPermission('Pages.Administration.Roles.Create'),
            edit: abp.auth.hasPermission('Pages.Administration.Roles.Edit'),
            'delete': abp.auth.hasPermission('Pages.Administration.Roles.Delete'),
            view: abp.auth.hasPermission('Pages.Administration.Roles')
        };

        var _createOrEditModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Roles/CreateOrEditModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Roles/_CreateOrEditModal.js',
            modalClass: 'CreateOrEditRoleModal',
            cssClass: 'scrollable-modal'
        });

        var _viewRoleModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/Roles/ViewRoleModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Roles/_ViewRoleModal.js',
            modalClass: 'ViewRoleModal'
        });

        var _upgradeModal = new app.ModalManager({
            viewUrl: abp.appPath + 'Falcon/SubscriptionManagement/UpgradeModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/SubscriptionManagement/_UpgradeModal.js',
            modalClass: 'UpgradeModal'
        });

        var _entityTypeHistoryModal = app.modals.EntityTypeHistoryModal.create();

        function entityHistoryIsEnabled() {
            return abp.custom.EntityHistory &&
                abp.custom.EntityHistory.IsEnabled &&
                _.filter(abp.custom.EntityHistory.EnabledEntities, function (entityType) {
                    return entityType === _entityTypeFullName;
                }).length === 1;
        }

        var dataTable = _$rolesTable.DataTable({
            paging: false,
            serverSide: false,
            createdRow: function (row, data, dataIndex) {
                $(row).find("td:nth-child(2),td:nth-child(3)").on("click", function () {
                    _createOrEditModal.open({ id: data.id });
                });
            },
            drawCallback: function (settings) {
                $('[data-toggle=m-tooltip]').tooltip();
            },
            listAction: {
                ajaxFunction: _roleService.getRoles,
                inputFilter: function () {
                    return {
                        permission: $('#PermissionSelectionCombo').val()
                    };
                }
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
                    targets: 3,
                    data: null,
                    orderable: false,
                    autoWidth: false,
                    defaultContent: '',
                    rowAction: {
                        text:  app.localize('Actions'),
                        items: [
                        //{
                        //    text: app.localize('View'),
                        //    visible: function () {
                        //        return _permissions.view;
                        //    },
                        //    action: function (data) {
                        //        _viewRoleModal.open(/*{ id: data.record.id }*/);
                        //    }
                        //},
                        {
                            text: app.localize('Edit'),
                            visible: function () {
                                return _permissions.edit;
                            },
                            action: function (data) {
                                _createOrEditModal.open({ id: data.record.id });
                            }
                        },
                        {
                            text: app.localize('Delete'),
                            visible: function (data) {
                                return !data.record.isStatic && _permissions.delete;
                            },
                            action: function (data) {
                                deleteRole(data.record);
                            }
                        },
                        {
                            text: app.localize('History'),
                            visible: function () {
                                return entityHistoryIsEnabled();
                            },
                            action: function (data) {
                                _entityTypeHistoryModal.open({
                                    entityTypeFullName: _entityTypeFullName,
                                    entityId: data.record.id,
                                    entityTypeDescription: data.record.displayName
                                });
                            }
                        }]
                    }
                },
                {
                    targets: 1,
                    data: "displayName",
                    render: function (displayName, type, row, meta) {
                        var $span = $('<span/>');
                        $span.append(displayName + " &nbsp;");

                        if (row.isStatic) {
                            $span.append(
                                $("<span/>")
                                    .addClass("badge badge-primary badge-inline")
                                    .attr("data-toggle", "kt-tooltip")
                                    .attr("title", app.localize('StaticRole_Tooltip'))
                                    .attr("data-placement", "top")
                                    .text(app.localize('Static'))
                                    .css("margin-right", "5px")
                            );
                        }

                        if (row.isDefault) {
                            $span.append(
                                $("<span/>")
                                    .addClass("badge badge-dark badge-inline")
                                    .attr("data-toggle", "kt-tooltip")
                                    .attr("title", app.localize('DefaultRole_Description'))
                                    .attr("data-placement", "top")
                                    .text(app.localize('Default'))
                                    .css("margin-right", "5px")
                            );
                        }

                        return $span[0].outerHTML;
                    }
                },
                {
                    targets: 2,
                    data: "creationTime",
                    render: function (creationTime) {
                        return moment(creationTime).format('L');
                    }
                }
            ]
        });

        function deleteRole(role) {
            abp.message.confirm(
                app.localize('RoleDeleteWarningMessage', role.displayName),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _roleService.deleteRole({
                            id: role.id
                        }).done(function () {
                            getRoles();
                            abp.notify.success(app.localize('SuccessfullyDeleted'));
                        });
                    }
                }
            );
        };

        $('#CreateNewRoleButton').click(function () {
            _createOrEditModal.open();
        });

        $('#UpgradeButton').click(function () {
            _upgradeModal.open({ featureName: "App.RoleManagement" });
        });

        $('#RefreshRolesButton').click(function (e) {
            e.preventDefault();
            getRoles();
        });

        function getRoles() {
            dataTable.ajax.reload();
        }

        abp.event.on('app.createOrEditRoleModalSaved', function () {
            getRoles();
        });

    });
})();