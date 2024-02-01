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
    input: true
  },
  {
    weight: 10,
    type: 'checkbox',
    label: 'Show in Summary',
    tooltip: 'Show field in summary table.',
    key: 'showSummary',
    input: true
  },
  {
    weight: 10,
    type: 'checkbox',
    label: 'Hidden',
    tooltip: 'A hidden field is still a part of the form, but is hidden from view.',
    key: 'hidden',
    input: true
  },
  //{
  //  weight: 20,
  //  type: 'textfield',
  //  input: true,
  //  key: 'fieldname',
  //  label: 'Field Name',
  //  placeholder: 'Field Name',
  //  tooltip: 'The name the field data will be associated with.'
  //},
  {
    weight: 30,
    type: 'checkbox',
    label: 'Lock if assembled',
    tooltip: 'Lock field if form is for an assembled record.',
    key: 'lockAssembled',
    input: true
  },
  {
    weight: 30,
    type: 'checkbox',
    label: 'Read Only',
    tooltip: 'Disable the form input.',
    key: 'disabled',
    input: true
  },
  //{
  //  weight: 30,
  //  type: 'checkbox',
  //  label: 'Field as Record Name',
  //  tooltip: 'Use this feild\'s name as the Record Name.',
  //  key: 'FieldasRecordName',
  //  input: true
  //},
  {
    weight: 30,
    type: 'checkbox',
    label: 'Required',
    tooltip: 'Make this field required.',
    key: 'required',
    input: true
  },
  {
    type: 'textfield',
    label: 'Default Value',
    key: 'defaultValue',
    weight: 40,
    placeholder: 'Default Value',
    tooltip: 'The will be the value for this field, before user interaction. Having a default value will override the placeholder text.',
    input: true
  },
  {
    weight: 50,
    type: 'checkbox',
    label: 'Hidden',
    tooltip: 'A hidden field is still a part of the form, but is hidden from view.',
    key: 'hidden',
    input: true
  },
  {
    type: 'number',
    input: true,
    weight: 60,
    key: 'decimalLimit',
    label: 'Decimal Places',
    tooltip: 'The maximum number of decimal places.'
  },
  {
    type: 'number',
    label: 'Minimum Value',
    key: 'validate.min',
    input: true,
    placeholder: 'Minimum Value',
    tooltip: 'The minimum value this field must have before the form can be submitted.',
    weight: 70
  },
  {
    type: 'number',
    label: 'Maximum Value',
    key: 'validate.max',
    input: true,
    placeholder: 'Maximum Value',
    tooltip: 'The maximum value this field can have before the form can be submitted.',
    weight: 80
  },
  {
    weight: 90,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
