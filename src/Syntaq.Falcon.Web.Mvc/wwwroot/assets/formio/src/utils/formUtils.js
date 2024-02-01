import get from 'lodash/get';
import has from 'lodash/has';
import clone from 'lodash/clone';
import forOwn from 'lodash/forOwn';
import isString from 'lodash/isString';
import isNaN from 'lodash/isNaN';
import isNil from 'lodash/isNil';
import isPlainObject from 'lodash/isPlainObject';
import round from 'lodash/round';
import chunk from 'lodash/chunk';
import pad from 'lodash/pad';

/**
 * Determine if a component is a layout component or not.
 *
 * @param {Object} component
 *   The component to check.
 *
 * @returns {Boolean}
 *   Whether or not the component is a layout component.
 */
export function isLayoutComponent(component) {
  return Boolean(
    (component.columns && Array.isArray(component.columns)) ||
    (component.rows && Array.isArray(component.rows)) ||
    (component.components && Array.isArray(component.components))
  );
}

/**
 * Iterate through each component within a form.
 *
 * @param {Object} components
 *   The components to iterate.
 * @param {Function} fn
 *   The iteration function to invoke for each component.
 * @param {Boolean} includeAll
 *   Whether or not to include layout components.
 * @param {String} path
 *   The current data path of the element. Example: data.user.firstName
 * @param {Object} parent
 *   The parent object.
 */
export function eachComponent(components, fn, includeAll, path, parent) {
  if (!components) return;
  path = path || '';
  components.forEach((component) => {
    if (!component) {
      return;
    }
    const hasColumns = component.columns && Array.isArray(component.columns);
    const hasRows = component.rows && Array.isArray(component.rows);
    const hasComps = component.components && Array.isArray(component.components);
    let noRecurse = false;
    const newPath = component.key ? (path ? (`${path}.${component.key}`) : component.key) : '';

    // Keep track of parent references.
    if (parent) {
      // Ensure we don't create infinite JSON structures.
      component.parent = clone(parent);
      delete component.parent.components;
      delete component.parent.componentMap;
      delete component.parent.columns;
      delete component.parent.rows;
    }

    if (includeAll || component.tree || (!hasColumns && !hasRows && !hasComps)) {
      noRecurse = fn(component, newPath);
    }
    const subPath = () => {
      if (
        component.key &&
        !['panel', 'table', 'well', 'columns', 'fieldset', 'tabs', 'form'].includes(component.type) &&
        (
          ['datagrid', 'container', 'editgrid'].includes(component.type) ||
          component.tree
        )
      ) {
        return newPath;
      }
      else if (
        component.key &&
        component.type === 'form'
      ) {
        return `${newPath}.data`;
      }
      return path;
    };

    if (!noRecurse) {
      if (hasColumns) {
        component.columns.forEach((column) =>
          eachComponent(column.components, fn, includeAll, subPath(), parent ? component : null));
      }

      else if (hasRows) {
        component.rows.forEach((row) => {
          if (Array.isArray(row)) {
            row.forEach((column) =>
              eachComponent(column.components, fn, includeAll, subPath(), parent ? component : null));
          }
        });
      }

      else if (hasComps) {
        eachComponent(component.components, fn, includeAll, subPath(), parent ? component : null);
      }
    }
  });
}

/**
 * Matches if a component matches the query.
 *
 * @param component
 * @param query
 * @return {boolean}
 */
export function matchComponent(component, query) {
  if (isString(query)) {
    return component.key === query;
  }
  else {
    let matches = false;
    forOwn(query, (value, key) => {
      matches = (get(component, key) === value);
      if (!matches) {
        return false;
      }
    });
    return matches;
  }
}

/**
 * Get a component by its key
 *
 * @param {Object} components
 *   The components to iterate.
 * @param {String|Object} key
 *   The key of the component to get, or a query of the component to search.
 *
 * @returns {Object}
 *   The component that matches the given key, or undefined if not found.
 */
export function getComponent(components, key, includeAll) {
  let result;
  eachComponent(components, (component, path) => {
    if (path === key) {
      component.path = path;
      result = component;
      return true;
    }
  }, includeAll);
  return result;
}

/**
 * Finds a component provided a query of properties of that component.
 *
 * @param components
 * @param query
 * @return {*}
 */
export function findComponents(components, query) {
  const results = [];
  eachComponent(components, (component, path) => {
    if (matchComponent(component, query)) {
      component.path = path;
      results.push(component);
    }
  }, true);
  return results;
}

/**
 * Flatten the form components for data manipulation.
 *
 * @param {Object} components
 *   The components to iterate.
 * @param {Boolean} includeAll
 *   Whether or not to include layout components.
 *
 * @returns {Object}
 *   The flattened components map.
 */
export function flattenComponents(components, includeAll) {
  const flattened = {};
  eachComponent(components, (component, path) => {
    flattened[path] = component;
  }, includeAll);
  return flattened;
}

/**
 * Returns if this component has a conditional statement.
 *
 * @param component - The component JSON schema.
 *
 * @returns {boolean} - TRUE - This component has a conditional, FALSE - No conditional provided.
 */
export function hasCondition(component) {
  return Boolean(
    (component.customConditional) ||
    (component.conditional && component.conditional.when) ||
    (component.conditional && component.conditional.json)
  );
}

/**
 * Extension of standard #parseFloat(value) function, that also clears input string.
 *
 * @param {any} value
 *   The value to parse.
 *
 * @returns {Number}
 *   Parsed value.
 */
export function parseFloatExt(value) {
  return parseFloat(isString(value)
    ? value.replace(/[^\de.+-]/gi, '')
    : value);
}

/**
 * Formats provided value in way how Currency component uses it.
 *
 * @param {any} value
 *   The value to format.
 *
 * @returns {String}
 *   Value formatted for Currency component.
 */
export function formatAsCurrency(value) {
  const parsedValue = parseFloatExt(value);

  if (isNaN(parsedValue)) {
    return '';
  }

  const parts = round(parsedValue, 2)
    .toString()
    .split('.');
  parts[0] = chunk(Array.from(parts[0]).reverse(), 3)
    .reverse()
    .map((part) => part
      .reverse()
      .join(''))
    .join(',');
  parts[1] = pad(parts[1], 2, '0');
  return parts.join('.');
}

/**
 * Escapes RegEx characters in provided String value.
 *
 * @param {String} value
 *   String for escaping RegEx characters.
 * @returns {string}
 *   String with escaped RegEx characters.
 */
export function escapeRegExCharacters(value) {
  return value.replace(/[-[\]/{}()*+?.\\^$|]/g, '\\$&');
}

/**
 * Get the value for a component key, in the given submission.
 *
 * @param {Object} submission
 *   A submission object to search.
 * @param {String} key
 *   A for components API key to search for.
 */
export function getValue(submission, key) {
  const search = (data) => {
    if (isPlainObject(data)) {
      if (has(data, key)) {
        return data[key];
      }

      let value = null;

      forOwn(data, (prop) => {
        const result = search(prop);
        if (!isNil(result)) {
          value = result;
          return false;
        }
      });

      return value;
    }
    else {
      return null;
    }
  };

  return search(submission.data);
}

/**
 * Iterate over all components in a form and get string values for translation.
 * @param form
 */
export function getStrings(form) {
  const properties = ['label', 'title', 'legend', 'tooltip', 'description', 'placeholder', 'prefix', 'suffix', 'errorLabel'];
  const strings = [];
  eachComponent(form.components, component => {
    properties.forEach(property => {
      if (component.hasOwnProperty(property) && component[property]) {
        strings.push({
          key: component.key,
          property,
          string: component[property]
        });
      }
    });
    if ((!component.dataSrc || component.dataSrc === 'values') && component.hasOwnProperty('values') && Array.isArray(component.values) && component.values.length) {
      component.values.forEach((value, index) => {
        strings.push({
          key: component.key,
          property: `value[${index}].label`,
          string: component.values[index].label
        });
      });
    }

    // Hard coded values from Day component
    if (component.type === 'day') {
      [
        'day',
        'month',
        'year',
        'Day',
        'Month',
        'Year',
        'january',
        'february',
        'march',
        'april',
        'may',
        'june',
        'july',
        'august',
        'september',
        'october',
        'november',
        'december'
      ].forEach(string => {
        strings.push({
          key: component.key,
          property: 'day',
          string,
        });
      });

      if (component.fields.day.placeholder) {
        strings.push({
          key: component.key,
          property: 'fields.day.placeholder',
          string: component.fields.day.placeholder,
        });
      }

      if (component.fields.month.placeholder) {
        strings.push({
          key: component.key,
          property: 'fields.month.placeholder',
          string: component.fields.month.placeholder,
        });
      }

      if (component.fields.year.placeholder) {
        strings.push({
          key: component.key,
          property: 'fields.year.placeholder',
          string: component.fields.year.placeholder,
        });
      }
    }

    if (component.type === 'editgrid') {
      const string = this.component.addAnother || 'Add Another';
      if (component.addAnother) {
        strings.push({
          key: component.key,
          property: 'addAnother',
          string,
        });
      }
    }

    if (component.type === 'select') {
      [
        'loading...',
        'Type to search'
      ].forEach(string => {
        strings.push({
          key: component.key,
          property: 'select',
          string,
        });
      });
    }
  }, true);

  return strings;
}
export function generateShowHideTabel(comps, showHidetabel, form) {
  //this.root.components
  eachComponent(comps, function(comp) {
    if (comp.type === 'datagrid') {
      generateShowHideTabel(comp.components, showHidetabel, form);
    }
    showHidetabel[comp.key] = comp._visible;
    form.visiteTabel[comp.key] = comp._visible;
  }, true);
}
/**
 * Iterate over all (show) components and generate the summary data as an array.
 * @param form
 */
export function generateSummaryData(form) {
  generateShowHideTabel(form.components, {}, form);
  return generateData(form);
  function generateData(form) {
    if (form.pages && form.wizard) {
      const wizarpages = form.wizard.components;
      const currentshowPages = {};
      for (const p of form.pages) {
        currentshowPages[p.key] = true;
      }
      for (const ap of wizarpages) {
        form.visiteTabel[ap.key] = currentshowPages.hasOwnProperty(ap.key) ? true : false;
      }
    }
    const FormSchema = form.schema;
    const SubmitedData = form.data;
    var summaryData = [];
    if (FormSchema.components !== undefined && SubmitedData !== undefined) {
      for (const i in FormSchema.components) {
        const type = FormSchema.components[i].type;
        if (!(type === 'panel' || type === 'sfapanel' || type === 'section')) {
          if (!FormSchema.components[i].showSummary) {
            continue;
          }
        }
        const label = FormSchema.components[i].label;
        const key = FormSchema.components[i].key;
        const hideLabel = FormSchema.components[i].hideLabel === true ? true : false;
        const applyLabelFirstRow = FormSchema.components[i].applyLabelFirstRow === true ? true : false;
        if (!form.visiteTabel[key]) {
          continue;
        }
        if (type === 'panel' || type === 'sfapanel') {
          const label = FormSchema.components[i].label;
          panelProcessing(label, type, FormSchema.components[i].components, summaryData, SubmitedData, 0.75, hideLabel);
        }
        else if (type === 'section') {
          const subSubmiedData = SubmitedData[key];
          sectionProcessing(label, key, type, FormSchema.components[i], summaryData, subSubmiedData, 0.75, hideLabel, applyLabelFirstRow);
        }
        else {
          selectAndCombine(label, key, type, FormSchema.components[i], summaryData, SubmitedData, 0.75);
        }
      }
    }
    return summaryData;
  }
  /* eslint-disable max-statements */
  function panelProcessing(label, type, Jsondata, summaryData, subData, currentLvl, hideLabel) {
    if (type === 'panel' || type === 'sfapanel') {
      if (!hideLabel) {
        const temp = {};
        temp.label = label;
        temp.title = true;
        temp.Lvl = currentLvl;
        summaryData.push(temp);
        currentLvl += 1;
      }
    }
    for (const i in Jsondata) {
      const type = Jsondata[i].type;
      if (!(type === 'panel' || type === 'sfapanel' || type === 'section')) {
        if (!Jsondata[i].showSummary) {
          continue;
        }
      }
      const label = Jsondata[i].label;
      const key = Jsondata[i].key;
      const hideLabel = Jsondata[i].hideLabel === true ? true : false;
      if (type === 'panel' || type === 'sfapanel') {
        const label = Jsondata[i].label;
        if (!form.visiteTabel[key]) {
          continue;
        }
        panelProcessing(label, type, Jsondata[i].components, summaryData, subData, currentLvl, hideLabel);
      }
      else if (type === 'section') {
        const subSubmiedData = subData[key];
        if (!form.visiteTabel[key]) {
          continue;
        }
        sectionProcessing(label, key, type, Jsondata[i], summaryData, subSubmiedData, currentLvl, hideLabel);
      }
      else {
        selectAndCombine(label, key, type, Jsondata[i], summaryData, subData, currentLvl);
      }
    }
  }
  /* eslint-disable max-statements */
  function sectionProcessing(label, key, type, Jsondata, summaryData, subData, currentLvl, hideLabel, applyLabelFirstRow) {
    if (type === 'section') {
      if (!hideLabel) {
        const temp = {};
        temp.label = label;
        temp.title = true;
        temp.Lvl = currentLvl;
        currentLvl += 1;
        summaryData.push(temp);
      }
    }
    const dividertitle = Jsondata.dividertitle ? Jsondata.dividertitle : '';
    Jsondata = Jsondata.components[0].components; // sectionpanel.components
    for (const r in subData) {
      if (r > 0 && dividertitle !== '') {
        const temp = {};
        temp.label = dividertitle;
        temp.Lvl = currentLvl;
        temp.title = true;
        summaryData.push(temp);
      }
      for (const i in Jsondata) {
        const label = Jsondata[i].label;
        const key = Jsondata[i].key;
        const type = Jsondata[i].type;
        if (!(type === 'panel' || type === 'sfapanel' || type === 'section')) {
          if (!Jsondata[i].showSummary) {
            continue;
          }
        }
        const hideLabel = Jsondata[i].hideLabel === true ? true : false;
        const applyLabelFirstRow = Jsondata[i].applyLabelFirstRow === true ? true : false;
        if (type === 'panel' || type === 'sfapanel') {
          const label = Jsondata[i].label;
          if (!form.visiteTabel[key]) {
            continue;
          }
          panelProcessing(label, type, Jsondata[i].components, summaryData, subData[r], currentLvl, hideLabel);
        }
        else if (type === 'section') {
          if (!form.visiteTabel[key]) {
            continue;
          }
          sectionProcessing(label, key, type, Jsondata[i], summaryData, subData[r][key], currentLvl, hideLabel, applyLabelFirstRow);
        }
        else {
          selectAndCombine(label, key, type, Jsondata[i], summaryData, subData[r], currentLvl);
        }
      }
    }
  }
  /* eslint-disable max-statements */
  function selectAndCombine(label, key, type, Jsondata, summaryData, subData, currentLvl) {
      var eachComponent = {};
      eachComponent.Lvl = currentLvl ? currentLvl : 0;
      switch (type) {
        case 'textfield':
        case 'sfatextfield'://
        case 'textarea':
        case 'sfatextarea'://
        case 'number':
        case 'sfanumber'://
        case 'checkbox':
        case 'select':
        case 'radio':
        case 'email':
        case 'sfaemail'://
        case 'url':
        case 'link'://
        case 'slider'://
        case 'sfadatetime'://
          eachComponent.label = label;
          eachComponent.value = subData[key] !== undefined ? subData[key] : '';
          summaryData.push(eachComponent);
          break;
        case 'sfacheckbox'://
          eachComponent.label = label;
          eachComponent.value = subData[key] !== undefined ? subData[key] : '';
          summaryData.push(eachComponent);
          break;
        case 'country'://
        case 'sfaselect'://
          eachComponent.label = label;
          eachComponent.value = subData[key] !== undefined ? subData[key]['value'] !== undefined ? subData[key]['value'] : '' : '';
          summaryData.push(eachComponent);
          break;
        case 'sfaradioyn'://
        case 'radiogroup'://
        case 'checkboxesgroup'://
          eachComponent.label = label;
          eachComponent.value = subData[key] !== undefined ? subData[key][key] !== undefined ? subData[key][key] : '' : '';
          summaryData.push(eachComponent);
          break;
        case 'summary':
        case 'submit':
          break;
        case 'addressgroup'://
          addressProcessing(Jsondata, key, subData, summaryData, currentLvl);
          break;
        case 'person'://
          personProcessing(Jsondata, key, subData, summaryData, currentLvl);
          break;
        case 'sfasignature'://
          eachComponent.label = label;
          eachComponent.value = subData[key];
          eachComponent.signature = true;
          summaryData.push(eachComponent);
          break;
        case 'popupform': {//
          break;
        }
        default:
          break;
      }
    }
  /* eslint-disable max-statements */
  function addressProcessing(Jsondata, key, subData, summaryData, currentLvl) {
      var addressComponent = {};
      if (subData[key] === undefined) return;
      if (subData[key].formatted_address !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = 'Formatted Address';
        addressComponent.value = subData[key].formatted_address;
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidecc === false && subData[key].Addr_Co_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.cclabel ? Jsondata.cclabel : 'Care of (C/-)';
        addressComponent.value = subData[key].Addr_Co_txt ? subData[key].Addr_Co_txt : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidelvl === false && subData[key].Addr_Level_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.lvllabel ? Jsondata.lvllabel : 'Bldg, Floor, Lvl';
        addressComponent.value = subData[key].Addr_Level_txt ? subData[key].Addr_Level_txt : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidestreet === false && subData[key].Addr_1_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.streetlabel ? Jsondata.streetlabel : 'Street No, Name (or Extended address)';
        addressComponent.value = subData[key].Addr_1_txt ? subData[key].Addr_1_txt : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidesuburb === false && subData[key].Addr_Suburb_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.suburblabel ? Jsondata.suburblabel : 'Suburb / City';
        addressComponent.value = subData[key].Addr_Suburb_txt ? subData[key].Addr_Suburb_txt : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidestate === false && subData[key].Addr_State_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.statelabel ? Jsondata.statelabel : 'State';
        addressComponent.value = subData[key].Addr_State_txt ? subData[key].Addr_State_txt : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidepostcode === false && subData[key].Addr_PC_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.postcodelabel ? Jsondata.postcodelabel : 'Zip / PostCode';
        addressComponent.value = subData[key].Addr_PC_txt ? subData[key].Addr_PC_txt : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidecountry === false && subData[key].Addr_Country_cho !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = Jsondata.countrylabel ? Jsondata.countrylabel : 'Country';
        addressComponent.value = subData[key].Addr_Country_cho ? subData[key].Addr_Country_cho : '';
        summaryData.push(addressComponent);
      }
      if (Jsondata.hidefulladdress === false && subData[key].Addr_txt !== '') {
        addressComponent = {};
        addressComponent.Lvl = currentLvl;
        addressComponent.label = 'Full Address';
        addressComponent.value = subData[key].Addr_txt ? subData[key].Addr_txt : '';
        summaryData.push(addressComponent);
      }
    }
  /* eslint-disable max-statements */
  function personProcessing(Jsondata, key, subData, summaryData, currentLvl) {
      var eachComponent = {};
      if (subData[key] === undefined) return;
      if (Jsondata.hidetitle === false && subData[key].Sal_cho !== '') {
        eachComponent = {};
        eachComponent.Lvl = currentLvl;
        eachComponent.label = 'Title';
        eachComponent.value = subData[key].Sal_cho ? subData[key].Sal_cho : '';
        summaryData.push(eachComponent);
      }
      if (subData[key].Name_First_txt !== '') {
        eachComponent = {};
        eachComponent.Lvl = currentLvl;
        eachComponent.label = 'First Name';
        eachComponent.value = subData[key].Name_First_txt ? subData[key].Name_First_txt : '';
        summaryData.push(eachComponent);
      }
      if (Jsondata.hidemidname === false && subData[key].Name_Middle_txt !== '') {
        eachComponent = {};
        eachComponent.Lvl = currentLvl;
        eachComponent.label = 'Middle Name';
        eachComponent.value = subData[key].Name_Middle_txt ? subData[key].Name_Middle_txt : '';
        summaryData.push(eachComponent);
      }
      if (subData[key].Name_Last_txt !== '') {
        eachComponent = {};
        eachComponent.Lvl = currentLvl;
        eachComponent.label = 'Last Name';
        eachComponent.value = subData[key].Name_Last_txt ? subData[key].Name_Last_txt : '';
        summaryData.push(eachComponent);
      }
      if (Jsondata.hidefullname === false && subData[key].Name_Full_scr !== '') {
        eachComponent = {};
        eachComponent.Lvl = currentLvl;
        eachComponent.label = 'Full Name';
        eachComponent.value = subData[key].Name_Full_scr ? subData[key].Name_Full_scr : '';
        summaryData.push(eachComponent);
      }
  }
 }

export function generateTable(form) {
  if (form) {
    const sData = generateSummaryData(form);
    if (sData !== null) {
      return loadingSummaryDataToTbody(sData, form);
    }
  }
}
/* eslint-disable max-statements */
function loadingSummaryDataToTbody(summaryJson, form) {
  const tbody = form.ce('tbody');
  for (const i in summaryJson) {
    const tr = form.ce('tr');
    if (summaryJson[i].title === undefined) {
      if (summaryJson[i].signature === undefined) {
        const td1 = form.ce('td');
        td1.style.paddingLeft = `${summaryJson[i].Lvl}rem`;
        td1.appendChild(form.text(summaryJson[i].label));
        const td2 = form.ce('td');
        td2.appendChild(form.text(summaryJson[i].value ? summaryJson[i].value : ''));
        tr.appendChild(td1);
        tr.appendChild(td2);
      }
      else if (summaryJson[i].signature === true) {
        const td1 = form.ce('td');
        td1.style.paddingLeft = `${summaryJson[i].Lvl}rem`;
        td1.appendChild(form.text(summaryJson[i].label));
        const td2 = form.ce('td', {
          style: 'text-align: -webkit-center;width: 60%;'
        });
        const imgdive = form.ce('div', {
          style: 'width: 50%;'
        });
        const signatureimg = form.ce('img', {
          style: 'width: 100%;',
          src: summaryJson[i].value ? `data:${summaryJson[i].value}` : ''
        });
        imgdive.appendChild(signatureimg);
        td2.appendChild(imgdive);
        tr.appendChild(td1);
        tr.appendChild(td2);
      }
    }
    else {
      const td = form.ce('td', {
        colspan: 2,
        style: 'font-weight:bold;'
      });
      td.style.paddingLeft = `${summaryJson[i].Lvl}rem`;
      td.appendChild(form.text(summaryJson[i].label));
      tr.appendChild(td);
    }
    tbody.appendChild(tr);
  }

  const SummaryGroup = form.ce('div', {
    class: 'form-group'
  });
  const table = form.ce('table', {
    class: 'table table-striped table-bordered'
  });
  // Build header.
  const thead = form.ce('thead');
  const thr = form.ce('tr');
  const th1 = form.ce('th', {
    style: 'text-align: center;'
  });
  th1.appendChild(form.text('Summary'));
  const th2 = form.ce('th', {
    style: 'text-align: center;'
  });
  th2.appendChild(form.text('Answer'));
  thr.appendChild(th1);
  thr.appendChild(th2);
  thead.appendChild(thr);
  table.appendChild(thead);
  // Build the body.
  table.appendChild(tbody);
  SummaryGroup.appendChild(table);
  //const tablestyle = '<style>.table{width: 100%;margin-bottom: 0;background-color: transparent;color: #212529;border-collapse: collapse}.table-bordered{border: 2px solid #f4f5f8}.table-bordered thead th{border-bottom-width: 2px}.table thead th{vertical-align: bottom;border-bottom: 2px solid #dee2e6}.table thead th, .table thead td{font-weight: 500;padding-top: 1rem;padding-bottom: 1rem}.table-striped tbody tr:nth-of-type(odd){background-color: rgba(0,0,0,.05)}.table-bordered td, .table-bordered th{border: 1px solid #dee2e6}.table td, .table th{padding: .75rem;vertical-align: top}</style>';
  return SummaryGroup.innerHTML;
}
