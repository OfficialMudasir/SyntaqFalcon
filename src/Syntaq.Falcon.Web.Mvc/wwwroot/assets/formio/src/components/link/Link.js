import BaseComponent from '../base/Base';
import Tooltip from 'tooltip.js';

export default class LinkComponent extends BaseComponent {
  static schema() {
    return {
      type: 'link',
      key: 'link',
      protected: false,
      persistent: false,
      label: 'Link',
      hideLabel: true,
      disabled: false,
      tableView: false,
      input: false,
      header: '',
      url:''
    };
  }

  static get builderInfo() {
    return {
      title: 'Website Link',
      icon: 'fas fa-link',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: LinkComponent.schema()
    };
  }

  get emptyValue() {
    return '';
  }

  get defaultSchema() {
    return LinkComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    return info;
  }

  createInput(container) {
    const weburl = this.component.header + this.component.url;
    const LinkGroup = this.ce('div', {
      class: 'form-group'
    });
    const ClickGroup = this.ce('div', {
      style: 'margin-top: 0.5em; display: block;'
    });
    this.a = this.ce('a', {
      class: 'control-label',
      style: 'text-decoration: underline; ' +
        'color: #58A3DC;' +
        'cursor: pointer;'
    });
    this.a.onclick = function() {
      window.open(`${weburl}`);
    };

    this.a.innerHTML = this.component.label;
    this.createTooltip(this.a, this.component);
    ClickGroup.appendChild(this.a);
    LinkGroup.appendChild(ClickGroup);
    container.appendChild(LinkGroup);
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
syntaq-component syntaq-component-link ${this.className}`,
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
