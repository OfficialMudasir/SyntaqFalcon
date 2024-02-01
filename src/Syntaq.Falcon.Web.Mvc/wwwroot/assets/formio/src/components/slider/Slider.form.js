import baseEditForm from '../base/Base.form';

import SliderEditDisplay from './editForm/Slider.edit.display';
import SliderEditValidation from './editForm/Slider.edit.validation';

export default function(...extend) {
  return baseEditForm(
    [
      {
        label: 'Display',
        key: 'sfadisplay',
        components: SliderEditDisplay,
        weight: 1,
        ignore: false
      },
      {
        label: 'Validation',
        key: 'sfavalidation',
        components: SliderEditValidation,
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

