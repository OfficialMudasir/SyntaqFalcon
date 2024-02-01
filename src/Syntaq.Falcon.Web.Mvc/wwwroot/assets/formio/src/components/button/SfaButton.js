import _ from 'lodash';
import ButtonComponent from '../button/Button';
import { flattenComponents } from '../../utils/utils';

export default class SfaButtonComponent extends ButtonComponent {
  static schema(...extend) {
    return ButtonComponent.schema({
      type: 'sfabutton',
      label: 'Button',
      key: 'sfaButton',
      action: 'submit',
      theme: 'default',
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Button',
      group: 'things',
      icon: 'far fa-hand-pointer',
      documentation: 'http://help.form.io/userguide/#button',
      weight: 110,
      schema: SfaButtonComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaButtonComponent.schema();
  }
  /* eslint-disable max-statements */
  build() {
    if (this.viewOnly || this.options.hideButtons) {
      this.component.hidden = true;
    }

    this.dataValue = false;
    this.hasError = false;
    this.createElement();
    this.createInput(this.element);
    this.addShortcut(this.buttonElement);
    if (this.component.leftIcon) {
      this.buttonElement.appendChild(this.ce('span', {
        class: this.component.leftIcon
      }));
      this.buttonElement.appendChild(this.text(' '));
    }

    if (!this.labelIsHidden()) {
      this.labelElement = this.text(this.addShortcutToLabel());
      this.buttonElement.appendChild(this.labelElement);
      this.createTooltip(this.buttonElement, null, this.iconClass('question-sign'));
    }
    if (this.component.rightIcon) {
      this.buttonElement.appendChild(this.text(' '));
      this.buttonElement.appendChild(this.ce('span', {
        class: this.component.rightIcon
      }));
    }

    let onChange = null;
    let onError = null;
    if (this.component.action === 'submit' && !this.options.builder) {
      const message = this.ce('div');
      this.on('submitButton', () => {
        this.loading = true;
        this.disabled = true;
      }, true);
      this.on('submitDone', () => {
        this.loading = false;
        this.disabled = false;
        this.empty(message);
        this.addClass(this.buttonElement, 'btn-success submit-success');
        this.removeClass(this.buttonElement, 'btn-danger submit-fail');
        this.addClass(message, 'has-success');
        this.removeClass(message, 'has-error');
        this.append(message);
      }, true);
      onChange = (value, isValid) => {
        this.removeClass(this.buttonElement, 'btn-success submit-success');
        this.removeClass(this.buttonElement, 'btn-danger submit-fail');
        if (isValid && this.hasError) {
          this.hasError = false;
          this.empty(message);
          this.removeChild(message);
          this.removeClass(message, 'has-success');
          this.removeClass(message, 'has-error');
        }
      };
      onError = () => {
        this.hasError = true;
        this.removeClass(this.buttonElement, 'btn-success submit-success');
        this.addClass(this.buttonElement, 'btn-danger submit-fail');
        this.empty(message);
        this.removeClass(message, 'has-success');
        this.addClass(message, 'has-error');
        this.append(message);
      };
    }

    if (this.component.action === 'url') {
      this.on('requestButton', () => {
        this.loading = true;
        this.disabled = true;
      }, true);
      this.on('requestDone', () => {
        this.loading = false;
        this.disabled = false;
      }, true);
    }

    this.on('change', (value) => {
      this.loading = false;
      this.disabled = this.options.readOnly || (this.component.disableOnInvalid && !value.isValid);
      if (onChange) {
        onChange(value, value.isValid);
      }
    }, true);

    this.on('error', () => {
      this.loading = false;
      if (onError) {
        onError();
      }
    }, true);

    this.addEventListener(this.buttonElement, 'click', (event) => {
      this.triggerReCaptcha();
      this.dataValue = true;
      if (this.component.action !== 'submit' && this.component.showValidations) {
        this.emit('checkValidity', this.data);
      }
      switch (this.component.action) {
        case 'saveState':
        case 'submit':
          if (!this.options.builder) {
            event.preventDefault();
            event.stopPropagation();
            this.emit('submitButton', {
              state: this.component.state || 'submitted'
            });
          }
          break;
        case 'event':
          this.emit(this.interpolate(this.component.event), this.data);
          this.events.emit(this.interpolate(this.component.event), this.data);
          this.emit('customEvent', {
            type: this.interpolate(this.component.event),
            component: this.component,
            data: this.data,
            event: event
          });
          break;
        case 'custom': {
          // Get the FormioForm at the root of this component's tree
          const form = this.getRoot();
          // Get the form's flattened schema components
          const flattened = flattenComponents(form.component.components, true);
          // Create object containing the corresponding HTML element components
          const components = {};
          _.each(flattened, (component, key) => {
            const element = form.getComponent(key);
            if (element) {
              components[key] = element;
            }
          });

          this.evaluate(this.component.custom, {
            form,
            flattened,
            components
          });
          break;
        }
        case 'url':
          this.emit('requestButton');
          this.emit('requestUrl', {
            url: this.interpolate(this.component.url),
            headers: this.component.headers
          });
          break;
        case 'reset':
          this.emit('resetForm');
          break;
        case 'delete':
          this.emit('deleteSubmission');
          break;
        case 'oauth':
          break;
      }
    });

    if (this.shouldDisable) {
      this.disabled = true;
    }

    function getUrlParameter(name) {
      name = name.replace(/[[]/, '\\[').replace(/[\]]/, '\\]');
      const regex = new RegExp(`[\\?&]${name}=([^&#]*)`);
      const results = regex.exec(location.search);
      if (!results) {
        return results;
      }
      return decodeURIComponent(results[1].replace(/\+/g, ' '));
    }

    // If this is an OpenID Provider initiated login, perform the click event immediately
    if ((this.component.action === 'oauth') && this.component.oauth && this.component.oauth.authURI) {
      const iss = getUrlParameter('iss');
      if (iss && (this.component.oauth.authURI.indexOf(iss) === 0)) {
        this.openOauth();
      }
    }

    this.autofocus();
    this.attachLogic();
  }
  /* eslint-disable max-statements */
  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfabutton ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }

  oAuthFunction() {
    if (this.root === this) {
      console.warn('You must add the OAuth button to a form for it to function properly');
      return;
    }

    // Display Alert if OAuth config is missing
    if (!this.component.oauth) {
      this.root.setAlert('danger', 'You must assign this button to an OAuth action before it will work.');
    }

    // Display Alert if oAuth has an error is missing
    if (this.component.oauth.error) {
      this.root.setAlert('danger', `The Following Error Has Occured${this.component.oauth.error}`);
    }

    this.openOauth(this.component.oauth);
  }
}
