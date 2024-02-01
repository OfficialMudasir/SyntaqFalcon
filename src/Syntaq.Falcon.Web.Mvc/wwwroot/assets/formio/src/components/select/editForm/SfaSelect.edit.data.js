//import { eachComponent } from '../../../utils/utils';

export default [
  {
    type: 'select',
    input: true,
    weight: 0,
    tooltip: 'The source to use for the select data. Values lets you provide your own values and labels. JSON lets you provide raw JSON data. URL lets you provide a URL to retrieve the JSON data from.',
    key: 'dataSrc',
    defaultValue: 'value',
    label: 'Data Source Type',
    dataSrc: 'values',
    data: {
      values: [
        // { label: 'Values', value: 'values' },
        { label: 'Value', value: 'value' },
        { label: 'Raw JSON', value: 'json' },
        { label: 'URL', value: 'url' },
        { label: 'Custom', value: 'custom' }
      ]
    }
  },
  {
    type: 'textarea',
    as: 'json',
    editor: 'ace',
    weight: 10,
    input: true,
    key: 'data.json',
    label: 'Data Source Raw JSON',
    tooltip: 'A raw JSON array to use as a data source.',
    placeholder: '[{ "label": "label1","value": "value1","Mtext": "aaa"},{ "label": "label2","value": "value2","Mtext": "bbb"}]',
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'json'] }
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'data.url',
    weight: 10,
    label: 'Data Source URL',
    placeholder: 'Data Source URL',
    tooltip: 'A URL that returns a JSON array to use as the data source.',
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'url'] }
    }
  },
  {
    type: 'checkbox',
    input: true,
    label: 'Lazy Load URL',
    key: 'lazyLoad',
    tooltip: 'When set, this will not fire off the request to the URL until this control is within focus. This can improve performance if you have many Select dropdowns on your form where the API\'s will only fire when the control is activated.',
    weight: 11,
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'url'] }
    }
  },
  {
    type: 'datagrid',
    input: true,
    label: 'Request Headers',
    key: 'data.headers',
    tooltip: 'Set any headers that should be sent along with the request to the url. This is useful for authentication.',
    weight: 11,
    components: [
      {
        label: 'Key',
        key: 'key',
        input: true,
        type: 'textfield'
      },
      {
        label: 'Value',
        key: 'value',
        input: true,
        type: 'textfield'
      }
    ],
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'url'] }
    }
  },
  {
    type: 'textfield',
    input: true,
    label: 'Data Path',
    key: 'selectValues',
    weight: 12,
    description: 'The object path to the iterable items.',
    tooltip: 'The property within the source data, where iterable items reside. For example: results.items or results[0].items',
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'url'] }
    }
  },
  {
    type: 'checkbox',
    input: true,
    key: 'disableLimit',
    label: 'Disable limiting response',
    tooltip: 'When enabled the request will not include the limit and skip options in the query string',
    weight: 15,
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'url'] }
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'searchField',
    label: 'Search Query Name',
    weight: 16,
    description: 'Name of URL query parameter',
    tooltip: 'The name of the search querystring parameter used when sending a request to filter results with. The server at the URL must handle this query parameter.',
    conditional: {
      json: {
        or: [
          { '===': [{ var: 'data.dataSrc' }, 'url'] },
          { '===': [{ var: 'data.dataSrc' }, 'resource'] }
        ]
      }
    }
  },
  {
    type: 'textfield',
    input: true,
    key: 'filter',
    label: 'Filter Query',
    weight: 18,
    description: 'The filter query for results.',
    tooltip: 'Use this to provide additional filtering using query parameters.',
    conditional: {
      json: {
        or: [
          { '===': [{ var: 'data.dataSrc' }, 'url'] },
          { '===': [{ var: 'data.dataSrc' }, 'resource'] }
        ]
      }
    }
  },
  {
    type: 'number',
    input: true,
    key: 'limit',
    label: 'Limit',
    weight: 18,
    description: 'Maximum number of items to view per page of results.',
    tooltip: 'Use this to limit the number of items to request or view.',
    conditional: {
      json: {
        or: [
          { '===': [{ var: 'data.dataSrc' }, 'url'] },
          { '===': [{ var: 'data.dataSrc' }, 'resource'] },
          { '===': [{ var: 'data.dataSrc' }, 'json'] }
        ]
      }
    }
  },
  {
    type: 'textarea',
    input: true,
    key: 'template',
    label: 'Item Template',
    editor: 'ace',
    as: 'html',
    rows: 3,
    weight: 18,
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'url'] }
    },
    tooltip: 'The HTML template for the result data items.'
  },
  {
    type: 'datagrid',
    input: true,
    label: 'Data Source Values',
    key: 'data.value',
    tooltip: 'Values to use as the data source. Labels are shown in the select field. Values are the corresponding values saved with the submission.',
    weight: 10,
    components: [
      {
        label: 'Label',
        key: 'label',
        input: true,
        type: 'textfield'
      },
      {
        label: 'Value',
        key: 'value',
        input: true,
        type: 'textfield',
        allowCalculateOverride: true,
        calculateValue: { _camelCase: [{ var: 'row.label' }] }
      },
      {
        label: 'Mtext',
        key: 'mtext',
        input: true,
        type: 'textfield',
        allowCalculateOverride: true,
        calculateValue: { _camelCase: [{ var: 'row.label' }] }
      }
    ],
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'value'] }
    }
  },
  {
    type: 'textarea',
    input: true,
    key: 'data.custom',
    label: 'Custom Values',
    editor: 'ace',
    rows: 10,
    weight: 14,
    placeholder: "values = data['mykey'];",
    tooltip: 'Write custom code to return the value options. The form data object is available.',
    conditional: {
      json: { '===': [{ var: 'data.dataSrc' }, 'custom'] }
    }
  },
];
