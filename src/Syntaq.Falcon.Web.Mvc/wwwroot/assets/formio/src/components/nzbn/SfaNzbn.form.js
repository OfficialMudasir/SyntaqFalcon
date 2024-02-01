import baseEditForm from '../base/Base.form';

import SfaNzbnEditDisplay from './editForm/SfaNzbn.edit.display';
import SfaNzbnEditData from './editForm/SfaNzbn.edit.data';
import SfaNzbnEditValidation from './editForm/SfaNzbn.edit.validation';

export default function(...extend) {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: SfaNzbnEditDisplay,
      weight: 1,
      ignore: false
    },
    {
      label: 'Data',
      key: 'sfadata',
      components: SfaNzbnEditData,
      weight: 2,
      ignore: false
    },
    {
      label: 'Validation',
      key: 'sfavalidation',
      components: SfaNzbnEditValidation,
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
