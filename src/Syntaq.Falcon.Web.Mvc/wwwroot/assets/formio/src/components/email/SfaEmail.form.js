import baseEditForm from '../base/Base.form';

import SfaEmailEditDisplay from './editForm/SfaEmail.edit.display';
import SfaEmailEditData from './editForm/SfaEmail.edit.data';
import SfaEmailEditValidation from './editForm/SfaEmail.edit.validation';

export default function() {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaEmailEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaEmailEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaEmailEditValidation,
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
        ignore: false
      }
    ]);
}
