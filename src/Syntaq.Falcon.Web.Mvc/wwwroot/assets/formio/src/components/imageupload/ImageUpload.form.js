import baseEditForm from '../base/Base.form';

import FileEditFile from './editForm/ImageUpload.edit.file';

export default function() {
  return baseEditForm([
    {
      label: 'File',
      key: 'file',
      components: FileEditFile
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
  ]);
}
