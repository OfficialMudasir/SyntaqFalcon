(function ($) {
  app.modals.ProjectReleaseLookupTableModal = function () {
    var _modalManager;
 
      var _$projectReleasesTable = $('#ProjectReleasesTable');
      var _projectReleasesService = abp.services.app.projectReleases;

      var _createOrEditModal = new app.ModalManager({
          viewUrl: abp.appPath + 'Falcon/ProjectReleases/CreateOrEditModal',
          scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/ProjectReleases/_CreateOrEditModal.js',
          modalClass: 'CreateOrEditProjectReleaseModal',
      });

      $("#PublishProjectTemplateButton").click(function () {
          var Id = $('#Modal_Id').val();
          var projectId = $('#Modal_ProjectId').val(); 
          var projectTemplateId = $('#Modal_ProjectTemplateId').val(); 
          _createOrEditModal.open({ id: null, projectTemplateId: projectTemplateId, projectId: projectId });
      });

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

      var _permissions = {
          create: abp.auth.hasPermission('Pages.ProjectReleases.Create'),
          edit: abp.auth.hasPermission('Pages.ProjectReleases.Edit'),
          delete: abp.auth.hasPermission('Pages.ProjectReleases.Delete'),
      };

      abp.event.on('app.createOrEditProjectReleaseModalSaved', function () {
          getProjectReleases();
      });
 
      var dataTableReleases = _$projectReleasesTable.DataTable({
          paging: true,
          serverSide: true,
          processing: true,
          listAction: {
              ajaxFunction: _projectReleasesService.getAll,
              inputFilter: function () {
                  return {
                      filter: '',
                      nameFilter: '',
                      notesFilter: '',
                      projectIdFilter: $('#Modal_ProjectId').val(),
                      requiredFilter: '',
                      minVersionMajorFilter: null,
                      maxVersionMajorFilter: null,
                      minVersionMinorFilter: null,
                      maxVersionMinorFilter: null,
                      minVersionRevisionFilter: null,
                      maxVersionRevisionFilter: null,
                      releaseTypeFilter: null,
                      projectEnvironmentNameFilter: null,
                  };
              },
          },
          columnDefs: [
              //{
              //    className: 'control responsive',
              //    orderable: false,
              //    render: function () {
              //        return '';
              //    },
              //    targets: 0,
              //},
              {
                  //width: 80,
                  targets: 8,
                  data: null,
                  orderable: false,
                  autoWidth: false,
                  defaultContent: '',
                  rowAction: {
                      cssClass: 'btn btn-brand dropdown-toggle',
                      text:   app.localize('Actions') + ' <span class="caret"></span>',
                      items: [
                          //{
                          //    text: app.localize('View'),
                          //    iconStyle: 'far fa-eye mr-2',
                          //    action: function (data) {
                          //        _viewProjectReleaseModal.open({ id: data.record.projectRelease.id });
                          //    },
                          //},
                          {
                              text: app.localize('View'),
                              //iconStyle: 'far fa-edit mr-2',
                              visible: function () {
                                  return _permissions.edit;
                              },
                              action: function (data) {
                         
                                  var projectId = $('#ProjectTemplateId').val();
                                  _createOrEditModal.open({ id: data.record.projectRelease.id, projectId: projectId, isViewMode: true });

                              },
                          },
                          {
                              text: app.localize('Promote'),
                              //iconStyle: 'far fa-edit mr-2',
                              visible: function () {
                                  return _permissions.edit;
                              },
                              action: function (data) {

                                  var projectId = $('#ProjectTemplateId').val();
                                  _createOrEditModal.open({ id: data.record.projectRelease.id, projectId: projectId, releaseIdToClone: data.record.projectRelease.id, isViewMode: true });

                              },
                          },
                          {
                              text: app.localize('Delete'),
                              //iconStyle: 'far fa-trash-alt mr-2',
                              visible: function () {
                                  return _permissions.delete;
                              },
                              action: function (data) {
                                  deleteProjectRelease(data.record.projectRelease);
                              },
                          },
                      ],
                  },
              },
              {
                  targets: 0,
                  data: 'projectRelease.name',
                  name: 'name',
                  render: function (name) { 
                      return `<div style="max-width:150px">${name}</div>`;
                  },
              },
              {
                  //STQ Modified
                  targets: 1,
                  data: 'projectRelease.creationTime',
                  name: 'CreationTime',
                  render: function (data, type, row) {
                      var dt = new Date(row.projectRelease.creationTime);
                      var dtoptions = { weekday: 'short', year: 'numeric', month: 'long', day: 'numeric' };
                      var creationTime = dt.toLocaleDateString('en-GB', dtoptions);
                      return creationTime;
                  },

                  //render: function (data, type, row) {
                  //    var dt = new Date(row.projectRelease.creationTime);
                  //    var dtoptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
                  //    var tmoptions = { hour: 'numeric', minute: 'numeric' };
                  //    var creationTime = dt.toLocaleTimeString('en-US', tmoptions) + ', ' + dt.toLocaleDateString('en-GB', dtoptions);
                  //    return creationTime;
                  //},
              },
              //{
              //    targets: 3,
              //    data: 'projectRelease.notes',
              //    name: 'notes',
              //},
              {
                  targets: 2,
                  data: 'projectRelease.required',
                  name: 'required',
                  render: function (required) {
                      if (required) {
                          return '<div class="text-center"><i class="fa fa-check text-success" title="True"></i></div>';
                      }
                      return '<div class="text-center"><i class="fa fa-times-circle" title="False"></i></div>';
                  },
              },
              {
                  targets: 3,
                  data: 'projectRelease.versionMajor',
                  name: 'versionMajor',
              },
              {
                  targets: 4,
                  data: 'projectRelease.versionMinor',
                  name: 'versionMinor',
              },
              {
                  targets: 5,
                  data: 'projectRelease.versionRevision',
                  name: 'versionRevision',
              },
              {
                  targets: 6,
                  data: 'projectRelease.releaseType',
                  name: 'releaseType',
                  render: function (releaseType) {
                      return app.localize('Enum_ProjectReleaseType_' + releaseType);
                  },
              },
              {
                  targets: 7,
                  data: 'projectEnvironmentName',
                  name: 'projectEnvironmentFk.name',
              },
          ],
      });

    $('#ProjectReleaseTable tbody').on('click', '[id*=selectbtn]', function () {
      var data = dataTable.row($(this).parents('tr')).data();
      _modalManager.setResult(data);
      _modalManager.close();
    });

    function getProjectReleases() {
        dataTableReleases.ajax.reload();
    }

    $('#GetProjectReleaseButton').click(function (e) {
      e.preventDefault();
      getProjectReleases();
    });

    $('#SelectButton').click(function (e) {
      e.preventDefault();
    });

    $(document).keypress(function (e) {
      if (e.which === 13 && e.target.tagName.toLocaleLowerCase() != 'textarea') {
        getProjectReleases();
      }
    });

      abp.event.on('app.createOrEditProjectReleaseModalSaved', function (userNotification) {
          getProjectReleases();
      });

      function deleteProjectRelease(projectRelease) {
   
        abp.message.confirm('', app.localize('AreYouSure'), function (isConfirmed) {
            if (isConfirmed) {
                _projectReleasesService
                    .delete({
                        id: projectRelease.id,
                    })
                    .done(function () {
                        getProjectReleases(true);
                        abp.notify.success(app.localize('SuccessfullyDeleted'));
                    });
            }
        });
    }

  };
})(jQuery);
