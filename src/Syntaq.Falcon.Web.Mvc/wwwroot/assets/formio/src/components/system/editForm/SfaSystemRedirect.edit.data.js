export default [
  {
    weight: 0,
    type: 'textfield',
    input: true,
    key: 'key',
    label: 'Field Name',
    hidden: true,
    defaultValue: 'sfasystemredirect'
  },
  {
    type: 'textfield',
    label: 'Default Value',
    key: 'defaultValue',
    weight: 10,
    placeholder: 'Default Value',
    tooltip: 'This will be the URL to redirect the user too after form submission.',
    input: true
  },
  {
    weight: 20,
    type: 'sfacheckbox',
    label: 'Hidden',
    tooltip: 'A hidden field is still a part of the form, but is hidden from view.',
    key: 'hidden',
    input: true
  },
  {
    weight: 30,
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
    weight: 40,
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
    weight: 50,
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  }
];
