import baseEditForm from '../base/Base.form';
import SectionPanelEditDisplay from './editForm/SectionPanel.edit.display';

export default function() {
  return baseEditForm([
    {
      key: 'display',
      components:SectionPanelEditDisplay
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

