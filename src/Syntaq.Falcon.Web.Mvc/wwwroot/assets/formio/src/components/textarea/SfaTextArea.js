/* global ace, Quill*/
/* global ace tinymce*/
import Formio from '../../Formio';
import _ from 'lodash';
import { uniqueName } from '../../utils/utils';
import TextAreaComponent from '../textarea/TextArea';

export default class SfaTextAreaComponent extends TextAreaComponent {
  static schema(...extend) {
    return TextAreaComponent.schema({
      type: 'sfatextarea',
      label: 'Text Area',
      key: 'sfaTextArea',
      rows: 3,
      wysiwyg: false,
      editor: 'text',
      as: 'string',
      logic: []
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Text Area',
      group: 'basic',
      icon: 'fa fa-font',
      documentation: 'http://help.form.io/userguide/#textarea',
      weight: 40,
      schema: SfaTextAreaComponent.schema()
    };
  }

  get defaultSchema() {
    return SfaTextAreaComponent.schema();
  }
  constructor(component, options, data) {
    component.logic = component.logic === undefined ? [] : component.logic;
    super(component, options, data);
    //STQ Modified
    this.editorValue = '';
  }
  get isPlain() {
    return (!this.component.wysiwyg && (this.component.editor === 'text' || !this.component.editor));
  }

  createElement() {
    // If the element is already created, don't recreate.
    if (this.element) {
      return this.element;
    }
    this.element = this.ce('div', {
      id: this.id,
      class: `col-${this.component.widthslider ? this.component.widthslider : 12} offset-${this.component.offsetslider ? this.component.offsetslider : 0}
syntaq-component syntaq-component-sfatextarea ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }

  createInput(container) {
    const _this = this;
    if (this.isPlain) {
      if (this.options.readOnly) {
        this.input = this.ce('div', {
          class: 'well'
        });
        container.appendChild(this.input);
        return this.input;
      }
      else {
        return super.createInput(container);
      }
    }

    if (this.htmlView) {
      this.input = this.ce('div', {
        class: 'well'
      });
      container.appendChild(this.input);
      return this.input;
    }

    // Add the input.
    if (this.component.editor === 'tinymce') {
      this.input = this.ce('div', {
        id: `${this.id}editor`,
        class: 'formio-wysiwyg-editor'
      });
    }
    else {
      this.input = this.ce('div', {
        class: 'formio-wysiwyg-editor'
      });
    }
    container.appendChild(this.input);
    this.addCounter(container);

    if (this.component.editor === 'ckeditor') {
      this.editorReady = this.addCKE(this.input, null, (newValue) => this.updateValue(null, this.getConvertedValueForCK(newValue))).then((editor) => {
        this.editor = editor;
        if (this.options.readOnly || this.component.disabled) {
          this.editor.isReadOnly = true;
        }

        // Set the default rows.
        //let value = '';
        //const numRows = parseInt(this.component.rows, 10);
        //for (let i = 0; i < numRows; i++) {
        //  value += '<p></p>';
        //}
        //editor.data.set(value);

        //STQ Modified
        // Check if the CKE Editor is already initialized and has a value
        var editorHtmlText = _this.data[_this.component.key];
        if (this.editor && editorHtmlText !== '') {
          editor.data.set(editorHtmlText);
        }
        else {
          // Set the default rows.
          let value = '';
          const numRows = parseInt(this.component.rows, 10);
          for (let i = 0; i < numRows; i++) {
            value += '<p></p>';
          }
          editor.data.set(value);
        }
        return editor;
      });
      return this.input;
    }

    if (this.component.editor === 'tinymce') {
      this.editorReady = this.addTinymce(this.input, null, (newValue) => this.updateValue(null, newValue)).then((mce) => {
        return mce;
      });
      return this.input;
    }

    // Normalize the configurations.
    //if (this.component.wysiwyg && this.component.wysiwyg.toolbarGroups) {
    //  console.warn('The WYSIWYG settings are configured for CKEditor. For this renderer, you will need to use configurations for the Quill Editor. See https://quilljs.com/docs/configuration for more information.');
    //  this.component.wysiwyg = this.wysiwygDefault;
    //  this.emit('componentEdit', this);
    //}
    //if (!this.component.wysiwyg || (typeof this.component.wysiwyg === 'boolean')) {
    //  this.component.wysiwyg = this.wysiwygDefault;
    //  this.emit('componentEdit', this);
    //}

    // Add the quill editor.
    if (this.component.editor === 'quill') {
      this.editorReady = this.addQuill(
        this.input,
        this.component.wysiwyg, () => {
          this.updateValue(null, this.getConvertedValueForQuill(this.quill.root.innerHTML));
        }
      ).then((quill) => {
        if (this.component.isUploadEnabled) {
          quill.getModule('toolbar').addHandler('image', imageHandler);
        }
        quill.root.spellcheck = this.component.spellcheck;
        if (this.options.readOnly || this.component.disabled) {
          quill.disable();
        }
        // STQ Modified To Remove Video Embed
        quill.getModule('toolbar').container.childNodes[7].childNodes[2].remove();

        return quill;
      }).catch(err => console.warn(err));
    }

    return this.input;

    function imageHandler() {
      let fileInput = this.container.querySelector('input.ql-image[type=file]');

      if (fileInput == null) {
        fileInput = document.createElement('input');
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('accept', 'image/*');
        fileInput.classList.add('ql-image');
        fileInput.addEventListener('change', () => {
          const files = fileInput.files;
          const range = this.quill.getSelection(true);

          if (!files || !files.length) {
            console.warn('No files selected');
            return;
          }

          this.quill.enable(false);
          const { uploadStorage, uploadUrl, uploadOptions, uploadDir } = _this.component;
          _this.uploadFile(
              uploadStorage,
              files[0],
              uniqueName(files[0].name),
              uploadDir || '', //should pass empty string if undefined
              null,
              uploadUrl,
              uploadOptions
            )
            .then(result => {
              return _this.downloadFile(result);
            })
            .then(result => {
              this.quill.enable(true);
              const Delta = Quill.import('delta');
              this.quill.updateContents(new Delta()
                .retain(range.index)
                .delete(range.length)
                .insert({ image: result.url })
                , Quill.sources.USER);
              fileInput.value = '';
            }).catch(error => {
              console.warn('Quill image upload failed');
              console.warn(error);
              this.quill.enable(true);
            });
        });
        this.container.appendChild(fileInput);
      }
      fileInput.click();
    }
  }
  addTinymce(element, settings, onChange) {
    settings = _.isEmpty(settings) ? null : settings;
    var jsurl = 'https://cdnjs.cloudflare.com/ajax/libs/tinymce/4.9.4/tinymce.min.js';
    //var jsurl = 'https://create.zumeforms.com.au/FormBuilder/tinymce/tinymce.min.js';
    return Formio.requireLibrary('tinymce', 'tinymce', jsurl, true)
      .then(() => {
        if (!element.parentNode) {
          return Promise.reject();
        }
        this.tinymce = new tinymce.Editor(element.id, {
          plugins: [
            'advlist autolink lists link image charmap print preview anchor',
            'searchreplace visualblocks code fullscreen',
            'table contextmenu paste textcolor colorpicker emoticons hr media'
          ],
          //themes: 'modern',
          branding: false,
          toolbar: 'insertfile undo redo | forecolor backcolor | styleselect | fontselect |  fontsizeselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image | code'
        }, tinymce.EditorManager);
        this.tinymce.on('Change', () => {
          onChange(this.tinymce.getContent());
        });
        //this.tinymce.on('SetContent', () => {
        //  this.dataValue = this.tinymce.getContent();
        //});
        this.tinymce.on('init', () => {
          this.tinymce.setContent(this.tempdata !== undefined ? this.tempdata : this.dataValue);
        });
        this.tinymce.render();
        return this.tinymce;
      });
  }

  getConvertedValueForQuill(value) {
    if (this.component.as && this.component.as === 'json' && value) {
      try {
        value = JSON.parse(value);
      }
      catch (err) {
        console.warn(err);
      }
    }
    var temp = value;
    value = '';
    value += '<html>';
    value += temp;
    value += '</html>';
    return value;
  }
  getConvertedValueForCK(value) {
    if (this.component.as && this.component.as === 'json' && value) {
      try {
        value = JSON.parse(value);
      }
      catch (err) {
        console.warn(err);
      }
    }
    var temp = value;
    value = '';
    value += '<html>';
    value += temp;
    value += '</html>';
    return value;
  }
}
