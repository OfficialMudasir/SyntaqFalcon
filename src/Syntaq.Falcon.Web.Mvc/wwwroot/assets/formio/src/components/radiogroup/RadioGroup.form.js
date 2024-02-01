import baseEditForm from '../base/Base.form';

import RadioGroupEditDisplay from './editForm/RadioGroup.edit.display';
import RadioGroupEditData from './editForm/RadioGroup.edit.data';
import RadioGroupEditValidation from './editForm/RadioGroup.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: RadioGroupEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: RadioGroupEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: RadioGroupEditValidation,
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
