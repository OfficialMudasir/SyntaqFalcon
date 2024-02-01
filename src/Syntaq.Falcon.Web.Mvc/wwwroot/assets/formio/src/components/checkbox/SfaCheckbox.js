import CheckboxComponent from '../checkbox/Checkbox';

export default class SfaCheckboxComponent extends CheckboxComponent {
  static schema(...extend) {
    return CheckboxComponent.schema({
      label: 'Check Box',
      key: 'sfaCheckbox',
      type: 'sfacheckbox',
      logic: [],
      //toggleStyle:false
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Check Box',
      icon: 'far fa-check-square',
      group: 'common',
      documentation: '',
      weight: 0,
      schema: SfaCheckboxComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaCheckboxComponent.schema();
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    const checkboxClass = `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfacheckbox ${this.className}`;
    this.element = this.ce('div', {
      id: this.id,
      class: `${this.component.toggleStyle ? 'btn-group form-check-inline toggleStyle' : checkboxClass}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  createLabel(container, input) {
    if (!this.component.toggleStyle) {
      super.createLabel(container, input);
    }
    else {
      this.labelElement = this.ce('span', {
        class: 'switch-padding'
      });
      this.labelSpan = this.ce('label', {
        class: 'switch',
        for: `toggle-${this.id}`
      });
      input.setAttribute('id', `toggle-${this.id}`);
      input.setAttribute('class', 'fldscheckbox');
      this.addInput(input, this.labelElement);
      this.labelElement.appendChild(this.labelSpan);
      this.labelText = this.ce('span', {
        class: 'switch-padding',
        style:'font-size: 22px'
      });
      this.labelText.appendChild(this.text(this.component.label));
      container.appendChild(this.labelElement);
      container.appendChild(this.labelText);
    }
  }
}
