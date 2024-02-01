import baseEditForm from '../base/Base.form';

import SfaHTMLEditDisplay from './editForm/SfaHTML.edit.display';
import SfaHTMLEditData from './editForm/SfaHTML.edit.data';
import SfaHTMLEditValidation from './editForm/SfaHTML.edit.validation';
import SfaHTMLEditLogic from './editForm/HTML.edit.logic';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaHTMLEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Data',
        key: 'sfadata',
        components: SfaHTMLEditData,
        weight: 2,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaHTMLEditValidation,
        weight: 3,
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
        components: SfaHTMLEditLogic,
        weight: 4,
        ignore: false
      }
    ], ...extend);
}
