(function() {
  $(function () {
    var _formsService = abp.services.app.forms;

    var _createOrEditJobModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/AppJobs/CreateOrEditModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AppJobs/_CreateOrEditModal.js',
      modalClass: 'CreateOrEditAppJobModal'
    });

    var _importFormSchemaModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Forms/ImportFormSchemaModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_ImportFormSchemaModal.js',
      modalClass: 'ImportFormSchemaModal'
    });

    var _uploadFormRulesSchemaModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Forms/UploadFormRulesSchemaModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_UploadFormRulesSchemaModal.js',
      modalClass: 'UploadFormRulesSchemaViewModel'
    });

    var _manageACLModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/AccessControlList/ManageACLModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/AccessControlList/_ManageACLModal.js',
      modalClass: 'ManageACLModal'
    });

    var _formSettingsModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Forms/FormSettingsModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_FormSettingsModal.js',
      modalClass: 'FormSettingsModal'
    });

    var _createEmbedModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Embedding/CreateModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Embedding/_CreateModal.js',
      modalClass: 'CreateEmbedModal'
    });

    var _createVersionModal = new app.ModalManager({
      viewUrl: abp.appPath + 'Falcon/Forms/CreateVersionModal',
      scriptUrl: abp.appPath + 'view-resources/Areas/Falcon/Views/Forms/_CreateVersionModal.js',
      modalClass: 'CreateVersionModal'
    });

    // Get Form Schema from app service
    if (JSONObj.Schema != null) {
      form = JSONObj.Schema;
    } else {
      _formsService.getSchema({
        id: JSONObj.Id
      }).done(function (form) {
        if (form === '' || !form) {
          form = '{"type": "form", "display": "form" }';
        }
      });
    }

      var formJSON = JSON.parse(form);
      //Initiate Form Builder
      Formio.icons = 'fontawesome';
      var builder = new Formio.FormBuilder(document.getElementById("builder"), formJSON, {
        builder: {
          common: {
            components: {
              //textfield: true,
              sfatextfield: true,
              sfanbnz: true,
              //number: true,
              sfanumber: true,
              sfaradioyn: true,
              sfaselect: true,
              sfadatetime: true,
              //checkbox: true,
              sfacheckbox: true,
              checkboxesgroup: true,
              radiogroup: true,
              person: true,
              addressgroup: true
            }
          },
          sfalayout: {
            components: {
              sfapanel: true,
              section: true,
              //sfaeditgrid: true,
              //panel: true,
             // datagrid: true,
              fieldset: true,
              heading: true,
              label: true,
              //Placeholder
              divider: true,
              helpnotes: true,
              //Summary Table
              summarytable: true
            }
          },
          other: {
            components: {
              sfaemail: true,
              sfatextarea: true,
              slider: true,
              image: true,
              //location: true, //broken
              country: true,
              link: true,
              sfahtmlelement: true
              //htmlelement: true,
              //survey: true //broken
            }
          },
          sfadvanced: {
            components: {
              nestedform: true,
              popupform: true,
              youtube: true,
              sfafile: true,
              imageupload: true,
              //signature: true,
              //camera/image
              sfasignature: true,
              sfabutton: true,
              currency: true
            }
          },
          sfasystem: {
            components: {
              sfaassignacl: true,
              sfasystemredirect: true
            }
          }
        }
      });

    //Initiate Form Renderer
    var formElement = document.getElementById('formio');
    Formio.createForm(formElement, formJSON, {});

    //Initiate Form JSON
    var jsonElement = document.getElementById('json');
    jsonElement.innerHTML = '';
    //jsonElement.appendChild(document.createTextNode(form));
    jsonElement.appendChild(document.createTextNode(JSON.stringify(formJSON, null, 4)));

    //flag to indicate there are unsaved changes or not
    var saved = true;
    //Warning there are unsaved changes in the form
    closeWarning(form)

    function closeWarning(form) {

      window.onbeforeunload = function (event) {

        if (!saved) {
          event = event || window.event;
          event.returnValue = 'Leave site? Changes that you made may not be saved.';
          //event.returnValue = swal({
          //  title: "Error",
          //  text: "Leave site? Changes that you made may not be saved."
          //});
        }
      }
    }
   // warning there are unsaved changes in the form -------END

    var setDisplay = function (display) {
      builder.setDisplay(display).then(function (instance) {
        instance.redraw();
        instance.on('change', function (form) {
          saved = false;
          if (form.components) {
            formElement.innerHTML = '';
            jsonElement.innerHTML = '';
            jsonElement.appendChild(document.createTextNode(JSON.stringify(form, null, 4)));
            Formio.createForm(formElement, form, {});//.then(onForm);
          }
        });
      });
    };

      //Handle the form version selection.
      //var formSelect = document.getElementById('form-select');
      //formSelect.addEventListener("change", function() {
      //  setDisplay(this.value);
      //});

      //$('#form-select').val(formJSON.display);
    setDisplay(formJSON.display);
  //});

    $('#btn-save').click(function () {
      SaveForm();
    });

    $('#btn-save-open').click(function () {
      SaveForm(OpenForm);
    });

    $('#btn-open-live').click(function () {
      OpenForm();
    });

    $('#btn-embed').click(function () {
      _createEmbedModal.open({ originalId: JSONObj.OriginalId, formId: JSONObj.Id, type: "Form" });
    });

    $('#btn-new-version').click(function () {
      _createVersionModal.open({ id: JSONObj.Id, originalId: JSONObj.OriginalId, version: JSONObj.Version });
    });

    $('#btn-share').click(function (event, Id) {
      _manageACLModal.open({ entityid: JSONObj.Id, entityname: JSONObj.Name, entitytype: 'Form' });
    });

    $('#btn-submission-settings').click(function (event, Id) {
      _createOrEditJobModal.open({ EntityId: JSONObj.Id });
    });

    $('#btn-form-settings').click(function (event, Id) {
      
      _formSettingsModal.open({ originalId: JSONObj.OriginalId, version: JSONObj.Version + "" });
    });

    $('#btn-script-editor').click(function (event, Id) {
      var winscript = window.open('/Falcon/forms/scripteditor?OriginalId=' + JSONObj.OriginalId + "&version=" + JSONObj.Version + "", "Script Editor", "width=1024,height=640,top=80,left=50");
      winscript.focus();
    });

    $('#btn-rules').click(function(event, Id) {
      var winrules = window.open("/Falcon/forms/ruleseditor?OriginalId=" + JSONObj.OriginalId + "&version=" + JSONObj.Version + "", "Rules Builder", "width=1110,height=640,top=80,left=50");
      winrules.focus();
    });

    function SaveForm(callback) {
      saved = true;
      var jsonSchema = JSON.stringify(JSON.parse($('#json').text()));
      var formversionname = $('#Form-Version-Name').val();

      _formsService.createOrEdit({
        id: JSONObj.Id,
        OriginalId: JSONObj.OriginalId,
        Description: JSONObj.Description,
        VersionName: formversionname,
        DocPDF: true,
        DocUserCanSave: true,
        DocWord: true,
        DocWordPaid: true,
        Name: JSONObj.Name,
        Version: JSONObj.Version,
        VersionName: JSONObj.VersionName,
        CurrentVersion: JSONObj.CurrentVersion,
        //PaymentCurr: 'AUD',
        //PaymentEnabled: false,
        FolderId: JSONObj.FolderId,
        IsIndex: '3',
        Schema: jsonSchema        
      }).done(function (result) {
        abp.notify.info(app.localize('Form Saved'));
        callback.apply();
      });
    };

    function OpenForm() {
      window.open('/Falcon/forms/load?OriginalId=' + JSONObj.OriginalId + '&version=' + JSONObj.Version + '', '_blank');
    };

      function uuidv4() {
      return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
      });
    }

    $('#ImportFormSchemaLink').click(function () {
      _importFormSchemaModal.open({ Id: JSONObj.Id });
    });

    $('#UploadFormRulesSchemaLink').click(function () {
      _uploadFormRulesSchemaModal.open({ Id: JSONObj.Id });
    });

    abp.event.on('app.FormSettingModalSaved', function () {
      saved = true;
    });

  });
})();
