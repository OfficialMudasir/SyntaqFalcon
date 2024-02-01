import baseEditForm from '../base/Base.form';

import SfaSystemEditData from './editForm/SfaSystemRedirect.edit.data';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaSystemEditData,
        weight: 1,
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
        ignore: true
      }
    ], ...extend);
}
