import SelectComponent from '../select/Select';
import countryList from './countryList';
import _ from 'lodash';

export default class CountryComponent extends SelectComponent {
  static schema(...extend) {
    return SelectComponent.schema({
      type: 'country',
      label: 'Country',
      key: 'country',
      data: {
        json: '[{"value":"Afghanistan","label":"Afghanistan","mtext":"AF"},{"value":"Åland Islands","label":"Åland Islands","mtext":"AX"},{"value":"Albania","label":"Albania","mtext":"AL"},{"value":"Algeria","label":"Algeria","mtext":"DZ"},{"value":"American Samoa","label":"American Samoa","mtext":"AS"},{"value":"Andorra","label":"Andorra","mtext":"AD"},{"value":"Angola","label":"Angola","mtext":"AO"},{"value":"Anguilla","label":"Anguilla","mtext":"AI"},{"value":"Antarctica","label":"Antarctica","mtext":"AQ"},{"value":"Antigua and Barbuda","label":"Antigua and Barbuda","mtext":"AG"},{"value":"Argentina","label":"Argentina","mtext":"AR"},{"value":"Armenia","label":"Armenia","mtext":"AM"},{"value":"Aruba","label":"Aruba","mtext":"AW"},{"value":"Australia","label":"Australia","mtext":"AU"},{"value":"Austria","label":"Austria","mtext":"AT"},{"value":"Azerbaijan","label":"Azerbaijan","mtext":"AZ"},{"value":"Bahamas","label":"Bahamas","mtext":"BS"},{"value":"Bahrain","label":"Bahrain","mtext":"BH"},{"value":"Bangladesh","label":"Bangladesh","mtext":"BD"},{"value":"Barbados","label":"Barbados","mtext":"BB"},{"value":"Belarus","label":"Belarus","mtext":"BY"},{"value":"Belgium","label":"Belgium","mtext":"BE"},{"value":"Belize","label":"Belize","mtext":"BZ"},{"value":"Benin","label":"Benin","mtext":"BJ"},{"value":"Bermuda","label":"Bermuda","mtext":"BM"},{"value":"Bhutan","label":"Bhutan","mtext":"BT"},{"value":"Bolivia, Plurinational State of","label":"Bolivia, Plurinational State of","mtext":"BO"},{"value":"Bonaire, Sint Eustatius and Saba","label":"Bonaire, Sint Eustatius and Saba","mtext":"BQ"},{"value":"Bosnia and Herzegovina","label":"Bosnia and Herzegovina","mtext":"BA"},{"value":"Botswana","label":"Botswana","mtext":"BW"},{"value":"Bouvet Island","label":"Bouvet Island","mtext":"BV"},{"value":"Brazil","label":"Brazil","mtext":"BR"},{"value":"British Indian Ocean Territory","label":"British Indian Ocean Territory","mtext":"IO"},{"value":"Brunei Darussalam","label":"Brunei Darussalam","mtext":"BN"},{"value":"Bulgaria","label":"Bulgaria","mtext":"BG"},{"value":"Burkina Faso","label":"Burkina Faso","mtext":"BF"},{"value":"Burundi","label":"Burundi","mtext":"BI"},{"value":"Cambodia","label":"Cambodia","mtext":"KH"},{"value":"Cameroon","label":"Cameroon","mtext":"CM"},{"value":"Canada","label":"Canada","mtext":"CA"},{"value":"Cape Verde","label":"Cape Verde","mtext":"CV"},{"value":"Cayman Islands","label":"Cayman Islands","mtext":"KY"},{"value":"Central African Republic","label":"Central African Republic","mtext":"CF"},{"value":"Chad","label":"Chad","mtext":"TD"},{"value":"Chile","label":"Chile","mtext":"CL"},{"value":"China","label":"China","mtext":"CN"},{"value":"Christmas Island","label":"Christmas Island","mtext":"CX"},{"value":"Cocos (Keeling) Islands","label":"Cocos (Keeling) Islands","mtext":"CC"},{"value":"Colombia","label":"Colombia","mtext":"CO"},{"value":"Comoros","label":"Comoros","mtext":"KM"},{"value":"Congo","label":"Congo","mtext":"CG"},{"value":"Congo, the Democratic Republic of the","label":"Congo, the Democratic Republic of the","mtext":"CD"},{"value":"Cook Islands","label":"Cook Islands","mtext":"CK"},{"value":"Costa Rica","label":"Costa Rica","mtext":"CR"},{"value":"Côte d\'Ivoire","label":"Côte d\'Ivoire","mtext":"CI"},{"value":"Croatia","label":"Croatia","mtext":"HR"},{"value":"Cuba","label":"Cuba","mtext":"CU"},{"value":"Curaçao","label":"Curaçao","mtext":"CW"},{"value":"Cyprus","label":"Cyprus","mtext":"CY"},{"value":"Czech Republic","label":"Czech Republic","mtext":"CZ"},{"value":"Denmark","label":"Denmark","mtext":"DK"},{"value":"Djibouti","label":"Djibouti","mtext":"DJ"},{"value":"Dominica","label":"Dominica","mtext":"DM"},{"value":"Dominican Republic","label":"Dominican Republic","mtext":"DO"},{"value":"Ecuador","label":"Ecuador","mtext":"EC"},{"value":"Egypt","label":"Egypt","mtext":"EG"},{"value":"El Salvador","label":"El Salvador","mtext":"SV"},{"value":"Equatorial Guinea","label":"Equatorial Guinea","mtext":"GQ"},{"value":"Eritrea","label":"Eritrea","mtext":"ER"},{"value":"Estonia","label":"Estonia","mtext":"EE"},{"value":"Ethiopia","label":"Ethiopia","mtext":"ET"},{"value":"Falkland Islands (Malvinas)","label":"Falkland Islands (Malvinas)","mtext":"FK"},{"value":"Faroe Islands","label":"Faroe Islands","mtext":"FO"},{"value":"Fiji","label":"Fiji","mtext":"FJ"},{"value":"Finland","label":"Finland","mtext":"FI"},{"value":"France","label":"France","mtext":"FR"},{"value":"French Guiana","label":"French Guiana","mtext":"GF"},{"value":"French Polynesia","label":"French Polynesia","mtext":"PF"},{"value":"French Southern Territories","label":"French Southern Territories","mtext":"TF"},{"value":"Gabon","label":"Gabon","mtext":"GA"},{"value":"Gambia","label":"Gambia","mtext":"GM"},{"value":"Georgia","label":"Georgia","mtext":"GE"},{"value":"Germany","label":"Germany","mtext":"DE"},{"value":"Ghana","label":"Ghana","mtext":"GH"},{"value":"Gibraltar","label":"Gibraltar","mtext":"GI"},{"value":"Greece","label":"Greece","mtext":"GR"},{"value":"Greenland","label":"Greenland","mtext":"GL"},{"value":"Grenada","label":"Grenada","mtext":"GD"},{"value":"Guadeloupe","label":"Guadeloupe","mtext":"GP"},{"value":"Guam","label":"Guam","mtext":"GU"},{"value":"Guatemala","label":"Guatemala","mtext":"GT"},{"value":"Guernsey","label":"Guernsey","mtext":"GG"},{"value":"Guinea","label":"Guinea","mtext":"GN"},{"value":"Guinea-Bissau","label":"Guinea-Bissau","mtext":"GW"},{"value":"Guyana","label":"Guyana","mtext":"GY"},{"value":"Haiti","label":"Haiti","mtext":"HT"},{"value":"Heard Island and McDonald Islands","label":"Heard Island and McDonald Islands","mtext":"HM"},{"value":"Holy See (Vatican City State)","label":"Holy See (Vatican City State)","mtext":"VA"},{"value":"Honduras","label":"Honduras","mtext":"HN"},{"value":"Hong Kong","label":"Hong Kong","mtext":"HK"},{"value":"Hungary","label":"Hungary","mtext":"HU"},{"value":"Iceland","label":"Iceland","mtext":"IS"},{"value":"India","label":"India","mtext":"IN"},{"value":"Indonesia","label":"Indonesia","mtext":"ID"},{"value":"Iran, Islamic Republic of","label":"Iran, Islamic Republic of","mtext":"IR"},{"value":"Iraq","label":"Iraq","mtext":"IQ"},{"value":"Ireland","label":"Ireland","mtext":"IE"},{"value":"Isle of Man","label":"Isle of Man","mtext":"IM"},{"value":"Israel","label":"Israel","mtext":"IL"},{"value":"Italy","label":"Italy","mtext":"IT"},{"value":"Jamaica","label":"Jamaica","mtext":"JM"},{"value":"Japan","label":"Japan","mtext":"JP"},{"value":"Jersey","label":"Jersey","mtext":"JE"},{"value":"Jordan","label":"Jordan","mtext":"JO"},{"value":"Kazakhstan","label":"Kazakhstan","mtext":"KZ"},{"value":"Kenya","label":"Kenya","mtext":"KE"},{"value":"Kiribati","label":"Kiribati","mtext":"KI"},{"value":"Korea, Democratic People\'s Republic of","label":"Korea, Democratic People\'s Republic of","mtext":"KP"},{"value":"Korea, Republic of","label":"Korea, Republic of","mtext":"KR"},{"value":"Kuwait","label":"Kuwait","mtext":"KW"},{"value":"Kyrgyzstan","label":"Kyrgyzstan","mtext":"KG"},{"value":"Lao People\'s Democratic Republic","label":"Lao People\'s Democratic Republic","mtext":"LA"},{"value":"Latvia","label":"Latvia","mtext":"LV"},{"value":"Lebanon","label":"Lebanon","mtext":"LB"},{"value":"Lesotho","label":"Lesotho","mtext":"LS"},{"value":"Liberia","label":"Liberia","mtext":"LR"},{"value":"Libya","label":"Libya","mtext":"LY"},{"value":"Liechtenstein","label":"Liechtenstein","mtext":"LI"},{"value":"Lithuania","label":"Lithuania","mtext":"LT"},{"value":"Luxembourg","label":"Luxembourg","mtext":"LU"},{"value":"Macao","label":"Macao","mtext":"MO"},{"value":"Macedonia, the former Yugoslav Republic of","label":"Macedonia, the former Yugoslav Republic of","mtext":"MK"},{"value":"Madagascar","label":"Madagascar","mtext":"MG"},{"value":"Malawi","label":"Malawi","mtext":"MW"},{"value":"Malaysia","label":"Malaysia","mtext":"MY"},{"value":"Maldives","label":"Maldives","mtext":"MV"},{"value":"Mali","label":"Mali","mtext":"ML"},{"value":"Malta","label":"Malta","mtext":"MT"},{"value":"Marshall Islands","label":"Marshall Islands","mtext":"MH"},{"value":"Martinique","label":"Martinique","mtext":"MQ"},{"value":"Mauritania","label":"Mauritania","mtext":"MR"},{"value":"Mauritius","label":"Mauritius","mtext":"MU"},{"value":"Mayotte","label":"Mayotte","mtext":"YT"},{"value":"Mexico","label":"Mexico","mtext":"MX"},{"value":"Micronesia, Federated States of","label":"Micronesia, Federated States of","mtext":"FM"},{"value":"Moldova, Republic of","label":"Moldova, Republic of","mtext":"MD"},{"value":"Monaco","label":"Monaco","mtext":"MC"},{"value":"Mongolia","label":"Mongolia","mtext":"MN"},{"value":"Montenegro","label":"Montenegro","mtext":"ME"},{"value":"Montserrat","label":"Montserrat","mtext":"MS"},{"value":"Morocco","label":"Morocco","mtext":"MA"},{"value":"Mozambique","label":"Mozambique","mtext":"MZ"},{"value":"Myanmar","label":"Myanmar","mtext":"MM"},{"value":"Namibia","label":"Namibia","mtext":"NA"},{"value":"Nauru","label":"Nauru","mtext":"NR"},{"value":"Nepal","label":"Nepal","mtext":"NP"},{"value":"Netherlands","label":"Netherlands","mtext":"NL"},{"value":"New Caledonia","label":"New Caledonia","mtext":"NC"},{"value":"New Zealand","label":"New Zealand","mtext":"NZ"},{"value":"Nicaragua","label":"Nicaragua","mtext":"NI"},{"value":"Niger","label":"Niger","mtext":"NE"},{"value":"Nigeria","label":"Nigeria","mtext":"NG"},{"value":"Niue","label":"Niue","mtext":"NU"},{"value":"Norfolk Island","label":"Norfolk Island","mtext":"NF"},{"value":"Northern Mariana Islands","label":"Northern Mariana Islands","mtext":"MP"},{"value":"Norway","label":"Norway","mtext":"NO"},{"value":"Oman","label":"Oman","mtext":"OM"},{"value":"Pakistan","label":"Pakistan","mtext":"PK"},{"value":"Palau","label":"Palau","mtext":"PW"},{"value":"Palestinian Territory, Occupied","label":"Palestinian Territory, Occupied","mtext":"PS"},{"value":"Panama","label":"Panama","mtext":"PA"},{"value":"Papua New Guinea","label":"Papua New Guinea","mtext":"PG"},{"value":"Paraguay","label":"Paraguay","mtext":"PY"},{"value":"Peru","label":"Peru","mtext":"PE"},{"value":"Philippines","label":"Philippines","mtext":"PH"},{"value":"Pitcairn","label":"Pitcairn","mtext":"PN"},{"value":"Poland","label":"Poland","mtext":"PL"},{"value":"Portugal","label":"Portugal","mtext":"PT"},{"value":"Puerto Rico","label":"Puerto Rico","mtext":"PR"},{"value":"Qatar","label":"Qatar","mtext":"QA"},{"value":"Réunion","label":"Réunion","mtext":"RE"},{"value":"Romania","label":"Romania","mtext":"RO"},{"value":"Russian Federation","label":"Russian Federation","mtext":"RU"},{"value":"Rwanda","label":"Rwanda","mtext":"RW"},{"value":"Saint Barthélemy","label":"Saint Barthélemy","mtext":"BL"},{"value":"Saint Helena, Ascension and Tristan da Cunha","label":"Saint Helena, Ascension and Tristan da Cunha","mtext":"SH"},{"value":"Saint Kitts and Nevis","label":"Saint Kitts and Nevis","mtext":"KN"},{"value":"Saint Lucia","label":"Saint Lucia","mtext":"LC"},{"value":"Saint Martin (French part)","label":"Saint Martin (French part)","mtext":"MF"},{"value":"Saint Pierre and Miquelon","label":"Saint Pierre and Miquelon","mtext":"PM"},{"value":"Saint Vincent and the Grenadines","label":"Saint Vincent and the Grenadines","mtext":"VC"},{"value":"Samoa","label":"Samoa","mtext":"WS"},{"value":"San Marino","label":"San Marino","mtext":"SM"},{"value":"Sao Tome and Principe","label":"Sao Tome and Principe","mtext":"ST"},{"value":"Saudi Arabia","label":"Saudi Arabia","mtext":"SA"},{"value":"Senegal","label":"Senegal","mtext":"SN"},{"value":"Serbia","label":"Serbia","mtext":"RS"},{"value":"Seychelles","label":"Seychelles","mtext":"SC"},{"value":"Sierra Leone","label":"Sierra Leone","mtext":"SL"},{"value":"Singapore","label":"Singapore","mtext":"SG"},{"value":"Sint Maarten (Dutch part)","label":"Sint Maarten (Dutch part)","mtext":"SX"},{"value":"Slovakia","label":"Slovakia","mtext":"SK"},{"value":"Slovenia","label":"Slovenia","mtext":"SI"},{"value":"Solomon Islands","label":"Solomon Islands","mtext":"SB"},{"value":"Somalia","label":"Somalia","mtext":"SO"},{"value":"South Africa","label":"South Africa","mtext":"ZA"},{"value":"South Georgia and the South Sandwich Islands","label":"South Georgia and the South Sandwich Islands","mtext":"GS"},{"value":"South Sudan","label":"South Sudan","mtext":"SS"},{"value":"Spain","label":"Spain","mtext":"ES"},{"value":"Sri Lanka","label":"Sri Lanka","mtext":"LK"},{"value":"Sudan","label":"Sudan","mtext":"SD"},{"value":"Suriname","label":"Suriname","mtext":"SR"},{"value":"Svalbard and Jan Mayen","label":"Svalbard and Jan Mayen","mtext":"SJ"},{"value":"Swaziland","label":"Swaziland","mtext":"SZ"},{"value":"Sweden","label":"Sweden","mtext":"SE"},{"value":"Switzerland","label":"Switzerland","mtext":"CH"},{"value":"Syrian Arab Republic","label":"Syrian Arab Republic","mtext":"SY"},{"value":"Taiwan, Province of China","label":"Taiwan, Province of China","mtext":"TW"},{"value":"Tajikistan","label":"Tajikistan","mtext":"TJ"},{"value":"Tanzania, United Republic of","label":"Tanzania, United Republic of","mtext":"TZ"},{"value":"Thailand","label":"Thailand","mtext":"TH"},{"value":"Timor-Leste","label":"Timor-Leste","mtext":"TL"},{"value":"Togo","label":"Togo","mtext":"TG"},{"value":"Tokelau","label":"Tokelau","mtext":"TK"},{"value":"Tonga","label":"Tonga","mtext":"TO"},{"value":"Trinidad and Tobago","label":"Trinidad and Tobago","mtext":"TT"},{"value":"Tunisia","label":"Tunisia","mtext":"TN"},{"value":"Turkey","label":"Turkey","mtext":"TR"},{"value":"Turkmenistan","label":"Turkmenistan","mtext":"TM"},{"value":"Turks and Caicos Islands","label":"Turks and Caicos Islands","mtext":"TC"},{"value":"Tuvalu","label":"Tuvalu","mtext":"TV"},{"value":"Uganda","label":"Uganda","mtext":"UG"},{"value":"Ukraine","label":"Ukraine","mtext":"UA"},{"value":"United Arab Emirates","label":"United Arab Emirates","mtext":"AE"},{"value":"United Kingdom","label":"United Kingdom","mtext":"GB"},{"value":"United States","label":"United States","mtext":"US"},{"value":"United States Minor Outlying Islands","label":"United States Minor Outlying Islands","mtext":"UM"},{"value":"Uruguay","label":"Uruguay","mtext":"UY"},{"value":"Uzbekistan","label":"Uzbekistan","mtext":"UZ"},{"value":"Vanuatu","label":"Vanuatu","mtext":"VU"},{"value":"Venezuela, Bolivarian Republic of","label":"Venezuela, Bolivarian Republic of","mtext":"VE"},{"value":"Viet Nam","label":"Viet Nam","mtext":"VN"},{"value":"Virgin Islands, British","label":"Virgin Islands, British","mtext":"VG"},{"value":"Virgin Islands, U.S.","label":"Virgin Islands, U.S.","mtext":"VI"},{"value":"Wallis and Futuna","label":"Wallis and Futuna","mtext":"WF"},{"value":"Western Sahara","label":"Western Sahara","mtext":"EH"},{"value":"Yemen","label":"Yemen","mtext":"YE"},{"value":"Zambia","label":"Zambia","mtext":"ZM"},{"value":"Zimbabwe","label":"Zimbabwe","mtext":"ZW"}]'
        //json:null {"value":" ","label":" ","mtext":""},
      },
      dataSrc: 'json',
      //dataSrc: 'values',
      template: '<span>{{ item.mtext }} - {{ item.label }}</span>',
      valueProperty: '',
      logic: [],
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Country List',
      group: 'basic',
      icon: 'fas fa-globe',
      weight: 70,
      documentation: 'http://help.form.io/userguide/#select',
      schema: CountryComponent.schema()
    };
  }

  get defaultSchema() {
    return CountryComponent.schema();
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    component.data = component.data === undefined ? {} : component.data;
    component.data.json = component.data.json === null ? countryList : component.data.json;
    super(component, options, data);
    this.countryList = countryList;
  }
  setItems(items, fromSearch) {
    super.setItems(items, fromSearch);
  }
  getValue() {
    if (this.viewOnly || this.loading || !this.selectOptions.length) {
      return this.dataValue;
    }
    let value = '';
    if (this.choices) {
      const valueData = this.choices.getValue(true);
      value = this.dataGenerator(valueData);
      // Make sure we don't get the placeholder
      if (
        !this.component.multiple &&
        this.component.placeholder &&
        (value === this.t(this.component.placeholder))
      ) {
        value = '';
      }
    }
    else {
      const values = [];
      _.each(this.selectOptions, (selectOption) => {
        if (selectOption.element && selectOption.element.selected) {
          values.push(selectOption.value);
        }
      });
      value = this.component.multiple ? values : values.shift();
    }
    // Choices will return undefined if nothing is selected. We really want '' to be empty.
    if (value === undefined || value === null || value.value ===' ') {
      value = this.emptyValue;
    }
    return value;
  }
  dataGenerator(value) {
    if (typeof value == 'string') {
      var dataTemp = JSON.parse(countryList);
      for (const i in dataTemp) {
        if (dataTemp[i].value === value) {
          value = dataTemp[i].value;
          this.component.labelName = dataTemp[i].label ? dataTemp[i].label : this.component.labelName;
        }
      }
    }
    return value;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-country ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;
    //this.selectOptions=countryList;
    this.hook('element', this.element);

    return this.element;
  }
}