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
    key: 'title',
    weight: 0,
    label: 'Title',
    placeholder: 'Title',
    tooltip: 'The Title for this Notes/Help.'
  },
  {
    type: 'sfatextarea',
    editor: 'tinymce',
    input: true,
    as: 'html',
    key: 'htmlcontent',
    label: 'Notes/Help',
    tooltip: 'Enter the content for this custom element.',
    placeholder: 'Notes or help content',
    weight: 20
  },
  {
    type: 'checkbox',
    input: true,
    key: 'popuphelp',
    label: 'Popup Dialog on click?',
    tooltip: 'Popup Dialog on click?',
    customClass: 'form-check-inline',
    weight: 30
  },
  {
    type: 'checkbox',
    input: true,
    key: 'hidecontent',
    label: 'Hide Notes/Help content',
    tooltip: 'Hide the Notes/Help content',
    customClass: 'form-check-inline',
    weight: 30,
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
    weight: 40
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
    weight: 50
  },
  {
    weight: 60,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
