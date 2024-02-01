import _ from 'lodash';
import moment from 'moment';
import EventEmitter from 'eventemitter2';
import i18next from 'i18next';
import Formio from './Formio';
import Promise from 'native-promise-only';
import Components from './components/Components';
import NestedComponent from './components/nested/NestedComponent';
import {
  currentTimezone, unescapeHTML, getStringFromComponentPath,
  searchComponents,
  convertStringToHTMLElement,
  getArrayFromComponentPath } from './utils/utils';

// Initialize the available forms.
Formio.forms = {};

// Allow people to register components.
Formio.registerComponent = Components.setComponent;

function getOptions(options) {
  options = _.defaults(options, {
    submitOnEnter: false,
    icons: Formio.icons || '',
    i18next,
    saveDraft: false,
    saveDraftThrottle: 5000
  });
  if (!options.events) {
    options.events = new EventEmitter({
      wildcard: false,
      maxListeners: 0
    });
  }
  return options;
}

/**
 * Renders a Form.io form within the webpage.
 */
export default class Webform extends NestedComponent {
  /**
   * Creates a new Form instance.
   *
   * @param {Object} element - The DOM element you wish to render this form within.
   * @param {Object} options - The options to create a new form instance.
   * @param {boolean} options.saveDraft - Set this if you would like to enable the save draft feature.
   * @param {boolean} options.saveDraftThrottle - The throttle for the save draft feature.
   * @param {boolean} options.readOnly - Set this form to readOnly
   * @param {boolean} options.noAlerts - Set to true to disable the alerts dialog.
   * @param {boolean} options.i18n - The translation file for this rendering. @see https://github.com/formio/formio.js/blob/master/i18n.js
   * @param {boolean} options.template - Provides a way to inject custom logic into the creation of every element rendered within the form.
   */
  /* eslint-disable max-statements */
  constructor(element, options) {
    super(null, getOptions(options));

    // Keep track of all available forms globally.
    Formio.forms[this.id] = this;

    // Set the base url.
    if (this.options.baseUrl) {
      Formio.setBaseUrl(this.options.baseUrl);
    }

    /**
     * The i18n configuration for this component.
     */
    let i18n = require('./i18n').default;
    if (options && options.i18n && !options.i18nReady) {
      // Support legacy way of doing translations.
      if (options.i18n.resources) {
        i18n = options.i18n;
      }
      else {
        _.each(options.i18n, (lang, code) => {
          if (code === 'options') {
            _.merge(i18n, lang);
          }
          else if (!i18n.resources[code]) {
            i18n.resources[code] = { translation: lang };
          }
          else {
            _.assign(i18n.resources[code].translation, lang);
          }
        });
      }

      options.i18n = i18n;
      options.i18nReady = true;
    }

    if (options && options.i18n) {
      this.options.i18n = options.i18n;
    }
    else {
      this.options.i18n = i18n;
    }

    // Set the language.
    if (this.options.language) {
      this.options.i18n.lng = this.options.language;
    }

    /**
     * The type of this element.
     * @type {string}
     */
    this.type = 'form';
    this._src = '';
    this._loading = false;
    this._submission = {};
    this._form = {};
    //this.errorLinks = document.querySelectorAll(".error-links");
    this.linkStyles = {
      color: 'red', // Keep the same danger color
      textDecoration: 'underline',
    };
    this.draftEnabled = false;
    this.savingDraft = true;
    if (this.options.saveDraftThrottle) {
      this.triggerSaveDraft = _.throttle(this.saveDraft.bind(this), this.options.saveDraftThrottle);
    }
    else {
      this.triggerSaveDraft = this.saveDraft.bind(this);
    }

    this.customErrors = [];
    this.TotalErrorElements = [];

    /**
     * Determines if this form should submit the API on submit.
     * @type {boolean}
     */
    this.nosubmit = false;

    /**
     * Determines if the form has tried to be submitted, error or not.
     *
     * @type {boolean}
     */
    this.submitted = false;

    /**
     * Determines if the form is being submitted at the moment.
     *
     * @type {boolean}
     */
    this.submitting = false;

    /**
     * The Formio instance for this form.
     * @type {Formio}
     */
    this.formio = null;

    /**
     * The loader HTML element.
     * @type {HTMLElement}
     */
    this.loader = null;

    /**
     * The alert HTML element
     * @type {HTMLElement}
     */
    this.alert = null;

    /**
     * Promise that is triggered when the submission is done loading.
     * @type {Promise}
     */
    this.onSubmission = null;

    /**
     * Promise that is triggered when the form is done building.
     * @type {Promise}
     */
    this.onFormBuild = null;

    /**
     * Determines if this submission is explicitly set.
     * @type {boolean}
     */
    this.submissionSet = false;

    /**
     * Promise that executes when the form is ready and rendered.
     * @type {Promise}
     *
     * @example
     * import Webform from 'formiojs/Webform';
     * let form = new Webform(document.getElementById('formio'));
     * form.formReady.then(() => {
     *   console.log('The form is ready!');
     * });
     * form.src = 'https://examples.form.io/example';
     */
    this.formReady = new Promise((resolve, reject) => {
      /**
       * Called when the formReady state of this form has been resolved.
       *
       * @type {function}
       */
      this.formReadyResolve = resolve;

      /**
       * Called when this form could not load and is rejected.
       *
       * @type {function}
       */
      this.formReadyReject = reject;
    });

    /**
     * Promise that executes when the submission is ready and rendered.
     * @type {Promise}
     *
     * @example
     * import Webform from 'formiojs/Webform';
     * let form = new Webform(document.getElementById('formio'));
     * form.submissionReady.then(() => {
     *   console.log('The submission is ready!');
     * });
     * form.src = 'https://examples.form.io/example/submission/234234234234234243';
     */
    this.submissionReady = new Promise((resolve, reject) => {
      /**
       * Called when the formReady state of this form has been resolved.
       *
       * @type {function}
       */
      this.submissionReadyResolve = resolve;

      /**
       * Called when this form could not load and is rejected.
       *
       * @type {function}
       */
      this.submissionReadyReject = reject;
    });

    /**
     * Promise to trigger when the element for this form is established.
     *
     * @type {Promise}
     */
    this.onElement = new Promise((resolve) => {
      /**
       * Called when the element has been resolved.
       *
       * @type {function}
       */
      this.elementResolve = resolve;
      this.setElement(element);
    });

    this.shortcuts = [];

    // Set language after everything is established.
    this.localize().then(() => {
      this.language = this.options.language;
    });

    // See if we need to restore the draft from a user.
    if (this.options.saveDraft && Formio.events) {
      Formio.events.on('formio.user', (user) => {
        this.formReady.then(() => {
          // Only restore a draft if the submission isn't explicitly set.
          if (!this.submissionSet) {
            this.restoreDraft(user._id);
          }
        });
      });
    }

    this.component.clearOnHide = false;
    this.visiteTabel = {};
  }
  /* eslint-enable max-statements */

  /**
   * Sets the language for this form.
   *
   * @param lang
   * @return {Promise}
   */
  set language(lang) {
    return new Promise((resolve, reject) => {
      this.options.language = lang;
      try {
        i18next.changeLanguage(lang, (err) => {
          if (err) {
            return reject(err);
          }
          this.redraw();
          this.emit('languageChanged');
          resolve();
        });
      }
      catch (err) {
        return reject(err);
      }
    });
  }

  /**
   * Add a language for translations
   *
   * @param code
   * @param lang
   * @param active
   * @return {*}
   */
  addLanguage(code, lang, active = false) {
    i18next.addResourceBundle(code, 'translation', lang, true, true);
    if (active) {
      this.language = code;
    }
  }

  /**
   * Perform the localization initialization.
   * @returns {*}
   */
  localize() {
    if (i18next.initialized) {
      return Promise.resolve(i18next);
    }
    i18next.initialized = true;
    return new Promise((resolve, reject) => {
      try {
        i18next.init(this.options.i18n, (err) => {
          // Get language but remove any ;q=1 that might exist on it.
          this.options.language = i18next.language.split(';')[0];
          if (err) {
            return reject(err);
          }
          resolve(i18next);
        });
      }
      catch (err) {
        return reject(err);
      }
    });
  }

  /**
   * Sets the the outside wrapper element of the Form.
   *
   * @param {HTMLElement} element - The element to set as the outside wrapper element for this form.
   */
  setElement(element) {
    if (!element) {
      return;
    }

    if (this.element) {
      this.element.removeEventListener('keydown', this.executeShortcuts.bind(this));
    }

    this.wrapper = element;
    this.element = this.ce('div');
    this.wrapper.appendChild(this.element);
    this.showElement(false);
    this.element.addEventListener('keydown', this.executeShortcuts.bind(this));
    let classNames = this.element.getAttribute('class');
    classNames += ' formio-form';
    this.addClass(this.wrapper, classNames);
    this.loading = true;
    this.elementResolve(element);
  }

  keyboardCatchableElement(element) {
    if (element.nodeName === 'TEXTAREA') {
      return false;
    }

    if (element.nodeName === 'INPUT') {
      return [
        'text',
        'email',
        'password'
      ].indexOf(element.type) === -1;
    }

    return true;
  }

  executeShortcuts(event) {
    const { target } = event;
    if (!this.keyboardCatchableElement(target)) {
      return;
    }

    const ctrl = event.ctrlKey || event.metaKey;
    const keyCode = event.keyCode;
    let char = '';

    if (65 <= keyCode && keyCode <= 90) {
      char = String.fromCharCode(keyCode);
    }
    else if (keyCode === 13) {
      char = 'Enter';
    }
    else if (keyCode === 27) {
      char = 'Esc';
    }

    _.each(this.shortcuts, (shortcut) => {
      if (shortcut.ctrl && !ctrl) {
        return;
      }

      if (shortcut.shortcut === char) {
        shortcut.element.click();
        event.preventDefault();
      }
    });
  }

  addShortcut(element, shortcut) {
    if (!shortcut || !/^([A-Z]|Enter|Esc)$/i.test(shortcut)) {
      return;
    }

    shortcut = _.capitalize(shortcut);

    if (shortcut === 'Enter' || shortcut === 'Esc') {
      // Restrict Enter and Esc only for buttons
      if (element.tagName !== 'BUTTON') {
        return;
      }

      this.shortcuts.push({
        shortcut,
        element
      });
    }
    else {
      this.shortcuts.push({
        ctrl: true,
        shortcut,
        element
      });
    }
  }

  removeShortcut(element, shortcut) {
    if (!shortcut || !/^([A-Z]|Enter|Esc)$/i.test(shortcut)) {
      return;
    }

    _.remove(this.shortcuts, {
      shortcut,
      element
    });
  }

  /**
   * Get the embed source of the form.
   *
   * @returns {string}
   */
  get src() {
    return this._src;
  }

  /**
   * Loads the submission if applicable.
   */
  loadSubmission() {
    this.loadingSubmission = true;
    if (this.formio.submissionId) {
      this.onSubmission = this.formio.loadSubmission().then(
        (submission) => this.setSubmission(submission),
        (err) => this.submissionReadyReject(err)
      ).catch(
        (err) => this.submissionReadyReject(err)
      );
    }
    else {
      this.submissionReadyResolve();
    }
    return this.submissionReady;
  }

  /**
   * Set the src of the form renderer.
   *
   * @param value
   * @param options
   */
  setSrc(value, options) {
    if (this.setUrl(value, options)) {
      this.nosubmit = false;
      this.formio.loadForm({ params: { live: 1 } }).then(
        (form) => {
          const setForm = this.setForm(form);
          this.loadSubmission();
          return setForm;
        }).catch((err) => {
        console.warn(err);
        this.formReadyReject(err);
      });
    }
  }

  /**
   * Set the Form source, which is typically the Form.io embed URL.
   *
   * @param {string} value - The value of the form embed url.
   *
   * @example
   * import Webform from 'formiojs/Webform';
   * let form = new Webform(document.getElementById('formio'));
   * form.formReady.then(() => {
   *   console.log('The form is formReady!');
   * });
   * form.src = 'https://examples.form.io/example';
   */
  set src(value) {
    this.setSrc(value);
  }

  /**
   * Get the embed source of the form.
   *
   * @returns {string}
   */
  get url() {
    return this._src;
  }

  /**
   * Sets the url of the form renderer.
   *
   * @param value
   * @param options
   */
  setUrl(value, options) {
    if (
      !value ||
      (typeof value !== 'string') ||
      (value === this._src)
    ) {
      return false;
    }
    this._src = value;
    this.nosubmit = true;
    this.formio = this.options.formio = new Formio(value, options);

    if (this.type === 'form') {
      // Set the options source so this can be passed to other components.
      this.options.src = value;
    }
    return true;
  }

  /**
   * Set the form source but don't initialize the form and submission from the url.
   *
   * @param {string} value - The value of the form embed url.
   */
  set url(value) {
    this.setUrl(value);
  }

  /**
   * Called when both the form and submission have been loaded.
   *
   * @returns {Promise} - The promise to trigger when both form and submission have loaded.
   */
  get ready() {
    return this.formReady.then(() => {
      return this.loadingSubmission ? this.submissionReady : true;
    });
  }

  /**
   * Returns if this form is loading.
   *
   * @returns {boolean} - TRUE means the form is loading, FALSE otherwise.
   */
  get loading() {
    return this._loading;
  }

  /**
   * Set the loading state for this form, and also show the loader spinner.
   *
   * @param {boolean} loading - If this form should be "loading" or not.
   */
  set loading(loading) {
    if (this._loading !== loading) {
      this._loading = loading;
      if (!this.loader && loading) {
        this.loader = this.ce('div', {
          class: 'loader-wrapper'
        });
        const spinner = this.ce('div', {
          class: 'loader text-center'
        });
        this.loader.appendChild(spinner);
      }
      /* eslint-disable max-depth */
      if (this.loader) {
        try {
          if (loading) {
            this.prependTo(this.loader, this.wrapper);
          }
          else {
            this.removeChildFrom(this.loader, this.wrapper);
          }
        }
        catch (err) {
          // ingore
        }
      }
      /* eslint-enable max-depth */
    }
  }

  /**
   * Sets the JSON schema for the form to be rendered.
   *
   * @example
   * import Webform from 'formiojs/Webform';
   * let form = new Webform(document.getElementById('formio'));
   * form.setForm({
   *   components: [
   *     {
   *       type: 'textfield',
   *       key: 'firstName',
   *       label: 'First Name',
   *       placeholder: 'Enter your first name.',
   *       input: true
   *     },
   *     {
   *       type: 'textfield',
   *       key: 'lastName',
   *       label: 'Last Name',
   *       placeholder: 'Enter your last name',
   *       input: true
   *     },
   *     {
   *       type: 'button',
   *       action: 'submit',
   *       label: 'Submit',
   *       theme: 'primary'
   *     }
   *   ]
   * });
   *
   * @param {Object} form - The JSON schema of the form @see https://examples.form.io/example for an example JSON schema.
   * @returns {*}
   */
  /* eslint-disable max-statements */
  setForm(form) {
    //if (form.display === 'wizard') {
    //  console.warn('You need to instantiate the FormioWizard class to use this form as a wizard.');
    //}
    //-----
    this.options.buttonSetting = form.buttons ? form.buttons : {};
    const buttons = form.buttons ? form.buttons : {};
    const isNestedform = this.options.isNestedform === undefined ? false : true;
    if (form.display === 'form' && !this.options.builder && !isNestedform) {
      const wizardNav = this.ce('ul', {
        class: 'list-inline',
        style: 'clear:both;'
      });
      [
        //{ name: 'Draft', method: 'draft', class: 'btn btn-default btn-secondary' },
        { name: 'Clear', method: 'cancel', class: 'btn btn-default btn-secondary' },
        { name: 'Save', method: 'save', class: 'btn btn-primary' },
        { name: 'Submit', method: 'submit', class: 'btn btn-primary' }
      ].forEach((button) => {
        const buttonWrapper = this.ce('li', {
          class: 'list-inline-item'
        });
        const buttonElement = this.ce('button', {
          class: `${button.class} btn-form-nav-${button.name}`
        });
        if (button.name === 'Save') {
          //buttonElement.setAttribute('disabled', buttons.save_able?)
          buttonElement.appendChild(this.text(buttons.save_label ? buttons.save_label : 'Save'));
          buttons.save_disable !== 'true'?
            '' : buttonElement.setAttribute('disabled', 'disabled');
          buttons.save_hide !== 'true' ?
            '' : buttonElement.setAttribute('hidden', true);
        }
        else if (button.name === 'Submit') {
          buttonElement.appendChild(this.text(buttons.submit_label ? buttons.submit_label : 'Submit'));
          buttons.submit_disable !== 'true' ?
            '' : buttonElement.setAttribute('disabled', 'disabled');
          buttons.submit_hide !== 'true' ?
            '' : buttonElement.setAttribute('hidden', true);
        }
        else if (button.name === 'Clear') {
          buttonElement.appendChild(this.text(buttons.clear_label ? buttons.clear_label : 'Clear'));
          buttons.clear_disable !== 'true' ?
            '' : buttonElement.setAttribute('disabled', 'disabled');
          buttons.clear_hide !== 'true' ?
            '' : buttonElement.setAttribute('hidden', true);
        }
        else if (button.name === 'Draft') {
          buttonElement.appendChild(this.text(buttons.draft_label ? buttons.draft_label : 'Draft'));
          buttons.draft_disable !== 'true' ?
            '' : buttonElement.setAttribute('disabled', 'disabled');
          buttons.draft_hide !== 'true' ?
            '' : buttonElement.setAttribute('hidden', true);
        }
        buttonElement.addEventListener('click', (event) => {
          event.preventDefault();
          if (button.method === 'save') {
            this.emit('save', this.submission.data);
          }
          else if (button.method === 'submit') {
            //this.emit('submit', this._submission);
            //this.onSubmit(this._submission);
            this.submit();
          }
          else if (button.method === 'cancel') {
            this.cancel();
          }
          else if (button.method === 'draft') {
            this.emit('draft', this.submission);
          }
        });
          buttonWrapper.appendChild(buttonElement);
          wizardNav.appendChild(buttonWrapper);
        });
      this.wrapper.appendChild(wizardNav);
    }
    //----

    if (this.onFormBuild) {
      return this.onFormBuild.then(
        () => this.createForm(form),
        (err) => this.formReadyReject(err)
      ).catch(
        (err) => this.formReadyReject(err)
      );
    }

    // Create the form.
    this._form = form;
    // Allow the form to provide component overrides.
    if (form && form.settings && form.settings.components) {
      this.options.components = form.settings.components;
    }

    this.initialized = false;
    return this.createForm(form).then(() => {
      this.emit('formLoad', form);
      return form;
    });
  }
  /* eslint-disable max-statements */

  /**
   * Gets the form object.
   *
   * @returns {Object} - The form JSON schema.
   */
  get form() {
    return this._form;
  }

  /**
   * Sets the form value.
   *
   * @alias setForm
   * @param {Object} form - The form schema object.
   */
  set form(form) {
    this.setForm(form);
  }

  /**
   * Returns the submission object that was set within this form.
   *
   * @returns {Object}
   */
  get submission() {
    return this.getValue();
  }

  /**
   * Sets the submission of a form.
   *
   * @example
   * import Webform from 'formiojs/Webform';
   * let form = new Webform(document.getElementById('formio'));
   * form.src = 'https://examples.form.io/example';
   * form.submission = {data: {
   *   firstName: 'Joe',
   *   lastName: 'Smith',
   *   email: 'joe@example.com'
   * }};
   *
   * @param {Object} submission - The Form.io submission object.
   */
  set submission(submission) {
    this.setSubmission(submission);
  }

  /**
   * Sets a submission and returns the promise when it is ready.
   * @param submission
   * @param flags
   * @return {Promise.<TResult>}
   */
  setSubmission(submission, flags) {
    return this.onSubmission = this.formReady.then(
      () => {
        // If nothing changed, still trigger an update.
        this.submissionSet = true;
        if (!this.setValue(submission, flags)) {
          this.triggerChange({
            noValidate: true
          });
        }
        return this.dataReady.then(() => this.submissionReadyResolve(submission));
      },
      (err) => this.submissionReadyReject(err)
    ).catch(
      (err) => this.submissionReadyReject(err)
    );
  }

  /**
   * Saves a submission draft.
   */
  saveDraft() {
    if (!this.draftEnabled) {
      return;
    }
    if (!this.formio) {
      console.warn('Cannot save draft because there is no formio instance.');
      return;
    }
    if (!Formio.getUser()) {
      console.warn('Cannot save draft unless a user is authenticated.');
      return;
    }
    const draft = _.cloneDeep(this.submission);
    draft.state = 'draft';
    if (!this.savingDraft) {
      this.savingDraft = true;
      this.formio.saveSubmission(draft).then((sub) => {
        this.savingDraft = false;
        this.emit('saveDraft', sub);
      });
    }
  }

  /**
   * Restores a draft submission based on the user who is authenticated.
   *
   * @param {userId} - The user id where we need to restore the draft from.
   */
  restoreDraft(userId) {
    if (!this.formio) {
      console.warn('Cannot restore draft because there is no formio instance.');
      return;
    }
    this.savingDraft = true;
    this.formio.loadSubmissions({
      params: {
        state: 'draft',
        owner: userId
      }
    }).then(submissions => {
      if (submissions.length > 0) {
        const draft = _.cloneDeep(submissions[0]);
        return this.setSubmission(draft).then(() => {
          this.draftEnabled = true;
          this.savingDraft = false;
          this.emit('restoreDraft', draft);
        });
      }
      // Enable drafts so that we can keep track of changes.
      this.draftEnabled = true;
      this.savingDraft = false;
      this.emit('restoreDraft', null);
    });
  }

  get schema() {
    const schema = this._form;
    schema.components = [];
    this.eachComponent((component) => schema.components.push(component.schema));
    return schema;
  }

  mergeData(_this, _that) {
    _.mergeWith(_this, _that, (thisValue, thatValue) => {
      if (Array.isArray(thisValue) && Array.isArray(thatValue) && thisValue.length !== thatValue.length) {
        return thatValue;
      }
    });
  }

  setValue(submission, flags) {
    if (!submission || !submission.data) {
      submission = { data: {} };
    }
    // Metadata needs to be available before setValue
    this._submission.metadata = submission.metadata || {};

    // Set the timezone in the options if available.
    if (
      !this.options.submissionTimezone &&
      submission.metadata &&
      submission.metadata.timezone
    ) {
      this.options.submissionTimezone = submission.metadata.timezone;
    }

    const changed = super.setValue(submission.data, flags);
    this.mergeData(this.data, submission.data);
    submission.data = this.data;
    this._submission = submission;
    return changed;
  }

  getValue() {
    if (!this._submission.data) {
      this._submission.data = {};
    }
    if (this.viewOnly) {
      return this._submission;
    }
    const submission = this._submission;
    submission.data = this.data;
    return this._submission;
  }

  /**
   * Create a new form.
   *
   * @param {Object} form - The form object that is created.
   * @returns {Promise.<TResult>}
   */
  createForm(form) {
    /**
     * {@link BaseComponent.component}
     */
    if (this.component) {
      this.component.components = form.components;
    }
    else {
      this.component = form;
    }
    return this.onFormBuild = this.render().then(() => {
      this.formReadyResolve();
      this.onFormBuild = null;
      this.setValue(this.submission);
      if (!this.changing) {
        this.triggerChange();
      }
      return form;
    }).catch((err) => {
      console.warn(err);
      this.formReadyReject(err);
    });
  }

  /**
   * Render the form within the HTML element.
   * @returns {Promise.<TResult>}
   */
  render() {
    return this.onElement.then(() => {
      const state = this.clear();
      this.showElement(false);
      clearTimeout(this.build(state));
      this.isBuilt = true;
      this.on('resetForm', () => this.resetValue(), true);
      this.on('deleteSubmission', () => this.deleteSubmission(), true);
      this.on('refreshData', () => this.updateValue(), true);
      setTimeout(() => this.emit('render'), 1);
    });
  }

  resetValue() {
    _.each(this.getComponents(), (comp) => (comp.resetValue()));
    this.setPristine(true);
  }

  /**
   * Sets a new alert to display in the error dialog of the form.
   *
   * @param {string} type - The type of alert to display. "danger", "success", "warning", etc.
   * @param {string} message - The message to show in the alert.
   */
  setAlert(type, message) {
    if (this.options.noAlerts) {
      if (!message) {
        this.emit('error', false);
      }
      return;
    }
    if (this.alert) {
      try {
        this.removeChild(this.alert);
        this.alert = null;
      }
      catch (err) {
        // ignore
      }
    }
    if (message) {
      this.alert = this.ce('div', {
        class: `alert alert-${type}`,
        role: 'alert',
        id: 'errorElementList'
      });
      this.alert.innerHTML = message;
    }
    if (!this.alert) {
      return;
    }
    this.prepend(this.alert);
  }
  /**
   * Build the form.
   */
  build(state) {
    this.on('submitButton', (options) => this.submit(false, options), true);
    this.on('checkValidity', (data) => this.checkValidity(null, true, data), true);
    this.addComponents(null, null, null, state);
    this.on('requestUrl', (args) => (this.submitUrl(args.url,args.headers)), true);
    return setTimeout(() => {
      this.onChange({
        noEmit: true
      });
    }, 1);
  }

  /**
   * Show the errors of this form within the alert dialog.
   *
   * @param {Object} error - An optional additional error to display along with the component errors.
   * @returns {*}
   */
  showErrors(error, triggerEvent) {
    this.loading = false;
    //this.data.TotalErrors = [];this.data.TotalErrorElements
    let errors = this.errors;
    if (this.data.FormErrors !== undefined && this.data.FormErrors.length > 0 && errors.length <= 0) {
      errors = this.data.FormErrors;
      this.TotalErrorElements = errors;
    }
    if (this.TotalErrorElements.length <= 0) {
      this.TotalErrorElements = this.TotalErrorElements.concat(errors);
    }
    else {
      errors = errors.concat(this.TotalErrorElements);
      // Remove duplicates from the errors array
      errors = errors.filter((error, index, self) =>
        index === self.findIndex(e => e.component.id === error.component.id)
      );
      this.TotalErrorElements = errors;
    }
    if (error) {
      if (Array.isArray(error)) {
        errors = errors.concat(error);
      }
      else {
        errors.push(error);
      }
    }

    errors = errors.concat(this.customErrors);

    if (!errors.length) {
      this.setAlert(false);
      return;
    }

    // Mark any components as invalid if in a custom message.
    errors.forEach((err) => {
      const { components = [] } = err;

      if (err.component) {
        components.push(err.component);
      }

      // STQ MOdified
      if (err.path === undefined) {
        err.path = [err.component.key];
        components.push(err.path);
      }
      else {
        components.push(err.path);
      }
      //if (err.path) {
      //  components.push(err.path);
      //}

      components.forEach((path) => {
        const component = this.getComponent(path, _.identity, err);
        const components = _.compact(Array.isArray(component) ? component : [component]);

        components.forEach((component) => component.setCustomValidity(err.message, true));
      });
    });

    //const message = `
    //  <p>${this.t('error')}</p>
    //  <ul>
    //    ${errors.map((err) => err ? `<li><strong>${err.message || err}</strong></li>` : '').join('')}
    //  </ul>
    //`;

    //STQ Modified
    const message = `
      <p>${this.t('error')}</p>
      <ul>
        ${errors.map((err, index) => err ? `<li><span id="${err.component.id}_error${index + 1}" class="alert-danger error-links" style="text-decoration: underline; cursor: pointer;" onclick="handleErrorLinkClick()"><strong id="${err.component.id}_error">${err.message || err}</strong></span></li>` : '').join('')}
      </ul>
    `;
    this.setAlert('danger', message);
    for (var i in this.components) {
      this.targetElement = this.components[i].errorContainer;
      if (this.targetElement) {
        break;
      }
    }
    try {
      if ((this.submitButton && this.submitButton.attributes.disabled !== undefined) || (this.nextButton && this.nextButton.attributes.disabled !== undefined)) {
        window.scrollTo({ top: 0, behavior: 'smooth', block: 'center' });
      }
      else {
        if (this.targetElement) {
          const formElement = this.targetElement.querySelector('input, select, textarea');
          // eslint-disable-next-line max-depth
          if (this.data.TargetId !== undefined || this.data.TargetId !== null) {
            const targetDiv = document.getElementById(this.data.TargetId);
            // eslint-disable-next-line max-depth
            if (targetDiv !== null) {
              const formEle = targetDiv.querySelector('input, select, textarea');
              // eslint-disable-next-line max-depth
              if (formEle) {
                formEle.focus();
                // Scroll to the form element
                formEle.scrollIntoView({ behavior: 'smooth', block: 'center' });
                this.data.TargetId = null;
              }
            }
            //else {
            //  window.scrollTo({ top: 0, behavior: 'smooth', block: 'center' });
            //}
          }
          else {
            // eslint-disable-next-line max-depth
            if (formElement) {
              formElement.focus();
              // Scroll to the form element
              formElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
          }
        }
      }
    }
    catch (e) {
      //ignore
    }
    if (triggerEvent) {
      this.emit('error', errors);
    }
    return errors;
  }

  /**
   * Called when the submission has completed, or if the submission needs to be sent to an external library.
   *
   * @param {Object} submission - The submission object.
   * @param {boolean} saved - Whether or not this submission was saved to the server.
   * @returns {object} - The submission object.
   */
  onSubmit(submission, saved) {
    this.loading = false;
    this.submitting = false;
    this.setPristine(true);
    this.setValue(submission, {
      noValidate: true,
      noCheck: true
    });
    this.setAlert('success', `<p>${this.t('complete')}</p>`);
    if (!submission.hasOwnProperty('saved')) {
      submission.saved = saved;
    }
    this.emit('submit', submission);
    if (saved) {
      this.emit('submitDone', submission);
    }
    return submission;
  }

  /**
   * Called when an error occurs during the submission.
   *
   * @param {Object} error - The error that occured.
   */
  onSubmissionError(error) {
    if (error) {
      // Normalize the error.
      if (typeof error === 'string') {
        error = { message: error };
      }

      if ('details' in error) {
        error = error.details;
      }
    }

    this.submitting = false;
    this.setPristine(false);
    return this.showErrors(error, true);
  }
  onSubmissionError_Old(error) {
    if (error) {
      // Normalize the error.
      if (typeof error === 'string') {
        error = { message: error };
      }

      if ('details' in error) {
        error = error.details;
      }
    }
    this.submitting = false;
    this.setPristine(false);
    this.emit('submitError', error);

    // Allow for silent cancellations (no error message, no submit button error state)
    if (error && error.silent) {
      this.emit('change', { isValid: true }, { silent: true });
      return false;
    }

    let errors;
    if (this.submitted) {
      errors = this.showErrors();
    }
    else {
      errors = this.showErrors(error, true);
    }
    if (this.root && this.root.alert) {
      this.scrollIntoView(this.root.alert);
    }
    return errors;
  }

  /**
   * Trigger the change event for this form.
   *
   * @param changed
   * @param flags
   */
  onChange(flags, changed) {
    // For any change events, clear any custom errors for that component.
    if (changed && changed.component) {
      this.customErrors = this.customErrors.filter(err => err.component && err.component !== changed.component.key);
    }

    super.onChange(flags, true);
    const value = _.clone(this._submission);
    value.changed = changed;
    value.isValid = this.checkData(value.data, flags);
    this.showElement(true);
    this.loading = false;
    // Updated Total Errors

    if (this.TotalErrorElements !== undefined) {
      const indexToRemove = this.TotalErrorElements.findIndex((error) => {
        if (value.changed !== undefined) {
          return value.changed.component.id === error.component.id;
        }
      });

      if (indexToRemove !== -1) {
        this.TotalErrorElements.splice(indexToRemove, 1);
        value.data.FormErrors = this.TotalErrorElements;
      }
    }

    // See if we need to save the draft of the form.
    if (flags && flags.modified && this.options.saveDraft) {
      this.triggerSaveDraft();
    }

    if (!flags || !flags.noEmit) {
      this.emit('change', value);
    }

    // The form is initialized after the first change event occurs.
    if (!this.initialized) {
      this.emit('initialized');
      this.initialized = true;
    }
    try {
      if (value.data.FormErrors.length > 0 || this.TotalErrorElements.length > 0) {
        if (value.data.FormErrors.length > this.TotalErrorElements.length) {
          value.data.FormErrors = this.TotalErrorElements;
        }
        this.showErrors(this.error);
      }
      else {
        const element = document.getElementById('errorElementList');
        element.style.display = 'none';
      }
    }
    catch (e) {
      //ignore error
    }
  }

  checkData(data, flags) {
    const valid = super.checkData(data, flags);
    if ((_.isEmpty(flags) || flags.noValidate) && this.submitted) {
      this.showErrors();
    }
    return valid;
  }

  /**
   * Send a delete request to the server.
   */
  deleteSubmission() {
    return this.formio.deleteSubmission()
      .then(() => {
        this.emit('submissionDeleted', this.submission);
        this.resetValue();
      });
  }

  /**
   * Cancels the submission.
   *
   * @alias reset
   */
  cancel(noconfirm) {
    if (noconfirm || confirm('Are you sure you want to cancel?')) {
      this.resetValue();
      return true;
    }
    else {
      return false;
    }
  }

  submitForm(options = {}) {
    return new Promise((resolve, reject) => {
      // Read-only forms should never submit.
      if (this.options.readOnly) {
        return resolve({
          submission: this.submission,
          saved: false
        });
      }

      const submission = _.cloneDeep(this.submission || {});

      // Add in metadata about client submitting the form
      submission.metadata = submission.metadata || {};
      _.defaults(submission.metadata, {
        timezone: _.get(this, '_submission.metadata.timezone', currentTimezone()),
        offset: parseInt(_.get(this, '_submission.metadata.offset', moment().utcOffset()), 10),
        referrer: document.referrer,
        browserName: navigator.appName,
        userAgent: navigator.userAgent,
        pathName: window.location.pathname,
        onLine: navigator.onLine,
      });

      submission.state = options.state || 'submitted';
      const isDraft = (submission.state === 'draft');
      this.hook('beforeSubmit', submission, (err) => {
        if (err) {
          return reject(err);
        }

        if (!isDraft && !submission.data) {
          return reject('Invalid Submission');
        }

        if (!isDraft && !this.checkValidity(submission.data, true)) {
          return reject();
        }

        this.getAllComponents().forEach((comp) => {
          const { persistent, key } = comp.component;
          if (persistent === 'client-only') {
            delete submission.data[key];
          }
        });

        this.hook('customValidation', submission, (err) => {
          if (err) {
            // If string is returned, cast to object.
            if (typeof err === 'string') {
              err = {
                message: err
              };
            }

            // Ensure err is an array.
            err = Array.isArray(err) ? err : [err];

            // Set as custom errors.
            this.customErrors = err;

            return reject();
          }

          this.loading = true;

          // Use the form action to submit the form if available.
          let submitFormio = this.formio;
          if (this._form && this._form.action) {
            submitFormio = new Formio(this._form.action, this.formio ? this.formio.options : {});
          }

          if (this.nosubmit || !submitFormio) {
            return resolve({
              submission,
              saved: false,
            });
          }

          // If this is an actionUrl, then make sure to save the action and not the submission.
          const submitMethod = submitFormio.actionUrl ? 'saveAction' : 'saveSubmission';
          submitFormio[submitMethod](submission).then((result) => resolve({
            submission: result,
            saved: true,
          })).catch(reject);
        });
      });
    });
  }

  executeSubmit(options) {
    this.submitted = true;
    this.submitting = true;
    return this.submitForm(options)
      .then(({ submission, saved }) => this.onSubmit(submission, saved))
      .catch((err) => Promise.reject(this.onSubmissionError(err)));
  }
 /**
   * Submits the form.
   *
   * @example
   * import Webform from 'formiojs/Webform';
   * let form = new Webform(document.getElementById('formio'));
   * form.src = 'https://examples.form.io/example';
   * form.submission = {data: {
   *   firstName: 'Joe',
   *   lastName: 'Smith',
   *   email: 'joe@example.com'
   * }};
   * form.submit().then((submission) => {
   *   console.log(submission);
   * });
   *
   * @param {boolean} before - If this submission occured from the before handlers.
   *
   * @returns {Promise} - A promise when the form is done submitting.
   */
  submit(before, options) {
    if (!before) {
      return this.beforeSubmit(options).then(() => this.executeSubmit(options));
    }
    else {
      return this.executeSubmit(options);
    }
  }

  submitUrl(URL,headers) {
    if (!URL) {
      return console.warn('Missing URL argument');
    }

    const submission = this.submission || {};
    const API_URL  = URL;
    const settings = {
      method: 'POST',
      headers: {}
    };

    if (headers && headers.length > 0) {
      headers.map((e) => {
        if (e.header !== '' && e.value !== '') {
          settings.headers[e.header] = e.value;
        }
      });
    }
    if (API_URL && settings) {
      try {
        Formio.makeStaticRequest(API_URL,settings.method,submission,settings.headers).then(() => {
          this.emit('requestDone');
          this.setAlert('success', '<p> Success </p>');
        });
      }
      catch (e) {
        this.showErrors(`${e.statusText} ${e.status}`);
        this.emit('error',`${e.statusText} ${e.status}`);
        console.error(`${e.statusText} ${e.status}`);
      }
    }
    else {
      this.emit('error', 'You should add a URL to this button.');
      this.setAlert('warning', 'You should add a URL to this button.');
      return console.warn('You should add a URL to this button.');
    }
  }
}

Webform.setBaseUrl = Formio.setBaseUrl;
Webform.setApiUrl = Formio.setApiUrl;
Webform.setAppUrl = Formio.setAppUrl;
