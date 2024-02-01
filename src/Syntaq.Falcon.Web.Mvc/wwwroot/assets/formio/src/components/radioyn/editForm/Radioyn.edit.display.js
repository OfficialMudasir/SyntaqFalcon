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
    label: 'Show in Summary',
    tooltip: 'Show field in summary table.',
    key: 'showSummary',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 10,
    type: 'checkbox',
    label: 'Important',
    tooltip: 'Used to highlight missing important fields to the user.',
    key: 'important',
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
    type: 'select',
    input: true,
    key: 'labelPosition',
    label: 'Label Position',
    tooltip: 'Position for the label for this field.',
    weight: 20,
    defaultValue: 'top',
    dataSrc: 'values',
    data: {
      values: [
        { label: 'Top', value: 'top' },
        { label: 'Left (Left-aligned)', value: 'left-left' },
        { label: 'Left (Right-aligned)', value: 'left-right' },
        { label: 'Right (Left-aligned)', value: 'right-left' },
        { label: 'Right (Right-aligned)', value: 'right-right' },
        { label: 'Bottom', value: 'bottom' }
      ]
    }
  },
  {
    type: 'number',
    input: true,
    key: 'labelWidth',
    label: 'Label Width',
    tooltip: 'The width of label on line in percentages.',
    clearOnHide: false,
    weight: 30,
    placeholder: '30',
    suffix: '%',
    validate: {
      min: 0,
      max: 100
    },
    conditional: {
      json: {
        and: [
          { '!==': [{ var: 'data.labelPosition' }, 'top'] },
          { '!==': [{ var: 'data.labelPosition' }, 'bottom'] },
        ]
      }
    }
  },
  {
    type: 'number',
    input: true,
    key: 'labelMargin',
    label: 'Label Margin',
    tooltip: 'The width of label margin on line in percentages.',
    clearOnHide: false,
    weight: 40,
    placeholder: '3',
    suffix: '%',
    validate: {
      min: 0,
      max: 100
    },
    conditional: {
      json: {
        and: [
          { '!==': [{ var: 'data.labelPosition' }, 'top'] },
          { '!==': [{ var: 'data.labelPosition' }, 'bottom'] },
        ]
      }
    }
  },
  {
    type: 'select',
    input: true,
    label: 'Options Label Position',
    key: 'optionsLabelPosition',
    tooltip: 'Position for the label for options for this field.',
    dataSrc: 'values',
    weight: 50,
    defaultValue: 'right',
    data: {
      values: [
        { label: 'Top', value: 'top' },
        { label: 'Left', value: 'left' },
        { label: 'Right', value: 'right' },
        { label: 'Bottom', value: 'bottom' }
      ]
    }
  },
  {
    weight: 70,
    type: 'checkbox',
    label: 'Read Only',
    tooltip: 'Disable the form input.',
    key: 'disabled',
    customClass: 'form-check-inline',
    input: true
  },
  //{
  //  weight: 70,
  //  type: 'checkbox',
  //  label: 'Field as Record Name',
  //  tooltip: 'Use this feild\'s name as the Record Name.',
  //  key: 'FieldasRecordName',
  //  customClass: 'form-check-inline',
  //  input: true
  //},
  {
    weight: 70,
    type: 'checkbox',
    label: 'Do not load this field from saved record',
    tooltip: 'Keeps field data fresh by not loading previously saved data.',
    key: 'DoNotLoadFromRecord',
    customClass: 'form-check-inline',
    input: true
  },
  {
    type: 'checkbox',
    input: true,
    key: 'inline',
    label: 'Inline Layout',
    tooltip: 'Displays the checkboxes/radios horizontally.',
    customClass: 'form-check-inline',
    weight: 70
  },
  {
    weight: 80,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  {
    weight: 90,
    type: 'textfield',
    input: true,
    key: 'errorLabel',
    label: 'Error Label',
    placeholder: 'Error Label',
    tooltip: 'The label for this field when an error occurs.'
  },
  {
    weight: 100,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    weight: 110,
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
    weight: 120,
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
    weight: 130,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
