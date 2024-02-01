//import nestedComponentForm from '../nested/NestedComponent.form';
import baseEditForm from '../base/Base.form';

import sfaPanelEditDisplay from './editForm/sfaPanel.edit.display';

export default function(...extend) {
  return baseEditForm([
    {
      label: 'Display',
      key: 'sfadisplay',
      title: 'Panel',
      components: sfaPanelEditDisplay,
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
      //components: '',
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
