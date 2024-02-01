import BaseComponent from '../base/Base';
import Tooltip from 'tooltip.js';
import { generateSummaryData } from '../../utils/utils';

export default class SummaryTableComponent extends BaseComponent {
  static schema() {
    return {
      type: 'summarytable',
      key: 'summarytable',
      protected: false,
      persistent: true,
      label: 'Summary Table',
      Caption: 'Caption',
      disabled: true,
      hideLabel: false,
      tableView: false,
      input: false
    };
  }

  static get builderInfo() {
    return {
      title: 'Summary Table',
      icon: 'fas fa-file-invoice',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: SummaryTableComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return SummaryTableComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  constructor(component, options, data) {
    super(component, options, data);
    this.tbody = this.ce('tbody');
  }
  build() {
    //super.build();
    this.createElement();
    if (this.component.label && !this.component.hideLabel) {
      this.creatTitle(this.element);
    }
    if (this.options.builder) {
      this.element.appendChild(this.ce('div', {
        class: 'text-muted text-center p-2'
      }, this.text('Summary Table')));
      return;
    }
    const SummaryGroup = this.ce('div', {
      class: 'form-group'
    });
    const table = this.ce('table', {
      class: 'table table-striped table-bordered',
      summary: this.component.label
    });
    // Build header.
    const caption = this.ce('caption', {}, this.text(this.component.Caption));
    const thead = this.ce('thead');
    const thr = this.ce('tr');
    const th1 = this.ce('th', {
      style: 'text-align: center;',
      scope: 'col'
    });
    th1.appendChild(this.text('Summary'));
    const th2 = this.ce('th', {
      style: 'text-align: center;',
      scope: 'col'
    });
    th2.appendChild(this.text('Answer'));
    thr.appendChild(th1);
    thr.appendChild(th2);
    thead.appendChild(thr);
    table.append(caption);
    table.appendChild(thead);
    // Build the body.
    table.appendChild(this.tbody);
    //const tbody = this.ce('tbody');
    //const tr = this.ce('tr');
    //const th3 = this.ce('th', {
    //  style: 'text-align: center;',
    //  scope: 'row'
    //});
    //const td1 = this.ce('td');
    //th3.appendChild(this.text('Demo'));
    //const td2 = this.ce('td');
    //td2.appendChild(this.text('Test'));
    //tr.appendChild(th3);
    ////tr.appendChild(td1);
    //tr.appendChild(td2);
    //tbody.appendChild(tr);
    //table.appendChild(tbody);
    SummaryGroup.appendChild(table);
    this.element.appendChild(SummaryGroup);
    this.errorContainer = this.element;
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-summarytable ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
  creatTitle(container) {
    const heading = this.ce('div', {
      class: `card-header bg-${this.component.theme} panel-heading`
    });
    this.sectionTitle = this.ce('h4', {
      class: 'card-title panel-title'
    });
    this.sectionTitle.appendChild(this.text(this.component.label));
    this.createTooltip(this.sectionTitle, this.component);
    const refresh = this.ce('i', {
      class: 'fas fa-sync text-muted',
      style: 'padding-left: 0.75rem;cursor: pointer;'
    });
    this.sectionTitle.appendChild(refresh);
    heading.appendChild(this.sectionTitle);
    this.loadTbaleData(refresh);
    container.appendChild(heading);
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
      if (this.tooltip) {
        return;
      }
      this.tooltip = new Tooltip(ttElement, {
        trigger: 'hover click',
        placement: 'right',
        html: true,
        title: this.interpolate(component.tooltip).replace(/(?:\r\n|\r|\n)/g, '<br />')
      });
    });
  }
  loadTbaleData(heading) {
    //heading.addEventListener('click', () => this.creatDataTable());
    heading.addEventListener('click', () => this.generateTable());
  }
  generateTable() {
    if (this.root) {
      const sData = generateSummaryData(this.root);
      if (sData !== null) {
        this.loadingSummaryDataToTbody(sData);
      }
    }
  }
  /* eslint-disable max-statements */
  loadingSummaryDataToTbody(summaryJson) {
    const tbody = this.ce('tbody');
    for (const i in summaryJson) {
      const tr = this.ce('tr');
      if (summaryJson[i].title === undefined) {
        if (summaryJson[i].signature === undefined) {
          //const th3 = this.ce('th', {
          //  style: 'text-align: center;',
          //  scope: 'row'
          //});
          const td1 = this.ce('td');
          td1.style.paddingLeft = `${summaryJson[i].Lvl}rem`;
          td1.appendChild(this.text(summaryJson[i].label));
          const td2 = this.ce('td');
          td2.appendChild(this.text(summaryJson[i].value ? summaryJson[i].value : ''));
          //tr.appendChild(th3);
          tr.appendChild(td1);
          tr.appendChild(td2);
        }
        else if (summaryJson[i].signature === true) {
          //const th3 = this.ce('th', {
          //  style: 'text-align: center;',
          //  scope: 'row'
          //});
          //tr.appendChild(th3);
          const td1 = this.ce('td');
          td1.style.paddingLeft = `${summaryJson[i].Lvl}rem`;
          td1.appendChild(this.text(summaryJson[i].label));
          const td2 = this.ce('td', {
            style: 'text-align: -webkit-center;width: 60%;'
          });
          const imgdive = this.ce('div', {
            style: 'width: 50%;'
          });
          const signatureimg = this.ce('img', {
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
        //const th3 = this.ce('th', {
        //   style: 'text-align: center;',
        //   scope: 'row'
        // });
        // tr.appendChild(th3);
        const td = this.ce('td', {
          colspan: 2,
          style: 'font-weight:bold;'
        });
        td.style.paddingLeft = `${summaryJson[i].Lvl}rem`;
        td.appendChild(this.text(summaryJson[i].label));
        tr.appendChild(td);
      }
      tbody.appendChild(tr);
    }
    this.tbody = tbody;
    this.redraw();
  }
}
