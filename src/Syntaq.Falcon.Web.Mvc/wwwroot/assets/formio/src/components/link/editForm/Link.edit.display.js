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
    weight: 10,
    type: 'textfield',
    input: true,
    key: 'label',
    label: 'Label',
    placeholder: 'Field Label',
    tooltip: 'The label for this field that will appear next to it.'
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
    type: 'select',
    input: true,
    weight: 30,
    //        tooltip: 'The source to use for the select data. Values lets you provide your own values and labels. JSON lets you provide raw JSON data. URL lets you provide a URL to retrieve the JSON data from.',
    key: 'header',
    defaultValue: 'http://',
    label: 'Link Protocol',
    dataSrc: 'values',
    searchEnabled:false,
    data: {
      values: [
        { label: 'Http', value: 'http://' },
        { label: 'Https', value: 'https://' },
        { label: 'Mailto', value: 'mailto:' }
      ]
    },
//    widthslider: 3
  },
  {
    key: 'url',
    input: true,
    type: 'sfatextfield',
    label: 'Link Address',
  },
  //{
  //  type: 'url',
  //  input: true,
  //  key: 'weburl',
  //  weight: 10,
  //  label: 'Link URL',
  //  placeholder: 'Enter a link URL with http:// or https://',
  //  tooltip: 'Enter a link URL.',
  //  validate: {
  //    customMessage:'Please input http:// or https:// at the beginng of the URL',
  //    pattern:'^((https|http|ftp|rtsp|mms)?:\\/\\/)[^\\s]+'
  //  }
  //},
//  {
//    type: 'sfapanel',
//    hideLabel: true,
//    input: true,
//    key: 'linkdata',
//    weight: 20,
////    customClass: 'col',
////    style: { 'float': 'left', 'height': 'min - content' },
//    components: [
//      {
//        type: 'sfaselect',
//        input: true,
//        weight: 0,
////        tooltip: 'The source to use for the select data. Values lets you provide your own values and labels. JSON lets you provide raw JSON data. URL lets you provide a URL to retrieve the JSON data from.',
//        key: 'header',
//        defaultValue: 'http://',
//        hideLabel: true,
//        label: 'Link URL',
//        dataSrc: 'values',
//        data: {
//          values: [
//            { label: 'HTTP://', value: 'http://' },
//            { label: 'HTTPS://', value: 'https://' },
//            { label: 'Mailto:', value: 'mailto:' }
//          ]
//        },
//        widthslider: 3
//      },
//      {
//        hideLabel:true,
//        key: 'url',
//        input: true,
//        type: 'sfatextfield',
//        widthslider: 9
//      }
//    ]
//  },
  {
    weight: 90,
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
    weight: 100,
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
    weight: 110,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
