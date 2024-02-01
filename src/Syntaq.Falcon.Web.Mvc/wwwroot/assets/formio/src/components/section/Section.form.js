import baseEditForm from '../base/Base.form';

import SectionEditDisplay from './editForm/Section.edit.display';

export default function() {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SectionEditDisplay,
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
      components: '',
      weight: 4,
      ignore: false
    }
  ]);
}
