import BaseComponent from '../base/Base';

export default class ImageComponent extends BaseComponent {
  static schema(...extend) {
    return BaseComponent.schema({
      type: 'image',
      key: 'image',
      protected: false,
      persistent: true,
      label: 'Image',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: false,
      image: false
      // uploadfile: [
      //   {
      //     storage: 'base64',
      //     name: '',
      //     url: '',
      //     size: 0,
      //     type: '',
      //     originalName: ''
      //   }],
      // fileMinSize: '0KB',
      // fileMaxSize: '2048KB'
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Image',
      icon: 'far fa-image',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: ImageComponent.schema()
    };
  }

  get emptyValue() {
    return '';
  }

  get defaultSchema() {
    return ImageComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    return info;
  }

  createInput(container) {
    const ImageGroup = this.ce('div', {
      class: 'form-group',
      style: 'margin-top: 0em;'
    });
    if (this.component.uploadfile) {
      if (this.component.uploadfile[0]) {
        const url = this.component.uploadfile[0].url.toString();
        const w = this.component.sizeslider/10*100;
        // const h = this.component.height;
        if (url !== '') {
          const image = this.ce('img',{
            src:url,
            style:`width:${w}%; height:auto;`
          });
          ImageGroup.appendChild(image);
        }
      }
      else {
        const noIcon = this.noImage();
        ImageGroup.appendChild(noIcon);
      }
    }
    container.appendChild(ImageGroup);
    this.errorContainer = container;
  }

  noImage() {
    const a = this.ce('a',{
      class:'btn btn-primary',
      style:'padding-bottom: 7px; ' +
        'padding-top: 7px; ' +
        'color: rgb(255, 255, 255); ' +
        'margin-top: 0.5em;'
    });
    const icon = this.ce('i',{
      class:'fa fa-picture-o fa-5'
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
    span.innerHTML = 'Image Properties';
    a.appendChild(icon);
    a.appendChild(span);
    return a;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-image ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
