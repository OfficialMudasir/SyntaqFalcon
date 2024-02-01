import baseEditForm from '../base/Base.form';

import SfaNumberEditDisplay from './editForm/SfaNumber.edit.display';
import SfaNumberEditData from './editForm/SfaNumber.edit.data';
import SfaNumberEditValidation from './editForm/SfaNumber.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaNumberEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaNumberEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaNumberEditValidation,
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
        ignore: true
      },
      {
        key: 'conditional',
        ignore: true
      },
      {
        key: 'logic',
        weight: 4,
        ignore: false
      }
    ], ...extend);
}
