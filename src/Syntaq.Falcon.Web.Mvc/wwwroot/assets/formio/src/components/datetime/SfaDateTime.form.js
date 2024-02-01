import baseEditForm from '../base/Base.form';

import SfaDateTimeEditDisplay from './editForm/SfaDateTime.edit.display';
import SfaDateTimeEditDateTime from './editForm/SfaDateTime.edit.datetime';
import SfaDateTimeEditData from './editForm/SfaDateTime.edit.data';
import SfaDateTimeEditValidation from './editForm/SfaDateTime.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaDateTimeEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Date/Time',
        key: 'sfadatetime',
        components: SfaDateTimeEditDateTime,
        weight: 2,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaDateTimeEditData,
        weight: 3,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaDateTimeEditValidation,
        weight: 4,
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
        weight: 5,
        ignore: false
      }
    ], ...extend);
}
