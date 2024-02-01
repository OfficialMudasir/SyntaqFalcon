export default [
  {
    type: 'textfield',
    label: 'Default Value',
    key: 'defaultValue',
    weight: 5,
    placeholder: 'Default Value',
    tooltip: 'The will be the value for this field, before user interaction. Having a default value will override the placeholder text.',
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
