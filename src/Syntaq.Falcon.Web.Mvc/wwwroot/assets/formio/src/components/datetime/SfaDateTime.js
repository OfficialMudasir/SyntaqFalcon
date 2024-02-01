import DateTimeComponent from '../datetime/DateTime';
export default class SfaDateTimeComponent extends DateTimeComponent {
  static schema(...extend) {
    return DateTimeComponent.schema({
      type: 'sfadatetime',
      label: 'Date Time',
      key: 'sfaDateTime',
      format: 'dd-MM-yyyy hh:mm a',
      useLocaleSettings: true,
      disabled: false,
      focus: true,
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Date Time',
      group: 'advanced',
      icon: 'far fa-calendar-alt',
      documentation: 'https://www.google.com/',
      weight: 40,
      schema: SfaDateTimeComponent.schema()
    };
  }

  constructor(component, options, data) {
    super(component, options, data);

    // Change the format to map to the settings.
    if (!this.component.enableDate) {
      this.component.format = this.component.format.replace(/dd-MM-yyyy /g, '');
    }
  }

  get defaultSchema() {
    return SfaDateTimeComponent.schema();
  }
  createElement() {
    const colOffset = `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}`;
    // If the element is already created, don't recreate.
    if (this.element) {
      this.element.className = `${colOffset}
syntaq-component syntaq-component-sfadatetime ${this.className}`;
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfadatetime ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
