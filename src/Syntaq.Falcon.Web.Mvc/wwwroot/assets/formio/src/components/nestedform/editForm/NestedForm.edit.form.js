export default [
  //{
  //  weight: 0,
  //  type: 'textfield',
  //  input: true,
  //  key: 'key',
  //  label: 'Field Name',
  //  tooltip: 'The name of this field in the API endpoint.',
  //  validate: {
  //    pattern: '(\\w|\\w[\\w-.]*\\w)',
  //    patternMessage: 'The property name must only contain alphanumeric characters, underscores, dots and dashes and should not be ended by dash or dot.'
  //  }
  //},
  {
    type: 'sfatextfield',
    input: true,
    key: 'label',
    label: 'Form name',
    placeholder: 'Embedded form\'s name',
    validate: {
      required: true,
      customMessage: ''
    },
    logic: [
      {
        name: 'set',
        trigger: {
          type: 'javascript',
          javascript: 'result = ((data.FormId !== undefined && data.FormId !== ""))?true:false;'
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
      url: '{{ location.protocol }}//{{window.location.hostname}}:{{window.location.port}}/api/services/app/Forms/GetFormsList'//'/form?limit=4294967295&select=_id,title'
    },
    template: '<span>{{ item.label }}</span>',
    valueProperty: 'value',
    authenticate: true,
    label: 'Form',
    labelName: '',
    key: 'FormId',
    weight: 10,
    validate: {
      required: true,
      customMessage: ''
    },
    tooltip: 'The form to load within this form component.'
  },
  {
    label: 'Field Name Substitute ',
    showSummary: false,
    key: 'FNSubstitute',
    tooltip:'When reusing a form block sometimes you need to change the field names. An example is when you have a form part you have made to capture client details. You would like to use the same form block for suppliers but the field names are wrong. In the text boxes below enter "Client" in the from box and "Supplier" in the to box. This means that the embedded form will change "client" to "supplier" where it finds it in the embedded form.',
    addAnother: '',
    minrows: 1,
    maxLength: 10,
    dividertitle: '',
    widthslider: '12',
    offsetslider: '0',
    type: 'section',
    input: true,
    tableView: true,
    components: [{
      label: 'SectionPanel',
      collapsible: false,
      mask: false,
      tableView: false,
      alwaysEnabled: false,
      type: 'sectionpanel',
      input: true,
      key: 'FNSubstitute_panel',
      components: [{
        label: 'Pattern',
        showSummary: false,
        key: 'FNPattern',
        FieldasRecordName: false,
        defaultValue: '',
        DoNotLoadFromRecord: false,
        showWordCount: false,
        showCharCount: false,
        widthslider: '6',
        offsetslider: '0',
        type: 'sfatextfield',
        input: true,
        tableView: true
      },
      {
        label: 'Replacement',
        showSummary: false,
        key: 'FNReplacement',
        FieldasRecordName: false,
        defaultValue: '',
        DoNotLoadFromRecord: false,
        showWordCount: false,
        showCharCount: false,
        widthslider: '6',
        offsetslider: '0',
        type: 'sfatextfield',
        input: true,
        tableView: true
      }
      ],
      row: '0-0'
    }
    ]
  },
  //{
  //  type: 'textfield',
  //  label: 'Form URL',
  //  key: 'formurl',
  //  tooltip: 'Form URL',
  //  input: true
  //},
  {
    type: 'checkbox',
    input: true,
    weight: 20,
    key: 'reference',
    label: 'Save as reference',
    tooltip: 'Using this option will save this field as a reference and link its value to the value of the origin record.'
  },
  //{
  //  weight: 180,
  //  type: 'slider',
  //  label: 'Width in Columns',
  //  key: 'widthslider',
  //  tooltip: 'Columns\'s width',
  //  defaultValue: 12,
  //  minValue: 1,
  //  maxValue: 12,
  //  step: 1
  //},
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
