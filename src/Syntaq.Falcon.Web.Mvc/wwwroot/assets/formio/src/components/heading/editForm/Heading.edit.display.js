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
    type: 'textfield',
    input: true,
    key: 'heading',
    weight: 0,
    label: 'Heading',
    placeholder: 'Input a heading',
    tooltip: 'The heading element.'
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
    weight: 20,
    type: 'number',
    label: 'Font Size',
    key: 'fontsize',
    tooltip: 'Font Size in px',
    defaultValue: 23,
    input: true,
//    inputType: 'number',
    //validate: {
    //  min: 1,
    //  max: 300,
    //  step:1
    //}
  },
  {
    weight: 30,
    type: 'checkbox',
    label: 'Bold',
    key: 'bold',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 40,
    type: 'checkbox',
    label: 'Italic',
    key: 'italic',
    customClass: 'form-check-inline',
    input: true
  },
  {
    weight: 50,
    type: 'checkbox',
    label: 'Underline',
    key: 'underline',
    customClass: 'form-check-inline',
    input: true
  },
  {
    type: 'textfield',
    input: true,
    key: 'fontcolour',
    weight: 60,
    label: 'Font Colour',
    placeholder: 'Input a RGB Colour',
    tooltip: 'Font Colour.'
  },
  {
    type: 'textfield',
    input: true,
    key: 'backcolour',
    weight: 70,
    label: 'Background Colour',
    placeholder: 'Input a RGB Colour',
    tooltip: 'Background Colour.'
  },
  {
    type: 'sfaselect',
    input: true,
    weight: 80,
    key: 'fontfamily',
    hideLabel: false,
    label: 'Font Family',
    dataSrc: 'values',
    searchEnabled: false,
    placeholder: 'Pick a font family',
    data: {
      values: [
        { label: '<p style="font-family: Arial;">Arial</p>', value: 'Arial, Arial, Helvetica, sans-serif' },
        { label: '<p style="font-family: Arial Black;">Arial Black</p>', value: '"Arial Black", "Arial Black", Gadget, sans-serif' },
        { label: '<p style="font-family: Comic Sans MS;">Comic Sans MS</p>', value: '"Comic Sans MS", "Comic Sans MS", cursive' },
        { label: '<p style="font-family: Courier New;">Courier New</p>', value: '"Courier New", "Courier New", Courier, monospace' },
        { label: '<p style="font-family: Georgia;">Georgia</p>', value: 'Georgia, Georgia, serif' },
        { label: '<p style="font-family: Impact;">Impact</p>', value: 'Impact, Charcoal, sans-serif' },
        { label: '<p style="font-family: Monaco;">Lucida Console</p>', value: '"Lucida Console", Monaco, monospace' },
        { label: '<p style="font-family: Lucida Grande;">Lucida Sans</p>', value: '"Lucida Sans Unicode", "Lucida Grande", sans-serif' },
        { label: '<p style="font-family: Palatino;">Palatino Linotype</p>', value: '"Palatino Linotype", "Book Antiqua", Palatino, serif' },
        { label: '<p style="font-family: Times;">Times New Roman</p>', value: '"Times New Roman", Times, serif' },
        { label: '<p style="font-family: Tahoma;">Tahoma</p>', value: 'Tahoma, Geneva, sans-serif' },
        { label: '<p style="font-family: Helvetica;">Trebuchet MS</p>', value: '"Trebuchet MS", Helvetica, sans-serif' },
        { label: '<p style="font-family: Verdana;">Verdana</p>', value: 'Verdana, Geneva, sans-serif' },
        { label: '<p style="font-family: Geneva;">Gill Sans</p>', value: '"Gill Sans", Geneva, sans-serif' }
      ]
    },
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
    weight: 90
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
    weight: 100
  },
  {
    weight: 110,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
