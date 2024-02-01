//import Formio from '../../../Formio';
//import _ from 'lodash';

export default [
  {
    type: 'sfatextfield',
    input: true,
    key: 'storage',
    placeholder: 'Select your file storage provider',
    weight: 0,
    hidden: true
  },
  //{
  //  type: 'textfield',
  //  input: true,
  //  key: 'url',
  //  label: 'Url',
  //  weight: 10,
  //  placeholder: 'Enter the url to post the files to.',
  //  tooltip: "See <a href='https://github.com/danialfarid/ng-file-upload#server-side' target='_blank'>https://github.com/danialfarid/ng-file-upload#server-side</a> for how to set up the server.",
  //  conditional: {
  //    json: { '===': [{ var: 'data.storage' }, 'url'] }
  //  }
  //},
  {
    type: 'checkbox',
    input: true,
    key: 'webcam',
    label: 'Enable web camera',
    tooltip: 'This will allow using an attached camera to directly take a picture instead of uploading an existing file.',
    weight: 32
  },
  {
    type: 'textfield',
    input: true,
    key: 'webcamSize',
    label: 'Webcam Width',
    placeholder: '320',
    tooltip: 'The webcam size for taking pictures.',
    weight: 38,
    conditional: {
      json: { '==': [{ var: 'data.webcam' }, true] }
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'filePattern',
    label: 'File Pattern',
    placeholder: '.pdf,.jpg,.doc,.docx',
    tooltip: '".pdf,.jpg,video/*,!.jog" comma separated wildcard to filter file names and types allowed you can exclude specific files by! at the beginning.',
    weight: 50
  },
  {
    type: 'textfield',
    input: true,
    key: 'fileMinSize',
    label: 'File Minimum Size',
    placeholder: '1MB',
    tooltip: '"10KB" or "10MB" of specified upload file minimum size.',
    weight: 60
  },
  {
    type: 'textfield',
    input: true,
    key: 'fileMaxSize',
    label: 'File Maximum Size',
    placeholder: '10MB',
    tooltip: '"10KB" or "10MB" of specified upload file Maximum size.',
    weight: 70
  }
];
