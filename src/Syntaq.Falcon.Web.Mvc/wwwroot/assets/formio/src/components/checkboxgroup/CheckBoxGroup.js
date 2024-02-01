import _ from 'lodash';
import RadioynComponent from '../radioyn/Radioyn';

export default class CheckBoxesComponent extends RadioynComponent {
  static schema(...extend) {
    return RadioynComponent.schema({
      type: 'checkboxesgroup',
      label: 'Check Box Group',
      key: 'checkBoxesGroup',
      values: '',
      inline: false
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Check Box Group',
      group: 'testGroup',
      icon: 'far fa-check-square',
      weight: 60,
      documentation: 'http://help.form.io/userguide/#selectboxes',
      schema: CheckBoxesComponent.schema()
    };
  }

  constructor(component, options, data) {
    super(component, options, data);
    this.component.inputType = 'checkbox';
    component.values = component.values !== '' ? component.values : this.emptyValue;
  }

  get defaultSchema() {
    return CheckBoxesComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.name += '[]';
    info.attr.type = 'checkbox';
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
      if ((value.hasOwnProperty(key) && value[key]) || value[key] !== this.defaultValue[key] && typeof this.defaultValue[key] !== 'undefined') {
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

  restoreValue() {
    // My restore value
    if (this.hasSetValue) {
      this.setValue(this.dataValue, {
        noUpdateEvent: true
      });
    }
    else {
      const defaultValue = this.defaultValue;
      if (!_.isNil(defaultValue)) {
        this.setValue(defaultValue, {
          noUpdateEvent: true
        });
      }
    }
  }

  get hasSetValue() {
    // My hasSetValue
    return this.hasValue() && !this.isEmpty(this.dataValue);
  }

  /**
   * Set the value of this component.
   *
   * @param value
   * @param flags
   */
  setValue(value, flags) {
    //
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
    //
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
