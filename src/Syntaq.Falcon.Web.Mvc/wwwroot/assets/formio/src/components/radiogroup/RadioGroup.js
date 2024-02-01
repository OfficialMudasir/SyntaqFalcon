import _ from 'lodash';
import RadioynComponent from '../radioyn/Radioyn';

export default class RadioGroupComponent extends RadioynComponent {
  static schema(...extend) {
    return RadioynComponent.schema({
      type: 'radiogroup',
      label: 'Radio Group',
      key: 'radioGroup',
      values: '',
      inline: false
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Radio Group',
      group: 'testGroup',
      icon: 'far fa-dot-circle',
      weight: 60,
      documentation: 'http://help.form.io/userguide/#selectboxes',
      schema: RadioGroupComponent.schema()
    };
  }

  constructor(component, options, data) {
    super(component, options, data);
    this.component.inputType = 'radio';
  }

  get defaultSchema() {
    return RadioGroupComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.name += '[]';
    info.attr.type = 'radio';
    info.attr.class = 'form-check-input';
    return info;
  }

  get emptyValue() {
    return [{ label: 'label1', value: 'value1', mtext: '' },
    { label: 'label2', value: 'value2', mtext: '' },
    { label: 'label3', value: 'value3', mtext: '' }];
  }

  /**
   * Only empty if the values are all false.
   *
   * @param value
   * @return {boolean}
   */
  isEmpty(value) {
    let empty = true;
    for (const key in value) {
      if (value.hasOwnProperty(key) && value[key]) {
        empty = false;
        break;
      }
    }

    return empty;
  }

  getValue() {
    if (this.viewOnly) {
      return this.dataValue;
    }
    const cKey = this.component.key;
    const result = {};
    let combineV ='';
    let combineMt ='';
    let index = 0;
    _.each(this.inputs, (input) => {
      result[input.value] = !!input.checked;
      if (input.checked) {
        combineV = `${combineV},${input.value}`;
        combineMt = `${combineMt},${this.component.values[index].mtext}`;
      }
      index++;
    });
    result[cKey] = combineV.substr(1);
    result['Mtext'] = combineMt.substr(1);
    return result;
  }

  /**
   * Set the value of this component.
   *
   * @param value
   * @param flags
   */
  setValue(value, flags) {
    //
    // If passing a string convert to a structure
    if (typeof value == 'string') {
      //
      if (this.viewOnly) {
        return this.dataValue;
      }
      const cKey = this.component.key;
      const result = {};
      _.each(this.inputs, (input) => {
        result[input.value] = false;
      });
      //
      result[cKey] = value;
      result[value] = true;
      result['Mtext'] = value;
      value = result;
    }

    value = value || {};
    flags = this.getFlags.apply(this, arguments);
    if (Array.isArray(value)) {
      _.each(value, (val) => {
        value[val] = true;
      });
    }

    _.each(this.inputs, (input) => {
      if (_.isUndefined(value[input.value])) {
        value[input.value] = false;
      }
      input.checked = !!value[input.value];
    });

    this.updateValue(flags);
  }

  getView(value) {
    if (!value) {
      return '';
    }
    return _(this.component.values || [])
      .filter((v) => value[v.value])
      .map('label')
      .join(', ');
  }
}
