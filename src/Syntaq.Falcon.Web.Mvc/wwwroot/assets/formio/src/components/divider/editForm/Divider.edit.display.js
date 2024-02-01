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
    type: 'select',
    input: true,
    label: 'Options Divider Style',
    key: 'dividerstyle',
    tooltip: 'Style for the divider for options for this field.',
    dataSrc: 'values',
    weight: 0,
    defaultValue: 'style1',
    data: {
      values: [
        { label: 'Invisible', value: 'style0' },
        { label: 'Solid', value: 'style1' },
        { label: 'Double solid', value: 'style2' },
        { label: 'Dashed', value: 'style3' },
        { label: 'Dotted', value: 'style4' },
        { label: 'Fancy top', value: 'style5' },
        { label: 'Fancy Bottom', value: 'style6' }
      ]
    }
  },
  {
    type: 'textarea',
    as: 'html',
    editor: 'ace',
    weight: 10,
    input: true,
    key: 'customecss',
    label: 'Custom Divider CSS',
    tooltip: 'Enter the CSS for this custom element.'
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
    weight: 10
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
    weight: 10
  },
  {
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  }
];
