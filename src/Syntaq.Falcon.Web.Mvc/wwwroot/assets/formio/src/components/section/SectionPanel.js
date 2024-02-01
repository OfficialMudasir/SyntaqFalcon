import NestedComponent from '../nested/NestedComponent';

export default class SectionPanelComponent extends NestedComponent {
  static schema(...extend) {
    return NestedComponent.schema({
      label: 'SectionPanel',
      type: 'sectionpanel',
 //     key: 'sectionPanel',
      title: '',
      theme: 'default',
      breadcrumb: 'default',
      components: [],
      clearOnHide: false,
      input: true,
      tableView: false,
      dataGridLabel: false,
      persistent: false,
      disableAddingRemovingRows:false
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'SectionPanel',
      icon: 'fa fa-list-alt',
//      group: 'layout',
      documentation: 'http://help.form.io/userguide/#panels',
      weight: 30,
      schema: SectionPanelComponent.schema()
    };
  }
  constructor(component, options, data) {
    super(component, options, data);
    this.noEdit = true;
  }

  get defaultSchema() {
    return SectionPanelComponent.schema();
  }

  getContainer() {
    return this.panelBody;
  }

  get className() {
    return `panel panel-${this.component.theme} ${super.className}`;
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

  build(state) {
    this.component.theme = this.component.theme || 'default';
    let panelClass = 'mb-2    ';
    panelClass += `panel panel-${this.component.theme} ${this.component.key}`;
    panelClass += this.component.customClass;
    this.element = this.ce('div', {
      id: this.id,
      class: panelClass,
      style: 'border: 0px solid #dee2e6 !important;'
    });
    this.element.component = this;
    this.panelBody = this.ce('div', {
      class: 'card-body panel-body',
      style:'border:none;'
    });
    if (this.component.title && !this.component.hideLabel) {
      const heading = this.ce('div', {
        class: `card-header bg-${this.component.theme} panel-heading`
      });
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
  //onChange(flags, fromRoot) {
  //  super.onChange(flags, fromRoot);
  //  this.redraw();
  //}
}
