import BaseComponent from '../base/Base';
import { eachComponent } from '../../utils/utils';

export default class PopupFormComponent extends BaseComponent {
  static schema() {
    return {
      type: 'popupform',
      key: 'popupform',
      protected: false,
      persistent: true,
      label: 'Popup Form',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: true,
      formId: '',
      theme: 'primary',
      size: 'md',
      validate: {
        required: false
      },
      width:800,//
      height:640//
    };
  }

  static get builderInfo() {
    return {
      title: 'Popup Form',
      icon: 'fas fa-file-invoice',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: PopupFormComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return PopupFormComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  constructor(component, options, data) {
    component.input = true;
    super(component, options, data);
    component.formId = (component.formId === '' && component.FormId !== undefined) ? component.FormId : component.formId;
  }
  /* eslint-disable max-statements */
  build() {
    super.build();
    var currentFormKeys = {};
    var sameLevelpopKeys = {};
    if (this.parent !== null && !this.options.preview) {
      var parentComponent = this.parent;
      while (parentComponent.type === 'components') {
        if (parentComponent.parent == null) {
          break;
        }
        parentComponent = parentComponent.parent;
      }
      if (parentComponent.type === 'components') {
        eachComponent(parentComponent.component.components, function(comp) {
          if (comp.type === 'popupform') {
            sameLevelpopKeys[comp.key] = true;
          }
          else {
            currentFormKeys[comp.key] = true;
          }
        }, true);
      }
      else if (parentComponent.type === 'datagrid') {
        eachComponent(parentComponent.component.components[0].components, function(comp) {
          if (comp.type === 'popupform') {
            sameLevelpopKeys[comp.key] = true;
          }
          else {
            currentFormKeys[comp.key] = true;
          }
        }, true);
      }
      else {
        eachComponent(parentComponent.components, function(comp) {
          if (comp.type === 'popupform') {
            sameLevelpopKeys[comp.key] = true;
          }
          else {
            currentFormKeys[comp.key] = true;
          }
        }, true);
      }
    }
    if (this.dataValue && this.dataValue !=='hasValue') {
      const result = JSON.parse(this.dataValue);
      for (var formKey in currentFormKeys) {
        if (result.hasOwnProperty(formKey)) {
          delete result[formKey];
        }
      }
      for (var key in result) {
        if (result.hasOwnProperty(key)) {
          this.parent.data[key] = result[key];
        }
        else {
          this.parent.data[key] = null;
        }
      }
      this.dataValue = 'hasValue';
    }
    const formData2 = {};
    if (this.parent !== null && this.parent !== this.root) {
      for (var kkey in this.parent.data) {
        formData2[kkey] = this.parent.data[kkey];
      }
    }
    else if (this.parent !== null) {
      for (var kkkey in this.parent.data) {
        if (!currentFormKeys.hasOwnProperty(kkkey)) {
          formData2[kkkey] = this.parent.data[kkkey];
        }
      }
    }

    for (var currentpopK in sameLevelpopKeys) {
      if (currentpopK !== this.key) {
        delete formData2[currentpopK];
      }
    }
    //this.dataValue = JSON.stringify(formData2);
    //this.dataValue = '';
    let formurl = '';
    if (this.component.formId) {
      //if ((window.location.port != null)) {
      //  formurl = `${location.protocol}//${window.location.hostname}:${window.location.port}/Falcon/forms/LoadMin?FormID=${this.component.FormId}`;
      //}
      //else {
      //  formurl = `${location.protocol}//${window.location.hostname}/Falcon/forms/LoadMin?FormID=${this.component.FormId}`;
      //}
      const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (searchParams, prop) => searchParams.get(prop),
      });
      formurl = `/Falcon/forms/LoadMin?FormID=${this.component.formId}&ProjectId=${params.ProjectId}`;
      formurl = window._SyntaqBaseURI + formurl;
    }

    const w = this.component.width;
    const h = this.component.height;
    const PopupFormGroup = this.ce('div', {
      class: 'form-group'
    });
    const ClickGroup = this.ce('div', {
      style: 'margin-top: 0.5em; display: block;'
    });
    const a = this.ce('a', {
      class: `btn btn-${this.component.theme} btn-${this.component.size} ${this.component.validate.required ?'control-label field-required':''}`,
      style: 'padding-bottom: 7px; ' +
        'padding-top: 7px; ' +
        'color: rgb(255, 255, 255); ' +
        'margin-top: 0.5em;'
    });
    const PopName = this.options.name;
    const PopKey = this.component.key;
    const PopRequired = this.component.validate.required;
    if (!this.options.builder) {
      a.onclick = function() {
        var pop = window.open(formurl, this.id, `width=${w},height=${h}`);

        // Post Message required to load popups in external domains
        window.addEventListener('message', function(e) {
          if (e.data === 'Ready') {
            var message = {
              'FormType': 'popup', 'PopName': PopName, 'PopKey': PopKey, 'PopRequired': PopRequired,'FormData': JSON.stringify(formData2)
            };
            pop.postMessage(message, '*');
          }
        }, false);

        pop;
      };
    }
    if (this.component.leftIcon) {
      const li = this.ce('i', {
        class: this.component.leftIcon
      });
      a.appendChild(li);
    }
    const span = this.ce('span', {    });
    span.innerHTML = ` ${this.component.label}`;
    a.appendChild(span);
    if (this.component.rightIcon) {
      const ri = this.ce('i', {
        class: this.component.rightIcon
      });
      a.appendChild(ri);
    }
    ClickGroup.appendChild(a);
    PopupFormGroup.appendChild(ClickGroup);
    this.element.appendChild(PopupFormGroup);
    this.errorContainer = this.element;
  }
  /* eslint-disable max-statements */
  createInput(container) {
    const InputWrapper = this.ce('div');
    const inputT = this.ce('input', {
      name: this.options.name,
      type: 'text',
      style:'display: none;'
    });
//    inputT.addEventListener('change', () => this.redraw());
    this.addInput(inputT, InputWrapper);
    container.appendChild(InputWrapper);
  }
  // This component don't need label
  createLabel() {
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-popupform ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
