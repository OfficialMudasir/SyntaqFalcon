import BaseComponent from '../base/Base';

export default class YouTubeComponent extends BaseComponent {
  static schema() {
    return {
      type: 'youtube',
      key: 'youtube',
      protected: false,
      persistent: true,
      label: 'YouTube',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: false,
      youtubeid:'',
      width:800,
      height:640
    };
  }

  static get builderInfo() {
    return {
      title: 'YouTube Popup',
      icon: 'fab fa-youtube',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: YouTubeComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return YouTubeComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  createInput(container) {
    const id = this.component.youtubeid;
    const w = this.component.width;
    const h = this.component.height;
    const YouTubeGroup = this.ce('div', {
      class: 'form-group'
    });
    const ClickGroup = this.ce('div', {
      style:'margin-top: 0.5em; display: block;'
    });
    const a = this.ce('a',{
      class:'btn btn-primary',
      style:'padding-bottom: 7px; ' +
        'padding-top: 7px; ' +
        'color: rgb(255, 255, 255); ' +
        'margin-top: 0.5em;',
   //   onclick:'popupyoutube()'
    });
    a.onclick = function() {
      window.open(`http://www.youtube.com/watch_popup?v=${id}`, '', `width=${w},height=${h}` );
    };
    const i = this.ce('i',{
      class:'fab fa-youtube'
    });
    const span = this.ce('span',{
      style:'padding-top: 0px; ' +
        'color: rgb(255, 255, 255); ' +
        'font-size: 15px; ' +
        'font-family: Arial, Arial, Helvetica, sans-serif; ' +
        'font-weight: normal; ' +
        'text-align: left; ' +
        'display: inline;'
    });
    span.innerHTML = ` ${this.component.label}`;
    a.appendChild(i);
    a.appendChild(span);
    ClickGroup.appendChild(a);
    YouTubeGroup.appendChild(ClickGroup);
    container.appendChild(YouTubeGroup);
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
syntaq-component syntaq-component-youtube ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
