import baseEditForm from '../base/Base.form';

import PopupFormEditDisplay from './editForm/PopupForm.edit.display';

export default function() {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: PopupFormEditDisplay,
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
        ignore: true
      },
      {
        key: 'conditional',
        ignore: false
      },
      {
        key: 'logic',
        ignore: false
      }
  ]);
}
