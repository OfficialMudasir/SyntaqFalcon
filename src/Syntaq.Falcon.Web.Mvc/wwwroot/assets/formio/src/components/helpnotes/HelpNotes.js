import BaseComponent from '../base/Base';

export default class HelpNotesComponent extends BaseComponent {
  static schema(...extend) {
    return BaseComponent.schema({
      type: 'helpnotes',
      key: 'HelpNotes',
      protected: false,
      persistent: true,
      label: 'Notes/Help editor',
      hideLabel: true,
      disabled: true,
      tableView: false,
      input: false,
      htmlcontent:'',
      popuphelp:false,
      hidecontent:false,
      title:''
    }, ...extend);
  }

  static get builderInfo() {
    return {
      title: 'Notes/Help editor',
      icon: 'far fa-sticky-note',
      group: 'testGroup',
      documentation: 'https://help.form.io/userguide/form-components/#custom',
      weight: 120,
      schema: HelpNotesComponent.schema()
    };
  }
  get emptyValue() {
    return '';
  }
  get defaultSchema() {
    return HelpNotesComponent.schema();
  }
  elementInfo() {
    const info = super.elementInfo();
    return info;
  }
  createInput(container) {
    const markID = `${this.component.id}-mark`;
    const title = this.component.title;
    const htmlcontent = this.component.htmlcontent;
    //const htmlcontent = this.interpolate(this.component.htmlcontent).replace(/(?:\r\n|\r|\n)/g, '<br />');
    const noteshelpGroup = this.ce('div', {
      class: 'form-group'
    });
    const titleGroup = this.ce('div', {
      style:'cursor: pointer;'
    });
    const titleSpan = this.ce('span',{
      class:'fas fa-question-circle',
      style:'font-size:1.1em; display:inline;'
    });
    const titleLabel = this.ce('label',{
      style:'display:inline;padding-left: 5px;font-weight: bold;'
    });
    titleLabel.innerHTML = title;
    titleGroup.appendChild(titleSpan);
    titleGroup.appendChild(titleLabel);
    let contentDiv;
    if (this.component.hidecontent) {
      contentDiv = this.ce('div',{ style:'display: none;' });
    }
    else {
      contentDiv = this.ce('div');
    }
    contentDiv.innerHTML = htmlcontent;
    noteshelpGroup.appendChild(titleGroup);
    noteshelpGroup.appendChild(contentDiv);
    if (this.component.popuphelp) {
      if (document.getElementById(markID)) {
        document.getElementById(markID).outerHTML = '';
      }
      //button for close
      const bT = this.ce('a', {
        class:'btn btn-primary'
      });
      bT.innerHTML = 'Close';
      bT.onclick = function() {
        document.getElementById(markID).outerHTML = '';
      };
      titleGroup.onclick = this.onclickPopup(bT,markID,title,htmlcontent);//
    }

    container.appendChild(noteshelpGroup);
    this.errorContainer = container;
  }
  /* eslint-disable max-statements */
  onclickPopup(bT, markID, title, htmlcontent) {
    return function() {
      var bgDiv = document.createElement('div');
      bgDiv.className = 'modal fade';
      bgDiv.style.opacity='1';
      bgDiv.style.display='block';
      bgDiv.id = markID;
      var dialogDiv = document.createElement('div');
      dialogDiv.className = 'modal-dialog';
      dialogDiv.style.top = '10%';
      var contentDiv = document.createElement('div');
      contentDiv.className = 'modal-content';
      contentDiv.style.marginTop = '25vh';
      contentDiv.style.resize = 'both';
      contentDiv.style.overflow = 'auto';
      // Header
      var headerDiv = document.createElement('div');
      headerDiv.className = 'modal-header';
      var headerH = document.createElement('h4');
      headerH.className = 'modal-title';
      //var icont = document.createElement('i');
      //icont.className = 'fas fa-question-circle';
      var headerSpan = document.createElement('span');
      headerSpan.innerHTML = title;
      headerSpan.style.marginLeft = '2vh';
      headerSpan.style.fontWeight = 'bold';
      //headerH.appendChild(icont);
      headerH.appendChild(headerSpan);
      headerDiv.appendChild(headerH);
      contentDiv.appendChild(headerDiv);
      //Body
      var bodyDiv = document.createElement('div');
      bodyDiv.className = 'modal-body';
      bodyDiv.style.wordBreak = 'break-word';
      bodyDiv.style.textAlign = 'justify';
      bodyDiv.style.visibility = 'visible';
      bodyDiv.style.position = 'relative';
      bodyDiv.style.overflowY = 'scroll';
      bodyDiv.style.height = '35vh';
      bodyDiv.style.padding = '1vh 2vh';
      bodyDiv.innerHTML = htmlcontent;
      contentDiv.appendChild(bodyDiv);
      // Close
      var footerDiv = document.createElement('div');
      footerDiv.className = 'modal-footer';
      footerDiv.appendChild(bT);
      contentDiv.appendChild(footerDiv);
      dialogDiv.appendChild(contentDiv);
      bgDiv.appendChild(dialogDiv);
      document.body.appendChild(bgDiv);
    };
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
syntaq-component syntaq-component-helpnotes ${this.className}`,
      style: ` ${this.customStyle}`
    });
    // Ensure you can get the component info from the element.
    this.element.component = this;

    this.hook('element', this.element);

    return this.element;
  }
}
