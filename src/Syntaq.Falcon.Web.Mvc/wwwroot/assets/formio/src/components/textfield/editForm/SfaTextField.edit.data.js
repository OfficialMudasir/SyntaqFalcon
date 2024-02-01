export default [
  {
    type: 'select',
    label: 'Input Format',
    key: 'inputFormat',
    weight: 0,
    placeholder: 'Input Format',
    tooltip: '',
    template: '<span>{{ item.label }}</span>',
    data: {
      values: [
        {
          value: 'plain',
          label: 'Plain'
        },
        {
          value: 'html',
          label: 'HTML'
        }, {
          value: 'raw',
          label: 'Raw (Insecure)'
        }
      ]
    },
    defaultValue: 'plain',
    input: true
  },
  {
    type: 'select',
    input: true,
    key: 'refreshOn',
    label: 'Refresh On',
    weight: 10,
    tooltip: 'Refresh data when another field changes.',
    dataSrc: 'custom',
    valueProperty: 'value',
    data: {
      custom: `
        values.push({label: 'Any Change', value: 'data'});
        utils.eachComponent(instance.root.editForm.components, function(component, path) {
          if (component.key !== data.key) {
            values.push({
              label: component.label || component.key,
              value: path
            });
          }
        });
      `
    }
  }
];
