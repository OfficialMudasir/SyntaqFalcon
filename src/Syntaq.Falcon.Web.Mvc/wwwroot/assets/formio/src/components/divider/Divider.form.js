import baseEditForm from '../base/Base.form';

import DividerEditDisplay from './editForm/Divider.edit.display';

export default function(...extend) {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: DividerEditDisplay,
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
  ], ...extend);
}
