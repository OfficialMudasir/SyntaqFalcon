/* globals google */
import _ from 'lodash';

import TextFieldComponent from '../textfield/TextField';
import Formio from '../../Formio';
import countryList from './countryList';

export default class AddressGroupComponent extends TextFieldComponent {
  static schema(...extend) {
    return TextFieldComponent.schema({
      type: 'addressgroup',
      label: 'Address Group',
      key: 'addressgroup',
      hideLabel: true,
      map: {
        region: '',
        key: ''
      },
      cclabel: '',
      lvllabel: '',
      streetlabel: '',
      suburblabel: '',
      statelabel: '',
      postcodelabel: '',
      countrylabel: '',
      logic: []
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Address Group',
      group: 'testGroup',
      icon: 'fas fa-map-marker-alt',
      documentation: 'http://help.form.io/userguide/#address',
      schema: AddressGroupComponent.schema()
    };
  }

  async init() {
   await fetch('https://extreme-ip-lookup.com/json/')
      .then(res => res.json())
      .then(response => {
        const region = response.countryCode;
        const mapApiKey = 'AIzaSyATF1HslbZRsA260bKMzNN_Ar5K5kUiN2I';// Syntaq Service
        const src = `https://maps.googleapis.com/maps/api/js?key=${mapApiKey}&libraries=places&callback=googleMapsCallback&region=${region}`;
        Formio.requireLibrary('googleMaps', 'google.maps.places', src);
      })
      .catch((data, status) => {
        console.log('Request failed');
        const mapApiKey = 'AIzaSyATF1HslbZRsA260bKMzNN_Ar5K5kUiN2I';// Syntaq Service
        const src = `https://maps.googleapis.com/maps/api/js?key=${mapApiKey}&libraries=places&callback=googleMapsCallback&region=AU&region=NZ`;
        Formio.requireLibrary('googleMaps', 'google.maps.places', src);
      });
  }

  constructor(component,options,data) {
    super(component, options, data);
    //this.init();
    const mapApiKey = 'AIzaSyATF1HslbZRsA260bKMzNN_Ar5K5kUiN2I';// Syntaq Service
    ////const mapApiKey = 'AIzaSyB6lY5JRMqobQofRr3YVJax1w04iex7Axc'; YiChao
    const region = window.regionSyntaq; // ${ region }
    //const region = component.map.logic;
    const src = `https://maps.googleapis.com/maps/api/js?key=${mapApiKey}&libraries=places&callback=googleMapsCallback&region=${region}`;
    //// const src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyB6lY5JRMqobQofRr3YVJax1w04iex7Axc&libraries=places&callback=googleMapsCallback&region=AU';
    //// const src = 'https://maps.googleapis.com/maps/api/js?key=AIzaSyB6lY5JRMqobQofRr3YVJax1w04iex7Axc&libraries=places&callback=googleMapsCallback&region=AU';
    Formio.requireLibrary('googleMaps', 'google.maps.places', src);
    this.countryList = countryList;
    // Keep track of the full addresses.
    this.addresses = [];
    component.logic = component.logic === undefined ? [] : component.logic;
    if (component.validate.required) {
      component.requiredstreet = !component.hidestreet;
      component.requiredsuburb = !component.hidesuburb;
      component.requiredstate = !component.hidestate;
      component.requiredpostcode = !component.hidepostcode;
      component.requiredcountry = !component.hidecountry;
    }
    else {
      component.requiredstreet = component.requiredstreet ? !component.hidestreet : false;
      component.requiredsuburb = component.requiredsuburb ? !component.hidesuburb : false;
      component.requiredstate = component.requiredstate ? !component.hidestate : false;
      component.requiredpostcode = component.requiredpostcode ? !component.hidepostcode : false;
      component.requiredcountry = component.requiredcountry ? !component.hidecountry : false;
    }
  }

  get defaultSchema() {
    return AddressGroupComponent.schema();
  }

  setValueAt(index, value, flags) {
    flags = flags || {};
    if (!flags.noDefault && (value === null || value === undefined)) {
      value = this.defaultValue;
    }
    if (!this.isEmpty(value)) {
      if (value.hasOwnProperty('address_components') && value.hasOwnProperty('adr_address')) {
        this.addresses[0] = value;
        const valueDecode = this.ce('textarea');
        valueDecode.innerHTML = value.adr_address;
        this.autofill(valueDecode.value);
      }
      else if (this.inputs && this.inputs[index] && value !== null && value !== undefined) {
        this.inputs[0].value = value.formatted_address === undefined ? '' : value.formatted_address;
        this.inputs[1].value = value.Addr_Co_txt === undefined ? '' : value.Addr_Co_txt;
        this.inputs[2].value = value.Addr_Level_txt === undefined ? '' : value.Addr_Level_txt;
        this.inputs[3].value = value.Addr_1_txt === undefined ? '' : value.Addr_1_txt;
        this.inputs[4].value = value.Addr_Suburb_txt === undefined ? '' : value.Addr_Suburb_txt;
        this.inputs[5].value = value.Addr_State_txt === undefined ? '' : value.Addr_State_txt;
        this.inputs[6].value = value.Addr_PC_txt === undefined ? '' : value.Addr_PC_txt;
        this.inputs[7].value = value.Addr_Country_cho === undefined ? '' : value.Addr_Country_cho;
        this.inputs[8].value = value.Addr_txt === undefined ? '' : value.Addr_txt;//
      }
    }
  }
  getValue() {
    if (!this.hasInput) {
      return;
    }
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
    const values = {};
    values['formatted_address'] = value[0] ? value[0] : '';
    values['Addr_Co_txt'] = value[1] ? value[1] : '';
    values['Addr_Level_txt'] = value[2] ? value[2] : '';
    values['Addr_1_txt'] = value[3] ? value[3] : '';
    values['Addr_Suburb_txt'] = value[4] ? value[4] : '';
    values['Addr_State_txt'] = value[5] ? value[5] : '';
    values['Addr_PC_txt'] = value[6] ? value[6] : '';
    values['Addr_Country_cho'] = value[7] ? value[7] : '';
    values['Addr_txt'] = value[8] ? value[8] : '';
    return values;
  }

  createInput(container) {
    super.createInput(container);
    const addressGroup = this.ce('div', {
      class: 'row form-group'
    });
    const CareGroup = this.createCareD();//
    const LvlGroup = this.createLvlD();//
    const StreetGroup = this.createStreetD();//
    const SuburbGroup = this.createSuburbD();//
    const StateGroup = this.createStateD();//
    const PostCodeGroup = this.createPostCodeD();//
    const CountryGroup = this.createCountryD();//
    const FullAddressGroup = this.createFullAddressD();//
    addressGroup.appendChild(CareGroup);
    addressGroup.appendChild(LvlGroup);
    addressGroup.appendChild(StreetGroup);
    addressGroup.appendChild(SuburbGroup);
    addressGroup.appendChild(StateGroup);
    addressGroup.appendChild(PostCodeGroup);
    addressGroup.appendChild(CountryGroup);
    addressGroup.appendChild(FullAddressGroup);
    container.appendChild(addressGroup);
    this.errorContainer = container;
  }
  createCareD() {
    const hide = this.component.hidecc ? 'none' : 'block';
    const label = this.component.cclabel ? this.component.cclabel : 'Care of (C/-)';
    const placeholder = this.component.ccPlacehoder ? this.component.ccPlacehoder : '';
    const id = `Care-${this.id}`;
    const careClass = this.component.hidelvl ? 'col-12' : 'col-md-6 col-lg-6 col-xs-12 col-sm-12';
    const CareGroup = this.ce('div', {
      class: careClass,
      style: `display:${hide};`
    });
    const CareGrouplabel = this.ce('label', {
      style: 'font-weight: bold;padding-top: 10px;'
    });
    CareGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
      id
    });
    this.addInput(inputL, InputWrapper);
    CareGroup.appendChild(CareGrouplabel);
    CareGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative;'
      });
      const tipName = `${this.component.key}_Addr_Co_txt`;
      tipsDiv.innerHTML = tipName;
      CareGroup.appendChild(tipsDiv);
    }
    return CareGroup;
  }
  createLvlD() {
    const hide = this.component.hidelvl ? 'none' : 'block';
    const label = this.component.lvllabel ? this.component.lvllabel : 'Bldg, Floor, Lvl';
    const placeholder = this.component.lvlPlacehoder ? this.component.lvlPlacehoder : '';
    const id = `Lvl-${this.id}`;
    const lvlClass = this.component.hidecc ? 'col-12' : 'col-md-6 col-lg-6 col-xs-12 col-sm-12';
    const LvlGroup = this.ce('div', {
      class: lvlClass,
      style: `display:${hide};`,
      placeholder: placeholder
    });
    const LvlGrouplabel = this.ce('label', {
      style: 'font-weight: bold;padding-top: 10px;'
    });
    LvlGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      id
    });
    this.addInput(inputL, InputWrapper);
    LvlGroup.appendChild(LvlGrouplabel);
    LvlGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative;'
      });
      const tipName = `${this.component.key}_Addr_Level_txt`;
      tipsDiv.innerHTML = tipName;
      LvlGroup.appendChild(tipsDiv);
    }
    return LvlGroup;
  }
  createStreetD() {
    const hide = this.component.hidestreet ? 'none' : 'block';
    const label = this.component.streetlabel ? this.component.streetlabel : 'Street No & Name (or Extended address)';
    const placeholder = this.component.streetPlacehoder ? this.component.streetPlacehoder : '';
    const id = `Street-${this.id}`;
    const StreetGroup = this.ce('div', {
      class: 'col-md-12 col-lg-12 col-xs-12 col-sm-12',
      style: `display:${hide};`
    });
    const StreetGrouplabel = this.ce('label', {
      class: `${this.component.requiredstreet ? 'field-required' : ''}`,
      style: 'font-weight: bold;padding-top: 10px;'
    });
    StreetGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
      id
    });
    this.addInput(inputL, InputWrapper);
    StreetGroup.appendChild(StreetGrouplabel);
    StreetGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative;'
      });
      const tipName = `${this.component.key}_Addr_1_txt`;
      tipsDiv.innerHTML = tipName;
      StreetGroup.appendChild(tipsDiv);
    }
    return StreetGroup;
  }
  createFullAddressD() {
    const hide = this.component.hidefulladdress ? 'none' : 'block';
    const id = `FullAddress-${this.id}`;
    const FullAddressGroup = this.ce('div', {
      class: 'col-md-12 col-lg-12 col-xs-12 col-sm-12',
      style: `display:${hide};`
    });
    const FullAddressGrouplabel = this.ce('label', {
      style: 'font-weight: bold;padding-top: 10px;'
    });
    //   FullAddressGrouplabel.appendChild(this.text('Street No & Name'));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      readonly: 1,
      id
    });
    this.addInput(inputL, InputWrapper);
    FullAddressGroup.appendChild(FullAddressGrouplabel);
    FullAddressGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative;'
      });
      const tipName = `${this.component.key}_Addr_txt`;
      tipsDiv.innerHTML = tipName;
      FullAddressGroup.appendChild(tipsDiv);
    }
    return FullAddressGroup;
  }
  createSuburbD() {
    const hide = this.component.hidesuburb ? 'none' : 'block';
    const label = this.component.suburblabel ? this.component.suburblabel : 'Suburb / City';
    const placeholder = this.component.suburbPlacehoder ? this.component.suburbPlacehoder : '';
    const id = `Suburb-${this.id}`;
    const subClass = this.component.hidestate ? 'col-12' : 'col-md-6 col-lg-6 col-xs-12 col-sm-12';
    const SuburbGroup = this.ce('div', {
      class: subClass,
      style: `display:${hide};`
    });
    const SuburbGrouplabel = this.ce('label', {
      class: `${this.component.requiredsuburb ? 'field-required' : ''}`,
      style: 'font-weight: bold;padding-top: 10px;'
    });
    SuburbGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
      id
    });
    this.addInput(inputL, InputWrapper);
    SuburbGroup.appendChild(SuburbGrouplabel);
    SuburbGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative;'
      });
      const tipName = `${this.component.key}_Addr_Suburb_txt`;
      tipsDiv.innerHTML = tipName;
      SuburbGroup.appendChild(tipsDiv);
    }
    return SuburbGroup;
  }
  createStateD() {
    const hide = this.component.hidestate ? 'none' : 'block';
    const label = this.component.statelabel ? this.component.statelabel : 'State';
    const placeholder = this.component.statePlacehoder ? this.component.statePlacehoder : '';
    const id = `State-${this.id}`;
    const stateClass = this.component.hidesuburb ? 'col-12' : 'col-md-6 col-lg-6 col-xs-12 col-sm-12';
    const StateGroup = this.ce('div', {
      class: stateClass,
      style: `display:${hide};`
    });
    const StateGrouplabel = this.ce('label', {
      class: `${this.component.requiredstate ? 'field-required' : ''}`,
      style: 'font-weight: bold;padding-top: 10px;'
    });
    StateGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
      id
    });
    this.addInput(inputL, InputWrapper);
    StateGroup.appendChild(StateGrouplabel);
    StateGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative'
      });
      const tipName = `${this.component.key}_Addr_State_txt`;
      tipsDiv.innerHTML = tipName;
      StateGroup.appendChild(tipsDiv);
    }
    return StateGroup;
  }
  createPostCodeD() {
    const hide = this.component.hidepostcode ? 'none' : 'block';
    const label = this.component.postcodelabel ? this.component.postcodelabel : 'Zip / PostCode';
    const placeholder = this.component.postcodePlacehoder ? this.component.postcodePlacehoder : '';
    const id = `PostCode-${this.id}`;
    const postClass = this.component.hidecountry ? 'col-12' : 'col-md-6 col-lg-6 col-xs-12 col-sm-12';
    const PostCodeGroup = this.ce('div', {
      class: postClass,
      style: `display:${hide};`
    });
    const PostCodeGrouplabel = this.ce('label', {
      class: `${this.component.requiredpostcode ? 'field-required' : ''}`,
      style: 'font-weight: bold;padding-top: 10px;'
    });
    PostCodeGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputL = this.ce('input', {
      type: 'text',
      class: 'form-control',
      placeholder: placeholder,
      id
    });
    this.addInput(inputL, InputWrapper);
    PostCodeGroup.appendChild(PostCodeGrouplabel);
    PostCodeGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative'
      });
      const tipName = `${this.component.key}_Addr_PC_txt`;
      tipsDiv.innerHTML = tipName;
      PostCodeGroup.appendChild(tipsDiv);
    }
    return PostCodeGroup;
  }
  createCountryD() {
    const hide = this.component.hidecountry ? 'none' : 'block';
    const label = this.component.countrylabel ? this.component.countrylabel : 'Country';
    const id = `Country-${this.id}`;
    const counClass = this.component.hidepostcode ? 'col-12' : 'col-md-6 col-lg-6 col-xs-12 col-sm-12';
    const CountryGroup = this.ce('div', {
      class: counClass,
      style: `display:${hide};`
    });
    const CountryGrouplabel = this.ce('label', {
      class: `${this.component.requiredcountry ? 'field-required' : ''}`,
      style: 'font-weight: bold;padding-top: 10px;'
    });
    CountryGrouplabel.appendChild(this.text(label));
    const InputWrapper = this.ce('div');
    const inputT = this.ce('select', {
      class: 'form-control',
      id
    });
    this.selectOptions(inputT, 'countryOption', this.countryList);
    this.addInput(inputT, InputWrapper);
    CountryGroup.appendChild(CountryGrouplabel);
    CountryGroup.appendChild(InputWrapper);
    if (this.options.builder) {
      const tipsDiv = this.ce('span', {
        class: 'component-btn-group',
        style: 'bottom: 3em;color: #856404; background-color: #fff3cd; border-color: #ffeeba;position:relative;'
      });
      const tipName = `${this.component.key}_Addr_Country_cho`;
      tipsDiv.innerHTML = tipName;
      CountryGroup.appendChild(tipsDiv);
    }
    return CountryGroup;
  }
  autofill(adrAddress) {
    this.clearSearch();
    const addressArray = adrAddress.split('</span>');
    for (const i in addressArray) {
      const partAddress = this.selectAddressPart(addressArray[i]);
      let temp = [];
      let index = 0;
      switch (partAddress) {
        case 'post-office-box':
          break;
        case 'street-address':
          var add = this.addresses[0];
          var longstreet = '';
          var streetnum = '';
          add.address_components.forEach(function(item, index) {
            item.types.forEach(function(itemtype, index) {
              if (itemtype === 'route') {
                longstreet = item.long_name;
              }
              else if (itemtype === 'street_number') {
                streetnum = item.long_name;
              }
            });
          });
          temp = addressArray[i].split('<span class="street-address">');
          index = temp.length - 1;
          // Build Level, Buidling, Unit
          var lvl = this.addresses[0].name;
          var blglvl = temp[0] === undefined ? '' : temp[0];
          if (blglvl === '' && (this.isSubpremise(lvl)) && (lvl.indexOf(longstreet) === -1)) {
            blglvl = this.addresses[0].name.concat(', ');
          }
          blglvl = blglvl.substr(0, blglvl.indexOf('/') + 1);
          this.inputs[2].value = blglvl;
          this.inputs[3].value = `${streetnum} ${longstreet}`.replace(/(^\s*)|(\s*$)/g, '');
          break;
        case 'extended-address':
          temp = addressArray[i].split('<span class="extended-address">');
          index = temp.length - 1;
          this.inputs[3].value += this.inputs[3].value === '' ? temp[index]: `, ${temp[index]}`;
          break;
        case 'locality':
          temp = addressArray[i].split('<span class="locality">');
          index = temp.length - 1;
          this.inputs[4].value = temp[index];
          break;
        case 'region':
          temp = addressArray[i].split('<span class="region">');
          index = temp.length - 1;
          this.inputs[5].value = temp[index];
          break;
        case 'postal-code':
          temp = addressArray[i].split('<span class="postal-code">');
          index = temp.length - 1;
          this.inputs[6].value = temp[index];
          break;
        case 'country-name':
          temp = addressArray[i].split('<span class="country-name">');
          index = temp.length - 1;
          this.inputs[7].value = temp[index];
          break;
      }
    }
    if (this.inputs.length > 0) {
      this.inputs[8].value = this.generateFullAddress();//
    }
  }
  generateFullAddress() {
    const hidePart = !(this.component.hidecc && this.component.hidelvl && this.component.hidestreet && this.component.hidesuburb &&
    this.component.hidestate && this.component.hidepostcode && this.component.hidecountry);
    const input1 = (hidePart && this.component.hidecc) || this.inputs[1].value === undefined ? '' : this.inputs[1].value;
    const input2 = (hidePart && this.component.hidelvl) || this.inputs[2].value === undefined ? '' : this.inputs[2].value;
    const input3 = (hidePart && this.component.hidestreet) || this.inputs[3].value === undefined ? '' : this.inputs[3].value;
    const input4 = (hidePart && this.component.hidesuburb) || this.inputs[4].value === undefined ? '' : this.inputs[4].value;
    const input5 = (hidePart && this.component.hidestate) || this.inputs[5].value === undefined ? '' : this.inputs[5].value;
    const input6 = (hidePart && this.component.hidepostcode) || this.inputs[6].value === undefined ? '' : this.inputs[6].value;
    const input7 = (hidePart && this.component.hidecountry) || this.inputs[7].value === undefined ? '' : this.inputs[7].value;
    let fulladdress = '';
    if (!this.isEmpty(input1) || !this.isEmpty(input2) || !this.isEmpty(input3)) {
      fulladdress = `${input1} ${input2} ${input3}, `;
    }
    if (!this.isEmpty(input4) || !this.isEmpty(input5) || !this.isEmpty(input6)) {
      if (!this.isEmpty(input7)) {
        fulladdress += `${input4} ${input5} ${input6}, ${input7}`;
      }
      else {
        fulladdress += `${input4} ${input5} ${input6}`;
      }
    }
    else {
      if (!this.isEmpty(input7)) {
        fulladdress += `${input7}`;
      }
    }
   fulladdress = fulladdress.replace(/(^\s*)|(\s*$)/g, '').replace(/^,/g, '').replace(/\s*,/g, ',').replace(/,$/g, '').replace('  ', ' ').replace('  ', ' ');
   return this.addBldg(fulladdress, this.inputs[0].value);
  }
/**
   * Start the autocomplete and the input listeners
   * @param fulladdress
   *   The autocomponent input value
   * @param oldaddress
   *   The address returned by google api
   */
  addBldg(fulladdress, oldaddress) {
    // NO LONGER NEEDED
    //if (/[^\s]+/.exec(oldaddress) != null) {
    //  var bldgsubs = /[^\s]+/.exec(oldaddress);
    //  if (typeof bldgsubs[0] !== 'undefined' && bldgsubs[0] !== null) {
    //    var bldg = bldgsubs[0];
    //    if ((!fulladdress.replace(' ', '').includes(bldg)) && /\d+\/\d+/.test(bldg)) {
    //      this.inputs[2].value = bldg.substr(0, bldg.indexOf('/') + 1);
    //      var streetnum = bldg.substr(bldg.indexOf('/') + 1);
    //      this.inputs[3].value = streetnum.concat(' ', this.inputs[3].value);
    //    }
    //  }
    //}
    return fulladdress;
  }
  clearSearch() {
    this.inputs[1].value = '';
    this.inputs[2].value = '';
    this.inputs[3].value = '';
    this.inputs[4].value = '';
    this.inputs[5].value = '';
    this.inputs[6].value = '';
    this.inputs[7].value = '';
    this.inputs[8].value = '';
  }
  selectAddressPart(partAddress) {
    const propertyList = ['post-office-box', 'street-address', 'extended-address', 'locality', 'region', 'postal-code', 'country-name'];
    for (const i in propertyList) {
      if (_.includes(partAddress, propertyList[i])) return propertyList[i];
      if (propertyList[i] === '') return '';
    }
  }
  onChange(flags, fromRoot) {
    if (this.inputs.length > 0) {
      this.inputs[8].value = this.generateFullAddress();
    }
    super.onChange(flags, fromRoot);
  }
  /**
   * Start the autocomplete and the input listeners
   *
   * @param input
   *   The input field
   * @param autoCompleteOptions
   *   The default option for the autocompletion
   */
  autoCompleteInit(input, autoCompleteOptions) {
    // Set attribute autoComplete to off
    input.setAttribute('autocomplete', 'off');
    console.log('in the autocomplete init');
    // Init suggestions list
    this.autoCompleteSuggestions = [];

    // Start Google AutocompleteService
    const autoComplete = new google.maps.places.AutocompleteService();

    // Create suggestions container
    const suggestionContainer = document.createElement('div');
    suggestionContainer.classList.add('pac-container', 'pac-logo');
    input.parentNode.appendChild(suggestionContainer);

    // Add listener on input field for input event
    this.addEventListener(input, 'input', () => {
      if (input.value) {
        const options = {
          input: input.value
        };
        autoComplete.getPlacePredictions(_.defaultsDeep(options, autoCompleteOptions),
          (suggestions, status) => {
            this.autoCompleteDisplaySuggestions(suggestions, status, suggestionContainer, input);
          });
      }
      else {
        this.autoCompleteCleanSuggestions(suggestionContainer);
        suggestionContainer.style.display = 'none';
      }
    });
    // Add listener on input field for blur event
    this.addEventListener(input, 'blur', () => {
      // Delay to allow click on suggestion list
      _.delay(() => {
        suggestionContainer.style.display = 'none';
      }, 100);
    });
    // Add listener on input field for focus event
    this.addEventListener(input, 'focus', () => {
      if (suggestionContainer.childElementCount) {
        suggestionContainer.style.display = 'block';
      }
    });
    // Add listener on input field for focus event
    this.addEventListener(window, 'resize', () => {
      // Set the same width as input field
      suggestionContainer.style.width = `${input.offsetWidth}px`;
    });
    // Add listiner on input field for key event
    this.autoCompleteKeyboardListener(suggestionContainer, input);
  }

  /**
   * Add listiner on input field for key event
   *
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteKeyboardListener(suggestionContainer, input) {
    this.autoCompleteKeyCodeListener = (event) => {
      if (input.value) {
        switch (event.keyCode) {
          case 38:
            // UP
            this.autoCompleteKeyUpInteraction(suggestionContainer, input);
            break;

          case 40:
            // DOWN
            this.autoCompleteKeyDownInteraction(suggestionContainer, input);
            break;

          case 9:
            // TAB
            this.autoCompleteKeyValidationInteraction(suggestionContainer, input);
            break;

          case 13:
            // ENTER
            this.autoCompleteKeyValidationInteraction(suggestionContainer, input);
            break;
        }
      }
    };

    this.addEventListener(input, 'keydown', this.autoCompleteKeyCodeListener);
  }

  /**
   * Action when key up is trigger
   *
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteKeyUpInteraction(suggestionContainer, input) {
    const elementSelected = document.querySelector('.pac-item-selected');
    if (!elementSelected) {
      // Returns the bottom of the list.
      return this.autoCompleteListDecorator(suggestionContainer.lastChild, input);
    }
    else {
      // Transverse the list in reverse order.
      const previousSibling = elementSelected.previousSibling;
      if (previousSibling) {
        this.autoCompleteListDecorator(previousSibling, input);
      }
      else {
        // Return to input value
        elementSelected.classList.remove('pac-item-selected');
        input.value = this.autoCompleteInputValue;
      }
    }
  }

  /**
   * Action when key down is trigger
   *
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteKeyDownInteraction(suggestionContainer, input) {
    const elementSelected = document.querySelector('.pac-item-selected');
    if (!elementSelected) {
      // Start at the top of the list.
      if (suggestionContainer.firstChild) {
        return this.autoCompleteListDecorator(suggestionContainer.firstChild, input);
      }
    }
    else {
      // Transverse the list from top down.
      const nextSibling = elementSelected.nextSibling;
      if (nextSibling) {
        this.autoCompleteListDecorator(nextSibling, input);
      }
      else {
        // Return to input value
        elementSelected.classList.remove('pac-item-selected');
        input.value = this.autoCompleteInputValue;
      }
    }
  }

  /**
   * Action when validation is trigger
   *
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteKeyValidationInteraction(suggestionContainer, input) {
    const elementSelected = document.querySelector('.pac-item-selected');
    if (elementSelected) {
      for (const suggestion of this.autoCompleteSuggestions) {
        const content = elementSelected.textContent || elementSelected.innerText;
        if (content === suggestion.description) {
          this.autoCompleteServiceListener(suggestion, suggestionContainer, input);
        }
      }
      elementSelected.classList.remove('pac-item-selected');
    }
  }

  /**
   * Highlight suggestion selected
   *
   * @param item
   *   Item selected in suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteListDecorator(item, input) {
    const elementSelected = document.querySelector('.pac-item-selected');
    if (elementSelected) {
      elementSelected.classList.remove('pac-item-selected');
    }
    input.value = item.textContent;
    item.classList.add('pac-item-selected');
  }

  /**
   * Filter method to return if the suggestion should be displayed
   *
   * @param data
   *   Data to check
   * @returns {Boolean}
   */
  autoCompleteFilterSuggestion(data) {
    const result = this.evaluate(this.component.map.autoCompleteFilter, {
      show: true,
      data
    }, 'show');
    if (result === null) {
      return true;
    }
    return result.toString() === 'true';
  }

  /**
   * Clean suggestions list
   *
   * @param suggestionContainer
   *   Container tag
   */
  autoCompleteCleanSuggestions(suggestionContainer) {
    // Clean click listener
    for (const suggestion of this.autoCompleteSuggestions) {
      suggestion.item.removeEventListener('click', suggestion.clickListener);
    }
    this.autoCompleteSuggestions = [];

    // Delete current suggestion list
    while (suggestionContainer.firstChild) {
      suggestionContainer.removeChild(suggestionContainer.firstChild);
    }
  }

  /**
   * Display suggestions when API returns value
   *
   * @param suggestions
   *   Suggestions returned
   * @param status
   *   State returned
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteDisplaySuggestions(suggestions, status, suggestionContainer, input) {
    // Set the same width as input field
    suggestionContainer.style.width = `${input.offsetWidth}px`;

    // Set the default input value
    this.autoCompleteInputValue = input.value;

    this.autoCompleteCleanSuggestions(suggestionContainer);
    if (status !== google.maps.places.PlacesServiceStatus.OK) {
      suggestionContainer.style.display = 'none';
      return;
    }

    for (const suggestion of suggestions) {
      if (this.autoCompleteFilterSuggestion(suggestion)) {
        this.autoCompleteSuggestions.push(suggestion);
        this.autoCompleteSuggestionBuilder(suggestion, suggestionContainer, input);
      }
    }

    if (!suggestionContainer.childElementCount) {
      this.autoCompleteCleanSuggestions(suggestionContainer);
      suggestionContainer.style.display = 'none';
    }
    else {
      suggestionContainer.style.display = 'block';
    }
  }

  /**
   * Draw a suggestion in the list
   *
   * @param suggestion
   *   Suggestion to draw
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  /* eslint-disable max-depth, max-statements */
  autoCompleteSuggestionBuilder(suggestion, suggestionContainer, input) {
    const item = document.createElement('div');
    item.classList.add('pac-item');

    const itemLogo = document.createElement('span');
    itemLogo.classList.add('pac-icon', 'pac-icon-marker');
    item.appendChild(itemLogo);

    // Draw Main part
    const itemMain = document.createElement('span');
    itemMain.classList.add('pac-item-query');
    if (suggestion.structured_formatting.main_text_matched_substrings) {
      const matches = suggestion.structured_formatting.main_text_matched_substrings;
      for (const k in matches) {
        const part = matches[k];
        if (k === 0 && part.offset > 0) {
          itemMain.appendChild(document.createTextNode(
            suggestion.structured_formatting.main_text.substring(0, part.offset)));
        }

        const itemBold = document.createElement('span');
        itemBold.classList.add('pac-matched');
        itemBold.appendChild(document.createTextNode(
          suggestion.structured_formatting.main_text.substring(part.offset, (part.offset + part.length))));
        itemMain.appendChild(itemBold);

        if (k === (matches.length - 1)) {
          const content = suggestion.structured_formatting.main_text.substring((part.offset + part.length));
          if (content.length > 0) {
            itemMain.appendChild(document.createTextNode(content));
          }
        }
      }
    }
    else {
      itemMain.appendChild(document.createTextNode(suggestion.structured_formatting.main_text));
    }
    item.appendChild(itemMain);

    // Draw secondary part
    if (suggestion.structured_formatting.secondary_text) {
      const itemSecondary = document.createElement('span');
      if (suggestion.structured_formatting.secondary_text_matched_substrings) {
        const matches = suggestion.structured_formatting.secondary_text_matched_substrings;
        for (const k in matches) {
          const part = matches[k];
          if (k === 0 && part.offset > 0) {
            itemSecondary.appendChild(document.createTextNode(
              suggestion.structured_formatting.secondary_text.substring(0, part.offset)));
          }

          const itemBold = document.createElement('span');
          itemBold.classList.add('pac-matched');
          itemBold.appendChild(document.createTextNode(
            suggestion.structured_formatting.secondary_text.substring(part.offset, (part.offset + part.length))));
          itemSecondary.appendChild(itemBold);

          if (k === (matches.length - 1)) {
            const content = suggestion.structured_formatting.secondary_text.substring((part.offset + part.length));
            if (content.length > 0) {
              itemSecondary.appendChild(document.createTextNode(content));
            }
          }
        }
      }
      else {
        itemSecondary.appendChild(document.createTextNode(suggestion.structured_formatting.secondary_text));
      }
      item.appendChild(itemSecondary);
    }

    suggestionContainer.appendChild(item);

    const clickListener = () => {
      input.value = suggestion.description;
      this.autoCompleteInputValue = suggestion.description;
      this.autoCompleteServiceListener(suggestion, suggestionContainer, input);
    };
    suggestion.clickListener = clickListener;
    suggestion.item = item;
    if ('addEventListener' in item) {
      item.addEventListener('click', clickListener, false);
    }
    else if ('attachEvent' in item) {
      item.attachEvent('onclick', clickListener);
    }
  }
  /* eslint-enable max-depth, max-statements */

  /**
   * Get detailed information and set it as value
   *
   * @param suggestion
   *   Suggestion to draw
   * @param suggestionContainer
   *   Suggestions container
   * @param input
   *   Input field to listen
   */
  autoCompleteServiceListener(suggestion, suggestionContainer, input) {
    const service = new google.maps.places.PlacesService(input);
    service.getDetails({
      placeId: suggestion.place_id,
    }, (place, status) => {
      if (status === google.maps.places.PlacesServiceStatus.OK) {
        this.setValue(place);
      }
    });
  }

  addInput(input, container) {
    if (!input) {
      return;
    }

    if (input && container) {
      if (input.getAttribute('id')) {
        input = container.appendChild(input);
      }
      else {
        const InputWrapper = this.ce('div', { class: 'input-group' });
        const InputAppend = this.ce('span', { class: 'input-group-text' });
        const searchButton = this.ce('span', { class: 'fa fa-search' });
        InputAppend.appendChild(searchButton);
        InputWrapper.appendChild(InputAppend);
        input.setAttribute('placeholder', 'Search For Address');
        InputWrapper.appendChild(input);
        container.appendChild(InputWrapper);
      }
    }

    this.inputs.push(input);
    this.hook('input', input, container);
    this.addFocusBlurEvents(input);
    this.addInputEventListener(input);
    this.addInputSubmitListener(input);
    //super.addInput(input, container);
    if (input.getAttribute('id')) {
      // console.log(`input--id----${input.getAttribute('id')}`);
    }
    else {
      Formio.libraryReady('googleMaps').then(() => {
        let autoCompleteOptions = {};
        if (this.component.map) {
          autoCompleteOptions = this.component.map.autoCompleteOptions || {};
          if (autoCompleteOptions.location) {
            const {
              lat,
              lng
            } = autoCompleteOptions.location;
            autoCompleteOptions.location = new google.maps.LatLng(lat, lng);
          }
        }

        if (this.component.map && this.component.map.autoCompleteFilter) {
          // Call custom autoComplete to filter suggestions
          this.autoCompleteInit(input, autoCompleteOptions);
        }
        else {
          const autocomplete = new google.maps.places.Autocomplete(input);
          autocomplete.addListener('place_changed', () => this.setValue(autocomplete.getPlace()));
        }
      });
    }
  }

  elementInfo() {
    const info = super.elementInfo();
    info.attr.class += ' address-search';
    return info;
  }

  getView(value) {
    return _.get(value, 'formatted_address', '');
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-addressgroup ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  validateRequired() {
    if (this.options.name === 'data[defaultValue]') {
      return true;
    }
    const result = !(this.component.validate.required && this.isEmpty(this.inputs[0].value)) &&
      !(this.component.requiredstreet && this.isEmpty(this.inputs[3].value)) &&
      !(this.component.requiredsuburb && this.isEmpty(this.inputs[4].value)) &&
      !(this.component.requiredstate && this.isEmpty(this.inputs[5].value)) &&
      !(this.component.requiredpostcode && this.isEmpty(this.inputs[6].value)) &&
      !(this.component.requiredcountry && this.isEmpty(this.inputs[7].value));
    return result;
  }
  /**
* Check the address string is subpremiss or not
* @param address string
*/
  isSubpremise(address) {
    var add = address.replace(' ','').toLowerCase();
    if ((add.indexOf('unit') !== -1 && /^unit\d+$/.test(add)) || (add.indexOf('flat') !== -1 && /^flat\d+$/.test(add)) || (add.indexOf('level') !== -1 && /^level\d+$/.test(add)) || (add.indexOf('floor') !== -1 && /^floor\d+$/.test(add))) {
      return true;
    }
    return false;
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
    (this.component.validate.required && this.isEmpty(this.inputs[0].value)) ? this.addClass(this.performInputMapping(this.inputs[0]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[0]), 'is-invalid');
    (this.component.requiredstreet && this.isEmpty(this.inputs[3].value)) ? this.addClass(this.performInputMapping(this.inputs[3]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[3]), 'is-invalid');
    (this.component.requiredsuburb && this.isEmpty(this.inputs[4].value)) ? this.addClass(this.performInputMapping(this.inputs[4]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[4]), 'is-invalid');
    (this.component.requiredstate && this.isEmpty(this.inputs[5].value)) ? this.addClass(this.performInputMapping(this.inputs[5]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[5]), 'is-invalid');
    (this.component.requiredpostcode && this.isEmpty(this.inputs[6].value)) ? this.addClass(this.performInputMapping(this.inputs[6]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[6]), 'is-invalid');
    (this.component.requiredcountry && this.isEmpty(this.inputs[7].value)) ? this.addClass(this.performInputMapping(this.inputs[7]), 'is-invalid') : this.removeClass(this.performInputMapping(this.inputs[7]), 'is-invalid');

    if (dirty && this.options.highlightErrors) {
      //this.addClass(this.element, 'alert alert-danger');
      this.addClass(this.element, 'alert-danger');
    }
  }
}
