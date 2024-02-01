import baseEditForm from '../base/Base.form';

import AddressGroupEditDisplay from './editForm/AddressGroup.edit.display';
import AddressGroupEditValidation from './editForm/AddressGroup.edit.validation';
import AddressGroupEditData from './editForm/AddressGroup.edit.data';

export default function() {
  return baseEditForm([
    {
      label: 'Display',
      key: 'sfadisplay',
      components: AddressGroupEditDisplay,
      weight: 1,
      ignore: false
    },
    {
      label: 'Data',
      key: 'sfadata',
      components: AddressGroupEditData,
      weight: 2,
      ignore: false
    },
    {
      label: 'Validation',
      key: 'sfavalidation',
      components: AddressGroupEditValidation,
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
      ignore: false
    }
  ]);
}
