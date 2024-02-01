import BaseComponent from '../base/Base';

export default class DividerComponent extends BaseComponent {
  static schema() {
    return {
      type: 'divider',
      key: 'divider',
      protected: false,
      persistent: true,
      label: 'divider',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: false,
      dividerstyle: 'style1',
      customecss: ''
    };
  }

  static get builderInfo() {
    return {
      title: 'Divider',
      icon: 'fas fa-minus',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: DividerComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return DividerComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  createInput(container) {
    const option = this.component.dividerstyle;
    const dividerGroup = this.ce('div', {
      class: 'form-group'
    });
    const customeCSS = this.component.customecss;
    if (option === 'style0') {
      dividerGroup.style.marginBottom = 0;
      if (this.options.builder) {
        dividerGroup.setAttribute('style', 'border: 1px dashed rgb(51, 153, 255);height: 0.1em;margin-top: 0em;position: relative;left: 0px;top: 0px;');
      }
    }
    else {
      const hr = this.dividerStyleSelector(option, customeCSS);
      dividerGroup.appendChild(hr);
    }

    container.appendChild(dividerGroup);
    this.errorContainer = container;
  }
  dividerStyleSelector(option, css) {
    //const none = this.ce('div', {
    //  style: 'margin-bottom: 2%;',
    //  class:  'col text-center'
    //});
    //const br = this.ce('br');
    //none.appendChild(br);
    //if (option === 'style0' && this.options.builder) {
    //  none.innerHTML = 'Invisible divider';
    //  return none;
    //}
    let hr = this.ce('hr');
    switch (option) {
      //case 'style0':
      //  return none;
      case 'style1':
        hr = this.ce('hr', {
          style: `border-top: 1px solid;${css}`
        });
        return hr;
      case 'style2':
        hr = this.ce('hr', {
          style: `border-top: 3px double;${css}`
        });
        return hr;
      case 'style3':
        hr = this.ce('hr', {
          style: `border-top: 1px dashed;${css}`
        });
        return hr;
      case 'style4':
        hr = this.ce('hr', {
          style: `border-top: 1px dotted;${css}`
        });
        return hr;
      case 'style5':
        hr = this.ce('hr', {
          style: `
          height: 30px; 
          border-color:black;
          border-style: solid; 
          border-width: 1px 0 0 0; 
          border-radius: 20px; ${css}`
        });

        return hr;
      case 'style6':
        hr = this.ce('hr', {
          style: `
          display: block; 
          content: ""; 
          height: 30px; 
          margin-top: -31px; 
          border-style: solid; 
          border-width: 0 0 1px 0; 
          border-radius: 20px; ${css}`
        });
        return hr;
      default:
        hr = this.ce('hr', {
          style: `border-top: 1px solid;${css}`
        });
        return hr;
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
syntaq-component syntaq-component-divider ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
