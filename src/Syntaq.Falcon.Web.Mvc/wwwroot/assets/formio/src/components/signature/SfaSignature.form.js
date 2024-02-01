import baseEditForm from '../base/Base.form';

import SfaSignatureEditDisplay from './editForm/SfaSignature.edit.display';
import SfaSignatureEditValidation from './editForm/SfaSignature.edit.validation';

export default function() {
  return baseEditForm([
    {
      label: 'Display',
      key: 'sfadisplay',
      components: SfaSignatureEditDisplay,
      weight: 1,
      ignore: false
    },
    {
      key: 'conditional',
      components: '',
      weight: 2,
      ignore: true
    },
    {
      label: 'Validation',
      key: 'sfavalidation',
      components: SfaSignatureEditValidation,
      weight: 2,
      ignore: false
    },
    {
      key: 'validation',
      weight: 2,
      ignore: true
    },
    {
      key: 'logic',
      components: '',
      weight: 4,
      ignore: false
    },
    {
      key: 'display',
      ignore: true
    },
    {
      key: 'data',
      weight: 2,
      ignore: true
    },
    {
      key: 'api',
      components: '',
      ignore: true
    }
  ]);
}
