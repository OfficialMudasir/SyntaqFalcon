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
      patternMessage: 'The field name must only contain alphanumeric characters, underscores, dots and dashes and should not be ended by dash or dot.'
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'footer',
    label: 'Footer Label',
    tooltip: 'The footer text that appears below the signature area.',
    placeholder: 'Footer Label',
    weight: 5
  },
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
    weight: 10,
    type: 'checkbox',
    label: 'Clear Value When Hidden',
    key: 'clearOnHide',
    tooltip: 'When a field is hidden, clear the value.',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 30,
    type: 'checkbox',
    label: 'Read Only',
    tooltip: 'Disable the form input.',
    key: 'disabled',
    customClass: 'form-check-inline',
    input: true
  },
  //{
  //  weight: 40,
  //  type: 'checkbox',
  //  label: 'Field as Record Name',
  //  tooltip: 'Use this feild\'s name as the Record Name.',
  //  key: 'FieldasRecordName',
  //  customClass: 'form-check-inline',
  //  input: true
  //},
  {
    type: 'textfield',
    input: true,
    key: 'width',
    label: 'Width',
    tooltip: 'The width of the signature area.',
    placeholder: 'Width',
    weight: 50
  },
  {
    type: 'textfield',
    input: true,
    key: 'height',
    label: 'Height',
    tooltip: 'The height of the signature area.',
    placeholder: 'Height',
    weight: 51
  },
  {
    type: 'textfield',
    input: true,
    key: 'backgroundColor',
    label: 'Background Color',
    tooltip: 'The background color of the signature area.',
    placeholder: 'Background Color',
    weight: 52
  },
  {
    type: 'textfield',
    input: true,
    key: 'penColor',
    label: 'Pen Color',
    tooltip: 'The ink color for the signature area.',
    placeholder: 'Pen Color',
    weight: 53
  },
  {
    weight: 60,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  {
    weight: 70,
    type: 'textfield',
    input: true,
    key: 'errorLabel',
    label: 'Error Label',
    placeholder: 'Error Label',
    tooltip: 'The label for this field when an error occurs.'
  },
  {
    weight: 80,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    //weight: 120,
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
    //weight: 130,
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
   // weight: 200,
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  },
  {
    weight: 130,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
