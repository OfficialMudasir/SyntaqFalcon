import baseEditForm from '../base/Base.form';

import PersonEditDisplay from './editForm/Person.edit.display';
import PersonEditData from './editForm/Person.edit.data';
import PersonEditValidation from './editForm/Person.edit.validation';

export default function() {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: PersonEditDisplay,
      weight: 1,
      ignore: false
    },
    {
      label: 'Data',
      key: 'sfadata',
      components: PersonEditData,
      weight: 2,
      ignore: false
    },
    {
      label: 'Validation',
      key: 'sfavalidation',
      components: PersonEditValidation,
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
  ]);
}
