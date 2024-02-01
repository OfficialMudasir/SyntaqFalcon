export default [
  {
    type: 'textfield',
    label: 'Default Value',
    key: 'defaultValue',
    weight: 100,
    placeholder: 'Default Value',
    tooltip: 'The will be the value for this field, before user interaction. Having a default value will override the placeholder text.',
    input: true
  },
  {
    weight: 100,
    type: 'sfacheckbox',
    label: 'Reset Default Value',
    tooltip: 'Reset Default Value as empty',
    key: 'ResetDV',
    customClass: 'form-check-inline',
    input: true,
    logic: [
      {
        name: 'resetC',
        trigger: {
          type: 'javascript',
          javascript: 'result = (data["ResetDV"] && form.components[0].components[1].components[0].defaultValue); '
        },
        actions: [
          {
            name: 'act',
            type: 'value',
            value: 'row.defaultValue = "";'
          }
        ]
      }
    ]
  },
  {
    weight: 20,
    type: 'htmlelement',
    input: false,
    content: '<div style="clear: both;"></div>',
  },
  {
    type: 'datagrid',
    input: true,
    label: 'Values',
    key: 'values',
    tooltip: 'The radio button values that can be picked for this field. Values are text submitted with the form data. Labels are text that appears next to the radio buttons on the form.',
    weight: 33,
    defaultValue: [{ label: 'Yes', value: 'true', mtext: '' },
      { label: 'No', value: 'false', mtext: '' }],
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
        calculateValue: { _camelCase: [{ var: 'row.value' }] }
      }
    ]
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
