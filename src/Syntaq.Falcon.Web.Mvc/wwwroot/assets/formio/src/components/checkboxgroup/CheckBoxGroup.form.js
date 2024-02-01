import baseEditForm from '../base/Base.form';

import CheckboxGroupEditDisplay from './editForm/CheckboxGroup.edit.display';
import CheckboxGroupEditData from './editForm/CheckboxGroup.edit.data';
import CheckboxGroupEditValidation from './editForm/CheckboxGroup.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: CheckboxGroupEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: CheckboxGroupEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: CheckboxGroupEditValidation,
        weight: 3,
        ignore: false
      },
      {
        key: 'display',
        ignore: true
      },
      {
        key: 'data',
        ignore: true
      },
      {
        key: 'validation',
        ignore: true
      },
      {
        key: 'api',
        components: '',
        ignore: true
      },
      {
        key: 'conditional',
        components: '',
        ignore: true
      },
      {
        key: 'logic',
        components: '',
        weight: 4,
        ignore: false
      }
    ], ...extend);
}
