import _ from 'lodash';
import BaseComponent from '../base/Base';

export default class SfaRadioYNComponent extends BaseComponent {
  static schema(...extend) {
    return BaseComponent.schema({
      type: 'sfaradioyn',
      inputType: 'radio',
      label: 'Yes/No Radio',
      key: 'sfaRadioYN',
      values: '',
      labelPosition:'',
//      defaultValue: [{ label: 'Yes', value: 'Yes',mtext:''  },
//        { label: 'No', value: 'No',mtext:''  }],
      fieldSet: false,
      logic: []
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Yes/No Radio',
      group: 'testGroup',
      icon: 'far fa-dot-circle',
      weight: 80,
      documentation: 'http://help.form.io/userguide/#radio',
      schema: SfaRadioYNComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaRadioYNComponent.schema();
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
    if (component.values === '') {
      component.values = this.emptyValue;
    }
    if (component.labelPosition === '') {
      component.labelPosition = 'top';
    }
  }
  //get emptyValue() {
  //  if (this.component.inputType === 'radio' && this.component.clearOnHide === true) {
  //    this.component.values = { false: false, true: false };
  //    return this.component.values;
  //  }
  //  else {
  //    return [{ label: 'Yes', value: 'true', mtext: '' },
  //    { label: 'No', value: 'false', mtext: '' }];
  //  }
  //}
  get emptyValue() {
    return [{ label: 'Yes', value: 'true', mtext: '' },
      { label: 'No', value: 'false', mtext: '' }];
  }
  elementInfo() {
    const info = super.elementInfo();
    info.type = 'input';
    info.changeEvent = 'click';
    info.attr.class = 'form-check-input';
    return info;
  }

  createInput(container) {
    const styleFloat = (this.component.labelPosition === 'top' || this.component.labelPosition === 'bottom') ?
      '' : 'float: inherit;';
    const inputGroup = this.ce('div', {
      //class: 'form-group',
      style: styleFloat
    });
    const labelOnTheTopOrOnTheLeft = this.optionsLabelOnTheTopOrLeft();
    const wrappers = [];
    _.each(this.component.values, (value) => {
      const wrapperClass = `form-check ${this.optionWrapperClass}`;
      const labelWrapper = this.ce('div', {
        class: wrapperClass
      });
      const label = this.ce('label', {
        class: 'form-check-label'
      });

      this.addShortcut(label, value.shortcut);

      // Determine the attributes for this input.
      let inputId = this.id;
      if (this.options.row) {
        //inputId += `-${this.options.row}`;
        inputId += `-${this.options.name.replace(/\D/g, '')}`;
      }
      inputId += `-${value.value}`;
      this.info.attr.id = inputId;
      this.info.attr.value = value.value;
      label.setAttribute('for', this.info.attr.id);

      // Create the input.
      const input = this.ce('input');
      _.each(this.info.attr, (attrValue, key) => {
        if (key === 'name' && this.component.inputType === 'radio') {
          attrValue += `[${this.id}]`;
        }
        input.setAttribute(key, attrValue);
      });

      const labelSpan = this.ce('span');
      if (value.label && labelOnTheTopOrOnTheLeft) {
        label.appendChild(labelSpan);
      }

      this.setInputLabelStyle(label);
      this.setInputStyle(input);

      this.addInput(input, label);

      if (value.label) {
        labelSpan.appendChild(this.text(this.addShortcutToLabel(value.label, value.shortcut)));
      }

      if (value.label && !labelOnTheTopOrOnTheLeft) {
        label.appendChild(labelSpan);
      }
      labelWrapper.appendChild(label);

      inputGroup.appendChild(labelWrapper);
      wrappers.push(labelWrapper);
    });
    this.wrappers = wrappers;
    container.appendChild(inputGroup);
    this.errorContainer = container;

    //STQ Modified 10906
    if (this.logic.length > 0) {
      var labels = container.querySelectorAll('.control-label');

      // Loop through each label element
      labels.forEach((label) => {
        // Get the "for" attribute of the label
        var labelFor = label.getAttribute('for');

        // Get the corresponding input element based on the "for" attribute
        var inputElement = container.querySelector(`#${labelFor}`);
        if (inputElement != null) {
          if (inputElement.type === 'radio') {
            // Check if the input element exists and its ID matches the "for" attribute
            if (inputElement && inputElement.id === labelFor) {
              // Update the "for" attribute of the label
              label.setAttribute('for', `${labelFor}-true`); // Replace 'new-id' with the desired ID
            }
          }
        }
      });
    }
  }

  get optionWrapperClass() {
    const inputType = this.component.inputType;
    const wrapperClass = (this.component.inline ? `form-check-inline ${inputType}-inline` : inputType);
    return wrapperClass;
  }

  optionsLabelOnTheTopOrLeft() {
    return ['top', 'left'].includes(this.component.optionsLabelPosition);
  }

  optionsLabelOnTheTopOrBottom() {
    return ['top', 'bottom'].includes(this.component.optionsLabelPosition);
  }

  setInputLabelStyle(label) {
    if (this.component.optionsLabelPosition === 'left') {
      _.assign(label.style, {
        textAlign: 'center',
        paddingLeft: 0,
      });
    }

    if (this.optionsLabelOnTheTopOrBottom()) {
      _.assign(label.style, {
        display: 'block',
        textAlign: 'center',
        paddingLeft: 0,
      });
    }
  }

  setInputStyle(input) {
    if (this.component.optionsLabelPosition === 'left') {
      _.assign(input.style, {
        position: 'initial',
        marginLeft: '7px'
      });
    }

    if (this.optionsLabelOnTheTopOrBottom()) {
      _.assign(input.style, {
        width: '100%',
        position: 'initial',
        marginLeft: 0
      });
    }
  }

  getValue() {
    if (this.viewOnly) {
      return this.dataValue;
    }
    const cKey = this.component.key;
    const value = {};
    let index = 0;
    _.each(this.inputs, (input) => {
      value[input.value] = !!input.checked;
      if (input.checked) {
        value[cKey] = input.value;
        value['Mtext'] = this.component.values[index].mtext;
      }
      index++;
    });
    return value;
  }

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
//    console.log('getView');
    if (!value) {
      return '';
    }
    if (!_.isString(value)) {
      return _.toString(value);
    }

    const option = _.find(this.component.values, (v) => v.value === value);

    return _.get(option, 'label');
  }

  setValueAt(index, value) {
    if (this.inputs && this.inputs[index] && value !== null && value !== undefined) {
      const inputValue = this.inputs[index].value;
      this.inputs[index].checked = (inputValue === value.toString());
//      console.log(`1- inputValue: ${inputValue}`);//
//      console.log(`2- value.toString(): ${JSON.stringify(value)}`);//
//      console.log(`3- this.inputs[${index}].checked: ${this.inputs[index].checked}`);//
    }
  }

  updateValue(flags, value) {
//    console.log(`updateValue----${JSON.stringify(value)}`);
    const changed = super.updateValue(flags, value);
//    console.log(`updateValue----changed${changed}`);
    if (changed) {
      //add/remove selected option class
      const value = this.dataValue;
      const optionSelectedClass = 'radio-selected';

      _.each(this.wrappers, (wrapper, index) => {
        const input = this.inputs[index];
        if (input.value.toString() === value.toString()) {
          //add class to container when selected
          this.addClass(wrapper, optionSelectedClass);
        }
        else {
          this.removeClass(wrapper, optionSelectedClass);
        }
      });
    }
    return changed;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    //height: min-content;float: left; padding-left: 0px; padding-right: 10px;
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-${this.component.type} ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  isEmpty(value) {
    let empty = true;
    if (value) {
      if (value.hasOwnProperty(this.component.key)) {
        empty = false;
      }
    }
    return empty;
  }
}
