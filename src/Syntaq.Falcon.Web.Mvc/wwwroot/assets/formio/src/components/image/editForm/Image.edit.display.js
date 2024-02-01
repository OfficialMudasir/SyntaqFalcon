export default [
  {
    weight: 0,
    type: 'textfield',
    input: true,
    key: 'key',
    label: 'Field Name',
    tooltip: 'The name of this field when submitted.',
    validate: {
      pattern: '(\\w|\\w[\\w-.]*\\w)',
      patternMessage: 'The property name must only contain alphanumeric characters, underscores, dots and dashes and should not be ended by dash or dot.'
    }
  },
  {
    type: 'file',
    input: true,
    label: 'Image',
    key: 'uploadfile',
    weight: 0,
    mask: false,
    tableView: true,
    alwaysEnabled: false,
    image: true,
    storage: 'base64',
    dir: '',
    // fileMinSize: '0KB',
    // fileMaxSize: '2048KB',
    webcam: false
  },
  {
    type: 'slider',
    label: 'Image size (%)',
    key: 'sizeslider',
    tooltip: 'Image\'s size',
    defaultValue: 5,
    minValue: 0,
    maxValue: 10,
    step: 1,
    weight: 10
  },
  {
    type: 'slider',
    label: 'Width in Columns',
    key: 'widthslider',
    tooltip: 'Columns\'s width',
    defaultValue: 12,
    minValue: 1,
    maxValue: 12,
    step: 1,
    weight: 20
  },
  {
    type: 'slider',
    label: 'Offset',
    key: 'offsetslider',
    tooltip: 'Columns\'s width',
    defaultValue: 0,
    minValue: 0,
    maxValue: 12,
    step: 1,
    weight: 30
  },
  {
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  }
];
