import baseEditForm from '../base/Base.form';

import ImportantFlagsEditDisplay from './editForm/ImportantFlags.edit.display';
import ImportantFlagsEditData from './editForm/ImportantFlags.edit.data';
import ImportantFlagsEditValidation from './editForm/ImportantFlags.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: ImportantFlagsEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: ImportantFlagsEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: ImportantFlagsEditValidation,
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
