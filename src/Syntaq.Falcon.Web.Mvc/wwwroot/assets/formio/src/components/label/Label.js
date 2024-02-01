import BaseComponent from '../base/Base';
import Tooltip from 'tooltip.js';

export default class LabelComponent extends BaseComponent {
  static schema() {
    return {
      type: 'label',
      key: 'label',
      protected: false,
      persistent: true,
      label: 'label',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: false,
      labelvalue: 'Label',
      fontsize: 15.6,
      bold: true,
      italic: false,
      underline: false,
      fontcolour: '',
      backcolour: '',
      fontfamily: ''
    };
  }

  static get builderInfo() {
    return {
      title: 'Label',
      icon: 'fas fa-font',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: LabelComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return LabelComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  createInput(container) {
    var style = `font-size:${this.component.fontsize}px;
font-family: ${this.component.fontfamily !== '' ? this.component.fontfamily : 'Poppins'};
${this.component.bold ? 'font-weight:bold;' : ';'}
${this.component.italic ? 'font-style:italic;' : ';'}
${this.component.underline ? 'text-decoration: underline;' : ';'}
color: ${this.component.fontcolour !== '' ? this.component.fontcolour : ''};
background: ${this.component.backcolour !== '' ? this.component.backcolour : ''};`;
    const LabelGroup = this.ce('div', {
      class: 'form-group',
      style: 'margin-bottom: 0;cursor: default;'
    });
    this.labell = this.ce('label', {
      class: 'control-label',
      style: style
    });
    this.labell.innerHTML = this.component.labelvalue;
    this.createTooltip(this.labell, this.component);
    LabelGroup.appendChild(this.labell);
    container.appendChild(LabelGroup);
    this.errorContainer = container;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-label ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  createTooltip(container, component, classes) {
    if (this.tooltip) {
      return;
    }
    component = component || this.component;
    classes = classes || `${this.iconClass('question-sign')} text-muted`;
    if (!component.tooltip) {
      return;
    }
    const ttElement = this.ce('i', {
      class: classes
    });
    container.appendChild(this.text(' '));
    container.appendChild(ttElement);

    ttElement.addEventListener('mouseover', () => {
      this.tooltip = new Tooltip(ttElement, {
        trigger: 'hover click',
        placement: 'right',
        html: true,
        title: this.interpolate(component.tooltip).replace(/(?:\r\n|\r|\n)/g, '<br />')
      });
    });
  }
}
