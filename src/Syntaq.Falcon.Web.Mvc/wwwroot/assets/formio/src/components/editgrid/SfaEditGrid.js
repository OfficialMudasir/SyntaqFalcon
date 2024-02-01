import EditGridComponent from '../editgrid/EditGrid';

export default class SfaEditGridComponent extends EditGridComponent {
  static schema(...extend) {
    return EditGridComponent.schema({
      type: 'sfaeditgrid',
      label: 'Edit Grid',
      key: 'sfaEditGrid'
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Edit Grid',
      icon: 'fas fa-th-large',
      group: 'data',
      documentation: 'http://help.form.io/userguide/#editgrid',
      weight: 40,
      schema: SfaEditGridComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaEditGridComponent.schema();
  }
}
