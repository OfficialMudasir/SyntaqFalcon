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
    label: 'Hidden',
    tooltip: 'A hidden field is still a part of the form, but is hidden from view.',
    key: 'hidden',
    customClass: 'form-check-inline',
    input: true
  },
  {
    type: 'select',
    key: 'action',
    label: 'Action',
    input: true,
    dataSrc: 'values',
    weight: 30,
    tooltip: 'This is the action to be performed by this button.',
    data: {
      values: [
        { label: 'Submit', value: 'submit' },
        { label: 'Save State', value: 'saveState' },
        { label: 'Event', value: 'event' },
        { label: 'Custom', value: 'custom' },
        { label: 'Reset', value: 'reset' },
        //{ label: 'OAuth', value: 'oauth' },
        { label: 'POST to URL', value: 'url' }
      ]
    }
  },
  {
    type: 'checkbox',
    input: true,
    inputType: 'checkbox',
    key: 'showValidations',
    label: 'Show Validations',
    weight: 40,
    tooltip: 'When the button is pressed, show any validation errors on the form.',
    conditional: {
      json: { '!==': [{ var: 'data.action' }, 'submit'] }
    }
  },
  {
    type: 'textfield',
    label: 'Button Event',
    key: 'event',
    input: true,
    weight: 50,
    tooltip: 'The event to fire when the button is clicked.',
    conditional: {
      json: { '===': [{ var: 'data.action' }, 'event'] }
    }
  },
  {
    type: 'textfield',
    inputType: 'url',
    key: 'url',
    input: true,
    weight: 60,
    label: 'Button URL',
    tooltip: 'The URL where the submission will be sent.',
    placeholder: 'https://example.form.io',
    conditional: {
      json: { '===': [{ var: 'data.action' }, 'url'] }
    }
  },
  {
    type: 'datagrid',
    key: 'headers',
    input: true,
    weight: 70,
    label: 'Headers',
    addAnother: 'Add Header',
    tooltip: 'Headers Properties and Values for your request',
    components: [
      {
        key: 'header',
        label: 'Header',
        input: true,
        type: 'textfield'
      },
      {
        key: 'value',
        label: 'Value',
        input: true,
        type: 'textfield'
      }
    ],
    conditional: {
      json: { '===': [{ var: 'data.action' }, 'url'] }
    }
  },
  {
    type: 'textarea',
    key: 'custom',
    label: 'Button Custom Logic',
    tooltip: 'The custom logic to evaluate when the button is clicked.',
    rows: 5,
    editor: 'ace',
    input: true,
    weight: 80,
    placeholder: "data['mykey'] = data['anotherKey'];",
    conditional: {
      json: { '===': [{ var: 'data.action' }, 'custom'] }
    }
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
    weight: 140,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    weight: 150,
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
    weight: 160,
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
    weight: 170,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
