import BaseComponent from '../base/Base';
import _ from 'lodash';

export default class SliderComponent extends BaseComponent {
  static schema(...extend) {
    return BaseComponent.schema({
      label: 'Slider',
      key: 'slider',
      type: 'slider',
      inputType: 'range',
      value: 0,
      minValue: 0, //
      maxValue: 10, //
      step: 1,
      logic: []
    }, ...extend);
  }
  static get builderInfo() {
    return {
      title: 'Slider',
      group: 'other',
      icon: 'fas fa-sliders-h',
      weight: 10,
      documentation: 'http://help.form.io/userguide/#html-element-component',
      schema: SliderComponent.schema()
    };
  }

  get defaultSchema() {
    return SliderComponent.schema();
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
  }
  get emptyValue() {
    return 0;
  }

  elementInfo() {
    const info = super.elementInfo();
    info.type = 'input';
    info.changeEvent = 'change';
    info.attr.class = 'form-check-input';
    return info;
  }
  createInput(container) {
    const value = this.component.defaultValue?this.component.defaultValue:this.component.value;
    const minValue = this.component.minValue;
    const maxValue = this.component.maxValue;
    const step = this.component.step;
    const inputGroup = this.ce('div', {
      class: 'form-group'
    });

    const labelWrapper = this.ce('div', {
      class: '',
      style: '',
    });

    const min = this.ce('span', {
      class: 'slider-min'
    });
    min.innerHTML = minValue;
    const max = this.ce('span', {
      class: 'slider-max pull-right'
    });
    max.innerHTML = maxValue;
    const label = this.ce('label', {
      class: 'form-label w-100'
    });
    const listID = `mark-${this.id.toString()}`;
    const input = this.ce('input', {
      type: 'range',
      class: 'form-range',
      min: minValue,
      max: maxValue,
      step: step,
      value: value,
      style: 'width: 100%;margin-left: 0px; right: 0px;',
      list: listID
    });
    this.addInput(input, label);
    const datalist = this.createListMark(minValue, maxValue, step, listID);
    labelWrapper.appendChild(min);
    labelWrapper.appendChild(max);
    labelWrapper.appendChild(label);
    this.addShowValue(labelWrapper);
    labelWrapper.appendChild(datalist);
    inputGroup.appendChild(labelWrapper);

    container.appendChild(inputGroup);
    this.errorContainer = container;
  }

  getValue() {
    if (this.viewOnly) {
      return this.dataValue;
    }
    let value = '';
    _.each(this.inputs, (input) => {
      if (input) {
        value = input.value;
      }
    });
    return value;
  }

  createListMark(min, max, step, listID) {
    const dataList = this.ce('datalist', {
      id: listID
    });
    for (let i = min; i <= max; i += step) {
      const option = this.ce('option');
      option.innerHTML = i + 0;
      if (i + step > max) {
        option.innerHTML = max + 0;
        dataList.appendChild(option);
        break;
      }
      dataList.appendChild(option);
    }
    return dataList;
  }

  addShowValue(container) {
    const divlab = this.ce('div',{
      style: ''
    });
    this.ShowValue = this.ce('span', {
      class: 'text-muted',
      style: 'margin-left: 4px'
    });
    divlab.appendChild(this.ShowValue);
    container.appendChild(divlab);
  }

  onChange(flags, fromRoot) {
    super.onChange(flags, fromRoot);
    const vv = `value: ${this.inputs[0].value}`;
    this.ShowValue.innerHTML = vv;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-slider ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
