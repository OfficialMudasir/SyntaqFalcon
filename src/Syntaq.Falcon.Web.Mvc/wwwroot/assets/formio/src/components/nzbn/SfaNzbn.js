import TextFieldComponent from '../textfield/TextField';
import _ from 'lodash';

export default class SfaNzbnComponent extends TextFieldComponent {
  static schema(...extend) {
    return TextFieldComponent.schema({
      label: 'NZBN Search',
      key: 'sfaNzbn',
      type: 'sfanzbn',
      mask: false,
      inputType: 'text',
      inputMask: '',
      defaultValue:'',
      validate: {
        minLength: '',
        maxLength: '',
        minWords: '',
        maxWords: '',
        pattern: ''
      },
      logic:[]
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'NZBN',
      icon: 'fas fa-kiwi-bird',
      group: 'common',
      documentation: '',
      weight: 0,
      schema: SfaNzbnComponent.schema()
    };
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
  }
  get defaultSchema() {
    return SfaNzbnComponent.schema();
  }
  get emptyValue() {
    return '';
  }
  elementInfo() {
    const info = super.elementInfo();
    // info.type = 'input';
    // info.changeEvent = 'change';
    // info.attr.class = 'form-check-input';
    return info;
  }
  createInput(container) {
    //  const id = this.id;
    const nzbnGroup = this.ce('div', {
      class: ''
    });
    const rowGroup = this.ce('div', {
      class: 'row formio-component formio-component-columns formio-component-columns'
    });
    //----
    var Entity;
    const searchGroup = this.createSearchD();
    const entityNGroup = this.createEntityND();
    const NzbnNGroup = this.createNzbnND();
    const EntityTypeDescriptionNGroup = this.createEntityTypeDescriptionND();
    const EntityStatusDescriptionNGroup = this.createEntityStatusDescriptionND();
    const EntityNGroup = this.createEntityND2();
    //---
    rowGroup.appendChild(searchGroup);
    rowGroup.appendChild(entityNGroup);
    rowGroup.appendChild(NzbnNGroup);
    rowGroup.appendChild(EntityTypeDescriptionNGroup);
    rowGroup.appendChild(EntityStatusDescriptionNGroup);
    rowGroup.appendChild(EntityNGroup);
    nzbnGroup.appendChild(rowGroup);
    container.appendChild(nzbnGroup);
    this.errorContainer = container;
  }

  getValue() {
    if (this.viewOnly) {
      return this.dataValue;
    }
    const value = [];
    let index = 0;
    _.each(this.inputs, (input) => {
      if (input) {
        value[index] = input.value;
        index++;
      }
    });
    const result =  {};
    result['SearchText'] = value[0] ? value[0] : '';
    result['EntityName'] = value[1] ? value[1] : '';
    result['nzbn'] = value[2] ? value[2] : '';
    result['entityTypeDescription'] = value[3] ? value[3] : '';
    result['entityStatusDescription'] = value[4] ? value[4] : '';
    result['entity'] = value[5] ? value[5] : '';
    return result;
  }
  setValueAt(index, value) {
    if (this.inputs && this.inputs[index] && value !== null && value !== undefined) {
      this.inputs[0].value = value.SearchText === undefined ? '' : value.SearchText;
      this.inputs[1].value = value.EntityName === undefined ? '' : value.EntityName;
      this.inputs[2].value = value.nzbn === undefined ? '' : value.nzbn;
      this.inputs[3].value = value.entityTypeDescription === undefined ? '' : value.entityTypeDescription;
      this.inputs[4].value = value.entityStatusDescription === undefined ? '' : value.entityStatusDescription;
      if (value.entity !== undefined && value.entity !== null) {
        var entity = JSON.stringify(value.entity);
        entity = entity.replace(/\\/g, '');
        entity = entity.replace(/""/g, '');
        entity = entity.replace(/^"/, '');
        entity = entity.replace(/"^/, '');
        this.inputs[5].value = value.entity === undefined ? '' : entity;
      }
    }
  }
  createSearchD() {
    const placeholder = this.component.placeholder ? this.component.placeholder : '';
    //----Tile
    const hide = this.component.hidetitle ? 'none' : 'block';
    //    const titleListID = `title-${this.id}`;
    //   const datalist = this.createTitleList(titleListID);
    const searchGroup = this.ce('div', {
      class: 'col-lg-12 dropdown',
      style: `display:${hide};padding-right: 0px;${this.options.builder ? 'padding-left: 0px;' : ''}`
    });
    const searchGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    const InputWrapper = this.ce('div', { class: 'input-group' });
    const inputT = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder
    });
    this.addInput(inputT, InputWrapper);
    searchGroup.appendChild(searchGroupDiv);
    const btnAppend = this.ce('button', { class: 'btn btn-secondary' });
    btnAppend.appendChild(this.text('Search'));
    InputWrapper.appendChild(btnAppend);
    searchGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('div', {
        class: 'component-btn-group',
        style: 'right: 10%;bottom: 20%;color: #856404; background-color: #fff3cd; border-color: #ffeeba;'
      });
      const tipName = `${this.component.key}_SearchText`;
      tipsDiv.innerHTML = tipName;
      searchGroup.appendChild(tipsDiv);
    }

    const outputGroup = this.ce('div', {
      class: 'col-8 p-0 nzbnoutput dropdown-content',
      style : 'max-height: 360px; overflow: auto'
    });
    searchGroup.appendChild(outputGroup);
    this.addEventListener(inputT, 'change', (event) => {
      this.getNZBNEntities(outputGroup, event.target.value);
    });
    this.addEventListener(btnAppend, 'click', (event) => {
      this.getNZBNEntities(outputGroup, inputT.value);
    });
    return searchGroup;
  }
  createEntityND() {
    const placeholder = this.component.entityNamePlaceholder ? this.component.entityNamePlaceholder : '';
    const entityNGroup = this.ce('div', {
      class: 'col-12 d-none'
    });
    const entityNGroupDiv = this.ce('label', {
      class: 'control-label',
      style: 'width: 100%',
    });
    entityNGroupDiv.appendChild(this.text('Entity Name'));
    const InputWrapper = this.ce('div');
    const inputF = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputF, InputWrapper);
    entityNGroup.appendChild(entityNGroupDiv);
    entityNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('div', {
        class: 'component-btn-group',
        style: 'right: 10%;bottom: 20%;color: #856404; background-color: #fff3cd; border-color: #ffeeba;'
      });
      const tipName = `${this.component.key}_EntityName`;
      tipsDiv.innerHTML = tipName;
      entityNGroup.appendChild(tipsDiv);
    }
    return entityNGroup;
  }
  createNzbnND() {
    const hide = this.component.hidemidname ? 'none' : 'block';
    const placeholder = this.component.middleNamePlaceholder ? this.component.middleNamePlaceholder : '';
    const NzbnNGroup = this.ce('div', {
      class: 'col-12 d-none',
      style: `display:${hide};`
    });
    const NzbnNGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    NzbnNGroupDiv.appendChild(this.text('NZBN'));
    const InputWrapper = this.ce('div');
    const inputM = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputM, InputWrapper);
    NzbnNGroup.appendChild(NzbnNGroupDiv);
    NzbnNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('div', {
        class: 'component-btn-group',
        style: 'right: 10%;bottom: 20%;color: #856404; background-color: #fff3cd; border-color: #ffeeba;'
      });
      const tipName = `${this.component.key}_nzbn`;
      tipsDiv.innerHTML = tipName;
      NzbnNGroup.appendChild(tipsDiv);
    }
    return NzbnNGroup;
  }
  createEntityTypeDescriptionND() {
    const placeholder = this.component.lastNamePlaceholder ? this.component.lastNamePlaceholder : '';
    const entityTypeDescriptionNGroup = this.ce('div', {
      class: 'col-12 d-none',
      style: this.options.builder ? 'padding-right: 0px;' : ''
    });
    const lastNGroupDiv = this.ce('label', {
      class: 'control-label',
      style: 'width: 100%'
    });
    lastNGroupDiv.appendChild(this.text('Entity Type Description'));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputL, InputWrapper);
    entityTypeDescriptionNGroup.appendChild(lastNGroupDiv);
    entityTypeDescriptionNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('div', {
        class: 'component-btn-group',
        style: 'right: 10%;bottom: 20%;color: #856404; background-color: #fff3cd; border-color: #ffeeba;'
      });
      const tipName = `${this.component.key}_entityTypeDescription`;
      tipsDiv.innerHTML = tipName;
      entityTypeDescriptionNGroup.appendChild(tipsDiv);
    }
    return entityTypeDescriptionNGroup;
  }
  createEntityStatusDescriptionND() {
    const hide = this.component.hidefullname ? 'none' : 'block';
    const entityStatusDescriptionNGroup = this.ce('div', {
      class: 'col-12 d-none',
      style: `top: 10px; display:${hide};${this.options.builder ? 'padding-left: 0px;padding-right: 0px;' : ''}`
    });
    const fullNGroupDiv = this.ce('div', {
      class: 'form-group has-feedback formio-component'
    });
    fullNGroupDiv.appendChild(this.text('Entity Status Description'));
    this.inputFull = this.ce('input', {
      name: `${this.options.name}[entityStatusDescription]`,
      type: 'text',
      class: 'form-control entityStatusDescription'//,
      //readonly: 1
    });
    this.addInput(this.inputFull, fullNGroupDiv);
    entityStatusDescriptionNGroup.appendChild(fullNGroupDiv);
    if (this.options.builder) {
      const tipsDiv = this.ce('div', {
        class: 'component-btn-group',
        style: 'right: 10%;bottom: 45%;color: #856404; background-color: #fff3cd; border-color: #ffeeba;'
      });
      const tipName = `${this.component.key}_entityStatusDescription`;
      tipsDiv.innerHTML = tipName;
      entityStatusDescriptionNGroup.appendChild(tipsDiv);
    }
    return entityStatusDescriptionNGroup;
  }
  createEntityND2() {
    const placeholder = this.component.lastNamePlaceholder ? this.component.lastNamePlaceholder : '';
    const entityNGroup = this.ce('div', {
      class: 'col-12 d-none',
      style: this.options.builder ? 'padding-right: 0px;' : ''
    });
    const lastNGroupDiv = this.ce('label', {
      class: 'control-label',
      style: 'width: 100%'
    });
    lastNGroupDiv.appendChild(this.text('Entity'));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputL, InputWrapper);
    entityNGroup.appendChild(lastNGroupDiv);
    entityNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('div', {
        class: 'component-btn-group',
        style: 'right: 10%;bottom: 20%;color: #856404; background-color: #fff3cd; border-color: #ffeeba;'
      });
      const tipName = `${this.component.key}_entity`;
      tipsDiv.innerHTML = tipName;
      entityNGroup.appendChild(tipsDiv);
    }
    return entityNGroup;
  }
  onChange(flags, fromRoot) {
    //const fullName = `${this.inputs[1].value} ${this.inputs[2].value} ${this.inputs[3].value}`;
    //this.inputs[4].value = fullName.replace('  ', ' ').replace(/(^\s*)|(\s*$)/g, '');
    //this.redraw();
    //this.dataValue['entityStatusDescription'] = this.inputs[4].value;
    super.onChange(flags, fromRoot);
  }
  validateRequired() {
    if (this.component.validate.required) {
      return !(this.isEmpty(this.inputs[1].value) || this.isEmpty(this.inputs[2].value));
    }
    else {
      return true;
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
syntaq-component syntaq-component-sfanzbn ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  /**
* Add a new input error to this element.
*
* @param message
* @param dirty
*/
  addInputError(message, dirty) {
    if (!message) {
      return;
    }

    if (this.errorElement) {
      const errorMessage = this.ce('p', {
        class: 'help-block'
      });
      errorMessage.appendChild(this.text(message));
      this.errorElement.appendChild(errorMessage);
    }

    // Add error classes
    this.addClass(this.element, 'has-error');
    (this.isEmpty(this.inputs[1].value)) ? this.addClass(this.performInputMapping(this.inputs[1]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[1]), 'is-invalid');
    (this.isEmpty(this.inputs[3].value)) ? this.addClass(this.performInputMapping(this.inputs[3]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[3]), 'is-invalid');

    if (dirty && this.options.highlightErrors) {
      //this.addClass(this.element, 'alert alert-danger');
      this.addClass(this.element, 'alert-danger');
    }
  }
  getNZBNEntities(outputGroup, searchterm) {
    var entities;
    var inputs = this.inputs;
    var comp = this;

    outputGroup.innerHTML = '<div class="d-flex align-items-center m-4"><strong>Loading...</strong><div class="spinner-border text-primary ml-auto" role="status" aria-hidden="true" style="width: 3rem; height: 3rem;"></div></div>';
    outputGroup.style.display = 'block';
    var table = document.createElement('Table');

    table.className = 'table table-striped table-hover table-light--dark no-footer dtr-column  border';
    var thead = document.createElement('thead');
    table.appendChild(thead);

    var row = document.createElement('tr');

    var colname = document.createElement('td');
    colname.appendChild(document.createTextNode('Business Name'));
    row.appendChild(colname);

    var colnzbn = document.createElement('td');
    colnzbn.appendChild(document.createTextNode('NZBN'));
    row.appendChild(colnzbn);

    var coltype = document.createElement('td');
    coltype.appendChild(document.createTextNode('Business Type'));
    row.appendChild(coltype);

    var colstatus = document.createElement('td');
    colstatus.appendChild(document.createTextNode('Status'));
    row.appendChild(colstatus);

    thead.appendChild(row);

    var tbody = document.createElement('tbody');
    tbody.className = 'small';
    table.appendChild(tbody);

    var xhr = new XMLHttpRequest();
    xhr.withCredentials = true;
    xhr.responseType = 'json';
    xhr.addEventListener('readystatechange', function() {
      if (this.readyState === 4 && this.status === 200) {
        outputGroup.style.display = 'block';
        var jsonResponse = xhr.response;

        if (Object.entries(jsonResponse.result.items).length === 0) {
          outputGroup.innerHTML = '';
          outputGroup.innerHTML = '<div class="d-flex align-items-center m-4"><strong>Please try different key words...</strong></div>';
          outputGroup.style.display = 'block';

          var syntaqcontent = document.getElementById('syntaq-content');
          syntaqcontent.addEventListener('click', function() {
            outputGroup.innerHTML = '';
            outputGroup.style.display = 'none';
          });
        }
        entities = jsonResponse.result.items;
        jsonResponse.result.items.forEach((obj, index) => {
          var row = document.createElement('tr');
          row.className = 'small';
          var colname = document.createElement('td');
          var colnametext = document.createElement('span');
          colnametext.className = 'text-primary';
          colnametext.appendChild(document.createTextNode(`${obj.entityName}`));
          colname.appendChild(colnametext);
          row.appendChild(colname);

          var colnzbn = document.createElement('td');
          colnzbn.appendChild(document.createTextNode(`${obj.nzbn}`));
          row.appendChild(colnzbn);

          var coltype = document.createElement('td');
          coltype.appendChild(document.createTextNode(`${obj.entityTypeDescription}`));
          row.appendChild(coltype);

          var colstatus = document.createElement('td');
          colstatus.appendChild(document.createTextNode(`${obj.entityStatusDescription}`));

          row.addEventListener('click', function() {
            inputs[0].value = entities[index].entityName === undefined ? '' : entities[index].entityName;
            inputs[1].value = entities[index].entityName === undefined ? '' : entities[index].entityName;
            inputs[2].value = entities[index].nzbn === undefined ? '' : entities[index].nzbn;
            inputs[3].value = entities[index].entityTypeDescription === undefined ? '' : entities[index].entityTypeDescription;
            inputs[4].value = entities[index].entityStatusDescription === undefined ? '' : entities[index].entityStatusDescription;
            outputGroup.innerHTML = '';
            outputGroup.style.display = 'none';
            var entityResult;
            var xhr = new XMLHttpRequest();
            xhr.withCredentials = true;
            xhr.responseType = 'json';
            xhr.addEventListener('readystatechange', function() {
              if (this.readyState === 4 && this.status === 200) {
                var jsonResponse = xhr.response;
                entityResult = jsonResponse.result;
                inputs[5].value = JSON.stringify(entityResult);
                comp.triggerChange();
              }
            });
            xhr.open('GET', `${window._SyntaqBaseURI}/api/services/app/Forms/GetNZBNEntity?nzbn=${entities[index].nzbn}`);
            xhr.send();
          }, false);
          row.appendChild(colstatus);
          table.appendChild(row);
          outputGroup.innerHTML = '';

          //const contentGroup = this.ce('div', {
          //  class: '',
          //  style: 'height:380px; overflow: auto'
          //});
          //contentGroup.appendChild(table);
          //outputGroup.appendChild(contentGroup);

          outputGroup.appendChild(table);

          //var footer = document.createElement('div');
          //footer.className = 'm-2';
          //footer.style = 'position: absolute; bottom: 0';
          //outputGroup.appendChild(footer);

          //var button = document.createElement('div');
          //button.appendChild(document.createTextNode('Close'));
          //button.className = 'btn btn-secondary btn-sm ml-auto';
          //button.addEventListener('click', function() {
          //  outputGroup.innerHTML = '';
          //  outputGroup.style.display = 'none';
          //});

          //footer.appendChild(button);

          var syntaqcontent = document.getElementById('syntaq-content');
          syntaqcontent.addEventListener('click', function() {
            outputGroup.innerHTML = '';
            outputGroup.style.display = 'none';
          });
        });
        //finish for each loop
      } //finish change status loop
      else if (this.readyState === 4) {
        outputGroup.innerHTML = '';
        outputGroup.style.display = 'none';
      }
    });
    xhr.open('GET', `${window._SyntaqBaseURI}/api/services/app/Forms/GetNZBNEntities?searchterm=${searchterm}`);
    xhr.send();
  }
}
