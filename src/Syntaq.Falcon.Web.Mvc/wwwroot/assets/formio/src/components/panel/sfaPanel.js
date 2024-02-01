import NestedComponent from '../nested/NestedComponent';

export default class sfaPanelComponent extends NestedComponent {
  static schema(...extend) {
    return NestedComponent.schema({
      label: 'Panel',
      type: 'sfapanel',
      key: 'sfapanel',
      title: 'Panel Heading',
      theme: 'default',
      breadcrumb: 'default',
      breadcrumbClickable: true,
      components: [],
      clearOnHide: false,
      input: false,
      tableView: false,
      dataGridLabel: false,
      persistent: false,
      //border: 1,
      //borderR:4
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Panel',
      icon: 'fa fa-window-maximize',
      group: 'layout',
      documentation: 'http://help.form.io/userguide/#panels',
      weight: 30,
      schema: sfaPanelComponent.schema()
    };
  }

  constructor(component, options, data) {
    component.breadcrumbClickable = component.breadcrumbClickable === undefined ? true : component.breadcrumbClickable;
    component.borderR = component.borderR === undefined ? 4 : component.borderR;
    component.border = component.border === undefined ? 1 : component.border;

    super(component, options, data);
    this.component.title = component.title === undefined ? 'Panel Heading' : component.title;
  }

  get defaultSchema() {
    return sfaPanelComponent.schema();
  }

  getContainer() {
    return this.panelBody;
  }

  get className() {
    return `panel panel-${this.component.theme} ${super.className} formio-component-${this.key}`;
  }

  getCollapseIcon() {
    const collapseIcon = this.getIcon(this.collapsed ? 'plus' : 'minus');
    this.addClass(collapseIcon, 'formio-collapse-icon');
    return collapseIcon;
  }

  setCollapsed(element) {
    super.setCollapsed(element);
    if (this.collapseIcon) {
      const newIcon = this.getCollapseIcon();
      this.panelTitle.replaceChild(newIcon, this.collapseIcon);
      this.collapseIcon = newIcon;
    }
  }
  createElement() {
    if (this.element) {
      return this.element;
    }
    this.element = null;
    this.component.theme = this.component.theme || 'default';
    let panelClass = `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
card `;
    panelClass += `panel panel-${this.component.theme} ${this.className}`;
    panelClass += this.component.customClass;
    panelClass += ` syntaq-component syntaq-component-sfapanel formio-component-${this.key}`;
    this.element = this.ce('div', {
      id: this.id,
      class: panelClass,
      style: ` border: ${this.component.border}px solid rgba(0, 0, 0, 0.125);border-radius: ${this.component.borderR}px;`
    });
    //this.element.component = this;

    //this.hook('element', this.element);

    return this.element;
  }
  build(state) {
    this.createElement();
    this.element.component = this;
    this.hook('element', this.element);
    this.panelBody = this.ce('div', {
      class: 'card-body panel-body'
    });
    this.component.label = this.component.title !==undefined ? this.component.title : this.component.label;
    if (this.component.title && !this.component.hideLabel) {
      //this.component.label = this.component.title;
      const heading = this.ce('div', {
        class: `card-header bg-${this.component.theme} panel-heading`,
        style: this.component.border === 0 ? 'border: none;' : ''
      });
      heading.style.borderTopLeftRadius = `${this.component.borderR * 0.8}px`;
      heading.style.borderTopRightRadius = `${this.component.borderR * 0.8}px`;
      this.panelTitle = this.ce('h4', {
        class: 'card-title panel-title'
      });
      if (this.component.collapsible) {
        this.collapseIcon = this.getCollapseIcon();
        this.panelTitle.appendChild(this.collapseIcon);
        this.panelTitle.appendChild(this.text(' '));
      }

      this.panelTitle.appendChild(this.text(this.component.title));
      this.createTooltip(this.panelTitle);
      heading.appendChild(this.panelTitle);
      this.setCollapseHeader(heading);
      this.element.appendChild(heading);
    }
    else {
      this.createTooltip(this.panelBody, this.component, `${this.iconClass('question-sign')} text-muted formio-hide-label-panel-tooltip`);
    }

    this.addComponents(null, null, null, state);
    this.element.appendChild(this.panelBody);
    this.setCollapsed();
    this.attachLogic();
  }
}
