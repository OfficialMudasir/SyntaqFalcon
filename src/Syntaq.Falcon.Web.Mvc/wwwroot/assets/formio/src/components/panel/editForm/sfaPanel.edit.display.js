export default [
  {
    weight: 0,
    type: 'textfield',
    input: true,
    key: 'key',
    label: 'Field Name',
    tooltip: 'The name of this panel.',
    validate: {
      pattern: '(\\w|\\w[\\w-.]*\\w)',
      patternMessage: 'The property name must only contain alphanumeric characters, underscores, dots and dashes and should not be ended by dash or dot.'
    }
  },
  {
    key: 'label',
    hidden: true,
    calculateValue: 'value = data.title'
  },
  {
    weight: 0,
    type: 'textfield',
    input: true,
    placeholder: 'Label',
    label: 'Label',
    key: 'title',
    tooltip: 'The title text that appears in the header of this panel.'
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
    label: 'Hidden',
    tooltip: 'A hidden field is still a part of the form, but is hidden from view.',
    key: 'hidden',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 30,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  //{
  //  weight: 30,
  //  type: 'select',
  //  input: true,
  //  label: 'Theme',
  //  key: 'theme',
  //  dataSrc: 'values',
  //  data: {
  //    values: [
  //      { label: 'Default', value: 'default' },
  //      { label: 'Primary', value: 'primary' },
  //      { label: 'Info', value: 'info' },
  //      { label: 'Success', value: 'success' },
  //      { label: 'Danger', value: 'danger' },
  //      { label: 'Warning', value: 'warning' }
  //    ]
  //  }
  //},
  // Stq exited
  {
    weight: 30,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    weight: 40,
    type: 'checkbox',
    label: 'Collapsible',
    tooltip: 'If checked, this will turn this Panel into a collapsible panel.',
    key: 'collapsible',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 50,
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
    weight: 60,
    type: 'fieldset',
    input: false,
    components: [
      {
        type: 'select',
        input: true,
        label: 'Breadcrumb Type',
        key: 'breadcrumb',
        dataSrc: 'values',
        data: {
          values: [
            { label: 'Default', value: 'default' },
            { label: 'Condensed', value: 'condensed' },
            { label: 'Hidden', value: 'none' },
          ]
        }
      },
      {
        input: true,
        type: 'checkbox',
        label: 'Allow click on Breadcrumb',
        key: 'breadcrumbClickable',
        defaultValue: true,
        conditional: {
          json: { '!==': [{ var: 'data.breadcrumb' }, 'none'] }
        }
      }
      //,
      //{
      //  weight: 70,
      //  label: 'Panel Navigation Buttons',
      //  optionsLabelPosition: 'right',
      //  values: [
      //    {
      //      label: 'Previous',
      //      value: 'previous',
      //    },
      //    {
      //      label: 'Cancel',
      //      value: 'cancel',
      //    },
      //    {
      //      label: 'Next',
      //      value: 'next',
      //    }
      //  ],
      //  inline: true,
      //  type: 'selectboxes',
      //  key: 'buttonSettings',
      //  input: true,
      //  inputType: 'checkbox',
      //  defaultValue: {
      //    previous: true,
      //    cancel: true,
      //    next: true
      //  },
      //}
    ],
    customConditional: 'show = instance.root.editForm.display === "wizard"',
  },
  {
    type: 'slider',
    label: 'Width in Columns',
    key: 'widthslider',
    tooltip: 'Columns\'s width',
    defaultValue: 12,
    minValue: 1,
    maxValue: 12,
    step: 1,
    weight: 80
  },
  {
    type: 'slider',
    label: 'Offset',
    key: 'offsetslider',
    tooltip: 'Columns\'s width',
    defaultValue: 0,
    minValue: 0,
    maxValue: 12,
    step: 1,
    weight: 90
  },
  {
    label: 'Border Width',
    key: 'border',
    widthslider: '6',
    type: 'sfanumber',
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
