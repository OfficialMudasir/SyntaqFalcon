import baseEditForm from '../base/Base.form';

import HelpNotesEditDisplay from './editForm/HelpNotes.edit.display';

export default function() {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: HelpNotesEditDisplay,
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
      ignore: true
    },
    {
      key: 'logic',
      ignore: true
    }
  ]);
}
