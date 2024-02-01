import NumberComponent from '../number/Number';

export default class SfaNumberComponent extends NumberComponent {
  static schema(...extend) {
    return NumberComponent.schema({
      type: 'sfanumber',
      label: 'Number',
      key: 'sfaNumber',
      validate: {
        min: '',
        max: '',
        step: 'any',
        integer: ''
      },
      logic: []
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Number',
      icon: 'fas fa-hashtag',
      group: 'basic',
      documentation: '',
      weight: 0,
      schema: SfaNumberComponent.schema()
    };
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
  }
  get defaultSchema() {
    return SfaNumberComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.type = 'text';
    info.attr.inputmode = 'numeric';
    info.changeEvent = 'input';
    return info;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfanumber ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  setValueAt(index, value) {
    value = value == null ? this.component.defaultValue : value;
    return super.setValueAt(index, this.formatValue(this.clearInput(value)));
  }
}
