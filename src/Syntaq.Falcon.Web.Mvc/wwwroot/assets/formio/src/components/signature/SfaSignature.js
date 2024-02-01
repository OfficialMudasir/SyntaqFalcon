//import SignaturePad from 'signature_pad/dist/signature_pad.js';
//import BaseComponent from '../base/Base';
import SignatureComponent from './Signature';
import _ from 'lodash';

export default class SfaSignatureComponent extends SignatureComponent {
  static schema(...extend) {
    return SignatureComponent.schema({
      type: 'sfasignature',
      label: 'Signature',
      key: 'sfasignature',
      footer: 'Sign above',
      width: '100%',
      height: '150px',
      penColor: 'black',
      backgroundColor: 'rgb(245,245,235)',
      minWidth: '0.5',
      maxWidth: '2.5'
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Signature',
      group: 'advanced',
      icon: 'fa fa-pencil',
      weight: 120,
      documentation: 'http://help.form.io/userguide/#signature',
      schema: SfaSignatureComponent.schema()
    };
  }

  constructor(component, options, data) {
    super(component, options, data);
    this.currentWidth = 0;
    this.scale = 1;
    if (!this.component.width) {
      this.component.width = '100%';
    }
    if (!this.component.height) {
      this.component.height = '200px';
    }
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfasignature ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  addDataPrefix(value) {
    if (value!==null && value !== undefined) {
      if (value.indexOf('image/png;base64,') === 0) {
        return value.replace('image/png;base64,', 'data:image/png;base64,');
      }
    }
    return value;
  }
  removeDataPrefix(value) {
    if (value !== null && value !== undefined) {
      if (value.indexOf('data:image/png;base64,') === 0) {
        return value.replace('data:image/png;base64,', 'image/png;base64,');
      }
    }
    return value;
  }
  setValue(value, flags) {
  //  console.log(`value -----${JSON.stringify(value)}`);
    flags = this.getFlags.apply(this, arguments);
    super.setValue(value, flags);
    value = this.addDataPrefix(value);
  //  console.log(`after addDataPrefix(value) value -----${JSON.stringify(value)}`);
    if (this.signaturePad) {
      if (value && !flags.noSign) {
        this.signatureImage.setAttribute('src', value);
        this.showCanvas(false);
      }
      if (!value) {
        this.signaturePad.clear();
      }
    }
    this.dataValue = this.removeDataPrefix(value);
  //  console.log(`after value -----${JSON.stringify(this.dataValue)}`);
  }
  getValue() {
    if (this.viewOnly) {
      return this.dataValue;
    }
    return this.dataValue = this.removeDataPrefix(this.dataValue);
  }
}
