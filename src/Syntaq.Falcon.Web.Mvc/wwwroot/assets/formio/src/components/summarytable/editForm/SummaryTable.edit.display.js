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
    label: 'Label',
    //validate: {
    //  required: true,
    //  customMessage: ''
    //},
    //logic: [
    //  {
    //    name: 'set',
    //    trigger: {
    //      type: 'javascript',
    //      javascript: 'result = ((data.FormId !== undefined && data.FormId !== ""))?true:false;'
    //    },
    //    actions: [
    //      {
    //        name: 'act',
    //        type: 'value',
    //        value: 'value = value===""?form.components[0].components[0].components[1].labelName:value;'
    //      }
    //    ]
    //  }
    //]
  },
  {
    type: 'sfatextfield',
    input: true,
    key: 'Caption',
    label: 'Caption',
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
  //{
  //  type: 'sfaselect',
  //  input: true,
  //  dataSrc: 'url',
  //  data: {
  //    url: '{{ location.protocol }}//{{window.location.hostname}}:{{window.location.port}}/api/services/app/Forms/GetFormsList'
  //  },
  //  template: '<span>{{ item.label }}</span>',
  //  valueProperty: 'value',
  //  authenticate: true,
  //  label: 'Form',
  //  labelName: '',
  //  key: 'FormId',
  //  weight: 10,
  //  validate: {
  //    required: true,
  //    customMessage: ''
  //  },
  //  tooltip: 'The form to load within this form component.'
  //},
  {
    weight: 15,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  {
    type: 'sfacheckbox',
    input: true,
    weight: 20,
    key: 'reference',
    label: 'Save as reference',
    tooltip: 'Using this option will save this field as a reference and link its value to the value of the origin record.'
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
