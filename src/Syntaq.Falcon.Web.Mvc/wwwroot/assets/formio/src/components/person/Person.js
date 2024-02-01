import BaseComponent from '../base/Base';
import _ from 'lodash';

export default class PersonComponent extends BaseComponent {
  static schema(...extend) {
    return BaseComponent.schema({
      label: 'Person',
      key: 'person',
      type: 'person',
      inputType: 'text',
      hideLabel: true,
      requiredfirstname: true,
      requiredlastname: true,
      logic: []
      //   multiple:true,
      //   value: '',
      //   title:'',
      //   firstName: '', //
      //   middleName: '', //
      //   lastName: '',
      //   fullName:''
    }, ...extend);
  }
  static get builderInfo() {
    return {
      title: 'Person',
      group: 'testGroup',
      icon: 'fas fa-user-circle',
      weight: 10,
      documentation: 'http://help.form.io/userguide/#html-element-component',
      schema: PersonComponent.schema()
    };
  }

  get defaultSchema() {
    return PersonComponent.schema();
  }

  get emptyValue() {
    return '';
  }
  constructor(component, options, data) {
    super(component, options, data);
    component.logic = component.logic === undefined ? [] : component.logic;
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
    const personGroup = this.ce('div', {
      class: 'form-group'
    });
    const rowGroup = this.ce('div', {
      class: 'row formio-component formio-component-columns formio-component-columns'
    });
    //----Tile
    const titleGroup = this.createTitleD();
    //----First Name
    const firstNGroup = this.createFirstND();
    //-----Middle Name
    const middleNGroup = this.createMiddleND();
    //-----Last Name
    const lastNGroup = this.createLastND();
    //------Full Name
    const fullNGroup = this.createFullND();
    //---
    rowGroup.appendChild(titleGroup);
    rowGroup.appendChild(firstNGroup);
    rowGroup.appendChild(middleNGroup);
    rowGroup.appendChild(lastNGroup);
    rowGroup.appendChild(fullNGroup);
    personGroup.appendChild(rowGroup);
    container.appendChild(personGroup);
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
    const result = {};
    result['Sal_cho'] = value[0] ? value[0] : '';
    result['Name_First_txt'] = value[1] ? value[1] : '';
    result['Name_Middle_txt'] = value[2] ? value[2] : '';
    result['Name_Last_txt'] = value[3] ? value[3] : '';
    //result['Name_Full_scr'] = value[4] ? value[4] : '';
    result['Name_Full_scr'] = `${result['Name_First_txt']} ${result['Name_Middle_txt']} ${result['Name_Last_txt']}`.replace('  ', ' ').replace(/(^\s*)|(\s*$)/g, '');
    return result;
  }
  setValueAt(index, value) {
    // console.log(`1.setValueAt-${index}--${JSON.stringify(value)}`);
    if (this.inputs && this.inputs[index] && value !== null && value !== undefined) {
      this.inputs[0].value = value.Sal_cho === undefined ? '' : value.Sal_cho;
      this.inputs[1].value = value.Name_First_txt === undefined ? '' : value.Name_First_txt;
      this.inputs[2].value = value.Name_Middle_txt === undefined ? '' : value.Name_Middle_txt;
      this.inputs[3].value = value.Name_Last_txt === undefined ? '' : value.Name_Last_txt;
      // this.inputs[4].value = value.fuName===undefined?'':value.fuName;
      this.inputs[4].value = `${this.inputs[1].value} ${this.inputs[2].value} ${this.inputs[3].value}`.replace('  ', ' ').replace(/(^\s*)|(\s*$)/g, '');
      // console.log(`2.setValueAt---${this.inputs[1].value} ${this.inputs[2].value} ${this.inputs[3].value}-${this.inputs[4].value}`);
    }
  }
  //setValue(value, flags) {
  //  super.setValue(value, flags);
  //  console.log(JSON.stringify(value));
  //}
  createTitleD() {
    //----Tile
    const hide = this.component.hidetitle ? 'none' : 'block';
    //    const titleListID = `title-${this.id}`;
    //   const datalist = this.createTitleList(titleListID);
    const titleGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-1 col-lg-1',
      style: `display:${hide};padding-right: 0px;${this.options.builder ? 'padding-left: 0px;' : ''}`
    });
    const titleGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    titleGroupDiv.appendChild(this.text('Title'));
    const InputWrapper = this.ce('div');
    const inputT = this.ce('select', {
      class: 'form-control form-select'
    });
    this.selectOptions(inputT, 'titleOption', this.titleList);
    this.addInput(inputT, InputWrapper);
    titleGroup.appendChild(titleGroupDiv);
    titleGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em; color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative'
      });
      const tipName = `${this.component.key}_Sal_cho`;
      tipsDiv.innerHTML = tipName;
      titleGroup.appendChild(tipsDiv);
    }
    //   titleGroup.appendChild(datalist);
    return titleGroup;
  }
  createFirstND() {
    const placeholder = this.component.firstNamePlaceholder ? this.component.firstNamePlaceholder : '';
    const firstNGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-4 col-lg-4'
    });
    const firstNGroupDiv = this.ce('label', {
      class: `${this.component.requiredfirstname ? 'field-required control-label' : 'control-label'}`,
      style: 'width: 100%',
    });
    firstNGroupDiv.appendChild(this.text('First Name'));
    const InputWrapper = this.ce('div');
    const inputF = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputF, InputWrapper);
    firstNGroup.appendChild(firstNGroupDiv);
    firstNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative'
      });
      const tipName = `${this.component.key}_Name_First_txt`;
      tipsDiv.innerHTML = tipName;
      firstNGroup.appendChild(tipsDiv);
    }
    return firstNGroup;
  }
  createMiddleND() {
    const hide = this.component.hidemidname ? 'none' : 'block';
    const placeholder = this.component.middleNamePlaceholder ? this.component.middleNamePlaceholder : '';
    const middleNGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-3 col-lg-3',
      style: `display:${hide};`
    });
    const middleNGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    middleNGroupDiv.appendChild(this.text('Middle Name'));
    const InputWrapper = this.ce('div');
    const inputM = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputM, InputWrapper);
    middleNGroup.appendChild(middleNGroupDiv);
    middleNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative'
      });
      const tipName = `${this.component.key}_Name_Middle_txt`;
      tipsDiv.innerHTML = tipName;
      middleNGroup.appendChild(tipsDiv);
    }
    return middleNGroup;
  }
  createLastND() {
    const placeholder = this.component.lastNamePlaceholder ? this.component.lastNamePlaceholder : '';
    const lastNGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-4 col-lg-4',
      style: this.options.builder ? 'padding-right: 0px;' : ''
    });
    const lastNGroupDiv = this.ce('label', {
      class: `${this.component.requiredlastname ? 'field-required control-label' : 'control-label'}`,
      style: 'width: 100%'
    });
    lastNGroupDiv.appendChild(this.text('Last Name'));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
    });
    this.addInput(inputL, InputWrapper);
    lastNGroup.appendChild(lastNGroupDiv);
    lastNGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative'
      });
      const tipName = `${this.component.key}_Name_Last_txt`;
      tipsDiv.innerHTML = tipName;
      lastNGroup.appendChild(tipsDiv);
    }
    return lastNGroup;
  }
  createFullND() {
    const hide = this.component.hidefullname ? 'none' : 'block';
    const fullNGroup = this.ce('div', {
      class: 'col col-xs-12 col-sm-12 col-md-12 mt-4',
      style: `top: 10px; display:${hide};${this.options.builder ? 'padding-left: 0px;padding-right: 0px;' : ''}`
    });
    const fullNGroupDiv = this.ce('div', {
      class: 'form-group has-feedback formio-component'
    });
    this.inputFull = this.ce('input', {
      name: `${this.options.name}[Name_Full_scr]`,
      type: 'text',
      class: 'form-control Name_Full_scr',
      readonly: 1
    });
    this.addInput(this.inputFull, fullNGroupDiv);
    fullNGroup.appendChild(fullNGroupDiv);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba; position:relative'
      });
      const tipName = `${this.component.key}_Name_Full_scr`;
      tipsDiv.innerHTML = tipName;
      fullNGroup.appendChild(tipsDiv);
    }
    return fullNGroup;
  }
  //createTitleList(titleID) {
  //  const dataList = this.ce('datalist', {
  //    id: titleID
  //  });
  //  const title = [' ', 'Mr', 'Mrs', 'Miss', 'Ms', 'Dr(M)', 'Dr(F)'];
  //  for (let i = 0; i < title.length; i++) {
  //    const option = this.ce('option', {
  //      value: `${title[i]}`
  //    });
  //    option.innerHTML = title[i];
  //    dataList.appendChild(option);
  //  }
  //  return dataList;
  //}

  onChange(flags, fromRoot) {
    const fullName = `${this.inputs[1].value} ${this.inputs[2].value} ${this.inputs[3].value}`;
    this.inputs[4].value = fullName.replace('  ', ' ').replace(/(^\s*)|(\s*$)/g, '');
    //this.redraw();
    //this.dataValue['Name_Full_scr'] = this.inputs[4].value;
    super.onChange(flags, fromRoot);
  }
  validateRequired() {
    // console.log(`1.setValueAt-${setting}--${JSON.stringify(value)}`);
    if (this.options.name === 'data[defaultValue]') {
      return true;
    }
    const result = !(this.component.requiredfirstname && this.isEmpty(this.inputs[1].value)) &&
      !(this.component.requiredlastname && this.isEmpty(this.inputs[3].value));
    return result;
  }
  get titleList() {
    if (this._title) {
      return this._title;
    }
    this._title = [
      { value: '', label: ' ' },
      { value: 'Mr', label: 'Mr' },
      { value: 'Mrs', label: 'Mrs' },
      { value: 'Miss', label: 'Miss' },
      { value: 'Ms', label: 'Ms' },
      { value: 'Dr(M)', label: 'Dr(M)' },
      { value: 'Dr(F)', label: 'Dr(F)' }
    ];
    return this._title;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-person ${this.className}`,
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
}
