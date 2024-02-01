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
    weight: 10,
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
    weight: 20,
    key: 'validate.customMessage',
    label: 'Custom Error Message',
    placeholder: 'Custom Error Message',
    type: 'textfield',
    tooltip: 'Error message displayed if any error occurred.',
    input: true
  }
];
