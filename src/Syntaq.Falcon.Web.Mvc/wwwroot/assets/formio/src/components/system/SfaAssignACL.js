import _ from 'lodash';
import TextFieldComponent from '../textfield/TextField';

export default class SfaAssignACLComponent extends TextFieldComponent {
  static schema(...extend) {
    return TextFieldComponent.schema({
      label: 'Assign ACL',
      key: 'sfaAssignACL',
      type: 'sfaassignacl',
      mask: false,
      inputType: 'text',
      inputMask: '',
      validate: {
        minLength: '',
        maxLength: '',
        minWords: '',
        maxWords: '',
        pattern: ''
        //},
        //hidden: true
      }//,
      //customClass: "system-field"
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Assign ACL',
      icon: 'fas fa-cog',
      group: 'not',
      documentation: '',
      weight: 0,
      schema: SfaAssignACLComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaAssignACLComponent.schema();
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.class = 'system-field';
    return info;
  }

  createInput(container) {
    const aclGroup = this.ce('div', {
      class: 'form-group'
    });
    const rowGroup = this.ce('div', {
      class: 'row formio-component formio-component-columns formio-component-columns'
    });
    //----ACL ID
    const idGroup = this.createID();
    const typeGroup = this.createType();
    const permissionGroup = this.createPermission();

    rowGroup.appendChild(idGroup);
    rowGroup.appendChild(typeGroup);
    rowGroup.appendChild(permissionGroup);
    aclGroup.appendChild(rowGroup);
    container.appendChild(aclGroup);
    this.errorContainer = container;
  }

  createID() {
    const idGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-2 col-lg-2',
      style: 'display:\'block\';'
    });
    const idGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    idGroupDiv.appendChild(this.text('Team/User Names'));
    const InputWrapper = this.ce('div');
    const inputT = this.ce('input', {
      type: 'text',
      class: 'form-control'
    });
    this.addInput(inputT, InputWrapper);
    idGroup.appendChild(idGroupDiv);
    idGroup.appendChild(InputWrapper);
    return idGroup;
  }

  createType() {
    //const typeListID = `type-${this.id}`;
    //const datalist = this.createTypeList(typeListID);
    const typeGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-2 col-lg-2',
      //style: `display:${hide};`
    });
    const typeGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    typeGroupDiv.appendChild(this.text('Entity Type'));
    const InputWrapper = this.ce('div');
    const inputT = this.ce('select', {
      class: 'form-control'
    });
    this.selectOptions(inputT, 'typeOption', this.typeList);
    this.addInput(inputT, InputWrapper);
    typeGroup.appendChild(typeGroupDiv);
    typeGroup.appendChild(InputWrapper);
    //typeGroup.appendChild(datalist);
    return typeGroup;
  }

  createPermission() {
    const permissionGroup = this.ce('div', {
      class: 'col-xs-12 col-sm-12 col-md-2 col-lg-2',
    });
    const permissionGroupDiv = this.ce('label', {
      style: 'width: 100%',
      class: 'control-label'
    });
    permissionGroupDiv.appendChild(this.text('Permission Type'));
    const InputWrapper = this.ce('div');
    const inputT = this.ce('select', {
      class: 'form-control'
    });
    this.selectOptions(inputT, 'permissionOption', this.permissionList);
    this.addInput(inputT, InputWrapper);
    permissionGroup.appendChild(permissionGroupDiv);
    permissionGroup.appendChild(InputWrapper);
    return permissionGroup;
  }

  get typeList() {
    if (this._type) {
      return this._type;
    }
    this._type = [
      { value: 'User', label: 'User' },
      { value: 'Team', label: 'Team' }
    ];
    return this._type;
  }

  get permissionList() {
    if (this._permission) {
      return this._permission;
    }
    this._permission = [
      { value: 'V', label: 'Can View' },
      { value: 'E', label: 'Can Edit' },
      { value: 'O', label: 'Set Owner' }
    ];
    return this._permission;
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
    result['ID'] = value[0] ? value[0] : '';
    result['Type'] = value[1] ? value[1] : '';
    result['Permission'] = value[2] ? value[2] : '';
    //result['lasName'] = value[3] ? value[3] : '';
    //result['fuName'] = value[4] ? value[4] : '';
    return result;
  }

  setValueAt(index, value) {
    // console.log(`1.setValueAt-${index}--${JSON.stringify(value)}`);
    if (this.inputs && this.inputs[index] && value !== null && value !== undefined) {
      this.inputs[0].value = value.ID === undefined ? '' : value.ID;
      this.inputs[1].value = value.Type === undefined ? '' : value.Type;
      this.inputs[2].value = value.Permission === undefined ? '' : value.Permission;
      //this.inputs[3].value = value.lasName === undefined ? '' : value.lasName;
      // this.inputs[4].value = value.fuName===undefined?'':value.fuName;
      //this.inputs[4].value = `${this.inputs[1].value} ${this.inputs[2].value} ${this.inputs[3].value}`.replace('  ', ' ').replace(/(^\s*)|(\s*$)/g, '');
      // console.log(`2.setValueAt---${this.inputs[1].value} ${this.inputs[2].value} ${this.inputs[3].value}-${this.inputs[4].value}`);
    }
  }

  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-xs-${this.component.widthslider ? this.component.widthslider : 12} 
      col-sm-${this.component.widthslider ? this.component.widthslider : 12} 
      col-md-${this.component.widthslider ? this.component.widthslider : 12} 
      col-lg-${this.component.widthslider ? this.component.widthslider : 12} 
      offset-xs-${this.component.offsetslider ? this.component.offsetslider : 0} 
      offset-sm-${this.component.offsetslider ? this.component.offsetslider : 0} 
      offset-md-${this.component.offsetslider ? this.component.offsetslider : 0}
      offset-lg-${this.component.offsetslider ? this.component.offsetslider : 0}
      ${this.className}`,
      style: `height: min-content;float: left; padding-left: 0px; padding-right: 10px; ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
