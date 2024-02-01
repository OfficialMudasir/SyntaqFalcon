import baseEditForm from '../base/Base.form';

import YouTubeEditDisplay from './editForm/YouTube.edit.display';

export default function() {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: YouTubeEditDisplay,
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
