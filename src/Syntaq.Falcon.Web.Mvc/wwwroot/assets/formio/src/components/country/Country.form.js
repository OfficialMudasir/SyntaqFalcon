import baseEditForm from '../base/Base.form';

import CountryEditDisplay from './editForm/Country.edit.display';
//import CountryEditData from './editForm/Country.edit.data';
import CountryEditValidation from './editForm/Country.edit.validation';

export default function(...extend) {
  return baseEditForm(
  [
    {
      label: 'Display',
      key: 'sfadisplay',
      components: CountryEditDisplay,
      weight: 1,
      ignore: false
    },
    //{
    //  label: 'Data',
    //  key: 'sfadata',
    //  components: CountryEditData,
    //  weight: 2,
    //  ignore: false
    //},
    {
      label: 'Validation',
      key: 'sfavalidation',
      components: CountryEditValidation,
      weight: 2,
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
      components: '',
      ignore: true
    },
    {
      key: 'logic',
      components: '',
      weight: 3,
      ignore: false
    }
  ], ...extend);
}
