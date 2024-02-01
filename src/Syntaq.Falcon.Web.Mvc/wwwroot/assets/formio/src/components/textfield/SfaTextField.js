import TextFieldComponent from '../textfield/TextField';

export default class SfaTextFieldComponent extends TextFieldComponent {
  static schema(...extend) {
    return TextFieldComponent.schema({
      label: 'Text Field',
      key: 'sfaTextField',
      type: 'sfatextfield',
      mask: false,
      inputType: 'text',
      inputMask: '',
      defaultValue:'',
      validate: {
        minLength: '',
        maxLength: '',
        minWords: '',
        maxWords: '',
        pattern: ''
      },
      logic:[]
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Text Box',
      icon: 'fas fa-i-cursor',
      group: 'common',
      documentation: '',
      weight: 0,
      schema: SfaTextFieldComponent.schema()
    };
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
  }
  get defaultSchema() {
    return SfaTextFieldComponent.schema();
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfatextfield ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
