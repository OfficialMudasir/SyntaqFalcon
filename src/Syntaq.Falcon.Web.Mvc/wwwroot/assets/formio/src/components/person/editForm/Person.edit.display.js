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
    weight: 30,
    type: 'checkbox',
    label: 'Read Only',
    tooltip: 'Disable the form input.',
    key: 'disabled',
    customClass: 'form-check-inline',
    input: true
  },
  //{
  //  weight: 30,
  //  type: 'checkbox',
  //  label: 'Field as Record Name',
  //  tooltip: 'Use this feild\'s name as the Record Name.',
  //  key: 'FieldasRecordName',
  //  customClass: 'form-check-inline',
  //  input: true
  //},
  {
    weight: 40,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  {
    weight: 50,
    type: 'textfield',
    input: true,
    key: 'errorLabel',
    label: 'Error Label',
    placeholder: 'Error Label',
    tooltip: 'The label for this field when an error occurs.'
  },
  {
    weight: 60,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    key: 'placeholderPanel',
    title: 'Placeholder',
    collapsible: false,
    collapsed: false,
    widthslider: '12',
    offsetslider: '0',
    border: 1,
    borderR: 4,
    type: 'sfapanel',
    input: false,
    label: 'Placeholder',
    weight: 65,
    components: [
      {
        key: 'firstNamePlaceholder',
        label: 'First name',
        placeholder: 'First name placeholder',
        widthslider: '4',
        offsetslider: '0',
        type: 'sfatextfield',
        input: true,
        logic: []
      },
      {
        key: 'middleNamePlaceholder',
        label: 'Middle name',
        placeholder: 'Middle name placeholder',
        widthslider: '4',
        offsetslider: '0',
        type: 'sfatextfield',
        input: true,
        logic: []
      },
      {
        key: 'lastNamePlaceholder',
        label: 'Last name ',
        placeholder: 'Last name placeholder',
        widthslider: '4',
        offsetslider: '0',
        type: 'sfatextfield',
        input: true,
        logic: []
      }
    ]
  },
  {
    weight: 75,
    type: 'htmlelement',
    input: false,
    content: '<div></div>',
    widthslider: 12,
    offsetslider: 0
  },
  {
    weight: 75,
    type: 'htmlelement',
    input: false,
    content: '<div></div>',
    widthslider: 12,
    offsetslider: 0
  },
  {
    weight: 75,
    type: 'checkbox',
    customClass: 'form-check-inline',
    input: true,
    key: 'hidetitle',
    label: 'Hide title',
    tooltip: 'Hide title',
  },
  {
    weight: 75,
    type: 'checkbox',
    customClass: 'form-check-inline',
    input: true,
    key: 'hidemidname',
    label: 'Hide middle name',
    tooltip: 'Hide middle name',
  },
  {
    weight: 75,
    type: 'checkbox',
    customClass: 'form-check-inline',
    input: true,
    key: 'hidefullname',
    label: 'Hide full name',
    tooltip: 'Hide full name',
  },
  {
    weight: 85,
    type: 'htmlelement',
    input: false,
    content: '<div></div>',
    widthslider: 12,
    offsetslider: 0
  },
  {
    weight: 85,
    type: 'htmlelement',
    input: false,
    content: '<div></div>',
    widthslider: 12,
    offsetslider: 0
  },
  {
    weight: 85,
    type: 'checkbox',
    input: true,
    customClass: 'form-check-inline',
    key: 'requiredfirstname',
    label: 'First Name Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
  },
  {
    weight: 85,
    type: 'checkbox',
    input: true,
    customClass: 'form-check-inline',
    key: 'requiredlastname',
    label: 'Last Name Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
  },
  {
    weight: 95,
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
    weight: 105,
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
    weight: 115,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
