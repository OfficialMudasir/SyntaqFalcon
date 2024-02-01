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
    type: 'sfatextfield',
    input: true,
    key: 'label',
    label: 'Button Label',
    placeholder: 'Button Text',
    validate: {
      required: true,
      customMessage: ''
    },
    logic: [
      {
        name: 'set',
        trigger: {
          type: 'javascript',
          javascript: 'result = ((data.formId !== undefined && data.formId !== ""))?true:false;'
        },
        actions: [
          {
            name: 'act',
            type: 'value',
            value: 'value = value===""?form.components[0].components[0].components[1].labelName:value;'
          }
        ]
      }
    ]
  },
  {
    type: 'sfaselect',
    input: true,
    dataSrc: 'url',
    data: {
      url: '{{ location.protocol }}//{{window.location.hostname}}:{{window.location.port}}/api/services/app/Forms/GetFormsList'
    },
    template: '<span>{{ item.label }}</span>',
    valueProperty: 'value',
    authenticate: true,
    label: 'Form',
    labelName: '',
    key: 'formId',
    weight: 10,
    validate: {
      required: true,
      customMessage: ''
    },
    tooltip: 'The form to load within this form component.'
  },
  {
    type: 'checkbox',
    input: true,
    weight: 20,
    key: 'reference',
    label: 'Save as reference',
    customClass: 'form-check-inline',
    tooltip: 'Using this option will save this field as a reference and link its value to the value of the origin record.'
  },
  {
    weight: 20,
    type: 'checkbox',
    label: 'Show in Summary',
    tooltip: 'Show field in summary table.',
    key: 'showSummary',
    customClass: 'form-check-inline',
    input: true
  },
  {
    type: 'select',
    key: 'theme',
    label: 'Theme',
    input: true,
    tooltip: 'The color theme of this button.',
    dataSrc: 'values',
    weight: 90,
    data: {
      values: [
        { label: 'Default', value: 'default' },
        { label: 'Primary', value: 'primary' },
        { label: 'Info', value: 'info' },
        { label: 'Success', value: 'success' },
        { label: 'Danger', value: 'danger' },
        { label: 'Warning', value: 'warning' }
      ]
    }
  },
  {
    type: 'select',
    key: 'size',
    label: 'Size',
    input: true,
    tooltip: 'The size of this button.',
    dataSrc: 'values',
    weight: 100,
    data: {
      values: [
        { label: 'Extra Small', value: 'xs' },
        { label: 'Small', value: 'sm' },
        { label: 'Medium', value: 'md' },
        { label: 'Large', value: 'lg' }
      ]
    }
  },
  {
    type: 'textfield',
    key: 'leftIcon',
    label: 'Left Icon',
    input: true,
    placeholder: 'Enter icon classes',
    tooltip: "This is the full icon class string to show the icon. Example: 'fa fa-plus'",
    weight: 110
  },
  {
    type: 'textfield',
    key: 'rightIcon',
    label: 'Right Icon',
    input: true,
    placeholder: 'Enter icon classes',
    tooltip: "This is the full icon class string to show the icon. Example: 'fa fa-plus'",
    weight: 120
  },
  {
    type: 'checkbox',
    key: 'disableOnInvalid',
    label: 'Disable on Form Invalid',
    tooltip: 'This will disable this field if the form is invalid.',
    input: true,
    customClass: 'form-check-inline',
    weight: 130
  },
  {
    type: 'checkbox',
    key: 'validate.required',
    label: 'Popup form has mandatory fields',
    tooltip: 'Popup form has mandatory fields.',
    input: true,
    customClass: 'form-check-inline',
    weight: 130
  },
  {
    weight: 140,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    weight: 180,
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
    weight: 190,
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
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  }
];
