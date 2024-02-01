import TextFieldComponent from '../textfield/TextField';

export default class SfaSystemRedirectComponent extends TextFieldComponent {
  static schema(...extend) {
    return TextFieldComponent.schema({
      label: 'sfasystemredirect',
      key: 'sfaSystemRedirect',
      type: 'sfasystemredirect',
      mask: false,
      inputType: 'text',
      inputMask: '',
      validate: {
        minLength: '',
        maxLength: '',
        minWords: '',
        maxWords: '',
        pattern: ''
        //},
        //hidden: true
      }//,
      //customClass: "system-field"
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'System Redirect',
      icon: 'fas fa-cog',
      group: 'not',
      documentation: '',
      weight: 0,
      schema: SfaSystemRedirectComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaSystemRedirectComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.class = 'system-field form-control';
    return info;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-xs-${this.component.widthslider ? this.component.widthslider : 12} 
      col-sm-${this.component.widthslider ? this.component.widthslider : 12} 
      col-md-${this.component.widthslider ? this.component.widthslider : 12} 
      col-lg-${this.component.widthslider ? this.component.widthslider : 12} 
      offset-xs-${this.component.offsetslider ? this.component.offsetslider : 0} 
      offset-sm-${this.component.offsetslider ? this.component.offsetslider : 0} 
      offset-md-${this.component.offsetslider ? this.component.offsetslider : 0}
      offset-lg-${this.component.offsetslider ? this.component.offsetslider : 0}
      ${this.className}`,
      style: `height: min-content;float: left; padding-left: 0px; padding-right: 10px; formio-component-textField; ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
