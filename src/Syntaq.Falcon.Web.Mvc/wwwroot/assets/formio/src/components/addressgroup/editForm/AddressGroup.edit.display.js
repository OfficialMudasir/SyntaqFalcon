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
    weight: 20,
    type: 'textarea',
    input: true,
    key: 'tooltip',
    label: 'Tooltip',
    placeholder: 'To add a tooltip to this field, enter text here.',
    tooltip: 'Adds a tooltip to the side of this field.'
  },
  {
    type: 'divider',
    key: 'divider-1',
    dividerstyle: 'style1',
    weight: 30,
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Label for Care of (C/-)',
    key: 'cclabel',
    tooltip: 'Label for Care of (C/-)',
    defaultValue: 'Care of (C/-)',
    weight: 40,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Placehoder for Care of (C/-)',
    placeholder: 'Placehoder',
    key: 'ccPlacehoder',
    tooltip: 'Placehoder for Care of (C/-)',
    weight: 45,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidecc',
    label: 'Hide Care of (C/-)',
    tooltip: 'Hide this field',
    weight: 50,
  },
  {
    type: 'divider',
    key: 'divider-2',
    dividerstyle: 'style3',
    weight: 60
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Label for Bldg, Floor, Lvl',
    key: 'lvllabel',
    tooltip: 'Label for Bldg, Floor, Lvl',
    defaultValue: 'Bldg, Floor, Lvl',
    weight: 70,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Placehoder for Bldg, Floor, Lvl',
    placeholder: 'Placehoder',
    key: 'lvlPlacehoder',
    tooltip: 'Placehoder for Bldg, Floor, Lvl',
    weight: 75,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidelvl',
    label: 'Hide Bldg, Floor, Lvl',
    tooltip: 'Hide this field',
    weight: 80
  },
  {
    type: 'divider',
    key: 'divider-3',
    dividerstyle: 'style3',
    weight: 90
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Label for Street No & Name (or Extended address)',
    key: 'streetlabel',
    tooltip: 'Label for Street No & Name (or Extended address)',
    defaultValue: 'Street No & Name (or Extended address)',
    weight: 100,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Placehoder for Street No & Name',
    placeholder: 'Placehoder',
    key: 'streetPlacehoder',
    tooltip: 'Placehoder for Street No & Name',
    weight: 105,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidestreet',
    label: 'Hide Street No & Name (or Extended address)',
    tooltip: 'Hide this field',
    widthslider: '6',
    offsetslider: '0',
    weight: 110
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'requiredstreet',
    label: 'Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
    widthslider: '6',
    offsetslider: '0',
    weight: 110
  },
  {
    type: 'divider',
    key: 'divider-4',
    dividerstyle: 'style3',
    weight: 120
  },

  {
    type: 'sfatextfield',
    input: true,
    label: 'Label for Suburb / City',
    key: 'suburblabel',
    tooltip: 'Label for Suburb / City',
    defaultValue: 'Suburb / City',
    weight: 130,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Placehoder for Suburb / City',
    placeholder: 'Placehoder',
    key: 'suburbPlacehoder',
    tooltip: 'Placehoder for Suburb / City',
    weight: 135,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidesuburb',
    label: 'Hide Suburb / City',
    tooltip: 'Hide this field',
    widthslider: '6',
    offsetslider: '0',
    weight: 140
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'requiredsuburb',
    label: 'Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
    widthslider: '6',
    offsetslider: '0',
    weight: 140
  },
  {
    type: 'divider',
    key: 'divider-5',
    dividerstyle: 'style3',
    weight: 150
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Label for State',
    key: 'statelabel',
    tooltip: 'Label for State',
    defaultValue: 'State',
    weight: 160,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Placehoder for State',
    placeholder: 'Placehoder',
    key: 'statePlacehoder',
    tooltip: 'Placehoder for State',
    weight: 165,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidestate',
    label: 'Hide State',
    tooltip: 'Hide this field',
    widthslider: '6',
    offsetslider: '0',
    weight: 170
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'requiredstate',
    label: 'Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
    widthslider: '6',
    offsetslider: '0',
    weight: 170
  },
  {
    type: 'divider',
    key: 'divider-6',
    dividerstyle: 'style3',
    weight: 180
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Label for Zip / PostCode',
    key: 'postcodelabel',
    tooltip: 'Label for Zip / PostCode',
    defaultValue: 'Zip / PostCode',
    weight: 190,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfatextfield',
    input: true,
    label: 'Placehoder for Zip / PostCode',
    placeholder: 'Placehoder',
    key: 'postcodePlacehoder',
    tooltip: 'Placehoder for Zip / PostCode',
    weight: 165,
    widthslider: '6',
    offsetslider: '0',
    logic: []
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidepostcode',
    label: 'Hide Zip / PostCode',
    tooltip: 'Hide this field',
    widthslider: '6',
    offsetslider: '0',
    weight: 200
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'requiredpostcode',
    label: 'Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
    widthslider: '6',
    offsetslider: '0',
    weight: 200
  },
  {
    type: 'divider',
    key: 'divider-7',
    dividerstyle: 'style3',
    weight: 210
  },
  {
    type: 'textfield',
    input: true,
    label: 'Label for Country',
    key: 'countrylabel',
    tooltip: 'Label for Country',
    defaultValue: 'Country',
    weight: 220
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidecountry',
    label: 'Hide Country',
    tooltip: 'Hide this field',
    widthslider: '6',
    offsetslider: '0',
    weight: 230
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'requiredcountry',
    label: 'Required',
    tooltip: 'A required field must be filled in before the form can be submitted.',
    widthslider: '6',
    offsetslider: '0',
    weight: 230
  },
  {
    type: 'divider',
    key: 'divider-8',
    dividerstyle: 'style3',
    weight: 240
  },
  {
    type: 'sfacheckbox',
    input: true,
    key: 'hidefulladdress',
    label: 'Hide Full address',
    tooltip: 'Hide Full address field',
    weight: 250
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
    weight: 260
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
    weight: 270
  },
  {
    weight: 280,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
