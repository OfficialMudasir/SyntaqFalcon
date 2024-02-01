import nestedComponentForm from '../nested/NestedComponent.form';

//import PanelEditDisplay from './editForm/Panel.edit.display';
import sfaPanelEditDisplay from './editForm/sfaPanel.edit.display';

export default function(...extend) {
  return nestedComponentForm([
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
  ], ...extend);
}
