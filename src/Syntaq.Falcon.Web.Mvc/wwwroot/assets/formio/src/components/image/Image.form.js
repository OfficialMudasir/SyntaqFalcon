import baseEditForm from '../base/Base.form';

import ImageEditDisplay from './editForm/Image.edit.display';

export default function() {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: ImageEditDisplay,
      weight: 1,
      ignore: false
    },
    //{
    //  label: 'Data',
    //  key: 'sfadata',
    //  components: RadioynEditData,
    //  weight: 2,
    //  ignore: false
    //},
    //{
    //  label: 'Validation',
    //  key: 'sfavalidation',
    //  components: RadioynEditValidation,
    //  weight: 3,
    //  ignore: false
    //},
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
      ignore: true
    }
  ]);
}
