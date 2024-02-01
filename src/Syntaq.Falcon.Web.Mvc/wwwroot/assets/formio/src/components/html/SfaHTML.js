import _ from 'lodash';
import HTMLComponent from '../html/HTML';

export default class SfaHTMLComponent extends HTMLComponent {
  static schema(...extend) {
    return HTMLComponent.schema({
      label: 'HTML',
      key: 'sfaHTMLElement',
      type: 'sfahtmlelement',
      tag: 'p',
      attrs: [],
      content: '',
      input: false
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'HTML Element',
      group: 'advanced',
      icon: 'fa fa-code',
      weight: 90,
      documentation: 'http://help.form.io/userguide/#html-element-component',
      schema: SfaHTMLComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaHTMLComponent.schema();
  }

  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfahtmlelement ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }

  build() {
    this.createElement();
    if (this.component.tag === '') {
      this.component.tag = 'div';
    }
    this.htmlElement = this.ce(this.component.tag, {
      id: this.id,
      class: this.component.className
    });
    _.each(this.component.attrs, (attr) => {
      if (attr.attr) {
        this.htmlElement.setAttribute(attr.attr, attr.value);
      }
    });
    if (this.component.content) {
      this.setHTML();
    }
    else {
      this.htmlElement.innerHTML = '<div class="col text-center">Please enter the HTML content</div>';
    }
    if (this.component.refreshOnChange) {
      this.on('change', () => this.setHTML(), true);
    }
    this.element.appendChild(this.htmlElement);
    this.attachLogic();
  }
//  setHTML() {
//    this.htmlElement.innerHTML = this.component.content !== '' ? this.interpolate(this.component.content) : '<div class="col text-center">Please enter the HTML content</div>';
//  }
}
