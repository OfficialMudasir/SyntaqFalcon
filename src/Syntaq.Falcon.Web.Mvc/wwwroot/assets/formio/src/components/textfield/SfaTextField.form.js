import baseEditForm from '../base/Base.form';

import SfaTextFieldEditDisplay from './editForm/SfaTextField.edit.display';
import SfaTextFieldEditData from './editForm/SfaTextField.edit.data';
import SfaTextFieldEditValidation from './editForm/SfaTextField.edit.validation';

export default function(...extend) {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: SfaTextFieldEditDisplay,
      weight: 1,
      ignore: false
    },
    {
      label: 'Data',
      key: 'sfadata',
      components: SfaTextFieldEditData,
      weight: 2,
      ignore: false
    },
    {
      label: 'Validation',
      key: 'sfavalidation',
      components: SfaTextFieldEditValidation,
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
