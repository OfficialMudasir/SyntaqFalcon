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
    key: 'validate.minLength',
    label: 'Minimum Length',
    placeholder: 'Minimum Length',
    type: 'number',
    tooltip: 'The minimum length requirement this field must meet.',
    input: true
  },
  {
    weight: 20,
    key: 'validate.maxLength',
    label: 'Maximum Length',
    placeholder: 'Maximum Length',
    type: 'number',
    tooltip: 'The maximum length requirement this field must meet.',
    input: true
  },
  {
    weight: 30,
    key: 'validate.minWords',
    label: 'Minimum Word Length',
    placeholder: 'Minimum Word Length',
    type: 'number',
    tooltip: 'The minimum amount of words that can be added to this field.',
    input: true
  },
  {
    weight: 40,
    key: 'validate.maxWords',
    label: 'Maximum Word Length',
    placeholder: 'Maximum Word Length',
    type: 'number',
    tooltip: 'The maximum amount of words that can be added to this field.',
    input: true
  },
  {
    weight: 50,
    key: 'validate.pattern',
    label: 'Regular Expression Pattern',
    placeholder: 'Regular Expression Pattern',
    type: 'textfield',
    tooltip: 'The regular expression pattern test that the field value must pass before the form can be submitted.',
    input: true
  },
  {
    weight: 60,
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
    weight: 70,
    key: 'validate.customMessage',
    label: 'Custom Error Message',
    placeholder: 'Custom Error Message',
    type: 'textfield',
    tooltip: 'Error message displayed if any error occurred.',
    input: true
  }
];
