export default [
  {
    weight: 0,
    type: 'checkbox',
    label: 'Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
    key: 'validate.required',
    input: true
  },
  {
    type: 'number',
    label: 'Minimum Value',
    key: 'validate.min',
    input: true,
    placeholder: 'Minimum Value',
    tooltip: 'The minimum value this field must have before the form can be submitted.',
    weight: 10
  },
  {
    type: 'number',
    label: 'Maximum Value',
    key: 'validate.max',
    input: true,
    placeholder: 'Maximum Value',
    tooltip: 'The maximum value this field can have before the form can be submitted.',
    weight: 20
  },
  {
    weight: 30,
    key: 'validate.pattern',
    label: 'Regular Expression Pattern',
    placeholder: 'Regular Expression Pattern',
    type: 'textfield',
    tooltip: 'The regular expression pattern test that the field value must pass before the form can be submitted.',
    input: true
  },
  {
    weight: 40,
    type: 'select',
    key: 'validateOn',
    defaultValue: 'change',
    input: true,
    label: 'Validate On',
    tooltip: 'Determines when this component should trigger front-end validation.',
    dataSrc: 'values',
    data: {
      values: [
        { label: 'Change', value: 'change' },
        { label: 'Blur', value: 'blur' }
      ]
    }
  },
  {
    weight: 50,
    key: 'validate.customMessage',
    label: 'Custom Error Message',
    placeholder: 'Custom Error Message',
    type: 'textfield',
    tooltip: 'Error message displayed if any error occurred.',
    input: true
  }
];
