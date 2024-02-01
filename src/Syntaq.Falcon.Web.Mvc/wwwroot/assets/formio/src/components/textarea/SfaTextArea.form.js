import baseEditForm from '../base/Base.form';

import SfaTextAreaEditDisplay from './editForm/SfaTextArea.edit.display';
import SfaTextAreaEditData from './editForm/SfaTextArea.edit.data';
import SfaTextAreaEditValidation from './editForm/SfaTextArea.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaTextAreaEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaTextAreaEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaTextAreaEditValidation,
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
