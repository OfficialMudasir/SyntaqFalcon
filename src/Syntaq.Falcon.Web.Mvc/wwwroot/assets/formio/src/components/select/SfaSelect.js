import SelectComponent from '../select/Select';
import Formio from '../../Formio';
import _ from 'lodash';

export default class SfaSelectComponent extends SelectComponent {
  static schema(...extend) {
    return SelectComponent.schema({
      label: 'Dropdown List',
      key: 'sfaSelect',
      type: 'sfaselect',
      data: {
        values: [],
        json: '',
        url: '',
        resource: '',
        custom: ''
      },
      dataSrc: '',
      labelName:'',//
      valueProperty: '',
      filter: '',
      searchEnabled: true,
      searchField: '',
      minSearch: 0,
      readOnlyValue: false,
      authenticate: false,
      template: '<span>{{ item.label }}</span>',
      selectFields: '',
      customOptions: {},
      logic: []
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Dropdown List',
      group: 'testGroup',
      icon: 'far fa-caret-square-down',
      documentation: 'http://help.form.io/userguide/#select',
      schema: SfaSelectComponent.schema()
    };
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
  }
  get defaultSchema() {
    return SfaSelectComponent.schema();
  }
  /* eslint-disable max-statements */
  updateItems(searchInput, forceUpdate) {
    if (!this.component.data) {
      console.warn(`Select component ${this.key} does not have data configuration.`);
      this.itemsLoadedResolve();
      return;
    }

    // Only load the data if it is visible.
    if (!this.checkConditions()) {
      this.itemsLoadedResolve();
      return;
    }

    switch (this.component.dataSrc) {
      case 'values':
      this.component.valueProperty = 'value';
      this.setItems(this.component.data.values);
      break;
      case 'value':
        this.setItems(this.component.data.value);
        break;
      case 'json':
        this.setItems(this.component.data.json);
        break;
      case 'custom':
        this.updateCustomItems();
        break;
      case 'resource': {
        // If there is no resource, or we are lazyLoading, wait until active.
        if (!this.component.data.resource || (!forceUpdate && !this.active)) {
          return;
        }
        let resourceUrl = this.options.formio ? this.options.formio.formsUrl : `${Formio.getProjectUrl()}/form`;
        resourceUrl += (`/${this.component.data.resource}/submission`);

        try {
          this.loadItems(resourceUrl, searchInput, this.requestHeaders);
        }
        catch (err) {
          console.warn(`Unable to load resources for ${this.key}`);
        }
        break;
      }
      case 'url': {
        if (!forceUpdate && !this.active) {
          // If we are lazyLoading, wait until activated.
          return;
        }
        let url = this.component.data.url;
        let method;
        let body;

        if (url.substr(0, 1) === '/') {
          let baseUrl = Formio.getProjectUrl();
          if (!baseUrl) {
            baseUrl = Formio.getBaseUrl();
          }
          url = baseUrl + this.component.data.url;
        }

        if (!this.component.data.method) {
          method = 'GET';
        }
        else {
          method = this.component.data.method;
          if (method.toUpperCase() === 'POST') {
            body = this.component.data.body;
          }
          else {
            body = null;
          }
        }
        const query = this.component.authenticate ? {} : { noToken: true };
        this.loadItems(url, searchInput, this.requestHeaders, query, method, body);
        break;
      }
    }
  }
  /* eslint-enable max-statements */
  setItems(items, fromSearch) {
    if (this.component.dataSrc === 'url') {
      if (items.result) {
        items = items.result;
      }
    }
    super.setItems(items, fromSearch);
    if (this.component.dataSrc !== '') { // === 'url' || this.component.dataSrc === 'json'
      if (typeof items == 'string') {
        try {
          items = JSON.parse(items);
        }
        catch (err) {
          console.warn(err.message);
          items = [];
        }
      }
      this.dataTemp = items;
    }
    else {
      this.dataTemp = [];
    }
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
    if (value === undefined || value === null) {
      value = '';
    }
    return value;
  }
  dataGenerator(value) {
    if (typeof value == 'string') {
      for (const i in this.dataTemp) {
        if (this.dataTemp[i].value === value) {
          value = this.dataTemp[i].value;
          this.component.labelName = this.dataTemp[i].label ? this.dataTemp[i].label : this.component.labelName;
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
syntaq-component syntaq-component-sfaselect ${this.className}`,
      style: `${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
