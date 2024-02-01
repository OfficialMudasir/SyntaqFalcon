import baseEditForm from '../base/Base.form';

import SfaEditGridEditDisplay from './editForm/SfaEditGrid.edit.display';
//import SfaEditGridEditData from './editForm/SfaEditGrid.edit.data';
import SfaEditGridEditValidation from './editForm/SfaEditGrid.edit.validation';

export default function() {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaEditGridEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        //components: SfaEditGridEditData,
        //weight: 2,
        ignore: true
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaEditGridEditValidation,
        weight: 2,
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
        components: '',
        weight: 3,
        ignore: false
      }
    ]);
}
