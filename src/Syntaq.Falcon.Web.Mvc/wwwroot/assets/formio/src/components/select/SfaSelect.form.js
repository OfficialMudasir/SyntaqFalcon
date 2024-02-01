import baseEditForm from '../base/Base.form';

import SfaSelectEditDisplay from './editForm/SfaSelect.edit.display';
import SfaSelectEditData from './editForm/SfaSelect.edit.data';
import SfaSelectEditValidation from './editForm/SfaSelect.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaSelectEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaSelectEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaSelectEditValidation,
        weight: 3,
        ignore: false
      },
      {
        key: 'display',
        components: '',
        ignore: true
      },
      {
        key: 'data',
        components: '',
        ignore: true
      },
      {
        key: 'validation',
        components: '',
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
