import _ from 'lodash';
import NestedComponent from '../nested/NestedComponent';
import BaseComponent from '../base/Base';
import Tooltip from 'tooltip.js';
import Validator from '../Validator';

export default class SectionComponent extends NestedComponent {
  static schema(...extend) {
    return NestedComponent.schema({
      label: 'Repeat Panel',
      key: 'section',
      type: 'section',
      clearOnHide: false,
      input: true,
      tree: true,
      hideLabel: false,
      applyLabelFirstRow: false,
      applyLabelFirstRowText: '',
      tooltip:'',
      collapsible: false,
      collapsed:false,
      disableAddingRemovingRows:false,
      components: [{
        label: 'SectionPanel',
        collapsible: false,
        mask: false,
        tableView: false,
        alwaysEnabled: false,
        type: 'sectionpanel',
        input: true,
        key: '',
        components: [],
        row: '0-0'
      }],
      removePlacement:'corner',
      addAnotherPosition:'bottom',
      //border: 1,
      //borderR: 4,
      theme: 'default'//
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Repeat Panel',
      icon: 'fa fa-th',
      group: 'testGroup',
      documentation: 'http://help.form.io/userguide/#datagrid',
      schema: SectionComponent.schema()
    };
  }

  constructor(component, options, data) {
    component.borderR = component.borderR === undefined ? 4 : component.borderR;
    //component.applyLabelFirstRowUpdated = component.applyLabelFirstRowUpdated === undefined ? false : component.applyLabelFirstRowUpdated;
    component.applyLabelFirstRow = component.applyLabelFirstRow === undefined ? false : component.applyLabelFirstRow;
    component.border = component.border === undefined ? 1 : component.border;
    component.initialrows = component.initialrows === undefined ? 1 : component.initialrows;
    super(component, options, data);
    this.type = 'datagrid';
    this.numRows = 0;
    this.numColumns = 0;
    this.rows = [];
    this.collapsed = !!this.component.collapsed;
    this.component.minrows = this.component.minrows > this.component.maxLength ?
      this.component.maxLength : this.component.minrows;
    //this.component.applyLabelFirstRow = this.component.applyLabelFirstRow ? this.component.applyLabelFirstRow : false;
    if (!this.options.builder && !this.data.hasOwnProperty('load')) {
      for (var i = this.dataValue.length; i < this.component.initialrows; i++) {
        //this.dataValue.push({ repeat: true });
        if (this.component.applyLabelFirstRow) {
          // eslint-disable-next-line max-depth
          if (i === 1) {
            this.dataValue.push({ repeat: true });
          }
          else {
            this.dataValue.push({ repeat: false });
          }
        }
        else {
          this.dataValue.push({ repeat: true });
        }
      }
    }
  }

  get defaultSchema() {
    return SectionComponent.schema();
  }

  getCollapseIcon() {
    const collapseIcon = this.getIcon(this.collapsed ? 'plus' : 'minus');
    this.addClass(collapseIcon, 'formio-collapse-icon');
    return collapseIcon;
  }

  setCollapsed() {
    //super.setCollapsed(this.tableElement);
    if (!this.component.collapsible || this.options.builder) {
      return;
    }
    if (this.collapsed) {
      this.tableElement.setAttribute('hidden', true);
      this.tableElement.style.visibility = 'hidden';
    }
    else {
      this.tableElement.removeAttribute('hidden');
      this.tableElement.style.visibility = 'visible';
    }
    if (this.collapseIcon) {
      const newIcon = this.getCollapseIcon();
      this.sectionTitle.replaceChild(newIcon, this.collapseIcon);
      this.collapseIcon = newIcon;
    }
  }
  setCollapseHeader(header) {
    if (this.component.collapsible) {
      this.addClass(header, 'formio-clickable');
      header.addEventListener('click', () => this.toggleCollapse());
    }
  }
  toggleCollapse() {
    this.collapsed = !this.collapsed;
    this.setCollapsed();
  }

  get emptyValue() {
    return [{ repeat:!this.component.disableAddingRemovingRows }];
  }

  get addAnotherPosition() {
    return _.get(this.component, 'addAnotherPosition', 'bottom');
  }

  setStaticValue(n) {
    this.dataValue = _.range(n).map(() => ({}));
  }

  hasAddButton() {
    const maxLength = _.get(this.component, 'maxLength');
    return !this.component.disableAddingRemovingRows &&
    !this.shouldDisable &&
      !this.options.builder &&
      !this.options.preview &&
      (!maxLength || (this.dataValue.length < maxLength));
  }

  hasExtraColumn() {
    const rmPlacement = _.get(this, 'component.removePlacement', 'col');
    return (this.hasRemoveButtons() && rmPlacement === 'col') || this.options.builder;
  }

  hasRemoveButtons() {
    return !this.component.disableAddingRemovingRows &&
      !this.shouldDisable &&
      !this.options.builder &&
      (this.dataValue.length >= _.get(this.component, 'validate.minLength', 0));
  }

  hasTopSubmit() {
    return this.hasAddButton() && ['top', 'both'].includes(this.addAnotherPosition);
  }

  hasBottomSubmit() {
    return this.hasAddButton() && ['bottom', 'both'].includes(this.addAnotherPosition);
  }

  hasChanged(before, after) {
    return !_.isEqual(before, after);
  }
  //OLD
  // eslint-disable-next-line max-statements
  build(state) {
    if (!this.options.builder) {
      this.component.defaultValue = [{ repeat: true }];
      for (var i = 1; i < this.component.initialrows; i++) {
        this.component.defaultValue.push({ repeat: true });
      }
    }
    this.component.components[0].key = `${this.component.key}_panel`;
    this.createElement();
    //this.createLabel(this.element);
    if (this.component.label && !this.component.hideLabel) {
      const heading = this.ce('div', {
        class: `card-header bg-${this.component.theme} panel-heading`,
        style: this.component.border === 0 ? 'border: none;' : ''
      });
      //heading.style.backgroundColor = '';
      heading.style.borderTopLeftRadius = `${this.component.borderR}px`;
      heading.style.borderTopRightRadius = `${this.component.borderR}px`;
      this.sectionTitle = this.ce('h4', {
        class: 'card-title panel-title'
      });
      if (this.component.collapsible) {
        this.collapseIcon = this.getCollapseIcon();
        this.sectionTitle.appendChild(this.collapseIcon);
        this.sectionTitle.appendChild(this.text(' '));
      }
      this.sectionTitle.appendChild(this.text(this.component.label));
      this.createTooltip(this.sectionTitle, this.component);
      heading.appendChild(this.sectionTitle);
      this.setCollapseHeader(heading);
      this.element.appendChild(heading);
    }
    let tableClass = 'datagrid-table   form-group formio-data-grid ';
    ['striped', 'bordered', 'hover', 'condensed'].forEach((prop) => {
      if (this.component[prop]) {
        tableClass += `table-${prop} `;
      }
    });
    this.tableElement = this.ce('table', {
      class: tableClass,
      style: this.component.border === 0 ? 'border: none;' : ''
     //style: 'width: 100%;',
    });
    this.tableElement.style.width = '100%';
    //STQ Modified
    this.tableElement.style.display = 'flex';
    this.tableElement.style.flexDirection = 'column';
    this.element.appendChild(this.tableElement);
    this.setCollapsed();
    if (!this.dataValue.length) {
      this.addNewValue();
    }
    this.visibleColumns = true;
    // this.panelBody = this.ce('div', {//
    //   class: 'card-body panel-body'
    // });
    // this.addComponents(null, null, null, state);
    // this.element.appendChild(this.panelBody);//
    this.errorContainer = this.element;
    this.restoreValue();
    this.createDescription(this.element);
    this.attachLogic();
  }

  setVisibleComponents() {
    // Add new values based on minLength.
    for (let dIndex = this.dataValue.length; dIndex < _.get(this.component, 'validate.minLength', 0); dIndex++) {
      this.dataValue.push({});
    }

    this.numColumns = this.hasExtraColumn() ? 1 : 0;
    this.numRows = this.dataValue.length;

    if (this.visibleColumns === true) {
      this.numColumns += this.component.components.length;
      this.visibleComponents = this.component.components;
      return this.visibleComponents;
    }

    this.visibleComponents = this.component.components.filter((comp) => this.visibleColumns[comp.key]);
    this.numColumns += this.visibleComponents.length;
  }

  buildRows() {
    this.setVisibleComponents();
    //console.log(`setVisibleComponents()----this.numColumns:${this.numColumns}====this.visibleComponents:${JSON.stringify(this.visibleComponents)}`);
    const state = this.destroy();
    this.empty(this.tableElement);
    const minrows = _.get(this.component, 'minrows');
    const maxLength = _.get(this.component, 'maxLength');
    // Build the rows.
    var tableRows = [];
//    console.log(`tableRows  --${JSON.stringify(tableRows)}`);//
    this.dataValue.forEach((row, rowIndex) => tableRows.push(this.buildRow(row, rowIndex, state.rows[rowIndex])));
    this.dataValue.forEach((row, rowIndex) => this.dataValue[rowIndex].index = rowIndex);
    //STQ Modified
    try {
      if (tableRows.length > 0) {
        tableRows.forEach((tblRow) => {
          tblRow.style.display = 'flex';
          tblRow.style.flexDirection = 'column';
        });
      }
    }
    catch (e) {
      //ignore
    }
    if (this.options.builder) {
      this.tableElement.appendChild(this.ce('tbody', {
        totalRows:tableRows.length
      }, tableRows));
    }
    else {
      // const inirows = _.get(this.component, 'inirows');
      // const flag = this.component.initialFlag === true?minrows:inirows;
      if (tableRows.length < minrows) {
          //this.addValue();
        this.addNewValue();
      }
      else if (tableRows.length > maxLength) {
        this.removeValue(tableRows.length-1);
      }
      this.tableElement.appendChild(this.ce('tbody', {
        totalRows:tableRows.length
      }, tableRows));
    }
    // Create the add row button footer element.
    if (this.hasBottomSubmit() && this.tableElement.childElementCount <= 1) {
      this.tableElement.appendChild(this.ce('tfoot', {
        class: 'sectionfooter',
        style:'background-color: inherit !important;'
      },
        this.ce('tr', null,
          this.ce('td', { colspan: this.numColumns, class: 'pr-1' },
            this.addButton()
          )
        )
      ));
    }
  }

  // Build the header.
  //createHeader() {
  //  const hasTopButton = this.hasTopSubmit();
  //  const hasEnd = this.hasExtraColumn() || hasTopButton;
  //  let needsHeader = false;
  //  const thead = this.ce('thead', null, this.ce('tr', null,
  //    [
  //      this.visibleComponents.map(comp => {
  //        const th = this.ce('th');
  //        if (comp.validate && comp.validate.required) {
  //          th.setAttribute('class', 'field-required');
  //        }
  //        const title = comp.label || comp.title;
  //        if (title && !comp.dataGridLabel) {
  //          needsHeader = true;
  //          th.appendChild(this.text(title));
  //          this.createTooltip(th, comp);
  //        }
  //        return th;
  //      }),
  //      hasEnd ? this.ce('th', null, (hasTopButton ? this.addButton(true) : null)) : null,
  //    ]
  //  ));
  //  return needsHeader ? thead : null;
  //}

  get dataValue() {
    const dataValue = super.dataValue;
    if (!dataValue || !Array.isArray(dataValue)) {
      return this.emptyValue;
    }
    return dataValue;
  }

  set dataValue(value) {
    super.dataValue = value;
  }

  get defaultValue() {
    const value = super.defaultValue;
    if (Array.isArray(value)) {
      return value;
    }
    if (value && (typeof value === 'object')) {
      return [value];
    }
    return this.emptyValue;
  }

  buildRow(rowData, index, state) {
    state = state || {};
    const components = _.get(this, 'component.components', []);
    const colsNum = components.length;
    const lastColIndex = colsNum - 1;
    const hasRmButton = this.hasRemoveButtons();
    const hasTopButton = this.hasTopSubmit();
    const rmPlacement = _.get(this, 'component.removePlacement', 'col');
    let useCorner = false;
    let lastColumn = null;
    this.rows[index] = {};
   //  if (hasRmButton && this.dataValue.length !== 1)
    if (hasRmButton) {
      if (rmPlacement === 'col') {
        //lastColumn = this.ce('td', null, this.locationIndex(index));
        lastColumn = this.ce('td', null, this.removeButton(index));
      }
      else {
        useCorner = true;
      }
    }
    // else if (this.options.builder) {
    //   lastColumn = this.ce('td', {
    //     id: `${this.id}-drag-container`,
    //     class: 'drag-container'
    //   }, this.ce('div', {
    //     id: `${this.id}-placeholder`,
    //     class: 'alert alert-info',
    //     style: 'text-align:center; margin-bottom: 0px;',
    //     role: 'alert'
    //   }, this.text('Drag and Drop a form component')));
    //   this.root.addDragContainer(lastColumn, this);
    // }
    return this.ce('tr', null,
      [
        components.map(
          (cmp, colIndex=0) => {
            const cell = this.buildComponent(
              cmp,
              colIndex=0,
              rowData,
              index,
              this.getComponentState(cmp, state)
            );

            if (hasRmButton && useCorner && lastColIndex === colIndex && cell) {
              cell.style.position = 'relative';
              cell.setAttribute('aria-describedby', this.component.label);
              //cell.style.backgroundColor = 'white';
              cell.style.border = this.component.border === 0 ? 'none' : '';
             // cell.append(this.locationIndex(index));
              cell.append(this.removeButton(index, 'small'));
              if (hasTopButton ) {
                cell.setAttribute('colspan', 2);
              }
            }
            //console.log(`buildComponent()---${cmp.type}---${colIndex}---${JSON.stringify(rowData)}---${index}---state:${JSON.stringify(state)}`);
            return cell;
          }
        ),
        lastColumn
      ]
    );
  }

  destroyRows() {
    const state = {};
    state.rows = state.rows || {};
    this.rows.forEach((row, rowIndex) => _.forIn(row, col => {
      state.rows[rowIndex] = state.rows[rowIndex] || {};
      const compState = this.removeComponent(col, row);
      if (col.key && compState) {
        state.rows[rowIndex][col.key] = compState;
      }
    }));
    this.rows = [];
    return state;
  }

  destroy() {
    const state = this.destroyRows();
    super.destroy();
    return state;
  }

  buildComponent(col, colIndex, row, rowIndex, state) {
    var container;
    const isVisible = this.visibleColumns &&
      (!this.visibleColumns.hasOwnProperty(col.key) || this.visibleColumns[col.key]);
    if (isVisible || this.options.builder) {
      container = this.ce('td');
      container.noDrop = true;
    }
    if (rowIndex > 0 && this.component.dividertitle) {
      const dividerDiv = this.ce('div', {
        class: 'dividerDiv mt-8'
      });
      const dividerText = this.ce('span',{
        class: 'dividerText'
      });
      dividerDiv.style.backgroundColor = 'rgba(0, 0, 0, 0.03)';
      dividerDiv.style.padding = '0.45rem 2.25rem';
      dividerDiv.style.fontSize = '1.275rem';
      dividerDiv.style.fontWeight = '500';
      this.addClass(dividerText, 'card-title');
      dividerText.appendChild(this.text(this.component.dividertitle?this.component.dividertitle:''));
      dividerDiv.appendChild(dividerText);
      container.appendChild(dividerDiv);
    }

    const column = _.clone(col);
    const options = _.clone(this.options);
    options.name += `[${rowIndex}]`;
    options.row = `${rowIndex}-${colIndex}`;
    options.inDataGrid = true;
    const comp = this.createComponent(_.assign({}, column, {
      row: options.row
    }), options, row, null, state);
    comp.rowIndex = rowIndex;
    //this.hook('addComponent', container, comp, this);
    this.rows[rowIndex][column.key] = comp;
    if (isVisible || this.options.builder) {
      container.appendChild(comp.getElement());
      return container;
    }
  }

  checkConditions(data) {
    let show = super.checkConditions(data);
    // If table isn't visible, don't bother calculating columns.
    if (!show) {
      return false;
    }
    let rebuild = false;
    if (this.visibleColumns === true) {
      this.visibleColumns = {};
    }
    this.component.components.forEach((col) => {
      let showColumn = false;
      this.rows.forEach((comps) => {
        if (comps && comps[col.key] && typeof comps[col.key].checkConditions === 'function') {
          showColumn |= comps[col.key].checkConditions(data);
        }
      });
      showColumn = showColumn && col.type !== 'hidden' && !col.hidden;
      if (
        (this.visibleColumns[col.key] && !showColumn) ||
        (!this.visibleColumns[col.key] && showColumn)
      ) {
        rebuild = true;
      }

      this.visibleColumns[col.key] = showColumn;
      show |= showColumn;
    });

    // If a rebuild is needed, then rebuild the table.
    if (rebuild) {
      this.restoreValue();
    }

    // Return if this table should show.
    return show;
  }

  updateValue(flags, value) {
    // Intentionally skip over nested component updateValue method to keep recursive update from occurring with sub components.
    return BaseComponent.prototype.updateValue.call(this, flags, value);
  }

  /* eslint-disable max-statements */
  setValue(value, flags) {
    flags = this.getFlags.apply(this, arguments);
    if (!value) {
      this.dataValue = this.defaultValue;
      this.buildRows();
      return;
    }
    if (!Array.isArray(value)) {
      if (typeof value === 'object') {
        value = [value];
      }
      else {
        this.buildRows();
        return;
      }
    }

    const changed = this.hasChanged(value, this.dataValue);

    //always should build if not built yet OR is trying to set empty value (in order to prevent deleting last row)
    let shouldBuildRows = !this.isBuilt || changed || _.isEqual(this.emptyValue, value);
    //check if visible columns changed
    let visibleColumnsAmount = 0;
    _.forEach(this.visibleColumns, (value) => {
      if (value) {
        visibleColumnsAmount++;
      }
    });
    const visibleComponentsAmount = this.visibleComponents ? this.visibleComponents.length : 0;
    //should build if visible columns changed
    shouldBuildRows = shouldBuildRows || visibleColumnsAmount !== visibleComponentsAmount;
    //loop through all rows and check if there is field in new value that differs from current value
    const keys = this.componentComponents.map((component) => {
      return component.key;
    });
    for (let i = 0; i < value.length; i++) {
      if (shouldBuildRows) {
        break;
      }
      if (value[i]['repeat'] !== undefined) {
        value[i].repeat = !this.component.disableAddingRemovingRows;
        //value[i].index = i;
      }
      const valueRow = value[i];
      for (let j = 0; j < keys.length; j++) {
        const key = keys[j];
        const newFieldValue = valueRow[key];
        const currentFieldValue = this.rows[i] && this.rows[i][key] ? this.rows[i][key].getValue() : undefined;
        const defaultFieldValue = this.rows[i] && this.rows[i][key] ? this.rows[i][key].defaultValue : undefined;
        const isMissingValue = newFieldValue === undefined && currentFieldValue === defaultFieldValue;
        if (!isMissingValue && !_.isEqual(newFieldValue, currentFieldValue)) {
          shouldBuildRows = true;
          break;
        }
      }
    }
    this.dataValue = value;
    if (shouldBuildRows) {
      this.buildRows();
    }
    this.rows.forEach((row, index) => {
      if (value.length <= index) {
        return;
      }
      _.forIn(row, component => this.setNestedValue(component, value[index], flags));
    });
    return changed;
  }
  /* eslint-enable max-statements */

  /**
   * Get the value of this component.
   *
   * @returns {*}
   */
  getValue() {
    for (var i = 0; i < this.components.length; i++) {
      this.components[i].updateValue({
        modified: true
      });
    }
    //this.components[0].updateValue({
    //  modified: true
    //});
    // const idendityname = `${this.component.key}_repeat`;
    // if (!this.component.disableAddingRemovingRows) {
    //   this.dataValue[0][idendityname]=true;
    //   return this.dataValue;
    // }
    // else {
    //   this.dataValue[0][idendityname]=false;
    //   return this.dataValue;
    // }
    if (!this.component.disableAddingRemovingRows) {
      this.dataValue[0]['repeat'] = true;
      return this.dataValue;
    }
    else {
      return this.dataValue;
    }
  }

  restoreComponentsContext() {
    this.rows.forEach((row, index) => _.forIn(row, (component) => component.data = this.dataValue[index]));
  }

  getComponent(path, fn) {
    path = Array.isArray(path) ? path : [path];
    const [key, ...remainingPath] = path;
    let result = [];

    if (!_.isString(key)) {
      return result;
    }

    this.everyComponent((component, components) => {
      if (component.component.key === key) {
        let comp = component;
        if (remainingPath.length > 0 && 'getComponent' in component) {
          comp = component.getComponent(remainingPath, fn);
        }
        else if (fn) {
          fn(component, components);
        }

        result = result.concat(comp);
      }
    });
    return result.length > 0 ? result : null;
  }

  //locationIndex(index) {
  //  const className = 'formio-datagrid-section-index';
  //  const locationLabel = this.ce('span', {
  //    class: className
  //  });
  //  const LocationTitle = index;
  //  locationLabel.appendChild(this.text(`Location #${LocationTitle}`));
  //  return locationLabel;
  //}

  /** @override **/
  removeButton(index, mode = 'basic') {
    // this.component.initialFlag = true;
    if (mode === 'small') {
      return this.removeButtonSmall(index);
    }
    return super.removeButton(index);
  }

  removeButtonSmall(index) {
//    const cmpType = _.get(this, 'component.type', 'datagrid');
    const className = 'btn btn-xxs btn-danger formio-datagrid-remove fa fa-times p-2 me-2 mt-8';
    const button = this.ce(
      'button',
      {
        type: 'button',
        tabindex: '-1',
        class: className,
      },
      //this.ce('i', { class: this.iconClass('remove') })
    );

    this.addEventListener(button, 'click', (event) => {
      event.preventDefault();
      this.removeValue(index);
    });

    return button;
  }

  /*** Row Groups ***/

  /**
   * @param {Numbers[]} groups
   * @param {Array<T>} coll - collection
   *
   * @return {Array<T[]>}
   */
  getRowChunks(groups, coll) {
    const [, chunks] = groups.reduce(
      ([startIndex, acc], size) => {
        const endIndex = startIndex +  size;
        return [endIndex, [...acc, [startIndex, endIndex]]];
      },
      [0, []]
    );

    return chunks.map(range => _.slice(coll, ...range));
  }

  hasRowGroups() {
    return _.get(this, 'component.enableRowGroups', false);
  }

  buildGroups() {
    const groups = _.get(this.component, 'rowGroups', []);
    const ranges = _.map(groups, 'numberOfRows');
    const rows = this.tableElement.querySelectorAll('tbody>tr');
    const tbody = this.tableElement.querySelector('tbody');
    const chunks = this.getRowChunks(ranges, rows);
    const firstElements = chunks.map(_.head);
    const groupElements = groups.map((g, index) => this.buildGroup(g, index, chunks[index]));

    groupElements.forEach((elt, index) => {
      const row = firstElements[index];

      if (row) {
        tbody.insertBefore(elt, row);
      }
    });
  }

  buildGroup({ label }, index, groupRows) {
    const hasToggle = _.get(this, 'component.groupToggle', false);
    const colsNumber = _.get(this, 'component.components', []).length;
    const cell = this.ce('td', {
      colspan: colsNumber,
      class: 'datagrid-group-label',
    }, [label]);
    const header = this.ce('tr', {
      class: `datagrid-group-header ${hasToggle ? 'clickable' : ''}`,
    }, cell);

    if (hasToggle) {
      this.addEventListener(header, 'click', () => {
        header.classList.toggle('collapsed');
        _.each(groupRows, row => {
          row.classList.toggle('hidden');
        });
      });
    }

    return header;
  }

  totalRowsNumber(groups) {
    return _.sum(_.map(groups, 'numberOfRows'));
  }
  addButton(justIcon) {
    const addButton = this.ce('button', {
      class: 'btn btn-sm btn-primary formio-button-add-row pull-right'
    });
    addButton.style.margin = '0.2vh 2vh';
    this.addEventListener(addButton, 'click', (event) => {
      event.preventDefault();
      this.addValue();
      this.getAllComponents().forEach((c) => {
        const message = Validator.check(c, c.data);
        c.setCustomValidity(message);
      });
  });

    const addIcon = this.ce('i', {
      class: this.iconClass('plus')
    });

    if (justIcon) {
      //addButton.appendChild(addIcon);
      return addButton;
    }
    else {
      addButton.appendChild(addIcon);
      addButton.appendChild(this.text(' '));
      addButton.appendChild(this.text(this.component.addAnother || 'Add Another'));
      return addButton;
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
syntaq-component syntaq-component-section formio-component-datagrid ${this.className}`,
      style: ` border: ${this.component.border}px solid rgba(0, 0, 0, 0.125);border-radius: ${this.component.borderR}px; ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  createTooltip(container, component, classes) {
    if (this.tooltip) {
      return;
    }
    component = component || this.component;
    classes = classes || `${this.iconClass('question-sign')} text-muted`;
    if (!component.tooltip) {
      return;
    }
    const ttElement = this.ce('i', {
      class: classes
    });
    container.appendChild(this.text(' '));
    container.appendChild(ttElement);

    ttElement.addEventListener('mouseover', () => {
      // stq modified
      if (!this.tooltip) {
        this.tooltip = new Tooltip(ttElement, {
          trigger: 'hover click',
          placement: 'right',
          html: true,
          title: this.interpolate(component.tooltip).replace(/(?:\r\n|\r|\n)/g, '<br />')
        });
      }
    });
  }
}
// const BaseGetSchema = Object.getOwnPropertyDescriptor(BaseComponent.prototype, 'schema');
// Object.defineProperty(DataGridComponent.prototype, 'schema', BaseGetSchema);
