import Promise from 'native-promise-only';
import _ from 'lodash';

import Webform from './Webform';
import Base from './components/base/Base';
import Formio from './Formio';
import { checkCondition, hasCondition, generateShowHideTabel } from './utils/utils';

export default class Wizard extends Webform {
  /**
   * Constructor for wizard based forms
   * @param element Dom element to place this wizard.
   * @param {Object} options Options object, supported options are:
   *    - breadcrumbSettings.clickable: true (default) determines if the breadcrumb bar is clickable or not
   *    - buttonSettings.show*(Previous, Next, Cancel): true (default) determines if the button is shown or not
   */
  constructor(element, options) {
    super(element, options);
    this.wizard = null;
    this.pages = [];
    this.globalComponents = [];
    this.page = 0;
    this.history = [];
    this._nextPage = 0;
    this._seenPages = [0];
    this.buttons = options.buttonSetting;
    this.formStyles = options.formStyleSetting;//
  }

  isLastPage() {
    const next = this.getNextPage(this.submission.data, this.page);

    if (_.isNumber(next)) {
      return 0 < next && next >= this.pages.length;
    }

    return _.isNull(next);
  }

  getPages(args = {}) {
    const { all = false } = args;
    const pageOptions = _.clone(this.options);
    const components = _.clone(this.components);
    const pages = this.pages
      .filter(all ? _.identity : (p, index) => this._seenPages.includes(index))
      .map((page, index) => this.createComponent(
        page,
        _.assign(pageOptions, { components: index === this.page ? components : null })
      ));

    this.components = components;

    return pages;
  }

  getComponents() {
    return this.submitting
      ? this.getPages({ all: this.isLastPage() })
      : super.getComponents();
  }

  resetValue() {
    this.getPages().forEach((page) => page.resetValue());
    this.setPristine(true);
  }

  setPage(num) {
    if (!this.wizard.full && num >= 0 && num < this.pages.length) {
      this.page = num;
      if (!this._seenPages.includes(num)) {
        this._seenPages = this._seenPages.concat(num);
      }
      return super.setForm(this.currentPage());
    }
    else if (this.wizard.full || !this.pages.length) {
      return super.setForm(this.getWizard());
    }
    return Promise.reject('Page not found');
  }

  getNextPage(data, currentPage) {
    const form = this.pages[currentPage];
    // Check conditional nextPage
    if (form) {
      const page = ++currentPage;
      if (form.nextPage) {
        const next = this.evaluate(form.nextPage, {
          next: page,
          data,
          page,
          form
        }, 'next');
        if (next === null) {
          return null;
        }

        const pageNum = parseInt(next, 10);
        if (!isNaN(parseInt(pageNum, 10)) && isFinite(pageNum)) {
          return pageNum;
        }

        return this.getPageIndexByKey(next);
      }

      return page;
    }

    return null;
  }

  getPreviousPage() {
    const prev = this.history.pop();
    if (typeof prev !== 'undefined') {
      return prev;
    }

    return this.page - 1;
  }

  beforeSubmit() {
    this._seenPages = Array(this.pages.length).fill().map((x, i) => i);
    return Promise.all(this.getPages().map((page) => {
      page.options.beforeSubmit = true;
      return page.beforeSubmit();
    }));
  }

  nextPage() {
    // Read-only forms should not worry about validation before going to next page, nor should they submit.
    if (this.options.readOnly) {
      this.history.push(this.page);
      return this.setPage(this.getNextPage(this.submission.data, this.page)).then(() => {
        this._nextPage = this.getNextPage(this.submission.data, this.page);
        this.emit('nextPage', { page: this.page, submission: this.submission });
      });
    }

    // Validate the form builed, before go to the next page
    if (this.checkCurrentPageValidity(this.submission.data, true)) {
      this.checkData(this.submission.data, {
        noValidate: true
      });
      return this.beforeNext().then(() => {
        this.history.push(this.page);
        return this.setPage(this.getNextPage(this.submission.data, this.page)).then(() => {
          this._nextPage = this.getNextPage(this.submission.data, this.page);
          this.emit('nextPage', { page: this.page, submission: this.submission });
        });
      });
    }
    else {
      return Promise.reject(this.showErrors(null, true));
    }
  }

  prevPage() {
    const prevPage = this.getPreviousPage();
    return this.setPage(prevPage).then(() => {
      this.emit('prevPage', { page: this.page, submission: this.submission });
    });
  }

  cancel(noconfirm) {
    if (super.cancel(noconfirm)) {
      this.history = [];
      return this.setPage(0);
    }
    else {
      return this.setPage();
    }
  }

  getPageIndexByKey(key) {
    let pageIndex = 0;
    this.pages.forEach((page, index) => {
      if (page.key === key) {
        pageIndex = index;
        return false;
      }
    });
    return pageIndex;
  }

  addGlobalComponents(page) {
    // If there are non-page components, then add them here. This is helpful to allow for hidden fields that
    // can propogate between pages.
    if (this.globalComponents.length) {
      page.components = this.globalComponents.concat(page.components);
    }
    return page;
  }

  getPage(pageNum) {
    if ((pageNum >= 0) && (pageNum < this.pages.length)) {
      return this.addGlobalComponents(this.pages[pageNum]);
    }
    return null;
  }

  getWizard() {
    let pageIndex = 0;
    let page = null;
    const wizard = _.clone(this.wizard);
    wizard.components = [];
    do {
      page = this.getPage(pageIndex);

      if (page) {
        wizard.components.push(page);
      }

      pageIndex = this.getNextPage(this.submission.data, pageIndex);
    } while (pageIndex);

    // Add all other components.
    this.wizard.components.forEach((component) => {
      if (component.type !== 'panel' && component.type !== 'sfapanel') {
        wizard.components.push(component);
      }
    });

    return wizard;
  }

  currentPage() {
    return this.getPage(this.page);
  }

  buildPages(form) {
    this.pages = [];
    form.components.forEach((component) => {
      if (component.type === 'panel' || component.type === 'sfapanel') {
        // Ensure that this page can be seen.
        if (checkCondition(component, this.data, this.data, this.wizard, this)) {
          this.pages.push(component);
        }
      }
      else if (component.type === 'hidden') {
        // Global components are hidden components that can propagate between pages.
        this.globalComponents.push(component);
      }
    });
    //this.buildWizardHeader();
    (this.formStyles.WizardHeader === 'num-wrapper') ? this.buildWizardHeaderC() : this.buildWizardHeader();
    this.buildWizardNav();
  }

  get schema() {
    return this.wizard;
  }

  setForm(form) {
    if (!form) {
      return;
    }
    this.wizard = form;
    this.buildPages(this.wizard);
    return this.setPage(this.page);
  }

  build() {
    super.build();
    this.formReady.then(() => {
      //this.buildWizardHeader();
      (this.formStyles.WizardHeader === 'num-wrapper') ? this.buildWizardHeaderC() : this.buildWizardHeader();
      this.buildWizardNav();
    });
  }

  hasButton(name, nextPage) {
    // Check for and initlize button settings object
    this.options.buttonSettings = _.defaults(this.options.buttonSettings, {
      showPrevious: true,
      showNext: true,
      showCancel: !this.options.readOnly
    });

    if (name === 'previous') {
      return (this.page > 0) && this.options.buttonSettings.showPrevious;
    }
    nextPage = (nextPage === undefined) ? this.getNextPage(this.submission.data, this.page) : nextPage;
    if (name === 'next') {
      return (nextPage !== null) && (nextPage < this.pages.length) && this.options.buttonSettings.showNext;
    }
    if (name === 'cancel') {
      return this.options.buttonSettings.showCancel;
    }
    if (name === 'submit') {
      return !this.options.readOnly && ((nextPage === null) || (this.page === (this.pages.length - 1)));
    }
    if (name === 'save') {
      return true;
    }
    if (name === 'draft') {
      return true;
    }
    return true;
  }

  buildWizardHeader() {
    if (this.wizardHeader) {
      this.wizardHeader.innerHTML = '';
    }

    const currentPage = this.currentPage();
    if (!currentPage || this.wizard.full) {
      return;
    }
    currentPage.breadcrumb = currentPage.breadcrumb || 'default';
    if (currentPage.breadcrumb.toLowerCase() === 'none') {
      return;
    }

    // Check for and initlize breadcrumb settings object
    //this.options.breadcrumbSettings = _.defaults(this.options.breadcrumbSettings, {
    //  clickable: true
    //});

    this.wizardHeader = this.ce('nav', {
      'aria-label': 'navigation'
    });

    this.wizardHeaderList = this.ce('ul', {
      class: 'pagination',
      style: 'display: inline-block;'
    });

    this.wizardHeader.appendChild(this.wizardHeaderList);

    // Add the header to the beginning.
    this.prepend(this.wizardHeader);

    const showHistory = (currentPage.breadcrumb.toLowerCase() === 'history');
    this.pages.forEach((page, i) => {
      // See if this page is in our history.
      if (showHistory && ((this.page !== i) && !this.history.includes(i))) {
        return;
      }
      // Set clickable based on breadcrumb settings
      // const clickable = this.page !== i && this.options.breadcrumbSettings.clickable;
      const breadcrumbClickable = this.pages[i].breadcrumbClickable === undefined ? true : this.pages[i].breadcrumbClickable;
      const clickable = this.page !== i && breadcrumbClickable;
      let pageClass = 'page-item ';
      pageClass += (i === this.page) ? 'active' : (clickable ? '' : 'disabled');

      const pageButton = this.ce('li', {
        class: pageClass,
        style: (clickable) ? 'display: inline;position: relative;float: left;margin: 1px 2px;cursor: pointer;' : 'display: inline;position: relative;float: left;margin: 1px 2px;'
      });

      // Navigate to the page as they click on it.

      if (clickable) {
        this.addEventListener(pageButton, 'click', (event) => {
          this.emit('wizardNavigationClicked', this.pages[i]);
          event.preventDefault();
          this.setPage(i);
        });
      }

      const pageLabel = this.ce('span', {
        class: 'page-link'
      });
      let pageTitle = page.title;
      if (currentPage.breadcrumb.toLowerCase() === 'condensed') {
        pageTitle = ((i === this.page) || showHistory) ? page.title : (i + 1);
        if (!pageTitle) {
          pageTitle = (i + 1);
        }
      }

      pageLabel.appendChild(this.text(pageTitle));
      pageButton.appendChild(pageLabel);
      this.wizardHeaderList.appendChild(pageButton);
    });
  }

  buildWizardHeaderC() {
    if (this.wizardHeader) {
      this.wizardHeader.innerHTML = '';
    }

    const currentPage = this.currentPage();
    if (!currentPage || this.wizard.full) {
      return;
    }
    currentPage.breadcrumb = currentPage.breadcrumb || 'default';
    if (currentPage.breadcrumb.toLowerCase() === 'none') {
      return;
    }

    // Check for and initlize breadcrumb settings object
    //this.options.breadcrumbSettings = _.defaults(this.options.breadcrumbSettings, {
    //  clickable: true
    //});

    this.wizardHeader = this.ce('nav', {
      'aria-label': 'navigation'
    });

    this.wizardHeaderList = this.ce('ul', {
      class: 'pagination col-12'
      //style: 'display: inline-block;'
    });

    this.wizardHeader.appendChild(this.wizardHeaderList);

    // Add the header to the beginning.
    this.prepend(this.wizardHeader);

    const showHistory = (currentPage.breadcrumb.toLowerCase() === 'history');
    this.pages.forEach((page, i) => {
      // See if this page is in our history.
      if (showHistory && ((this.page !== i) && !this.history.includes(i))) {
        return;
      }
      // Set clickable based on breadcrumb settings
      // const clickable = this.page !== i && this.options.breadcrumbSettings.clickable;
      const breadcrumbClickable = this.pages[i].breadcrumbClickable === undefined ? true : this.pages[i].breadcrumbClickable;
      const clickable = this.page !== i && breadcrumbClickable;
      let pageClass = 'page-item ';
      pageClass += (i === this.page) ? 'active' : (clickable ? '' : 'disabled');

      const pageButton = this.ce('li', {
        class: `${pageClass} col`,
        style: (clickable) ? 'cursor: pointer;' : ''
        //style: (clickable) ? 'display: inline;position: relative;float: left;margin: 1px 2px;cursor: pointer;' : 'display: inline;position: relative;float: left;margin: 1px 2px;'
      });
      ((0 !== i) && (this.pages.length - 1 !== i)) ? pageButton.style.textAlign = '-webkit-center' : '';
      // Navigate to the page as they click on it.

      if (clickable) {
        this.addEventListener(pageButton, 'click', (event) => {
          this.emit('wizardNavigationClicked', this.pages[i]);
          event.preventDefault();
          this.setPage(i);
        });
      }
      //------------
      const pageDiv = this.ce('div', {
        style: 'display: table-cell;text-align: center;position: relative;',
        class: (this.pages.length - 1 === i) ? 'float-right' : ''
      });
      const pageLabel = this.ce('span', {
        class: 'page-link',
        style: 'width: 40px;height: 40px;text-align: center;padding: 6px 0;font-size: 20px;line-height: 1.428571429;border-radius: 50%;display: inline-block;'
      });
      const pageNum = i + 1;
      pageLabel.appendChild(this.text(pageNum));
      const pagePtag = this.ce('p', {
        style: 'font-family: Helvetica;font-size: 18px;font-weight: 300;color: rgb(63, 82, 93);margin: 5px 0px;'
      });
      pagePtag.appendChild(this.text(page.title));
      pageDiv.appendChild(pageLabel);
      pageDiv.appendChild(pagePtag);

      pageButton.appendChild(pageDiv);
      this.wizardHeaderList.appendChild(pageButton);
    });
  }
  pageId(page) {
    if (page.key) {
      // Some panels have the same key....
      return `${page.key}-${page.title}`;
    }
    else if (
      page.components &&
      page.components.length > 0
    ) {
      return this.pageId(page.components[0]);
    }
    else {
      return page.title;
    }
  }

  onChange(flags, changed) {
    var $focusedname = document.activeElement.name;
    var $openedPanels = document.querySelectorAll("div.panel-body[style*='visibility: visible']");

    super.onChange(flags, changed);

    // Only rebuild if there is a page change.
    let pageIndex = 0;
    let rebuild = false;
    this.wizard.components.forEach((component) => {
      if (component.type !== 'panel' && component.type !== 'sfapanel') {
        return;
      }

      if (hasCondition(component)) {
        const hasPage = this.pages && this.pages[pageIndex]
          && (this.pageId(this.pages[pageIndex]) === this.pageId(component));
        const shouldShow = checkCondition(component, this.data, this.data, this.wizard, this);
        if ((shouldShow && !hasPage) || (!shouldShow && hasPage)) {
          rebuild = true;
          return false;
        }
        if (shouldShow) {
          pageIndex++;
        }
      }
      else {
        pageIndex++;
      }
    });

    if (rebuild) {
      this.setForm(this.wizard);
    }
    $openedPanels.forEach(function(panel) {
      panel.style.visibility = 'visible';
      panel.removeAttribute('hidden');
    });
    var element = document.getElementsByName($focusedname)[0];
    if (element !== undefined) {
      element.focus();
    }

    // Update Wizard Nav
    const nextPage = this.getNextPage(this.submission.data, this.page);
    if (this._nextPage !== nextPage) {
      this.buildWizardNav(nextPage);
      this.emit('updateWizardNav', { oldpage: this._nextPage, newpage: nextPage, submission: this.submission });
      this._nextPage = nextPage;
    }
    generateShowHideTabel(this.components, {}, this.root);
  }

  buildWizardNav(nextPage) {
    const buttons = this.buttons ? this.buttons : {};
    if (this.wizardNav) {
      this.wizardNav.innerHTML = '';
      this.removeChild(this.wizardNav);
    }
    if (this.wizard.full) {
      return;
    }
    this.wizardNav = this.ce('ul', {
      class: 'list-inline',
      style: 'clear:both;'
    });
    this.element.appendChild(this.wizardNav);
    [
      //{ tag: 'draft', name: buttons.draft_label ? buttons.draft_label : 'Draft', method: 'draft', class: 'btn btn-default btn-secondary', disable: buttons.draft_disable, hiden: buttons.draft_hide },
      { tag: 'cancel', name: buttons.clear_label ? buttons.clear_label : 'Clear', method: 'cancel', class: 'btn btn-default btn-secondary', disable: buttons.clear_disable, hiden: buttons.clear_hide },
      { tag: 'save', name: buttons.save_label ? buttons.save_label : 'Save', method: 'save', class: 'btn btn-primary', disable: buttons.save_disable, hiden: buttons.save_hide },
      { tag: 'previous', name: buttons.previous_label ? buttons.previous_label : 'Previous', method: 'prevPage', class: 'btn btn-primary', disable: buttons.previous_disable, hiden: buttons.previous_hide },
      { tag: 'next', name: buttons.next_label ? buttons.next_label : 'Next', method: 'nextPage', class: 'btn btn-primary', disable: buttons.next_disable, hiden: buttons.next_hide },
      { tag: 'submit', name: buttons.submit_label ? buttons.submit_label : 'Submit', method: 'submit', class: 'btn btn-primary', disable: buttons.submit_disable, hiden: buttons.submit_hide }
    ].forEach((button) => {
      if (!this.hasButton(button.tag, nextPage)) {
        return;
      }
      const buttonWrapper = this.ce('li', {
        class: 'list-inline-item'
      });
      const buttonProp = `${button.tag}Button`;
      const buttonElement = this[buttonProp] = this.ce('button', {
        class: `${button.class} btn-wizard-nav-${button.tag} btn-${button.tag}-${this.page}`
      });
      buttonElement.appendChild(this.text(button.name));
      button.disable !== 'true' ?
        '' : buttonElement.setAttribute('disabled', 'disabled');
      button.hiden !== 'true' ?
        '' : buttonElement.setAttribute('hidden', true);
      this.addEventListener(this[buttonProp], 'click', (event) => {
        event.preventDefault();
        if (button.method === 'save') {
          this.emit('save', this.submission.data);
        }
        else if (button.method === 'draft') {
          this.emit('draft', this.submission);
        }
        else {
          // Disable the button until done.
          buttonElement.setAttribute('disabled', 'disabled');
          this.setLoading(buttonElement, true);

          // Call the button method, then re-enable the button.
          this[button.method]().then(() => {
            buttonElement.removeAttribute('disabled');
            this.setLoading(buttonElement, false);
          }).catch(() => {
            buttonElement.removeAttribute('disabled');
            this.setLoading(buttonElement, false);
          });
        }
      });
      buttonWrapper.appendChild(this[buttonProp]);
      this.wizardNav.appendChild(buttonWrapper);
    });
  }

  checkCurrentPageValidity(...args) {
    return super.checkValidity(...args);
  }

  checkPagesValidity(pages, ...args) {
    const isValid = Base.prototype.checkValidity.apply(this, args);
    return pages.reduce((check, pageComp) => {
      return pageComp.checkValidity(...args) && check;
    }, isValid);
  }

  checkValidity(data, dirty) {
    return this.checkPagesValidity(this.getPages(), data, dirty);
  }

  get errors() {
    if (this.isLastPage()) {
      const pages = this.getPages({ all: true });

      this.checkPagesValidity(pages, this.submission.data, true);

      return pages.reduce((errors, pageComp) => {
        return errors.concat(pageComp.errors || []);
      }, []);
    }

    return super.errors;
  }
}

Wizard.setBaseUrl = Formio.setBaseUrl;
Wizard.setApiUrl = Formio.setApiUrl;
Wizard.setAppUrl = Formio.setAppUrl;
