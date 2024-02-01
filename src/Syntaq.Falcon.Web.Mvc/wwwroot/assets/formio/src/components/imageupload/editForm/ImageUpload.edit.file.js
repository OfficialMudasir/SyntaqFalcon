
export default [
  {
    weight: 0,
    type: 'textfield',
    input: true,
    key: 'key',
    label: 'Field Name',
    tooltip: 'The name of this field in the API endpoint.',
    validate: {
      pattern: '(\\w|\\w[\\w-.]*\\w)',
      patternMessage: 'The property name must only contain alphanumeric characters, underscores, dots and dashes and should not be ended by dash or dot.'
    }
  },
  // {
  //   type: 'textfield',
  //   input: true,
  //   key: 'imageSize',
  //   label: 'Image Size',
  //   placeholder: '100',
  //   tooltip: 'The image size for previewing images.',
  //   weight: 40,
  //   conditional: {
  //     json: { '==': [{ var: 'data.image' }, true] }
  //   }
  // },
  {
    type: 'textfield',
    input: true,
    key: 'webcamSize',
    label: 'Webcam Width',
    placeholder: '320',
    tooltip: 'The webcam size for taking pictures.',
    weight: 10,
    conditional: {
      json: { '==': [{ var: 'data.webcam' }, true] }
    }
  },
  // {
  //   type: 'textfield',
  //   input: true,
  //   key: 'filePattern',
  //   label: 'File Pattern',
  //   placeholder: '.pdf,.jpg',
  //   tooltip: 'See <a href=\'https://github.com/danialfarid/ng-file-upload#full-reference\' target=\'_blank\'>https://github.com/danialfarid/ng-file-upload#full-reference</a> for how to specify file patterns.',
  //   weight: 50
  // },
  {
    type: 'textfield',
    input: true,
    key: 'fileMinSize',
    label: 'File Minimum Size',
    placeholder: '1MB',
    tooltip: 'See <a href=\'https://github.com/danialfarid/ng-file-upload#full-reference\' target=\'_blank\'>https://github.com/danialfarid/ng-file-upload#full-reference</a> for how to specify file sizes.',
    weight: 20
  },
  {
    type: 'textfield',
    input: true,
    key: 'fileMaxSize',
    label: 'File Maximum Size',
    placeholder: '10MB',
    tooltip: 'See <a href=\'https://github.com/danialfarid/ng-file-upload#full-reference\' target=\'_blank\'>https://github.com/danialfarid/ng-file-upload#full-reference</a> for how to specify file sizes.',
    weight: 30
  },
  {
    weight: 40,
    type: 'slider',
    label: 'Image size (%)',
    key: 'imageSize',
    tooltip: 'Image\'s width',
    defaultValue: 50,
    minValue: 0,
    maxValue: 100,
    step: 10
  },
  {
    weight: 50,
    type: 'slider',
    label: 'Width in Columns',
    key: 'widthslider',
    tooltip: 'Columns\'s width',
    defaultValue: 12,
    minValue: 1,
    maxValue: 12,
    step: 1
  },
  {
    weight: 60,
    type: 'slider',
    label: 'Offset',
    key: 'offsetslider',
    tooltip: 'Columns\'s width',
    defaultValue: 0,
    minValue: 0,
    maxValue: 12,
    step: 1
  },
  {
    weight: 70,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
