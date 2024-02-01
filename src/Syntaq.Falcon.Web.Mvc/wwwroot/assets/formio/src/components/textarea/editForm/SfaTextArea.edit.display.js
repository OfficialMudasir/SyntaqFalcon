import _ from 'lodash';
import Formio from '../../../Formio';

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
    type: 'select',
    input: true,
    key: 'labelPosition',
    label: 'Label Position',
    tooltip: 'Position for the label for this field.',
    weight: 20,
    defaultValue: 'top',
    dataSrc: 'values',
    data: {
      values: [
        { label: 'Top', value: 'top' },
        { label: 'Left (Left-aligned)', value: 'left-left' },
        { label: 'Left (Right-aligned)', value: 'left-right' },
        { label: 'Right (Left-aligned)', value: 'right-left' },
        { label: 'Right (Right-aligned)', value: 'right-right' },
        { label: 'Bottom', value: 'bottom' }
      ]
    }
  },
  {
    type: 'number',
    input: true,
    key: 'labelWidth',
    label: 'Label Width',
    tooltip: 'The width of label on line in percentages.',
    clearOnHide: false,
    weight: 30,
    placeholder: '30',
    suffix: '%',
    validate: {
      min: 0,
      max: 100
    },
    conditional: {
      json: {
        and: [
          { '!==': [{ var: 'data.labelPosition' }, 'top'] },
          { '!==': [{ var: 'data.labelPosition' }, 'bottom'] },
        ]
      }
    }
  },
  {
    type: 'number',
    input: true,
    key: 'labelMargin',
    label: 'Label Margin',
    tooltip: 'The width of label margin on line in percentages.',
    clearOnHide: false,
    weight: 40,
    placeholder: '3',
    suffix: '%',
    validate: {
      min: 0,
      max: 100
    },
    conditional: {
      json: {
        and: [
          { '!==': [{ var: 'data.labelPosition' }, 'top'] },
          { '!==': [{ var: 'data.labelPosition' }, 'bottom'] },
        ]
      }
    }
  },
  {
    weight: 60,
    type: 'checkbox',
    label: 'Read Only',
    tooltip: 'Disable the form input.',
    key: 'disabled',
    customClass: 'form-check-inline',
    input: true
  },
  //{
  //  weight: 60,
  //  type: 'checkbox',
  //  label: 'Field as Record Name',
  //  tooltip: 'Use this feild\'s name as the Record Name.',
  //  key: 'FieldasRecordName',
  //  customClass: 'form-check-inline',
  //  input: true
  //},
  {
    weight: 60,
    type: 'sfahtmlelement',
    input: false,
    content: '<div></div>',
    widthslider: 12,
    offsetslider: 0
  },
  {
    type: 'sfatextfield',
    label: 'Default Value',
    key: 'defaultValue',
    weight: 70,
    placeholder: 'Default Value',
    tooltip: 'The will be the value for this field, before user interaction. Having a default value will override the placeholder text.',
    input: true
  },
  {
    weight: 70,
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>'
  },
  {
    weight: 80,
    type: 'checkbox',
    label: 'Do not load this field from saved record',
    tooltip: 'Keeps field data fresh by not loading previously saved data.',
    key: 'DoNotLoadFromRecord',
    customClass: 'form-check-inline',
    input: true
  },
  {
    type: 'number',
    input: true,
    key: 'rows',
    label: 'Rows',
    weight: 90,
    tooltip: 'This allows control over how many rows are visible in the text area.',
    placeholder: 'Enter the amount of rows'
  },
  {
    weight: 100,
    type: 'textfield',
    input: true,
    key: 'placeholder',
    label: 'Placeholder',
    placeholder: 'Placeholder',
    tooltip: 'The placeholder text that will appear when this field is empty.'
  },
  {
    weight: 110,
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
    key: 'editor',
    label: 'Editor',
    tooltip: 'Select the type of WYSIWYG editor to use for this text area.',
    dataSrc: 'values',
    data: {
      values: [
        { label: 'None', value: 'text' },
        //{ label: 'Quill', value: 'quill' },
        { label: 'CKEditor', value: 'ckeditor' }//,
        //{ label: 'TinyMCE', value: 'tinymce' },
        //{ label: 'ACE', value: 'ace' }
      ]
    },
    weight: 120
  },
  {
    type: 'checkbox',
    input: true,
    key: 'isUploadEnabled',
    label: 'Enable Image Upload',
    weight: 120.1,
    conditional: {
      json: {
        or: [
          {
            '===': [
              { var: 'data.editor' },
              'quill'
            ]
          },
          {
            '==': [
              { var: 'data.editor' },
              ''
            ]
          }
        ]
      }
    }
  },
  {
    type: 'select',
    input: true,
    key: 'uploadStorage',
    label: 'Image Upload Storage',
    placeholder: 'Select your file storage provider',
    weight: 120.2,
    tooltip: 'Which storage to save the files in.',
    valueProperty: 'value',
    dataSrc: 'custom',
    data: {
      custom() {
        return _.map(Formio.providers.storage, (storage, key) => ({
          label: storage.title,
          value: key
        }));
      }
    },
    conditional: {
      json: {
        '===': [
          { var: 'data.isUploadEnabled' },
          true
        ]
      }
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'uploadUrl',
    label: 'Image Upload Url',
    weight: 120.3,
    placeholder: 'Enter the url to post the files to.',
    tooltip: 'See <a href=\'https://github.com/danialfarid/ng-file-upload#server-side\' target=\'_blank\'>https://github.com/danialfarid/ng-file-upload#server-side</a> for how to set up the server.',
    conditional: {
      json: { '===': [{ var: 'data.uploadStorage' }, 'url'] }
    }
  },
  {
    type: 'textarea',
    key: 'uploadOptions',
    label: 'Image Upload Custom request options',
    tooltip: 'Pass your custom xhr options(optional)',
    rows: 5,
    editor: 'ace',
    input: true,
    weight: 120.4,
    placeholder: `{
      "withCredentials": true
    }`,
    conditional: {
      json: {
        '===': [{
          var: 'data.uploadStorage'
        }, 'url']
      }
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'uploadDir',
    label: 'Image Upload Directory',
    placeholder: '(optional) Enter a directory for the files',
    tooltip: 'This will place all the files uploaded in this field in the directory',
    weight: 120.5,
    conditional: {
      json: {
        '===': [
          { var: 'data.isUploadEnabled' },
          true
        ]
      }
    }
  },
  {
    type: 'select',
    input: true,
    key: 'as',
    label: 'Save As',
    dataSrc: 'values',
    tooltip: 'This setting determines how the value should be entered and stored in the database.',
    clearOnHide: true,
    data: {
      values: [
        { label: 'String', value: 'string' },
        { label: 'JSON', value: 'json' },
        { label: 'HTML', value: 'html' }
      ]
    },
    conditional: {
      json: {
        or: [
          {
            '===': [
              { var: 'data.editor' },
              'quill'
            ]
          },
          {
            '===': [
              { var: 'data.editor' },
              'ace'
            ]
          }
        ]
      }
    },
    weight: 130
  },
  {
    type: 'textarea',
    input: true,
    editor: 'ace',
    rows: 10,
    as: 'json',
    label: 'Editor Settings',
    tooltip: 'Enter the WYSIWYG editor JSON configuration.',
    key: 'wysiwyg',
    customDefaultValue(value, component, row, data, instance) {
      return instance.wysiwygDefault;
    },
    conditional: {
      json: {
        or: [
          {
            '===': [
              { var: 'data.editor' },
              'quill'
            ]
          },
          {
            '===': [
              { var: 'data.editor' },
              'ace'
            ]
          }
        ]
      }
    },
    weight: 140
  },
  {
    weight: 150,
    type: 'textfield',
    input: true,
    key: 'errorLabel',
    label: 'Error Label',
    placeholder: 'Error Label',
    tooltip: 'The label for this field when an error occurs.'
  },
  {
    weight: 160,
    type: 'textfield',
    input: true,
    key: 'customClass',
    label: 'Custom CSS Class',
    placeholder: 'Custom CSS Class',
    tooltip: 'Custom CSS class to add to this component.'
  },
  {
    weight: 170,
    type: 'checkbox',
    customClass: 'form-check-inline',
    input: true,
    key: 'showWordCount',
    label: 'Show Word Counter'
  },
  {
    weight: 170,
    type: 'checkbox',
    customClass: 'form-check-inline',
    input: true,
    key: 'showCharCount',
    label: 'Show Character Counter'
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
    weight: 200,
    type: 'textfield',
    input: true,
    key: 'tabindex',
    label: 'Tab Index',
    placeholder: 'Tab Index',
    tooltip: 'Sets the tabindex attribute of this component to override the tab order of the form. See the <a href=\\\'https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/tabindex\\\'>MDN documentation</a> on tabindex for more information.'
  }
];
