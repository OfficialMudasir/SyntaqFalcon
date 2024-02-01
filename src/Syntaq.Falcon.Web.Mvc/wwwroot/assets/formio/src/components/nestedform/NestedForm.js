import _ from 'lodash';
import BaseComponent from '../base/Base';
import Promise from 'native-promise-only';
import { eachComponent } from '../../utils/utils';
import Form from '../../Form';
import formJson from './JSONdata';

export default class NestedFormComponent extends BaseComponent {
  static schema(...extend) {
    return BaseComponent.schema({
      label: '',
      type: 'nestedform',
      key: 'nestedForm',
      reference: true,
      FormId: '',
      formurl:''
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Nested Form',
      icon: 'fab fa-wpforms',
      group: 'testGroup',
      documentation: 'http://help.form.io/userguide/#form',
      schema: NestedFormComponent.schema()
    };
  }

  constructor(component, options, data) {
    super(component, options, data);
    this.subForm = null;
    this.formSrc = '';
    this.subFormReady = new Promise((resolve, reject) => {
      this.subFormReadyResolve = resolve;
      this.subFormReadyReject = reject;
    });
  }

  get defaultSchema() {
    return NestedFormComponent.schema();
  }

  get emptyValue() {
    return { data: {} };
  }

  destroy() {
    const state = super.destroy() || {};
    if (this.subForm) {
      this.subForm.destroy();
    }
    return state;
  }

  /**
   * Render a subform.
   *
   * @param form
   * @param options
   */
  renderSubForm(form, options) {
    if (this.options.builder) {
      this.element.appendChild(this.ce('div', {
        class: 'text-muted text-center p-2'
      }, this.text(this.component.label ? this.component.label:'undefined')));
      return;
    }
    // Iterate through every component and hide the submit button.
    eachComponent(form.components, (component) => {
      if (((component.type === 'button') || (component.type === 'sfabutton')) &&
        ((component.action === 'submit') || (component.action === 'event') || (component.label === 'Submit'))) {
        component.hidden = true;
      }
    });
   // this.component.ecomponents = form.components;//
    options.isNestedform = true;
    (new Form(this.element, form, options)).render().then((instance) => {
      this.subForm = instance;
      this.subForm.on('change', () => {
        this.dataValue = this.subForm.getValue();
        this.onChange();
      });
      this.restoreValue();
      this.subFormReadyResolve(this.subForm);
      return this.subForm;
    });
  }

  /**
   * Load the subform.
   */
  /* eslint-disable max-statements */
  loadSubForm() {
    // Only load the subform if the subform isn't loaded and the conditions apply.
    if (this.subFormLoaded) {
      return this.subFormReady;
    }
    this.subFormLoaded = true;
    const srcOptions = {};
    if (this.component.reference) {
      this.component.submit = true;
    }
    let formObj = formJson;
    let formurl = '';
    if (this.component.FormId) {
      if ((window.location.port != null)) {
        //this.component.formurl = `${location.protocol}//${window.location.hostname}:${window.location.port}/api/services/app/Forms/GetSchema?Id=${this.component.FormId}`;
        formurl = `${location.protocol}//${window.location.hostname}:${window.location.port}/api/services/app/Forms/GetSchema?Id=${this.component.FormId}`;
      }
      else {
        //this.component.formurl = `${location.protocol}//${window.location.hostname}/api/services/app/Forms/GetSchema?Id=${this.component.FormId}`;
        formurl = `${location.protocol}//${window.location.hostname}/api/services/app/Forms/GetSchema?Id=${this.component.FormId}`;
      }
    }
    // If in Builder, it don't need to call the schema
    //if (this.options.builder) {
    //  this.element.appendChild(this.ce('div', {
    //    class: 'text-muted text-center p-2'
    //  }, this.text(this.component.label ? this.component.label : 'undefined')));
    //  return this.subFormReady;
    //}
    this.element.appendChild(this.ce('div', {
      class: 'text-muted text-center p-2'
    }, this.text(this.component.label ? this.component.label : 'undefined')));
    if (this.options.preview) {
    const result = fetch(formurl)
      .then(response => response.json())
      .then(data => {
        const temp = JSON.parse(data.result);
        formObj = temp;
          this.renderSubForm(formObj, srcOptions);
          return this.subFormReady;
        })
      .catch(error => console.error());
      return result;
    }

    return this.subFormReady;
  }
  /* eslint-enable max-statements */

  checkValidity(data, dirty) {
    if (this.subForm) {
      return this.subForm.checkValidity(this.dataValue.data, dirty);
    }

    return super.checkValidity(data, dirty);
  }

  checkConditions(data) {
    return (super.checkConditions(data) && this.subForm)
      ? this.subForm.checkConditions(this.dataValue.data)
      : false;
  }

  calculateValue(data, flags) {
    if (this.subForm) {
      return this.subForm.calculateValue(this.dataValue.data, flags);
    }

    return super.calculateValue(data, flags);
  }

  setPristine(pristine) {
    super.setPristine(pristine);
    if (this.subForm) {
      this.subForm.setPristine(pristine);
    }
  }

  /**
   * Submit the form before the next page is triggered.
   */
  beforeNext() {
    // If we wish to submit the form on next page, then do that here.
    if (this.component.submit) {
      return this.loadSubForm().then(() => {
        return this.subForm.submitForm().then(result => {
          this.dataValue = result.submission;
          return this.dataValue;
        }).catch(err => {
          this.subForm.onSubmissionError(err);
          return Promise.reject(err);
        });
      });
    }
    else {
      return super.beforeNext();
    }
  }

  /**
   * Submit the form before the whole form is triggered.
   */
  beforeSubmit() {
    const submission = this.dataValue;

    // This submission has already been submitted, so just return the reference data.
    if (submission && submission._id && submission.form) {
      this.dataValue = this.component.reference ? {
        _id: submission._id,
        form: submission.form
      } : submission;
      return Promise.resolve(this.dataValue);
    }

    // This submission has not been submitted yet.
    if (this.component.submit) {
      return this.loadSubForm().then(() => {
        return this.subForm.submitForm()
          .then(result => {
            this.subForm.loading = false;
            this.dataValue = this.component.reference ? {
              _id: result.submission._id,
              form: result.submission.form
            } : result.submission;
            return this.dataValue;
          })
          .catch(() => {});
      });
    }
    else {
      return super.beforeSubmit();
    }
  }

  build() {
    this.createElement();
    // Do not restore the value when building before submission.
    if (!this.options.beforeSubmit) {
      this.restoreValue();
    }
    this.attachLogic();
  }

  setValue(submission, flags) {
    const changed = super.setValue(submission, flags);
    (this.subForm ? Promise.resolve(this.subForm) : this.loadSubForm())
      .then((form) => {
        if (_.isEmpty(submission.data)) {
          submission.data = this.dataValue.data;
        }
      });

    return changed;
  }

  getValue() {
    if (this.subForm) {
      return this.subForm.getValue();
    }
    return this.dataValue;
  }

  getAllComponents() {
    if (!this.subForm) {
      return [];
    }
    return this.subForm.getAllComponents();
  }

  updateSubFormVisibility() {
    if (this.subForm) {
      this.subForm.parentVisible = this.visible;
    }
  }

  get visible() {
    return super.visible;
  }
//this.subForm.parentVisible: true
  set visible(value) {
    super.visible = value;
    this.updateSubFormVisibility();
  }

  get parentVisible() {
    return super.parentVisible;
  }

  set parentVisible(value) {
    super.parentVisible = value;
    this.updateSubFormVisibility();
  }
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-nestedform ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
