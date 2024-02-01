import BaseComponent from '../base/Base';
import Tooltip from 'tooltip.js';

export default class HeadingComponent extends BaseComponent {
  static schema() {
    return {
      type: 'heading',
      key: 'heading',
      protected: false,
      persistent: true,
      label: 'heading',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: false,
      heading: 'Heading',
      fontsize: 24,
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
      title: 'Heading',
      icon: 'fas fa-heading',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: HeadingComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return HeadingComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  createInput(container) {
    var style = `font-size:${this.component.fontsize}px;
font-family: ${this.component.fontfamily !== '' ? this.component.fontfamily : 'sans-serif'};
${this.component.bold ? 'font-weight:bold;' : ';'}
${this.component.italic ? 'font-style:italic;' : ';'}
${this.component.underline ? 'text-decoration: underline;' : ';'}
color: ${this.component.fontcolour !== '' ? this.component.fontcolour : ''};
background: ${this.component.backcolour !== '' ? this.component.backcolour : ''};`;
    const HeadingGroup = this.ce('div', {
      class: 'form-group',
      style: 'margin-top: 0em;'
    });
    const H3 = this.ce('h3',{
      style:'padding: 0px; margin: 0px; display: block;'
    });
    this.labell = this.ce('h3',{
      style: `${style} text-align: left;display: block;`
    });
    this.labell.innerHTML = this.component.heading;
    this.createTooltip(this.labell, this.component);
    H3.appendChild(this.labell);
    HeadingGroup.appendChild(H3);
    container.appendChild(HeadingGroup);
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
syntaq-component syntaq-component-heading ${this.className}`,
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
