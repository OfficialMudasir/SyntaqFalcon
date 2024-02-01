import Promise from 'native-promise-only';
var Cookies = require('js-cookie');
var $ = require('jquery');

const url = (formio) => {
  const xhrRequest = (data) => {
    return new Promise((resolve, reject) => {
      var AntiForgeryToken = Cookies.get('XSRF-TOKEN');

      var ContentDisposition = 'form-data; name="file"; filename="';
      ContentDisposition = ContentDisposition.concat(data.file.name, '";');

      var ContentType = 'multipart/form-data; boundary=';
      ContentType = ContentType.concat((new Date()).getTime().toString());

      var formData = new FormData();
      formData.append('file', data.file, data.file.name);
      formData.append('FileType', data.file.type);
      formData.append('FileName', data.file.name);

      var UploadedFiles = data.dataValue;
      var PrevUpload = false;

      if (UploadedFiles !== undefined || UploadedFiles.length !== 0) {
        var i;
        for (i = 0; i < UploadedFiles.length; i++) {
          if (UploadedFiles[i].name === data.file.name) {
            formData.append('FileToken', UploadedFiles[i].token);
            formData.append('IsPrevUpload', true);
            PrevUpload = true;
          }
        }
      }

      if (PrevUpload !== true) {
        formData.append('FileToken', uuidv4());
        formData.append('IsPrevUpload', false);
      }

      const URL = window._SyntaqBaseURI;

      $.ajax({
        //url: 'https://falcon.syntaq.com/Files/UploadFile',
        url: `${ URL }/Files/UploadFile`,
        data: formData,
        cache: false,
        enctype: ContentType,
        processData: false,
        contentType: false,
        method: 'POST',
        beforeSend: function(request) {
          request.setRequestHeader('X-XSRF-TOKEN', AntiForgeryToken);
          request.setRequestHeader('Content-Disposition', ContentDisposition);
        },
        success: function(returned) {
          returned.result.message = 'File Uploaded.';
          resolve(returned.result);
        },
        error: function(returned) {
          reject('Unable to upload file');
        }
      });
    });
  };

  return {
    title: 'Url',
    name: 'url',
    uploadFile(file, name, dir, progressCallback, dataValue) {
      const uploadRequest = function(form) {
        return xhrRequest(
        {
            file,
            name,
            dir,
            dataValue
        }
        ).then(response => {
            return {
              file: true,
              name,
              token: response.fileToken,
              size: file.size,
              type: response.fileType,
              data: response.message
          };
          });
      };
      if (file.private && formio.formId) {
        return formio.loadForm().then((form) => uploadRequest(form));
      }
      else {
        return uploadRequest();
      }
    },
    downloadFile(file) {
      if (file.private) {
        if (formio.submissionId && file.data) {
          file.data.submission = formio.submissionId;
        }
        return xhrRequest(file.url, file.name, {}, JSON.stringify(file)).then(response => response.data);
      }

      // Return the original as there is nothing to do.
      return Promise.resolve(file);
    }
  };
};

function uuidv4() {
  return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
    (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
  );
}

url.title = 'Url';
export default url;
