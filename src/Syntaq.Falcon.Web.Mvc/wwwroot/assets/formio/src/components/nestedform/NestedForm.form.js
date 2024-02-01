import nestedComponentForm from '../nested/NestedComponent.form';

import EmbeddedFormEditForm from './editForm/NestedForm.edit.form';

export default function(...extend) {
  return nestedComponentForm([
    {
      label: 'Form',
      key: 'form',
      weight: 10,
      components: EmbeddedFormEditForm
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
