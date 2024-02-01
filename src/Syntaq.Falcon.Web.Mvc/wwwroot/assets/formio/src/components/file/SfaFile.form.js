import baseEditForm from '../base/Base.form';

import SfaFileEditDisplay from './editForm/SfaFile.edit.display';
import SfaFileEditFile from './editForm/SfaFile.edit.file';
import SfaFileEditValidation from './editForm/SfaFile.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SfaFileEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'File',
        key: 'sfafile',
        weight: 2,
        components: SfaFileEditFile
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SfaFileEditValidation,
        weight: 3,
        ignore: false
      },
      {
        key: 'display',
        components: '',
        ignore: true
      },
      {
        key: 'data',
        components: '',
        ignore: true
      },
      {
        key: 'validation',
        components: '',
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
        weight: 4,
        ignore: false
      }
    ], ...extend);
}
