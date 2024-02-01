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
    weight: 0,
    type: 'textfield',
    input: true,
    key: 'label',
    label: 'Label',
    placeholder: 'Field Label',
    tooltip: 'The label for this field that will appear next to it.'
  },
  {
    weight: 10,
    type: 'checkbox',
    label: 'Hide Label',
    tooltip: 'Hide the label of this component. This allows you to show the label in the form builder, but not when it is rendered.',
    key: 'hideLabel',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 10,
    type: 'checkbox',
    label: 'Apply Label On First Row',
    tooltip: 'Apply Label On First Row Of this Component',
    key: 'applyLabelFirstRow',
    customClass: 'form-check-inline',
    input: true
  },
  //{
  //  type: 'checkbox',
  //  label: 'Disable Adding / Removing Rows',
  //  key: 'disableAddingRemovingRows',
  //  tooltip: 'Check if you want to hide Add Another button and Remove Row button',
  //  weight: 20,
  //  input: true,
  //  clearOnHide: false
  //},
  {
    weight: 10,
    type: 'checkbox',
    label: 'Show in Summary',
    tooltip: 'Show field in summary table.',
    key: 'showSummary',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 10,
    type: 'checkbox',
    label: 'Hidden',
    tooltip: 'A hidden field is still a part of the form, but is hidden from view.',
    key: 'hidden',
    customClass: 'form-check-inline',
    input: true
  },
  {
    type: 'textfield',
    label: 'Add Another Text',
    key: 'addAnother',
    tooltip: 'Set the text of the Add Another button.',
    placeholder: 'Add Another',
    weight: 30,
    input: true,
    customConditional: 'show = !data.disableAddingRemovingRows'
  },
  {
    type: 'number',
    label: 'Initial rows',
    key: 'initialrows',
    tooltip: 'Initial rows',
    defaultValue: 1,
    weight: 35,
    input: true
  },
  {
    type: 'number',
    label: 'Minimum rows',
    key: 'minrows',
    tooltip: 'Minimum rows',
    defaultValue: 1,
    weight: 40,
    input: true
  },
  {
    type: 'number',
    label: 'Maximum rows',
    key: 'maxLength',
    tooltip: 'Maximum rows',
    defaultValue: 10,
    weight: 50,
    input: true
  },
  {
    type: 'textfield',
    label: 'Repeat Divider Title',
    key: 'dividertitle',
    tooltip: 'Repeat Divider Title',
    weight: 60,
    input: true
  },
  {
    weight: 70,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  {
    weight: 80,
    type: 'textfield',
    input: true,
    key: 'errorLabel',
    label: 'Error Label',
    placeholder: 'Error Label',
    tooltip: 'The label for this field when an error occurs.'
  },
  {
    weight: 90,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    weight: 100,
    type: 'checkbox',
    label: 'Collapsible',
    tooltip: 'If checked, this will turn this Panel into a collapsible panel.',
    key: 'collapsible',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 110,
    type: 'checkbox',
    label: 'Initially Collapsed',
    tooltip: 'Determines the initial collapsed state of this Panel.',
    key: 'collapsed',
    input: true,
    conditional: {
      json: { '===': [{ var: 'data.collapsible' }, true] }
    }
  },
  {
    weight: 120,
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
    weight: 130,
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
    weight: 140,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  },
  {
    label: 'Border Width',
    key: 'border',
    widthslider: '6',
    type: 'sfanumber',
    defaultValue: 1,
    input: true,
    validate: {
      pattern: '^([0-9]|([1-9][0-9])|50)$',
      customMessage: 'The Border Width should be an integer and between 0 to 50.',
      min: 0,
      max: 50
    },
    logic: []
  },
  {
    label: 'Border Radius',
    key: 'borderR',
    widthslider: '6',
    type: 'sfanumber',
    defaultValue: 4,
    input: true,
    validate: {
      pattern: '^([0-9]|([1-9][0-9])|50)$',
      customMessage: 'The Border Radius should be an integer and between 0 to 50.',
      min: 0,
      max: 50
    },
    logic: []
  },
  {
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  }
];
