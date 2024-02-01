import TextFieldComponent from '../textfield/TextField';

export default class SfaEmailComponent extends TextFieldComponent {
  static schema(...extend) {
    return TextFieldComponent.schema({
      type: 'sfaemail',
      label: 'Email',
      key: 'sfaEmail',
      inputType: 'email',
      kickbox: {
        enabled: false
      },
      logic: []
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Email',
      group: 'advanced',
      icon: 'far fa-envelope',
      documentation: 'http://help.form.io/userguide/#email',
      weight: 10,
      schema: SfaEmailComponent.schema()
    };
  }

  constructor(component, options, data) {
    super(component, options, data);
    component.logic = component.logic === undefined ? [] : component.logic;
    this.validators.push('email');
  }

  get defaultSchema() {
    return SfaEmailComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.type = this.component.mask ? 'password' : 'email';
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
syntaq-component syntaq-component-sfaemail ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
