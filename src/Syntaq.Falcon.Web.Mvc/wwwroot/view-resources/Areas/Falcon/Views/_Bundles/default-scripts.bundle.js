"use strict";

// Class definition
var KTBlockUI = function(element, options) {
    //////////////////////////////
    // ** Private variables  ** //
    //////////////////////////////
    var the = this;

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default options
    var defaultOptions = {
        zIndex: false,
        overlayClass: '',
        overflow: 'hidden',
        message: '<span class="spinner-border text-primary"></span>'
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('blockui') ) {
            the = KTUtil.data(element).get('blockui');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.element = element;
        the.overlayElement = null;
        the.blocked = false;
        the.positionChanged = false;
        the.overflowChanged = false;

        // Bind Instance
        KTUtil.data(the.element).set('blockui', the);
    }

    var _block = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.blockui.block', the) === false ) {
            return;
        }

        var isPage = (the.element.tagName === 'BODY');
       
        var position = KTUtil.css(the.element, 'position');
        var overflow = KTUtil.css(the.element, 'overflow');
        var zIndex = isPage ? 10000 : 1;

        if (the.options.zIndex > 0) {
            zIndex = the.options.zIndex;
        } else {
            if (KTUtil.css(the.element, 'z-index') != 'auto') {
                zIndex = KTUtil.css(the.element, 'z-index');
            }
        }

        the.element.classList.add('blockui');

        if (position === "absolute" || position === "relative" || position === "fixed") {
            KTUtil.css(the.element, 'position', 'relative');
            the.positionChanged = true;
        }

        if (the.options.overflow === 'hidden' && overflow === 'visible') {           
            KTUtil.css(the.element, 'overflow', 'hidden');
            the.overflowChanged = true;
        }

        the.overlayElement = document.createElement('DIV');    
        the.overlayElement.setAttribute('class', 'blockui-overlay ' + the.options.overlayClass);
        
        the.overlayElement.innerHTML = the.options.message;

        KTUtil.css(the.overlayElement, 'z-index', zIndex);

        the.element.append(the.overlayElement);
        the.blocked = true;

        KTEventHandler.trigger(the.element, 'kt.blockui.after.blocked', the) === false
    }

    var _release = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.blockui.release', the) === false ) {
            return;
        }

        the.element.classList.add('blockui');
        
        if (the.positionChanged) {
            KTUtil.css(the.element, 'position', '');
        }

        if (the.overflowChanged) {
            KTUtil.css(the.element, 'overflow', '');
        }

        if (the.overlayElement) {
            KTUtil.remove(the.overlayElement);
        }        

        the.blocked = false;

        KTEventHandler.trigger(the.element, 'kt.blockui.released', the);
    }

    var _isBlocked = function() {
        return the.blocked;
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('blockui');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.block = function() {
        _block();
    }

    the.release = function() {
        _release();
    }

    the.isBlocked = function() {
        return _isBlocked();
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTBlockUI.getInstance = function(element) {
    if (element !== null && KTUtil.data(element).has('blockui')) {
        return KTUtil.data(element).get('blockui');
    } else {
        return null;
    }
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTBlockUI;
}
"use strict";
// DOCS: https://javascript.info/cookie

// Class definition
var KTCookie = function() {
    return {
        // returns the cookie with the given name,
        // or undefined if not found
        get: function(name) {
            var matches = document.cookie.match(new RegExp(
                "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
            ));

            return matches ? decodeURIComponent(matches[1]) : null;
        },

        // Please note that a cookie value is encoded,
        // so getCookie uses a built-in decodeURIComponent function to decode it.
        set: function(name, value, options) {
            if ( typeof options === "undefined" || options === null ) {
                options = {};
            }

            options = Object.assign({}, {
                path: '/'
            }, options);

            if ( options.expires instanceof Date ) {
                options.expires = options.expires.toUTCString();
            }

            var updatedCookie = encodeURIComponent(name) + "=" + encodeURIComponent(value);

            for ( var optionKey in options ) {
                if ( options.hasOwnProperty(optionKey) === false ) {
                    continue;
                }

                updatedCookie += "; " + optionKey;
                var optionValue = options[optionKey];

                if ( optionValue !== true ) {
                    updatedCookie += "=" + optionValue;
                }
            }

            document.cookie = updatedCookie;
        },

        // To remove a cookie, we can call it with a negative expiration date:
        remove: function(name) {
            this.set(name, "", {
                'max-age': -1
            });
        }
    }
}();

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTCookie;
}

"use strict";

// Class definition
var KTDialer = function(element, options) {
    ////////////////////////////
    // ** Private variables  ** //
    ////////////////////////////
    var the = this;

    if (!element) {
        return;
    }

    // Default options
    var defaultOptions = {
        min: null,
        max: null,
        step: 1,
        decimals: 0,
        prefix: "",
        suffix: ""
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    // Constructor
    var _construct = function() {
        if ( KTUtil.data(element).has('dialer') === true ) {
            the = KTUtil.data(element).get('dialer');
        } else {
            _init();
        }
    }

    // Initialize
    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);

        // Elements
        the.element = element;
        the.incElement = the.element.querySelector('[data-kt-dialer-control="increase"]');
        the.decElement = the.element.querySelector('[data-kt-dialer-control="decrease"]');
        the.inputElement = the.element.querySelector('input[type]'); 
        
        // Set Values
        if (_getOption('decimals')) {
            the.options.decimals = parseInt(_getOption('decimals'));
        }
        
        if (_getOption('prefix')) {
            the.options.prefix = _getOption('prefix');
        }
        
        if (_getOption('suffix')) {
            the.options.suffix = _getOption('suffix');
        }
        
        if (_getOption('step')) {
            the.options.step = parseFloat(_getOption('step'));
        }

        if (_getOption('min')) {
            the.options.min = parseFloat(_getOption('min'));
        }

        if (_getOption('max')) {
            the.options.max = parseFloat(_getOption('max'));
        }

        the.value = parseFloat(the.inputElement.value.replace(/[^\d.]/g, ''));  

        _setValue();

        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('dialer', the);
    }

    // Handlers
    var _handlers = function() {
        KTUtil.addEvent(the.incElement, 'click', function(e) {
            e.preventDefault();
        
            _increase();
        });

        KTUtil.addEvent(the.decElement, 'click', function(e) {
            e.preventDefault();

            _decrease();
        });

        KTUtil.addEvent(the.inputElement, 'change', function(e) {
            e.preventDefault();

            _setValue();
        });
    }

    // Event handlers
    var _increase = function() {
        // Trigger "after.dialer" event
        KTEventHandler.trigger(the.element, 'kt.dialer.increase', the);

        the.inputElement.value = the.value + the.options.step;
        _setValue();

        // Trigger "before.dialer" event
        KTEventHandler.trigger(the.element, 'kt.dialer.increased', the);

        return the;
    }

    var _decrease = function() {
        // Trigger "after.dialer" event
        KTEventHandler.trigger(the.element, 'kt.dialer.decrease', the);

        the.inputElement.value = the.value - the.options.step;        
        _setValue();

        // Trigger "before.dialer" event
        KTEventHandler.trigger(the.element, 'kt.dialer.decreased', the);

        return the;
    }

    // Set Input Value
    var _setValue = function() {
        // Trigger "after.dialer" event
        KTEventHandler.trigger(the.element, 'kt.dialer.change', the);

        the.value = parseFloat(the.inputElement.value.replace(/[^\d.]/g, '')); 
        
        if (the.value < the.options.min) {
            the.value = the.options.min;
        }

        if (the.value > the.options.max) {
            the.value = the.options.max;
        }

        the.inputElement.value = _format(the.value);

        // Trigger "after.dialer" event
        KTEventHandler.trigger(the.element, 'kt.dialer.changed', the);
    }

    // Format
    var _format = function(val){
        return the.options.prefix + parseFloat(val).toFixed(the.options.decimals) + the.options.suffix;              
    }

    // Get option
    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-dialer-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-dialer-' + name);
            var value = attr;            

            return value;
        } else {
            return null;
        }
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('dialer');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.increase = function() {
        return _increase();
    }

    the.decrease = function() {
        return _decrease();
    }

    the.getElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTDialer.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('dialer') ) {
        return KTUtil.data(element).get('dialer');
    } else {
        return null;
    }
}

// Create instances
KTDialer.createInstances = function(selector = '[data-kt-dialer="true"]') {
    // Get instances
    var elements = document.body.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            // Initialize instances
            new KTDialer(elements[i]);
        }
    }
}

// Global initialization
KTDialer.init = function() {
    KTDialer.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTDialer.init);
} else {
    KTDialer.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTDialer;
}
"use strict";

// Class definition
var KTDrawer = function(element, options) {
    //////////////////////////////
    // ** Private variables  ** //
    //////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default options
    var defaultOptions = {
        overlay: true,
        direction: 'end',
        baseClass: 'drawer',
        overlayClass: 'drawer-overlay'
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('drawer') ) {
            the = KTUtil.data(element).get('drawer');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('drawer');
        the.element = element;
        the.overlayElement = null;
        the.name = the.element.getAttribute('data-kt-drawer-name');
        the.shown = false;
        the.lastWidth;
        the.toggleElement = null;

        // Set initialized
        the.element.setAttribute('data-kt-drawer', 'true');

        // Event Handlers
        _handlers();

        // Update Instance
        _update();

        // Bind Instance
        KTUtil.data(the.element).set('drawer', the);
    }

    var _handlers = function() {
        var togglers = _getOption('toggle');
        var closers = _getOption('close');

        if ( togglers !== null && togglers.length > 0 ) {
            KTUtil.on(body, togglers, 'click', function(e) {
                e.preventDefault();

                the.toggleElement = this;
                _toggle();
            });
        }

        if ( closers !== null && closers.length > 0 ) {
            KTUtil.on(body, closers, 'click', function(e) {
                e.preventDefault();

                the.closeElement = this;
                _hide();
            });
        }
    }

    var _toggle = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.drawer.toggle', the) === false ) {
            return;
        }

        if ( the.shown === true ) {
            _hide();
        } else {
            _show();
        }

        KTEventHandler.trigger(the.element, 'kt.drawer.toggled', the);
    }

    var _hide = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.drawer.hide', the) === false ) {
            return;
        }

        the.shown = false;

        _deleteOverlay();

        body.removeAttribute('data-kt-drawer-' + the.name, 'on');
        body.removeAttribute('data-kt-drawer');

        KTUtil.removeClass(the.element, the.options.baseClass + '-on');

        if ( the.toggleElement !== null ) {
            KTUtil.removeClass(the.toggleElement, 'active');
        }

        KTEventHandler.trigger(the.element, 'kt.drawer.after.hidden', the) === false
    }

    var _show = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.drawer.show', the) === false ) {
            return;
        }

        the.shown = true;

        _createOverlay();
        body.setAttribute('data-kt-drawer-' + the.name, 'on');
        body.setAttribute('data-kt-drawer', 'on');

        KTUtil.addClass(the.element, the.options.baseClass + '-on');

        if ( the.toggleElement !== null ) {
            KTUtil.addClass(the.toggleElement, 'active');
        }

        KTEventHandler.trigger(the.element, 'kt.drawer.shown', the);
    }

    var _update = function() {
        var width = _getWidth();
        var direction = _getOption('direction');

        // Reset state
        if ( KTUtil.hasClass(the.element, the.options.baseClass + '-on') === true && String(body.getAttribute('data-kt-drawer-' + the.name + '-')) === 'on' ) {
            the.shown = true;
        } else {
            the.shown = false;
        }       

        // Activate/deactivate
        if ( _getOption('activate') === true ) {
            KTUtil.addClass(the.element, the.options.baseClass);
            KTUtil.addClass(the.element, the.options.baseClass + '-' + direction);
            KTUtil.css(the.element, 'width', width, true);

            the.lastWidth = width;
        } else {
            KTUtil.css(the.element, 'width', '');

            KTUtil.removeClass(the.element, the.options.baseClass);
            KTUtil.removeClass(the.element, the.options.baseClass + '-' + direction);

            _hide();
        }
    }

    var _createOverlay = function() {
        if ( _getOption('overlay') === true ) {
            the.overlayElement = document.createElement('DIV');

            KTUtil.css(the.overlayElement, 'z-index', KTUtil.css(the.element, 'z-index') - 1); // update

            body.append(the.overlayElement);

            KTUtil.addClass(the.overlayElement, _getOption('overlay-class'));

            KTUtil.addEvent(the.overlayElement, 'click', function(e) {
                e.preventDefault();
                _hide();
            });
        }
    }

    var _deleteOverlay = function() {
        if ( the.overlayElement !== null ) {
            KTUtil.remove(the.overlayElement);
        }
    }

    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-drawer-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-drawer-' + name);
            var value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }

            return value;
        } else {
            var optionName = KTUtil.snakeToCamel(name);

            if ( the.options[optionName] ) {
                return KTUtil.getResponsiveValue(the.options[optionName]);
            } else {
                return null;
            }
        }
    }

    var _getWidth = function() {
        var width = _getOption('width');

        if ( width === 'auto') {
            width = KTUtil.css(the.element, 'width');
        }

        return width;
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('drawer');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.toggle = function() {
        return _toggle();
    }

    the.show = function() {
        return _show();
    }

    the.hide = function() {
        return _hide();
    }

    the.isShown = function() {
        return the.shown;
    }

    the.update = function() {
        _update();
    }

    the.goElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTDrawer.getInstance = function(element) {
    if (element !== null && KTUtil.data(element).has('drawer')) {
        return KTUtil.data(element).get('drawer');
    } else {
        return null;
    }
}

// Hide all drawers and skip one if provided
KTDrawer.hideAll = function(skip = null, selector = '[data-kt-drawer="true"]') {
    var items = document.querySelectorAll(selector);

    if (items && items.length > 0) {
        for (var i = 0, len = items.length; i < len; i++) {
            var item = items[i];
            var drawer = KTDrawer.getInstance(item);

            if (!drawer) {
                continue;
            }

            if ( skip ) {
                if ( item !== skip ) {
                    drawer.hide();
                }
            } else {
                drawer.hide();
            }
        }
    }
}

// Update all drawers
KTDrawer.updateAll = function(selector = '[data-kt-drawer="true"]') {
    var items = document.querySelectorAll(selector);

    if (items && items.length > 0) {
        for (var i = 0, len = items.length; i < len; i++) {
            var item = items[i];
            var drawer = KTDrawer.getInstance(item);

            if (drawer) {
                drawer.update();;
            }
        }
    }
}

// Create instances
KTDrawer.createInstances = function(selector = '[data-kt-drawer="true"]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Initialize Menus
    var elements = body.querySelectorAll(selector);
    var drawer;

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            drawer = new KTDrawer(elements[i]);
        }
    }
}

// Toggle instances
KTDrawer.handleShow = function() {
    // External drawer toggle handler
    KTUtil.on(document.body,  '[data-kt-drawer-show="true"][data-kt-drawer-target]', 'click', function(e) {
        var element = document.querySelector(this.getAttribute('data-kt-drawer-target'));

        if (element) {
            KTDrawer.getInstance(element).show();
        } 
    });
}

// Dismiss instances
KTDrawer.handleDismiss = function() {
    // External drawer toggle handler
    KTUtil.on(document.body,  '[data-kt-drawer-dismiss="true"]', 'click', function(e) {
        var element = this.closest('[data-kt-drawer="true"]');

        if (element) {
            var drawer = KTDrawer.getInstance(element);
            if (drawer.isShown()) {
                drawer.hide();
            }
        } 
    });
}

// Window resize Handling
window.addEventListener('resize', function() {
    var timer;
    var body = document.getElementsByTagName("BODY")[0];

    KTUtil.throttle(timer, function() {
        // Locate and update drawer instances on window resize
        var elements = body.querySelectorAll('[data-kt-drawer="true"]');

        if ( elements && elements.length > 0 ) {
            for (var i = 0, len = elements.length; i < len; i++) {
                var drawer = KTDrawer.getInstance(elements[i]);
                if (drawer) {
                    drawer.update();
                }
            }
        }
    }, 200);
});

// Global initialization
KTDrawer.init = function() {
    KTDrawer.createInstances();
    KTDrawer.handleShow();
    KTDrawer.handleDismiss();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTDrawer.init);
} else {
    KTDrawer.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTDrawer;
}
"use strict";

// Class definition
var KTEventHandler = function() {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var _handlers = {};

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////
    var _triggerEvent = function(element, name, target, e) {
        if ( KTUtil.data(element).has(name) === true ) {
            var handlerId = KTUtil.data(element).get(name);

            if ( _handlers[name] && _handlers[name][handlerId] ) {
                var handler = _handlers[name][handlerId];

                if ( handler.name === name ) {
                    if ( handler.one == true ) {
                        if ( handler.fired == false ) {
                            _handlers[name][handlerId].fired = true;

                            return handler.callback.call(this, target, e);
                        }
                    } else {
                        return handler.callback.call(this, target, e);
                    }
                }
            }
        }

        return null;
    }

    var _addEvent = function(element, name, callback, one) {
        var handlerId = KTUtil.getUniqueId('event');

        KTUtil.data(element).set(name, handlerId);

        if ( !_handlers[name] ) {
            _handlers[name] = {};
        }

        _handlers[name][handlerId] = {
            name: name,
            callback: callback,
            one: one,
            fired: false
        };
    }

    var _removeEvent = function(element, name) {
        var handlerId = KTUtil.data(element).get(name);

        if (_handlers[name] && _handlers[name][handlerId]) {
            delete _handlers[name][handlerId];
        }
    }

    ////////////////////////////
    // ** Public Methods  ** //
    ////////////////////////////
    return {
        trigger: function(element, name, target, e) {
            return _triggerEvent(element, name, target, e);
        },

        on: function(element, name, handler) {
            return _addEvent(element, name, handler);
        },

        one: function(element, name, handler) {
            return _addEvent(element, name, handler, true);
        },

        off: function(element, name) {
            return _removeEvent(element, name);
        },

        debug: function() {
            for (var b in _handlers) {
                if ( _handlers.hasOwnProperty(b) ) console.log(b);
            }
        }
    }
}();

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTEventHandler;
}

"use strict";

// Class definition
var KTFeedback = function(options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    // Default options
    var defaultOptions = {
        'width' : 100,
        'placement' : 'top-center',
        'content' : '',
        'type': 'popup'
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        _init();
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('feedback');
        the.element;
        the.shown = false;

        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('feedback', the);
    }

    var _handlers = function() {
        KTUtil.addEvent(the.element, 'click', function(e) {
            e.preventDefault();

            _go();
        });
    }

    var _show = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.feedback.show', the) === false ) {
            return;
        }

        if ( the.options.type === 'popup') {
            _showPopup();
        }

        KTEventHandler.trigger(the.element, 'kt.feedback.shown', the);

        return the;
    }

    var _hide = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.feedback.hide', the) === false ) {
            return;
        }

        if ( the.options.type === 'popup') {
            _hidePopup();
        }

        the.shown = false;

        KTEventHandler.trigger(the.element, 'kt.feedback.hidden', the);

        return the;
    }

    var _showPopup = function() {
        the.element = document.createElement("DIV");

        KTUtil.addClass(the.element, 'feedback feedback-popup');
        KTUtil.setHTML(the.element, the.options.content);

        if (the.options.placement == 'top-center') {
            _setPopupTopCenterPosition();
        }

        body.appendChild(the.element);

        KTUtil.addClass(the.element, 'feedback-shown');

        the.shown = true;
    }

    var _setPopupTopCenterPosition = function() {
        var width = KTUtil.getResponsiveValue(the.options.width);
        var height = KTUtil.css(the.element, 'height');

        KTUtil.addClass(the.element, 'feedback-top-center');

        KTUtil.css(the.element, 'width', width);
        KTUtil.css(the.element, 'left', '50%');
        KTUtil.css(the.element, 'top', '-' + height);
    }

    var _hidePopup = function() {
        the.element.remove();
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('feedback');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.show = function() {
        return _show();
    }

    the.hide = function() {
        return _hide();
    }

    the.isShown = function() {
        return the.shown;
    }

    the.getElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTFeedback;
}

"use strict";

// Class definition
var KTImageInput = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var the = this;

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default Options
    var defaultOptions = {
        
    };

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('image-input') === true ) {
            the = KTUtil.data(element).get('image-input');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('image-input');

        // Elements
        the.element = element;
        the.inputElement = KTUtil.find(element, 'input[type="file"]');
        the.wrapperElement = KTUtil.find(element, '.image-input-wrapper');
        the.cancelElement = KTUtil.find(element, '[data-kt-image-input-action="cancel"]');
        the.removeElement = KTUtil.find(element, '[data-kt-image-input-action="remove"]');
        the.hiddenElement = KTUtil.find(element, 'input[type="hidden"]');
        the.src = KTUtil.css(the.wrapperElement, 'backgroundImage');

        // Set initialized
        the.element.setAttribute('data-kt-image-input', 'true');

        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('image-input', the);
    }

    // Init Event Handlers
    var _handlers = function() {
        KTUtil.addEvent(the.inputElement, 'change', _change);
        KTUtil.addEvent(the.cancelElement, 'click', _cancel);
        KTUtil.addEvent(the.removeElement, 'click', _remove);
    }

    // Event Handlers
    var _change = function(e) {
        e.preventDefault();

        if ( the.inputElement !== null && the.inputElement.files && the.inputElement.files[0] ) {
            // Fire change event
            if ( KTEventHandler.trigger(the.element, 'kt.imageinput.change', the) === false ) {
                return;
            }

            var reader = new FileReader();

            reader.onload = function(e) {
                KTUtil.css(the.wrapperElement, 'background-image', 'url('+ e.target.result +')');
            }

            reader.readAsDataURL(the.inputElement.files[0]);

            KTUtil.addClass(the.element, 'image-input-changed');
            KTUtil.removeClass(the.element, 'image-input-empty');

            // Fire removed event
            KTEventHandler.trigger(the.element, 'kt.imageinput.changed', the);
        }
    }

    var _cancel = function(e) {
        e.preventDefault();

        // Fire cancel event
        if ( KTEventHandler.trigger(the.element, 'kt.imageinput.cancel', the) === false ) {
            return;
        }

        KTUtil.removeClass(the.element, 'image-input-changed');
        KTUtil.removeClass(the.element, 'image-input-empty');
        KTUtil.css(the.wrapperElement, 'background-image', the.src);
        the.inputElement.value = "";

        if ( the.hiddenElement !== null ) {
            the.hiddenElement.value = "0";
        }

        // Fire canceled event
        KTEventHandler.trigger(the.element, 'kt.imageinput.canceled', the);
    }

    var _remove = function(e) {
        e.preventDefault();

        // Fire remove event
        if ( KTEventHandler.trigger(the.element, 'kt.imageinput.remove', the) === false ) {
            return;
        }

        KTUtil.removeClass(the.element, 'image-input-changed');
        KTUtil.addClass(the.element, 'image-input-empty');
        KTUtil.css(the.wrapperElement, 'background-image', "none");
        the.inputElement.value = "";

        if ( the.hiddenElement !== null ) {
            the.hiddenElement.value = "1";
        }

        // Fire removed event
        KTEventHandler.trigger(the.element, 'kt.imageinput.removed', the);
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('image-input');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.getInputElement = function() {
        return the.inputElement;
    }

    the.goElement = function() {
        return the.element;
    }
    
    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTImageInput.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('image-input') ) {
        return KTUtil.data(element).get('image-input');
    } else {
        return null;
    }
}

// Create instances
KTImageInput.createInstances = function(selector = '[data-kt-image-input]') {
    // Initialize Menus
    var elements = document.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            new KTImageInput(elements[i]);
        }
    }
}

// Global initialization
KTImageInput.init = function() {
    KTImageInput.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTImageInput.init);
} else {
    KTImageInput.init();
}

// Webpack Support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTImageInput;
}

"use strict";

// Class definition
var KTMenu = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var the = this;

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default Options
    var defaultOptions = {
        dropdown: {
            hoverTimeout: 200,
            zindex: 105
        },

        accordion: {
            slideSpeed: 250,
            expand: false
        }
    };

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('menu') === true ) {
            the = KTUtil.data(element).get('menu');
        } else {
            _init();
        }
    }

    var _init = function() {
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('menu');
        the.element = element;
        the.triggerElement;

        // Set initialized
        the.element.setAttribute('data-kt-menu', 'true');

        _setTriggerElement();
        _update();

        KTUtil.data(the.element).set('menu', the);
    }

    var _destroy = function() {  // todo

    }

    // Event Handlers
    // Toggle handler
    var _click = function(element, e) {
        e.preventDefault();

        var item = _getItemElement(element);

        if ( _getItemOption(item, 'trigger') !== 'click' ) {
            return;
        }

        if ( _getItemOption(item, 'toggle') === false ) {
            _show(item);
        } else {
            _toggle(item);
        }
    }

    // Link handler
    var _link = function(element, e) {
        if ( KTEventHandler.trigger(the.element, 'kt.menu.link.click', the) === false )  {
            return;
        }

        // Dismiss all shown dropdowns
        KTMenu.hideDropdowns();

        KTEventHandler.trigger(the.element, 'kt.menu.link.clicked', the);
    }

    // Dismiss handler
    var _dismiss = function(element, e) {
        var item = _getItemElement(element);
        var items = _getItemChildElements(item);

        if ( item !== null && _getItemSubType(item) === 'dropdown') {
            _hide(item); // hide items dropdown
            // Hide all child elements as well
            
            if ( items.length > 0 ) {
                for (var i = 0, len = items.length; i < len; i++) {
                    if ( items[i] !== null &&  _getItemSubType(items[i]) === 'dropdown') {
                        _hide(tems[i]);
                    }
                }
            }
        }
    }

    // Mouseover handle
    var _mouseover = function(element, e) {
        var item = _getItemElement(element);

        if ( item === null ) {
            return;
        }

        if ( _getItemOption(item, 'trigger') !== 'hover' ) {
            return;
        }

        if ( KTUtil.data(item).get('hover') === '1' ) {
            clearTimeout(KTUtil.data(item).get('timeout'));
            KTUtil.data(item).remove('hover');
            KTUtil.data(item).remove('timeout');
        }

        _show(item);
    }

    // Mouseout handle
    var _mouseout = function(element, e) {
        var item = _getItemElement(element);

        if ( item === null ) {
            return;
        }

        if ( _getItemOption(item, 'trigger') !== 'hover' ) {
            return;
        }

        var timeout = setTimeout(function() {
            if ( KTUtil.data(item).get('hover') === '1' ) {
                _hide(item);
            }
        }, the.options.dropdown.hoverTimeout);

        KTUtil.data(item).set('hover', '1');
        KTUtil.data(item).set('timeout', timeout);
    }

    // Toggle item sub
    var _toggle = function(item) {
        if ( !item ) {
            item = the.triggerElement;
        }

        if ( _isItemSubShown(item) === true ) {
            _hide(item);
        } else {
            _show(item);
        }
    }

    // Show item sub
    var _show = function(item) {
        if ( !item ) {
            item = the.triggerElement;
        }

        if ( _isItemSubShown(item) === true ) {
            return;
        }

        if ( _getItemSubType(item) === 'dropdown' ) {
            _showDropdown(item); // // show current dropdown
        } else if ( _getItemSubType(item) === 'accordion' ) {
            _showAccordion(item);
        }

        // Remember last submenu type
        KTUtil.data(item).set('type', _getItemSubType(item));  // updated
    }

    // Hide item sub
    var _hide = function(item) {
        if ( !item ) {
            item = the.triggerElement;
        }

        if ( _isItemSubShown(item) === false ) {
            return;
        }
        
        if ( _getItemSubType(item) === 'dropdown' ) {
            _hideDropdown(item);
        } else if ( _getItemSubType(item) === 'accordion' ) {
            _hideAccordion(item);
        }
    }

    // Reset item state classes if item sub type changed
    var _reset = function(item) {        
        if ( _hasItemSub(item) === false ) {
            return;
        }

        var sub = _getItemSubElement(item);

        // Reset sub state if sub type is changed during the window resize
        if ( KTUtil.data(item).has('type') && KTUtil.data(item).get('type') !== _getItemSubType(item) ) {  // updated
            KTUtil.removeClass(item, 'hover'); 
            KTUtil.removeClass(item, 'show'); 
            KTUtil.removeClass(sub, 'show'); 
        }  // updated
    }

    // Update all item state classes if item sub type changed
    var _update = function() {
        var items = the.element.querySelectorAll('.menu-item[data-kt-menu-trigger]');

        if ( items && items.length > 0 ) {
            for (var i = 0, len = items.length; i < len; i++) {
                _reset(items[i]);
            }
        }
    }

    // Set external trigger element
    var _setTriggerElement = function() {
        var target = document.querySelector('[data-kt-menu-target="# ' + the.element.getAttribute('id')  + '"]');

        if ( target !== null ) {
            the.triggerElement = target;
        } else if ( the.element.closest('[data-kt-menu-trigger]') ) {
            the.triggerElement = the.element.closest('[data-kt-menu-trigger]');
        } else if ( the.element.parentNode && KTUtil.child(the.element.parentNode, '[data-kt-menu-trigger]')) {
            the.triggerElement = KTUtil.child(the.element.parentNode, '[data-kt-menu-trigger]');
        }

        if ( the.triggerElement ) {
            KTUtil.data(the.triggerElement).set('menu', the);
        }
    }

    // Test if menu has external trigger element
    var _isTriggerElement = function(item) {
        return ( the.triggerElement === item ) ? true : false;
    }

    // Test if item's sub is shown
    var _isItemSubShown = function(item) {
        var sub = _getItemSubElement(item);

        if ( sub !== null ) {
            if ( _getItemSubType(item) === 'dropdown' ) {
                if ( KTUtil.hasClass(sub, 'show') === true && sub.hasAttribute('data-popper-placement') === true ) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return KTUtil.hasClass(item, 'show');
            }
        } else {
            return false;
        }
    }

    // Test if item dropdown is permanent
    var _isItemDropdownPermanent = function(item) {
        return _getItemOption(item, 'permanent') === true ? true : false;
    }

    // Test if item's parent is shown
    var _isItemParentShown = function(item) {
        return KTUtil.parents(item, '.menu-item.show').length > 0;
    }

    // Test of it is item sub element
    var _isItemSubElement = function(item) {
        return KTUtil.hasClass(item, 'menu-sub');
    }

    // Test if item has sub
    var _hasItemSub = function(item) {
        return (KTUtil.hasClass(item, 'menu-item') && item.hasAttribute('data-kt-menu-trigger'));
    }

    // Get link element
    var _getItemLinkElement = function(item) {
        return KTUtil.child(item, '.menu-link');
    }

    // Get toggle element
    var _getItemToggleElement = function(item) {
        if ( the.triggerElement ) {
            return the.triggerElement;
        } else {
            return _getItemLinkElement(item);
        }
    }

    // Get item sub element
    var _getItemSubElement = function(item) {
        if ( _isTriggerElement(item) === true ) {
            return the.element;
        } if ( item.classList.contains('menu-sub') === true ) {
            return item;
        } else if ( KTUtil.data(item).has('sub') ) {
            return KTUtil.data(item).get('sub');
        } else {
            return KTUtil.child(item, '.menu-sub');
        }
    }

    // Get item sub type
    var _getItemSubType = function(element) {
        var sub = _getItemSubElement(element);

        if ( sub && parseInt(KTUtil.css(sub, 'z-index')) > 0 ) {
            return "dropdown";
        } else {
            return "accordion";
        }
    }

    // Get item element
    var _getItemElement = function(element) {
        var item, sub;

        // Element is the external trigger element
        if (_isTriggerElement(element) ) {
            return element;
        }   

        // Element has item toggler attribute
        if ( element.hasAttribute('data-kt-menu-trigger') ) {
            return element;
        }

        // Element has item DOM reference in it's data storage
        if ( KTUtil.data(element).has('item') ) {
            return KTUtil.data(element).get('item');
        }

        // Item is parent of element
        if ( (item = element.closest('.menu-item[data-kt-menu-trigger]')) ) {
            return item;
        }

        // Element's parent has item DOM reference in it's data storage
        if ( (sub = element.closest('.menu-sub')) ) {
            if ( KTUtil.data(sub).has('item') === true ) {
                return KTUtil.data(sub).get('item')
            } 
        }
    }

    // Get item parent element
    var _getItemParentElement = function(item) {  
        var sub = item.closest('.menu-sub');
        var parentItem;

        if ( KTUtil.data(sub).has('item') ) {
            return KTUtil.data(sub).get('item');
        }

        if ( sub && (parentItem = sub.closest('.menu-item[data-kt-menu-trigger]')) ) {
            return parentItem;
        }

        return null;
    }

    // Get item parent elements
    var _getItemParentElements = function(item) {
        var parents = [];
        var parent;
        var i = 0;

        do {
            parent = _getItemParentElement(item);
            
            if ( parent ) {
                parents.push(parent);
                item = parent;
            }           

            i++;
        } while (parent !== null && i < 20);

        if ( the.triggerElement ) {
            parents.unshift(the.triggerElement);
        }

        return parents;
    }

    // Get item child element
    var _getItemChildElement = function(item) {
        var selector = item;
        var element;

        if ( KTUtil.data(item).get('sub') ) {
            selector = KTUtil.data(item).get('sub');
        }

        if ( selector !== null ) {
            //element = selector.querySelector('.show.menu-item[data-kt-menu-trigger]');
            element = selector.querySelector('.menu-item[data-kt-menu-trigger]');

            if ( element ) {
                return element;
            } else {
                return null;
            }
        } else {
            return null;
        }
    }   
    
    // Get item child elements
    var _getItemChildElements = function(item) {
        var children = [];
        var child;
        var i = 0;

        do {
            child = _getItemChildElement(item);
            
            if ( child ) {
                children.push(child);
                item = child;
            }           

            i++;
        } while (child !== null && i < 20);

        return children;
    }

    // Show item dropdown
    var _showDropdown = function(item) {
        // Handle dropdown show event
        if ( KTEventHandler.trigger(the.element, 'kt.menu.dropdown.show', item) === false )  {
            return;
        }

        // Hide all currently shown dropdowns except current one
        KTMenu.hideDropdowns(item); 

        var toggle = _isTriggerElement(item) ? item : _getItemLinkElement(item);
        var sub = _getItemSubElement(item);

        var width = _getItemOption(item, 'width');
        var height = _getItemOption(item, 'height');

        var zindex = the.options.dropdown.zindex; // update
        var parentZindex = KTUtil.getHighestZindex(item); // update

        // Apply a new z-index if dropdown's toggle element or it's parent has greater z-index // update
        if ( parentZindex !== null && parentZindex >= zindex ) {
            zindex = parentZindex + 1;
        }

        if ( zindex > 0 ) {
            KTUtil.css(sub, 'z-index', zindex);
        }

        if ( width !== null ) {
            KTUtil.css(sub, 'width', width);
        }

        if ( height !== null ) {
            KTUtil.css(sub, 'height', height);
        }

        KTUtil.css(sub, 'display', '');
        KTUtil.css(sub, 'overflow', '');

        // Init popper(new)
        _initDropdownPopper(item, sub); 

        KTUtil.addClass(item, 'show');
        KTUtil.addClass(item, 'menu-dropdown');
        KTUtil.addClass(sub, 'show');

        // Append the sub the the root of the menu
        if ( _getItemOption(item, 'overflow') === true ) {
            document.body.appendChild(sub);
            KTUtil.data(item).set('sub', sub);
            KTUtil.data(sub).set('item', item);
            KTUtil.data(sub).set('menu', the);
        } else {
            KTUtil.data(sub).set('item', item);
        }

        // Handle dropdown shown event
        KTEventHandler.trigger(the.element, 'kt.menu.dropdown.shown', item);
    }

    // Hide item dropdown
    var _hideDropdown = function(item) {
        // Handle dropdown hide event
        if ( KTEventHandler.trigger(the.element, 'kt.menu.dropdown.hide', item) === false )  {
            return;
        }

        var sub = _getItemSubElement(item);

        KTUtil.css(sub, 'z-index', '');
        KTUtil.css(sub, 'width', '');
        KTUtil.css(sub, 'height', '');

        KTUtil.removeClass(item, 'show');
        KTUtil.removeClass(item, 'menu-dropdown');
        KTUtil.removeClass(sub, 'show');

        // Append the sub back to it's parent
        if ( _getItemOption(item, 'overflow') === true ) {
            if (item.classList.contains('menu-item')) {
                item.appendChild(sub);
            } else {
                KTUtil.insertAfter(the.element, item);
            }
            
            KTUtil.data(item).remove('sub');
            KTUtil.data(sub).remove('item');
            KTUtil.data(sub).remove('menu');
        } 

        // Destroy popper(new)
        _destroyDropdownPopper(item);
        
        // Handle dropdown hidden event 
        KTEventHandler.trigger(the.element, 'kt.menu.dropdown.hidden', item);
    }

    // Init dropdown popper(new)
    var _initDropdownPopper = function(item, sub) {
        // Setup popper instance
        var reference;
        var attach = _getItemOption(item, 'attach');

        if ( attach ) {
            if ( attach === 'parent') {
                reference = item.parentNode;
            } else {
                reference = document.querySelector(attach);
            }
        } else {
            reference = item;
        }

        var popper = Popper.createPopper(reference, sub, _getDropdownPopperConfig(item)); 
        KTUtil.data(item).set('popper', popper);
    }

    // Destroy dropdown popper(new)
    var _destroyDropdownPopper = function(item) {
        if ( KTUtil.data(item).has('popper') === true ) {
            KTUtil.data(item).get('popper').destroy();
            KTUtil.data(item).remove('popper');
        }
    }

    // Prepare popper config for dropdown(see: https://popper.js.org/docs/v2/)
    var _getDropdownPopperConfig = function(item) {
        // Placement
        var placement = _getItemOption(item, 'placement');
        if (!placement) {
            placement = 'right';
        }

        // Offset
        var offsetValue = _getItemOption(item, 'offset');
        var offset = offsetValue ? offsetValue.split(",") : [];

        // Strategy
        var strategy = _getItemOption(item, 'overflow') === true ? 'absolute' : 'fixed';

        var altAxis = _getItemOption(item, 'flip') !== false ? true : false;

        var popperConfig = {
            placement: placement,
            strategy: strategy,
            modifiers: [{
                name: 'offset',
                options: {
                    offset: offset
                }
            }, {
                name: 'preventOverflow',
                options: {
                    altAxis: altAxis
                }
            }, {
                name: 'flip', 
                options: {
                    flipVariations: false
                }
            }]
        };

        return popperConfig;
    }

    // Show item accordion
    var _showAccordion = function(item) {
        if ( KTEventHandler.trigger(the.element, 'kt.menu.accordion.show', item) === false )  {
            return;
        }

        if ( the.options.accordion.expand === false ) {
            _hideAccordions(item);
        }

        var sub = _getItemSubElement(item);

        if ( KTUtil.data(item).has('popper') === true ) {
            _hideDropdown(item);
        }

        KTUtil.addClass(item, 'hover'); // updateWW

        KTUtil.addClass(item, 'showing');

        KTUtil.slideDown(sub, the.options.accordion.slideSpeed, function() {
            KTUtil.removeClass(item, 'showing');
            KTUtil.addClass(item, 'show');
            KTUtil.addClass(sub, 'show');

            KTEventHandler.trigger(the.element, 'kt.menu.accordion.shown', item);
        });        
    }

    // Hide item accordion
    var _hideAccordion = function(item) {
        if ( KTEventHandler.trigger(the.element, 'kt.menu.accordion.hide', item) === false )  {
            return;
        }
        
        var sub = _getItemSubElement(item);

        KTUtil.addClass(item, 'hiding');

        KTUtil.slideUp(sub, the.options.accordion.slideSpeed, function() {
            KTUtil.removeClass(item, 'hiding');
            KTUtil.removeClass(item, 'show');
            KTUtil.removeClass(sub, 'show');

            KTUtil.removeClass(item, 'hover'); // update

            KTEventHandler.trigger(the.element, 'kt.menu.accordion.hidden', item);
        });
    }

    // Hide all shown accordions of item
    var _hideAccordions = function(item) {
        var itemsToHide = KTUtil.findAll(the.element, '.show[data-kt-menu-trigger]');
        var itemToHide;

        if (itemsToHide && itemsToHide.length > 0) {
            for (var i = 0, len = itemsToHide.length; i < len; i++) {
                itemToHide = itemsToHide[i];

                if ( _getItemSubType(itemToHide) === 'accordion' && itemToHide !== item && item.contains(itemToHide) === false && itemToHide.contains(item) === false ) {
                    _hideAccordion(itemToHide);
                }
            }
        }
    }

    // Get item option(through html attributes)
    var _getItemOption = function(item, name) {
        var attr;
        var value = null;

        if ( item && item.hasAttribute('data-kt-menu-' + name) ) {
            attr = item.getAttribute('data-kt-menu-' + name);
            value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }
        }

        return value;
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('menu');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Event Handlers
    the.click = function(element, e) {
        return _click(element, e);
    }

    the.link = function(element, e) {
        return _link(element, e);
    }

    the.dismiss = function(element, e) {
        return _dismiss(element, e);
    }

    the.mouseover = function(element, e) {
        return _mouseover(element, e);
    }

    the.mouseout = function(element, e) {
        return _mouseout(element, e);
    }

    // General Methods
    the.getItemTriggerType = function(item) {
        return _getItemOption(item, 'trigger');
    }

    the.getItemSubType = function(element) {
       return _getItemSubType(element);
    }

    the.show = function(item) {
        return _show(item);
    }

    the.hide = function(item) {
        return _hide(item);
    }

    the.reset = function(item) {
        return _reset(item);
    }

    the.update = function() {
        return _update();
    }

    the.getElement = function() {
        return the.element;
    }

    the.getItemLinkElement = function(item) {
        return _getItemLinkElement(item);
    }

    the.getItemToggleElement = function(item) {
        return _getItemToggleElement(item);
    }

    the.getItemSubElement = function(item) {
        return _getItemSubElement(item);
    }

    the.getItemParentElements = function(item) {
        return _getItemParentElements(item);
    }

    the.isItemSubShown = function(item) {
        return _isItemSubShown(item);
    }

    the.isItemParentShown = function(item) {
        return _isItemParentShown(item);
    }

    the.getTriggerElement = function() {
        return the.triggerElement;
    }

    the.isItemDropdownPermanent = function(item) {
        return _isItemDropdownPermanent(item);
    }

    the.destroy = function() {
        return _destroy();
    }

    // Accordion Mode Methods
    the.hideAccordions = function(item) {
        return _hideAccordions(item);
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }
};

// Get KTMenu instance by element
KTMenu.getInstance = function(element) {
    var menu;
    var item;

    // Element has menu DOM reference in it's DATA storage
    if ( KTUtil.data(element).has('menu') ) {
        return KTUtil.data(element).get('menu');
    }

    // Element has .menu parent 
    if ( menu = element.closest('.menu') ) {
        if ( KTUtil.data(menu).has('menu') ) {
            return KTUtil.data(menu).get('menu');
        }
    }
    
    // Element has a parent with DOM reference to .menu in it's DATA storage
    if ( KTUtil.hasClass(element, 'menu-link') ) {
        var sub = element.closest('.menu-sub');

        if ( KTUtil.data(sub).has('menu') ) {
            return KTUtil.data(sub).get('menu');
        }
    } 

    return null;
}

// Hide all dropdowns and skip one if provided
KTMenu.hideDropdowns = function(skip) {
    var items = document.querySelectorAll('.show.menu-dropdown[data-kt-menu-trigger]');

    if (items && items.length > 0) {
        for (var i = 0, len = items.length; i < len; i++) {
            var item = items[i];
            var menu = KTMenu.getInstance(item);

            if ( menu && menu.getItemSubType(item) === 'dropdown' ) {
                if ( skip ) {
                    if ( menu.getItemSubElement(item).contains(skip) === false && item.contains(skip) === false &&  item !== skip ) {
                        menu.hide(item);
                    }
                } else {
                    menu.hide(item);
                }
            }
        }
    }
}

// Update all dropdowns popover instances
KTMenu.updateDropdowns = function() {
    var items = document.querySelectorAll('.show.menu-dropdown[data-kt-menu-trigger]');

    if (items && items.length > 0) {
        for (var i = 0, len = items.length; i < len; i++) {
            var item = items[i];

            if ( KTUtil.data(item).has('popper') ) {
                KTUtil.data(item).get('popper').forceUpdate();
            }
        }
    }
}

// Global handlers
KTMenu.initGlobalHandlers = function() {
    // Dropdown handler
    document.addEventListener("click", function(e) {
        var items = document.querySelectorAll('.show.menu-dropdown[data-kt-menu-trigger]');
        var menu;
        var item;
        var sub;
        var menuObj;

        if ( items && items.length > 0 ) {
            for ( var i = 0, len = items.length; i < len; i++ ) {
                item = items[i];
                menuObj = KTMenu.getInstance(item);

                if (menuObj && menuObj.getItemSubType(item) === 'dropdown') {
                    menu = menuObj.getElement();
                    sub = menuObj.getItemSubElement(item);

                    if ( item === e.target || item.contains(e.target) ) {
                        continue;
                    }
                    
                    if ( sub === e.target || sub.contains(e.target) ) {
                        continue;
                    }
                        
                    menuObj.hide(item);
                }
            }
        }
    });

    // Sub toggle handler(updated)
    KTUtil.on(document.body,  '.menu-item[data-kt-menu-trigger] > .menu-link, [data-kt-menu-trigger]:not(.menu-item):not([data-kt-menu-trigger="auto"])', 'click', function(e) {
        var menu = KTMenu.getInstance(this);

        if ( menu !== null ) {
            return menu.click(this, e);
        }
    });

    // Link handler
    KTUtil.on(document.body,  '.menu-item:not([data-kt-menu-trigger]) > .menu-link', 'click', function(e) {
        var menu = KTMenu.getInstance(this);

        if ( menu !== null ) {
            return menu.link(this, e);
        }
    });

    // Dismiss handler
    KTUtil.on(document.body,  '[data-kt-menu-dismiss="true"]', 'click', function(e) {
        var menu = KTMenu.getInstance(this);

        if ( menu !== null ) {
            return menu.dismiss(this, e);
        }
    });

    // Mouseover handler
    KTUtil.on(document.body,  '[data-kt-menu-trigger], .menu-sub', 'mouseover', function(e) {
        var menu = KTMenu.getInstance(this);

        if ( menu !== null && menu.getItemSubType(this) === 'dropdown' ) {
            return menu.mouseover(this, e);
        }
    });

    // Mouseout handler
    KTUtil.on(document.body,  '[data-kt-menu-trigger], .menu-sub', 'mouseout', function(e) {
        var menu = KTMenu.getInstance(this);

        if ( menu !== null && menu.getItemSubType(this) === 'dropdown' ) {
            return menu.mouseout(this, e);
        }
    });

    // Resize handler
    window.addEventListener('resize', function() {
        var menu;
        var timer;

        KTUtil.throttle(timer, function() {
            // Locate and update Offcanvas instances on window resize
            var elements = document.querySelectorAll('[data-kt-menu="true"]');

            if ( elements && elements.length > 0 ) {
                for (var i = 0, len = elements.length; i < len; i++) {
                    menu = KTMenu.getInstance(elements[i]);
                    if (menu) {
                        menu.update();
                    }
                }
            }
        }, 200);
    });
}

// Global instances
KTMenu.createInstances = function(selector = '[data-kt-menu="true"]') {
    // Initialize menus
    var elements = document.querySelectorAll(selector);
    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            new KTMenu(elements[i]);
        }
    }
}

// Global initialization
KTMenu.init = function() {
    // Global Event Handlers
    KTMenu.initGlobalHandlers();

    // Lazy Initialization
    KTMenu.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTMenu.init);
} else {
   KTMenu.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTMenu;
}

"use strict";

// Class definition
var KTPasswordMeter = function(element, options) {
    ////////////////////////////
    // ** Private variables  ** //
    ////////////////////////////
    var the = this;

    if (!element) {
        return;
    }

    // Default Options
    var defaultOptions = {
        minLength: 8,
        checkUppercase: true,        
        checkLowercase: true,
        checkDigit: true,
        checkChar: true,
        scoreHighlightClass: 'active'
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    // Constructor
    var _construct = function() {
        if ( KTUtil.data(element).has('password-meter') === true ) {
            the = KTUtil.data(element).get('password-meter');
        } else {
            _init();
        }
    }

    // Initialize
    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.score = 0;
        the.checkSteps = 5;

        // Elements
        the.element = element;
        the.inputElement = the.element.querySelector('input[type]');
        the.visibilityElement = the.element.querySelector('[data-kt-password-meter-control="visibility"]');
        the.highlightElement = the.element.querySelector('[data-kt-password-meter-control="highlight"]'); 

        // Set initialized
        the.element.setAttribute('data-kt-password-meter', 'true');
        
        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('password-meter', the);
    }

    // Handlers
    var _handlers = function() {
        the.inputElement.addEventListener('input', function() {
            _check();
        });

        if (the.visibilityElement) {
            the.visibilityElement.addEventListener('click', function() {
                _visibility();
            });
        }
    }   

    // Event handlers
    var _check = function() {
        var score = 0;
        var checkScore = _getCheckScore();
        
        if (_checkLength() === true) {
            score = score + checkScore;
        }

        if (the.options.checkUppercase === true && _checkLowercase() === true) {
            score = score + checkScore;
        }

        if (the.options.checkLowercase === true && _checkUppercase() === true ) {
            score = score + checkScore;
        }

        if (the.options.checkDigit === true && _checkDigit() === true ) {
            score = score + checkScore;
        }

        if (the.options.checkChar === true && _checkChar() === true ) {
            score = score + checkScore;
        }

        the.score = score;

        _highlight();
    }

    var _checkLength = function() {
        return the.inputElement.value.length >= the.options.minLength;  // 20 score
    }

    var _checkLowercase = function() {
        return /[a-z]/.test(the.inputElement.value);  // 20 score
    }

    var _checkUppercase = function() {
        return /[A-Z]/.test(the.inputElement.value);  // 20 score
    }

    var _checkDigit = function() {
        return /[0-9]/.test(the.inputElement.value);  // 20 score
    }

    var _checkChar = function() {
        return /[~`!#$%\^&*+=\-\[\]\\';,/{}|\\":<>\?]/g.test(the.inputElement.value);  // 20 score
    }    

    var _getCheckScore = function() {
        var count = 1;
        
        if (the.options.checkUppercase === true) {
            count++;
        }

        if (the.options.checkLowercase === true) {
            count++;
        }

        if (the.options.checkDigit === true) {
            count++;
        }

        if (the.options.checkChar === true) {
            count++;
        }

        the.checkSteps = count;

        return 100 / the.checkSteps;
    }
    
    var _highlight = function() {
        var items = [].slice.call(the.highlightElement.querySelectorAll('div'));
        var total = items.length;
        var index = 0;
        var checkScore = _getCheckScore();
        var score = _getScore();

        items.map(function (item) {
            index++;

            if ( (checkScore * index * (the.checkSteps / total)) <= score ) {
                item.classList.add('active');
            } else {
                item.classList.remove('active');
            }            
        });
    }

    var _visibility = function() {
        var visibleIcon = the.visibilityElement.querySelector('i:not(.d-none), .svg-icon:not(.d-none)');
        var hiddenIcon = the.visibilityElement.querySelector('i.d-none, .svg-icon.d-none');
        
        if (the.inputElement.getAttribute('type').toLowerCase() === 'password' ) {
            the.inputElement.setAttribute('type', 'text');
        }  else {
            the.inputElement.setAttribute('type', 'password');
        }        

        visibleIcon.classList.add('d-none');
        hiddenIcon.classList.remove('d-none');

        the.inputElement.focus();
    }

    var _reset = function() {
        the.score = 0;

        _highlight();
    }

    // Gets current password score
    var _getScore = function() {
       return the.score;
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('password-meter');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.check = function() {
        return _check();
    }

    the.getScore = function() {
        return _getScore();
    }

    the.reset = function() {
        return _reset();
    }

    the.destroy = function() {
        return _destroy();
    }
};

// Static methods
KTPasswordMeter.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('password-meter') ) {
        return KTUtil.data(element).get('password-meter');
    } else {
        return null;
    }
}

// Create instances
KTPasswordMeter.createInstances = function(selector = '[data-kt-password-meter]') {
    // Get instances
    var elements = document.body.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            // Initialize instances
            new KTPasswordMeter(elements[i]);
        }
    }
}

// Global initialization
KTPasswordMeter.init = function() {
    KTPasswordMeter.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTPasswordMeter.init);
} else {
    KTPasswordMeter.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTPasswordMeter;
}
"use strict";

// Class definition
var KTScroll = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if (!element) {
        return;
    }

    // Default options
    var defaultOptions = {
        saveState: true
    };

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('scroll') ) {
            the = KTUtil.data(element).get('scroll');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);

        // Elements
        the.element = element;        
        the.id = the.element.getAttribute('id');

        // Set initialized
        the.element.setAttribute('data-kt-scroll', 'true');

        // Update
        _update();

        // Bind Instance
        KTUtil.data(the.element).set('scroll', the);
    }

    var _setupHeight = function() {
        var heightType = _getHeightType();
        var height = _getHeight();

        // Set height
        if ( height !== null && height.length > 0 ) {
            KTUtil.css(the.element, heightType, height);
        } else {
            KTUtil.css(the.element, heightType, '');
        }
    }

    var _setupState = function () {
        if ( _getOption('save-state') === true && typeof KTCookie !== 'undefined' && the.id ) {
            if ( KTCookie.get(the.id + 'st') ) {
                var pos = parseInt(KTCookie.get(the.id + 'st'));

                if ( pos > 0 ) {
                    the.element.scrollTop = pos;
                }
            }
        }
    }

    var _setupScrollHandler = function() {
        if ( _getOption('save-state') === true && typeof KTCookie !== 'undefined' && the.id ) {
            the.element.addEventListener('scroll', _scrollHandler);
        } else {
            the.element.removeEventListener('scroll', _scrollHandler);
        }
    }

    var _destroyScrollHandler = function() {
        the.element.removeEventListener('scroll', _scrollHandler);
    }

    var _resetHeight = function() {
        KTUtil.css(the.element, _getHeightType(), '');
    }

    var _scrollHandler = function () {
        KTCookie.set(the.id + 'st', the.element.scrollTop);
    }

    var _update = function() {
        // Activate/deactivate
        if ( _getOption('activate') === true || the.element.hasAttribute('data-kt-scroll-activate') === false ) {
            _setupHeight();
            _setupScrollHandler();
            _setupState();
        } else {
            _resetHeight()
            _destroyScrollHandler();
        }        
    }

    var _getHeight = function() {
        var height = _getOption(_getHeightType());

        if ( height instanceof Function ) {
            return height.call();
        } else if ( height !== null && typeof height === 'string' && height.toLowerCase() === 'auto' ) {
            return _getAutoHeight();
        } else {
            return height;
        }
    }

    var _getAutoHeight = function() {
        var height = KTUtil.getViewPort().height;

        var dependencies = _getOption('dependencies');
        var wrappers = _getOption('wrappers');
        var offset = _getOption('offset');

        // Height dependencies
        if ( dependencies !== null ) {
            var elements = document.querySelectorAll(dependencies);

            if ( elements && elements.length > 0 ) {
                for ( var i = 0, len = elements.length; i < len; i++ ) {
                    var element = elements[i];

                    if ( KTUtil.visible(element) === false ) {
                        continue;
                    }

                    height = height - parseInt(KTUtil.css(element, 'height'));
                    height = height - parseInt(KTUtil.css(element, 'margin-top'));
                    height = height - parseInt(KTUtil.css(element, 'margin-bottom'));

                    if (KTUtil.css(element, 'border-top')) {
                        height = height - parseInt(KTUtil.css(element, 'border-top'));
                    }

                    if (KTUtil.css(element, 'border-bottom')) {
                        height = height - parseInt(KTUtil.css(element, 'border-bottom'));
                    }
                }
            }
        }

        // Wrappers
        if ( wrappers !== null ) {
            var elements = document.querySelectorAll(wrappers);
            if ( elements && elements.length > 0 ) {
                for ( var i = 0, len = elements.length; i < len; i++ ) {
                    var element = elements[i];

                    if ( KTUtil.visible(element) === false ) {
                        continue;
                    }

                    height = height - parseInt(KTUtil.css(element, 'margin-top'));
                    height = height - parseInt(KTUtil.css(element, 'margin-bottom'));
                    height = height - parseInt(KTUtil.css(element, 'padding-top'));
                    height = height - parseInt(KTUtil.css(element, 'padding-bottom'));

                    if (KTUtil.css(element, 'border-top')) {
                        height = height - parseInt(KTUtil.css(element, 'border-top'));
                    }

                    if (KTUtil.css(element, 'border-bottom')) {
                        height = height - parseInt(KTUtil.css(element, 'border-bottom'));
                    }
                }
            }
        }

        // Custom offset
        if ( offset !== null ) {
            height = height - parseInt(offset);
        }

        height = height - parseInt(KTUtil.css(the.element, 'margin-top'));
        height = height - parseInt(KTUtil.css(the.element, 'margin-bottom'));
        
        if (KTUtil.css(element, 'border-top')) {
            height = height - parseInt(KTUtil.css(element, 'border-top'));
        }

        if (KTUtil.css(element, 'border-bottom')) {
            height = height - parseInt(KTUtil.css(element, 'border-bottom'));
        }

        height = String(height) + 'px';

        return height;
    }

    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-scroll-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-scroll-' + name);

            var value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }

            return value;
        } else {
            var optionName = KTUtil.snakeToCamel(name);

            if ( the.options[optionName] ) {
                return KTUtil.getResponsiveValue(the.options[optionName]);
            } else {
                return null;
            }
        }
    }

    var _getHeightType = function() {
        if (_getOption('height')) {
            return 'height';
        } if (_getOption('min-height')) {
            return 'min-height';
        } if (_getOption('max-height')) {
            return 'max-height';
        }
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('scroll');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    the.update = function() {
        return _update();
    }

    the.getHeight = function() {
        return _getHeight();
    }

    the.getElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }
};

// Static methods
KTScroll.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('scroll') ) {
        return KTUtil.data(element).get('scroll');
    } else {
        return null;
    }
}

// Create instances
KTScroll.createInstances = function(selector = '[data-kt-scroll="true"]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Initialize Menus
    var elements = body.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            new KTScroll(elements[i]);
        }
    }
}

// Window resize handling
window.addEventListener('resize', function() {
    var timer;
    var body = document.getElementsByTagName("BODY")[0];

    KTUtil.throttle(timer, function() {
        // Locate and update Offcanvas instances on window resize
        var elements = body.querySelectorAll('[data-kt-scroll="true"]');

        if ( elements && elements.length > 0 ) {
            for (var i = 0, len = elements.length; i < len; i++) {
                var scroll = KTScroll.getInstance(elements[i]);
                if (scroll) {
                    scroll.update();
                }
            }
        }
    }, 200);
});

// Global initialization
KTScroll.init = function() {
    KTScroll.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTScroll.init);
} else {
    KTScroll.init();
}

// Webpack Support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTScroll;
}

"use strict";

// Class definition
var KTScrolltop = function(element, options) {
    ////////////////////////////
    // ** Private variables  ** //
    ////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default options
    var defaultOptions = {
        offset: 300,
        speed: 600
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        if (KTUtil.data(element).has('scrolltop')) {
            the = KTUtil.data(element).get('scrolltop');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('scrolltop');
        the.element = element;

        // Set initialized
        the.element.setAttribute('data-kt-scrolltop', 'true');

        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('scrolltop', the);
    }

    var _handlers = function() {
        var timer;

        window.addEventListener('scroll', function() {
            KTUtil.throttle(timer, function() {
                _scroll();
            }, 200);
        });

        KTUtil.addEvent(the.element, 'click', function(e) {
            e.preventDefault();

            _go();
        });
    }

    var _scroll = function() {
        var offset = parseInt(_getOption('offset'));

        var pos = KTUtil.getScrollTop(); // current vertical position

        if ( pos > offset ) {
            if ( body.hasAttribute('data-kt-scrolltop') === false ) {
                body.setAttribute('data-kt-scrolltop', 'on');
            }
        } else {
            if ( body.hasAttribute('data-kt-scrolltop') === true ) {
                body.removeAttribute('data-kt-scrolltop');
            }
        }
    }

    var _go = function() {
        var speed = parseInt(_getOption('speed'));

        KTUtil.scrollTop(0, speed);
    }

    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-scrolltop-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-scrolltop-' + name);
            var value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }

            return value;
        } else {
            var optionName = KTUtil.snakeToCamel(name);

            if ( the.options[optionName] ) {
                return KTUtil.getResponsiveValue(the.options[optionName]);
            } else {
                return null;
            }
        }
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('scrolltop');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.go = function() {
        return _go();
    }

    the.getElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }
};

// Static methods
KTScrolltop.getInstance = function(element) {
    if (element && KTUtil.data(element).has('scrolltop')) {
        return KTUtil.data(element).get('scrolltop');
    } else {
        return null;
    }
}

// Create instances
KTScrolltop.createInstances = function(selector = '[data-kt-scrolltop="true"]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Initialize Menus
    var elements = body.querySelectorAll(selector);
    var scrolltop;

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            scrolltop = new KTScrolltop(elements[i]);
        }
    }
}

// Global initialization
KTScrolltop.init = function() {
    KTScrolltop.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTScrolltop.init);
} else {
    KTScrolltop.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTScrolltop;
}

"use strict";

// Class definition
var KTSearch = function(element, options) {
    ////////////////////////////
    // ** Private variables  ** //
    ////////////////////////////
    var the = this;

    if (!element) {
        return;
    }

    // Default Options
    var defaultOptions = {
        minLength: 2,  // Miniam text lenght to query search
        keypress: true,  // Enable search on keypress 
        enter: true,  // Enable search on enter key press
        layout: 'menu',  // Use 'menu' or 'inline' layout options to display search results
        responsive: null, // Pass integer value or bootstrap compatible breakpoint key(sm,md,lg,xl,xxl) to enable reponsive form mode for device width below the breakpoint value
        showOnFocus: true // Always show menu on input focus
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    // Construct
    var _construct = function() {
        if ( KTUtil.data(element).has('search') === true ) {
            the = KTUtil.data(element).get('search');
        } else {
            _init();
        }
    }

    // Init
    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.processing = false;

        // Elements
        the.element = element;               
        the.contentElement = _getElement('content');     
        the.formElement = _getElement('form');         
        the.inputElement = _getElement('input');
        the.spinnerElement = _getElement('spinner');
        the.clearElement = _getElement('clear');
        the.toggleElement = _getElement('toggle');   
        the.submitElement = _getElement('submit');
        the.toolbarElement = _getElement('toolbar');   

        the.resultsElement = _getElement('results');
        the.suggestionElement = _getElement('suggestion'); 
        the.emptyElement = _getElement('empty'); 

        // Set initialized
        the.element.setAttribute('data-kt-search', 'true');
        
        // Layout
        the.layout = _getOption('layout');
        
        // Menu
        if ( the.layout === 'menu' ) {
            the.menuObject = new KTMenu(the.contentElement);
        } else {
            the.menuObject = null;
        }

        // Update
        _update();

        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('search', the);
    }

    // Handlera
    var _handlers = function() {
        // Focus
        the.inputElement.addEventListener('focus', _focus);

        // Blur
        the.inputElement.addEventListener('blur', _blur);

        // Keypress
        if ( _getOption('keypress') === true ) {
            the.inputElement.addEventListener('input', _input);
        }

        // Submit
        if ( the.submitElement ) {
            the.submitElement.addEventListener('click', _search);
        }

        // Enter
        if ( _getOption('enter') === true ) {
            the.inputElement.addEventListener('keypress', _enter);
        }

        // Clear 
        if ( the.clearElement ) {
            the.clearElement.addEventListener('click', _clear);
        }

        // Menu
        if ( the.menuObject ) {
            // Toggle menu
            if ( the.toggleElement ) {
                the.toggleElement.addEventListener('click', _show);

                the.menuObject.on('kt.menu.dropdown.show', function(item) {
                    if (KTUtil.visible(the.toggleElement)) {
                        the.toggleElement.classList.add('active');
                        the.toggleElement.classList.add('show');
                    } 
                });
    
                the.menuObject.on('kt.menu.dropdown.hide', function(item) {
                    if (KTUtil.visible(the.toggleElement)) {
                        the.toggleElement.classList.remove('active');
                        the.toggleElement.classList.remove('show');
                    }
                });
            }            

            the.menuObject.on('kt.menu.dropdown.shown', function() {
                the.inputElement.focus();
            });
        } 

        // Window resize handling
        window.addEventListener('resize', function() {
            var timer;

            KTUtil.throttle(timer, function() {
                _update();
            }, 200);
        });
    }

    // Focus
    var _focus = function() {
        the.element.classList.add('focus');

        if ( _getOption('show-on-focus') === true || the.inputElement.value.length >= minLength ) {
            _show();
        }        
    }

    // Blur
    var _blur = function() {        
        the.element.classList.remove('focus');
    }

    // Enter 
    var _enter = function(e) {
        var key = e.charCode || e.keyCode || 0;

        if (key == 13) {
            e.preventDefault();

            _search();
        }
    }

    // Input
    var _input = function() {
        if ( _getOption('min-length') )  {
            var minLength = parseInt(_getOption('min-length'));

            if ( the.inputElement.value.length >= minLength ) {
                _search();
            } else if ( the.inputElement.value.length === 0 ) {
                _clear();
            }
        }
    }

    // Search
    var _search = function() {
        if (the.processing === false) {
            // Show search spinner
            if (the.spinnerElement) {
                the.spinnerElement.classList.remove("d-none");
            }
            
            // Hide search clear button
            if (the.clearElement) {
                the.clearElement.classList.add("d-none");
            }

            // Hide search toolbar
            if (the.toolbarElement) {
                the.toolbarElement.classList.add("d-none");
            }

            // Focus input
            the.inputElement.focus();

            the.processing = true;
            KTEventHandler.trigger(the.element, 'kt.search.process', the);
        }
    }

    // Complete
    var _complete = function() {
        if (the.spinnerElement) {
            the.spinnerElement.classList.add("d-none");
        }

        // Show search toolbar
        if (the.clearElement) {
            the.clearElement.classList.remove("d-none");
        }

        if ( the.inputElement.value.length === 0 ) {
            _clear();
        }

        // Focus input
        the.inputElement.focus();

        _show();

        the.processing = false;
    }

    // Clear
    var _clear = function() {
        if ( KTEventHandler.trigger(the.element, 'kt.search.clear', the) === false )  {
            return;
        }

        // Clear and focus input
        the.inputElement.value = "";
        the.inputElement.focus();

        // Hide clear icon
        if (the.clearElement) {
            the.clearElement.classList.add("d-none");
        }

        // Show search toolbar
        if (the.toolbarElement) {
            the.toolbarElement.classList.remove("d-none");
        }

        // Hide menu
        if ( _getOption('show-on-focus') === false ) {
            _hide();
        }

        KTEventHandler.trigger(the.element, 'kt.search.cleared', the);
    }

    // Update
    var _update = function() {
        // Handle responsive form
        if (the.layout === 'menu') {
            var responsiveFormMode = _getResponsiveFormMode();

            if ( responsiveFormMode === 'on' && the.contentElement.contains(the.formElement) === false ) {
                the.contentElement.prepend(the.formElement);
                the.formElement.classList.remove('d-none');                
            } else if ( responsiveFormMode === 'off' && the.contentElement.contains(the.formElement) === true ) {
                the.element.prepend(the.formElement);
                the.formElement.classList.add('d-none');
            }
        }
    }

    // Show menu
    var _show = function() {
        if ( the.menuObject ) {
            _update();

            the.menuObject.show(the.element);
        }
    }

    // Hide menu
    var _hide = function() {
        if ( the.menuObject ) {
            _update();

            the.menuObject.hide(the.element);
        }
    }

    // Get option
    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-search-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-search-' + name);
            var value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }

            return value;
        } else {
            var optionName = KTUtil.snakeToCamel(name);

            if ( the.options[optionName] ) {
                return KTUtil.getResponsiveValue(the.options[optionName]);
            } else {
                return null;
            }
        }
    }

    // Get element
    var _getElement = function(name) {
        return the.element.querySelector('[data-kt-search-element="' + name + '"]');
    }

    // Check if responsive form mode is enabled
    var _getResponsiveFormMode = function() {
        var responsive = _getOption('responsive');
        var width = KTUtil.getViewPort().width;

        if (!responsive) {
            return null;
        }

        var breakpoint = KTUtil.getBreakpoint(responsive);

        if (!breakpoint ) {
            breakpoint = parseInt(responsive);
        }

        if (width < breakpoint) {
            return "on";
        } else {
            return "off";
        }
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('search');
    }    

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.show = function() {
        return _show();
    }

    the.hide = function() {
        return _hide();
    }

    the.update = function() {
        return _update();
    }

    the.search = function() {
        return _search();
    }

    the.complete = function() {
        return _complete();
    }

    the.clear = function() {
        return _clear();
    }

    the.isProcessing = function() {
        return the.processing;
    }

    the.getQuery = function() {
        return the.inputElement.value;
    }    

    the.getMenu = function() {
        return the.menuObject;
    }

    the.getFormElement = function() {
        return the.formElement;
    }

    the.getInputElement = function() {
        return the.inputElement;
    }

    the.getContentElement = function() {
        return the.contentElement;
    }

    the.getElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }
};

// Static methods
KTSearch.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('search') ) {
        return KTUtil.data(element).get('search');
    } else {
        return null;
    }
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTSearch;
}

"use strict";

// Class definition
var KTStepper = function(element, options) {
    //////////////////////////////
    // ** Private variables  ** //
    //////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default Options
    var defaultOptions = {
        startIndex: 1,
        animation: false,
        animationSpeed: '0.3s',
        animationNextClass: 'animate__animated animate__slideInRight animate__fast',
        animationPreviousClass: 'animate__animated animate__slideInLeft animate__fast'
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('stepper') === true ) {
            the = KTUtil.data(element).get('stepper');
        } else {
            _init();
        }
    }

    var _init = function() {
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('stepper');

        the.element = element;

        // Set initialized
        the.element.setAttribute('data-kt-stepper', 'true');

        // Elements
        the.steps = KTUtil.findAll(the.element, '[data-kt-stepper-element="nav"]');
        the.btnNext = KTUtil.find(the.element, '[data-kt-stepper-action="next"]');
        the.btnPrevious = KTUtil.find(the.element, '[data-kt-stepper-action="previous"]');
        the.btnSubmit = KTUtil.find(the.element, '[data-kt-stepper-action="submit"]');

        // Variables
        the.totalStepsNumber = the.steps.length;
        the.passedStepIndex = 0;
        the.currentStepIndex = 1;
        the.clickedStepIndex = 0;

        // Set Current Step
        if ( the.options.startIndex > 1 ) {
            _goTo(the.options.startIndex);
        }

        // Event Handlers
        KTUtil.addEvent(the.btnNext, 'click', function(e) {
            e.preventDefault();

            KTEventHandler.trigger(the.element, 'kt.stepper.next', the);
        });

        KTUtil.addEvent(the.btnPrevious, 'click', function(e) {
            e.preventDefault();

            KTEventHandler.trigger(the.element, 'kt.stepper.previous', the);
        });

        KTUtil.on(the.element, '[data-kt-stepper-action="step"]', 'click', function(e) {
            e.preventDefault();

            if ( the.steps && the.steps.length > 0 ) {
                for (var i = 0, len = the.steps.length; i < len; i++) {
                    if ( the.steps[i] === this ) {
                        the.clickedStepIndex = i + 1;

                        KTEventHandler.trigger(the.element, 'kt.stepper.click', the);

                        return;
                    }
                }
            }
        });

        // Bind Instance
        KTUtil.data(the.element).set('stepper', the);
    }

    var _goTo = function(index) {
        // Trigger "change" event
        KTEventHandler.trigger(the.element, 'kt.stepper.change', the);

        // Skip if this step is already shown
        if ( index === the.currentStepIndex || index > the.totalStepsNumber || index < 0 ) {
            return;
        }

        // Validate step number
        index = parseInt(index);

        // Set current step
        the.passedStepIndex = the.currentStepIndex;
        the.currentStepIndex = index;

        // Refresh elements
        _refreshUI();

        // Trigger "changed" event
        KTEventHandler.trigger(the.element, 'kt.stepper.changed', the);

        return the;
    }

    var _goNext = function() {
        return _goTo( _getNextStepIndex() );
    }

    var _goPrevious = function() {
        return _goTo( _getPreviousStepIndex() );
    }

    var _goLast = function() {
        return _goTo( _getLastStepIndex() );
    }

    var _goFirst = function() {
        return _goTo( _getFirstStepIndex() );
    }

    var _refreshUI = function() {
        var state = '';

        if ( _isLastStep() ) {
            state = 'last';
        } else if ( _isFirstStep() ) {
            state = 'first';
        } else {
            state = 'between';
        }

        // Set state class
        KTUtil.removeClass(the.element, 'last');
        KTUtil.removeClass(the.element, 'first');
        KTUtil.removeClass(the.element, 'between');

        KTUtil.addClass(the.element, state);

        // Step Items
        var elements = KTUtil.findAll(the.element, '[data-kt-stepper-element="nav"], [data-kt-stepper-element="content"], [data-kt-stepper-element="info"]');

        if ( elements && elements.length > 0 ) {
            for (var i = 0, len = elements.length; i < len; i++) {
                var element = elements[i];
                var index = KTUtil.index(element) + 1;

                KTUtil.removeClass(element, 'current');
                KTUtil.removeClass(element, 'completed');
                KTUtil.removeClass(element, 'pending');

                if ( index == the.currentStepIndex ) {
                    KTUtil.addClass(element, 'current');

                    if ( the.options.animation !== false && element.getAttribute('data-kt-stepper-element') == 'content' ) {
                        KTUtil.css(element, 'animationDuration', the.options.animationSpeed);

                        var animation = _getStepDirection(the.passedStepIndex) === 'previous' ?  the.options.animationPreviousClass : the.options.animationNextClass;
                        KTUtil.animateClass(element, animation);
                    }
                } else {
                    if ( index < the.currentStepIndex ) {
                        KTUtil.addClass(element, 'completed');
                    } else {
                        KTUtil.addClass(element, 'pending');
                    }
                }
            }
        }
    }

    var _isLastStep = function() {
        return the.currentStepIndex === the.totalStepsNumber;
    }

    var _isFirstStep = function() {
        return the.currentStepIndex === 1;
    }

    var _isBetweenStep = function() {
        return _isLastStep() === false && _isFirstStep() === false;
    }

    var _getNextStepIndex = function() {
        if ( the.totalStepsNumber >= ( the.currentStepIndex + 1 ) ) {
            return the.currentStepIndex + 1;
        } else {
            return the.totalStepsNumber;
        }
    }

    var _getPreviousStepIndex = function() {
        if ( ( the.currentStepIndex - 1 ) > 1 ) {
            return the.currentStepIndex - 1;
        } else {
            return 1;
        }
    }

    var _getFirstStepIndex = function(){
        return 1;
    }

    var _getLastStepIndex = function() {
        return the.totalStepsNumber;
    }

    var _getTotalStepsNumber = function() {
        return the.totalStepsNumber;
    }

    var _getStepDirection = function(index) {
        if ( index > the.currentStepIndex ) {
            return 'next';
        } else {
            return 'previous';
        }
    }

    var _getStepContent = function(index) {
        var content = KTUtil.findAll(the.element, '[data-kt-stepper-element="content"]');

        if ( content[index-1] ) {
            return content[index-1];
        } else {
            return false;
        }
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('stepper');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.getElement = function(index) {
        return the.element;
    }

    the.goTo = function(index) {
        return _goTo(index);
    }

    the.goPrevious = function() {
        return _goPrevious();
    }

    the.goNext = function() {
        return _goNext();
    }

    the.goFirst = function() {
        return _goFirst();
    }

    the.goLast = function() {
        return _goLast();
    }

    the.getCurrentStepIndex = function() {
        return the.currentStepIndex;
    }

    the.getNextStepIndex = function() {
        return the.nextStepIndex;
    }

    the.getPassedStepIndex = function() {
        return the.passedStepIndex;
    }

    the.getClickedStepIndex = function() {
        return the.clickedStepIndex;
    }

    the.getPreviousStepIndex = function() {
        return the.PreviousStepIndex;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTStepper.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('stepper') ) {
        return KTUtil.data(element).get('stepper');
    } else {
        return null;
    }
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTStepper;
}

"use strict";

// Class definition
var KTSticky = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default Options
    var defaultOptions = {
        offset: 200,
        releaseOffset: 0,
        reverse: false,
        animation: true,
        animationSpeed: '0.3s',
        animationClass: 'animation-slide-in-down'
    };
    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('sticky') === true ) {
            the = KTUtil.data(element).get('sticky');
        } else {
            _init();
        }
    }

    var _init = function() {
        the.element = element;
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('sticky');
        the.name = the.element.getAttribute('data-kt-sticky-name');
        the.attributeName = 'data-kt-sticky-' + the.name;
        the.eventTriggerState = true;
        the.lastScrollTop = 0;
        the.scrollHandler;

        // Set initialized
        the.element.setAttribute('data-kt-sticky', 'true');

        // Event Handlers
        window.addEventListener('scroll', _scroll);

        // Initial Launch
        _scroll();

        // Bind Instance
        KTUtil.data(the.element).set('sticky', the);
    }

    var _scroll = function(e) {
        var offset = _getOption('offset');
        var releaseOffset = _getOption('release-offset');
        var reverse = _getOption('reverse');
        var st;
        var attrName;
        var diff;

        // Exit if false
        if ( offset === false ) {
            return;
        }

        offset = parseInt(offset);
        releaseOffset = releaseOffset ? parseInt(releaseOffset) : 0;
        st = KTUtil.getScrollTop();
        diff = document.documentElement.scrollHeight - window.innerHeight - KTUtil.getScrollTop();

        if ( reverse === true ) {  // Release on reverse scroll mode
            if ( st > offset && (releaseOffset === 0 || releaseOffset < diff)) {
                if ( body.hasAttribute(the.attributeName) === false) {
                    _enable();
                    body.setAttribute(the.attributeName, 'on');
                }

                if ( the.eventTriggerState === true ) {
                    KTEventHandler.trigger(the.element, 'kt.sticky.on', the);
                    KTEventHandler.trigger(the.element, 'kt.sticky.change', the);

                    the.eventTriggerState = false;
                }
            } else { // Back scroll mode
                if ( body.hasAttribute(the.attributeName) === true) {
                    _disable();
                    body.removeAttribute(the.attributeName);
                }

                if ( the.eventTriggerState === false ) {
                    KTEventHandler.trigger(the.element, 'kt.sticky.off', the);
                    KTEventHandler.trigger(the.element, 'kt.sticky.change', the);
                    the.eventTriggerState = true;
                }
            }

            the.lastScrollTop = st;
        } else { // Classic scroll mode
            if ( st > offset && (releaseOffset === 0 || releaseOffset < diff)) {
                if ( body.hasAttribute(the.attributeName) === false) {
                    _enable();
                    body.setAttribute(the.attributeName, 'on');
                }

                if ( the.eventTriggerState === true ) {
                    KTEventHandler.trigger(the.element, 'kt.sticky.on', the);
                    KTEventHandler.trigger(the.element, 'kt.sticky.change', the);
                    the.eventTriggerState = false;
                }
            } else { // back scroll mode
                if ( body.hasAttribute(the.attributeName) === true ) {
                    _disable();
                    body.removeAttribute(the.attributeName);
                }

                if ( the.eventTriggerState === false ) {
                    KTEventHandler.trigger(the.element, 'kt.sticky.off', the);
                    KTEventHandler.trigger(the.element, 'kt.sticky.change', the);
                    the.eventTriggerState = true;
                }
            }
        }

        if (releaseOffset > 0) {
            if ( diff < releaseOffset ) {
                the.element.setAttribute('data-kt-sticky-released', 'true');
            } else {
                the.element.removeAttribute('data-kt-sticky-released');
            }
        }        
    }

    var _enable = function(update) {
        var top = _getOption('top');
        var left = _getOption('left');
        var right = _getOption('right');
        var width = _getOption('width');
        var zindex = _getOption('zindex');

        if ( update !== true && _getOption('animation') === true ) {
            KTUtil.css(the.element, 'animationDuration', _getOption('animationSpeed'));
            KTUtil.animateClass(the.element, 'animation ' + _getOption('animationClass'));
        }

        if ( zindex !== null ) {
            KTUtil.css(the.element, 'z-index', zindex);
            KTUtil.css(the.element, 'position', 'fixed');
        }

        if ( top !== null ) {
            KTUtil.css(the.element, 'top', top);
        }

        if ( width !== null ) {
            if (width['target']) {
                var targetElement = document.querySelector(width['target']);
                if (targetElement) {
                    width = KTUtil.css(targetElement, 'width');
                }
            }

            KTUtil.css(the.element, 'width', width);
        }

        if ( left !== null ) {
            if ( String(left).toLowerCase() === 'auto' ) {
                var offsetLeft = KTUtil.offset(the.element).left;

                if ( offsetLeft > 0 ) {
                    KTUtil.css(the.element, 'left', String(offsetLeft) + 'px');
                }
            }
        }
    }

    var _disable = function() {
        KTUtil.css(the.element, 'top', '');
        KTUtil.css(the.element, 'width', '');
        KTUtil.css(the.element, 'left', '');
        KTUtil.css(the.element, 'right', '');
        KTUtil.css(the.element, 'z-index', '');
        KTUtil.css(the.element, 'position', '');
    }

    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-sticky-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-sticky-' + name);
            var value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }

            return value;
        } else {
            var optionName = KTUtil.snakeToCamel(name);

            if ( the.options[optionName] ) {
                return KTUtil.getResponsiveValue(the.options[optionName]);
            } else {
                return null;
            }
        }
    }

    var _destroy = function() {
        window.removeEventListener('scroll', _scroll);
        KTUtil.data(the.element).remove('sticky');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Methods
    the.update = function() {
        if ( body.hasAttribute(the.attributeName) === true ) {
            _disable();
            body.removeAttribute(the.attributeName);
            _enable(true);
            body.setAttribute(the.attributeName, 'on');
        }
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTSticky.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('sticky') ) {
        return KTUtil.data(element).get('sticky');
    } else {
        return null;
    }
}

// Create instances
KTSticky.createInstances = function(selector = '[data-kt-sticky="true"]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Initialize Menus
    var elements = body.querySelectorAll(selector);
    var sticky;

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            sticky = new KTSticky(elements[i]);
        }
    }
}

// Window resize handler
window.addEventListener('resize', function() {
    var timer;
    var body = document.getElementsByTagName("BODY")[0];

    KTUtil.throttle(timer, function() {
        // Locate and update Offcanvas instances on window resize
        var elements = body.querySelectorAll('[data-kt-sticky="true"]');

        if ( elements && elements.length > 0 ) {
            for (var i = 0, len = elements.length; i < len; i++) {
                var sticky = KTSticky.getInstance(elements[i]);
                if (sticky) {
                    sticky.update();
                }
            }
        }
    }, 200);
});

// Global initialization
KTSticky.init = function() {
    KTSticky.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTSticky.init);
} else {
    KTSticky.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTSticky;
}

"use strict";

// Class definition
var KTSwapper = function(element, options) {
    ////////////////////////////
    // ** Private Variables  ** //
    ////////////////////////////
    var the = this;

    if ( typeof element === "undefined" || element === null ) {
        return;
    }

    // Default Options
    var defaultOptions = {
        mode: 'append'
    };

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('swapper') === true ) {
            the = KTUtil.data(element).get('swapper');
        } else {
            _init();
        }
    }

    var _init = function() {
        the.element = element;
        the.options = KTUtil.deepExtend({}, defaultOptions, options);

        // Set initialized
        the.element.setAttribute('data-kt-swapper', 'true');

        // Initial update
        _update();

        // Bind Instance
        KTUtil.data(the.element).set('swapper', the);
    }

    var _update = function(e) {
        var parentSelector = _getOption('parent');

        var mode = _getOption('mode');
        var parentElement = parentSelector ? document.querySelector(parentSelector) : null;
       

        if (parentElement && element.parentNode !== parentElement) {
            if (mode === 'prepend') {
                parentElement.prepend(element);
            } else if (mode === 'append') {
                parentElement.append(element);
            }
        }
    }

    var _getOption = function(name) {
        if ( the.element.hasAttribute('data-kt-swapper-' + name) === true ) {
            var attr = the.element.getAttribute('data-kt-swapper-' + name);
            var value = KTUtil.getResponsiveValue(attr);

            if ( value !== null && String(value) === 'true' ) {
                value = true;
            } else if ( value !== null && String(value) === 'false' ) {
                value = false;
            }

            return value;
        } else {
            var optionName = KTUtil.snakeToCamel(name);

            if ( the.options[optionName] ) {
                return KTUtil.getResponsiveValue(the.options[optionName]);
            } else {
                return null;
            }
        }
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('swapper');
    }

    // Construct Class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Methods
    the.update = function() {
        _update();
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTSwapper.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('swapper') ) {
        return KTUtil.data(element).get('swapper');
    } else {
        return null;
    }
}

// Create instances
KTSwapper.createInstances = function(selector = '[data-kt-swapper="true"]') {
    // Initialize Menus
    var elements = document.querySelectorAll(selector);
    var swapper;

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            swapper = new KTSwapper(elements[i]);
        }
    }
}

// Window resize handler
window.addEventListener('resize', function() {
    var timer;

    KTUtil.throttle(timer, function() {
        // Locate and update Offcanvas instances on window resize
        var elements = document.querySelectorAll('[data-kt-swapper="true"]');

        if ( elements && elements.length > 0 ) {
            for (var i = 0, len = elements.length; i < len; i++) {
                var swapper = KTSwapper.getInstance(elements[i]);
                if (swapper) {
                    swapper.update();
                }                
            }
        }
    }, 200);
});

// Global initialization
KTSwapper.init = function() {
    KTSwapper.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTSwapper.init);
} else {
    KTSwapper.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTSwapper;
}

"use strict";

// Class definition
var KTToggle = function(element, options) {
    ////////////////////////////
    // ** Private variables  ** //
    ////////////////////////////
    var the = this;
    var body = document.getElementsByTagName("BODY")[0];

    if (!element) {
        return;
    }

    // Default Options
    var defaultOptions = {
        saveState: true
    };

    ////////////////////////////
    // ** Private methods  ** //
    ////////////////////////////

    var _construct = function() {
        if ( KTUtil.data(element).has('toggle') === true ) {
            the = KTUtil.data(element).get('toggle');
        } else {
            _init();
        }
    }

    var _init = function() {
        // Variables
        the.options = KTUtil.deepExtend({}, defaultOptions, options);
        the.uid = KTUtil.getUniqueId('toggle');

        // Elements
        the.element = element;

        the.target = document.querySelector(the.element.getAttribute('data-kt-toggle-target')) ? document.querySelector(the.element.getAttribute('data-kt-toggle-target')) : the.element;
        the.state = the.element.hasAttribute('data-kt-toggle-state') ? the.element.getAttribute('data-kt-toggle-state') : '';
        the.attribute = 'data-kt-' + the.element.getAttribute('data-kt-toggle-name');

        // Event Handlers
        _handlers();

        // Bind Instance
        KTUtil.data(the.element).set('toggle', the);
    }

    var _handlers = function() {
        KTUtil.addEvent(the.element, 'click', function(e) {
            e.preventDefault();

            _toggle();
        });
    }

    // Event handlers
    var _toggle = function() {
        // Trigger "after.toggle" event
        KTEventHandler.trigger(the.element, 'kt.toggle.change', the);

        if ( _isEnabled() ) {
            _disable();
        } else {
            _enable();
        }

        // Trigger "before.toggle" event
        KTEventHandler.trigger(the.element, 'kt.toggle.changed', the);

        return the;
    }

    var _enable = function() {
        if ( _isEnabled() === true ) {
            return;
        }

        KTEventHandler.trigger(the.element, 'kt.toggle.enable', the);

        the.target.setAttribute(the.attribute, 'on');

        if (the.state.length > 0) {
            the.element.classList.add(the.state);
        }        

        if ( typeof KTCookie !== 'undefined' && the.options.saveState === true ) {
            KTCookie.set(the.attribute, 'on');
        }

        KTEventHandler.trigger(the.element, 'kt.toggle.enabled', the);

        return the;
    }

    var _disable = function() {
        if ( _isEnabled() === false ) {
            return;
        }

        KTEventHandler.trigger(the.element, 'kt.toggle.disable', the);

        the.target.removeAttribute(the.attribute);

        if (the.state.length > 0) {
            the.element.classList.remove(the.state);
        } 

        if ( typeof KTCookie !== 'undefined' && the.options.saveState === true ) {
            KTCookie.remove(the.attribute);
        }

        KTEventHandler.trigger(the.element, 'kt.toggle.disabled', the);

        return the;
    }

    var _isEnabled = function() {
        return (String(the.target.getAttribute(the.attribute)).toLowerCase() === 'on');
    }

    var _destroy = function() {
        KTUtil.data(the.element).remove('toggle');
    }

    // Construct class
    _construct();

    ///////////////////////
    // ** Public API  ** //
    ///////////////////////

    // Plugin API
    the.toggle = function() {
        return _toggle();
    }

    the.enable = function() {
        return _enable();
    }

    the.disable = function() {
        return _disable();
    }

    the.isEnabled = function() {
        return _isEnabled();
    }

    the.goElement = function() {
        return the.element;
    }

    the.destroy = function() {
        return _destroy();
    }

    // Event API
    the.on = function(name, handler) {
        return KTEventHandler.on(the.element, name, handler);
    }

    the.one = function(name, handler) {
        return KTEventHandler.one(the.element, name, handler);
    }

    the.off = function(name) {
        return KTEventHandler.off(the.element, name);
    }

    the.trigger = function(name, event) {
        return KTEventHandler.trigger(the.element, name, event, the, event);
    }
};

// Static methods
KTToggle.getInstance = function(element) {
    if ( element !== null && KTUtil.data(element).has('toggle') ) {
        return KTUtil.data(element).get('toggle');
    } else {
        return null;
    }
}

// Create instances
KTToggle.createInstances = function(selector = '[data-kt-toggle]') {
    var body = document.getElementsByTagName("BODY")[0];

    // Get instances
    var elements = body.querySelectorAll(selector);

    if ( elements && elements.length > 0 ) {
        for (var i = 0, len = elements.length; i < len; i++) {
            // Initialize instances
            new KTToggle(elements[i]);
        }
    }
}

// Global initialization
KTToggle.init = function() {
    KTToggle.createInstances();
};

// On document ready
if (document.readyState === 'loading') {
   document.addEventListener('DOMContentLoaded', KTToggle.init);
} else {
    KTToggle.init();
}

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTToggle;
}
"use strict";

/**
 * @class KTUtil  base utilize class that privides helper functions
 */

// Polyfills

// Element.matches() polyfill
if (!Element.prototype.matches) {
    Element.prototype.matches = function(s) {
        var matches = (this.document || this.ownerDocument).querySelectorAll(s),
            i = matches.length;
        while (--i >= 0 && matches.item(i) !== this) {}
        return i > -1;
    };
}

/**
 * Element.closest() polyfill
 * https://developer.mozilla.org/en-US/docs/Web/API/Element/closest#Polyfill
 */
if (!Element.prototype.closest) {
	Element.prototype.closest = function (s) {
		var el = this;
		var ancestor = this;
		if (!document.documentElement.contains(el)) return null;
		do {
			if (ancestor.matches(s)) return ancestor;
			ancestor = ancestor.parentElement;
		} while (ancestor !== null);
		return null;
	};
}

/**
 * ChildNode.remove() polyfill
 * https://gomakethings.com/removing-an-element-from-the-dom-the-es6-way/
 * @author Chris Ferdinandi
 * @license MIT
 */
(function (elem) {
	for (var i = 0; i < elem.length; i++) {
		if (!window[elem[i]] || 'remove' in window[elem[i]].prototype) continue;
		window[elem[i]].prototype.remove = function () {
			this.parentNode.removeChild(this);
		};
	}
})(['Element', 'CharacterData', 'DocumentType']);


//
// requestAnimationFrame polyfill by Erik Möller.
//  With fixes from Paul Irish and Tino Zijdel
//
//  http://paulirish.com/2011/requestanimationframe-for-smart-animating/
//  http://my.opera.com/emoller/blog/2011/12/20/requestanimationframe-for-smart-er-animating
//
//  MIT license
//
(function() {
    var lastTime = 0;
    var vendors = ['webkit', 'moz'];
    for (var x = 0; x < vendors.length && !window.requestAnimationFrame; ++x) {
        window.requestAnimationFrame = window[vendors[x] + 'RequestAnimationFrame'];
        window.cancelAnimationFrame =
            window[vendors[x] + 'CancelAnimationFrame'] || window[vendors[x] + 'CancelRequestAnimationFrame'];
    }

    if (!window.requestAnimationFrame)
        window.requestAnimationFrame = function(callback) {
            var currTime = new Date().getTime();
            var timeToCall = Math.max(0, 16 - (currTime - lastTime));
            var id = window.setTimeout(function() {
                callback(currTime + timeToCall);
            }, timeToCall);
            lastTime = currTime + timeToCall;
            return id;
        };

    if (!window.cancelAnimationFrame)
        window.cancelAnimationFrame = function(id) {
            clearTimeout(id);
        };
}());

// Source: https://github.com/jserz/js_piece/blob/master/DOM/ParentNode/prepend()/prepend().md
(function(arr) {
    arr.forEach(function(item) {
        if (item.hasOwnProperty('prepend')) {
            return;
        }
        Object.defineProperty(item, 'prepend', {
            configurable: true,
            enumerable: true,
            writable: true,
            value: function prepend() {
                var argArr = Array.prototype.slice.call(arguments),
                    docFrag = document.createDocumentFragment();

                argArr.forEach(function(argItem) {
                    var isNode = argItem instanceof Node;
                    docFrag.appendChild(isNode ? argItem : document.createTextNode(String(argItem)));
                });

                this.insertBefore(docFrag, this.firstChild);
            }
        });
    });
})([Element.prototype, Document.prototype, DocumentFragment.prototype]);

// getAttributeNames
if (Element.prototype.getAttributeNames == undefined) {
  Element.prototype.getAttributeNames = function () {
    var attributes = this.attributes;
    var length = attributes.length;
    var result = new Array(length);
    for (var i = 0; i < length; i++) {
      result[i] = attributes[i].name;
    }
    return result;
  };
}

// Global variables
window.KTUtilElementDataStore = {};
window.KTUtilElementDataStoreID = 0;
window.KTUtilDelegatedEventHandlers = {};

var KTUtil = function() {
    var resizeHandlers = [];

    /**
     * Handle window resize event with some
     * delay to attach event handlers upon resize complete
     */
    var _windowResizeHandler = function() {
        var _runResizeHandlers = function() {
            // reinitialize other subscribed elements
            for (var i = 0; i < resizeHandlers.length; i++) {
                var each = resizeHandlers[i];
                each.call();
            }
        };

        var timer;

        window.addEventListener('resize', function() {
            KTUtil.throttle(timer, function() {
                _runResizeHandlers();
            }, 200);
        });
    };

    return {
        /**
         * Class main initializer.
         * @param {object} settings.
         * @returns null
         */
        //main function to initiate the theme
        init: function(settings) {
            _windowResizeHandler();
        },

        /**
         * Adds window resize event handler.
         * @param {function} callback function.
         */
        addResizeHandler: function(callback) {
            resizeHandlers.push(callback);
        },

        /**
         * Removes window resize event handler.
         * @param {function} callback function.
         */
        removeResizeHandler: function(callback) {
            for (var i = 0; i < resizeHandlers.length; i++) {
                if (callback === resizeHandlers[i]) {
                    delete resizeHandlers[i];
                }
            }
        },

        /**
         * Trigger window resize handlers.
         */
        runResizeHandlers: function() {
            _runResizeHandlers();
        },

        resize: function() {
            if (typeof(Event) === 'function') {
                // modern browsers
                window.dispatchEvent(new Event('resize'));
            } else {
                // for IE and other old browsers
                // causes deprecation warning on modern browsers
                var evt = window.document.createEvent('UIEvents');
                evt.initUIEvent('resize', true, false, window, 0);
                window.dispatchEvent(evt);
            }
        },

        /**
         * Get GET parameter value from URL.
         * @param {string} paramName Parameter name.
         * @returns {string}
         */
        getURLParam: function(paramName) {
            var searchString = window.location.search.substring(1),
                i, val, params = searchString.split("&");

            for (i = 0; i < params.length; i++) {
                val = params[i].split("=");
                if (val[0] == paramName) {
                    return unescape(val[1]);
                }
            }

            return null;
        },

        /**
         * Checks whether current device is mobile touch.
         * @returns {boolean}
         */
        isMobileDevice: function() {
            var test = (this.getViewPort().width < this.getBreakpoint('lg') ? true : false);

            if (test === false) {
                // For use within normal web clients
                test = navigator.userAgent.match(/iPad/i) != null;
            }

            return test;
        },

        /**
         * Checks whether current device is desktop.
         * @returns {boolean}
         */
        isDesktopDevice: function() {
            return KTUtil.isMobileDevice() ? false : true;
        },

        /**
         * Gets browser window viewport size. Ref:
         * http://andylangton.co.uk/articles/javascript/get-viewport-size-javascript/
         * @returns {object}
         */
        getViewPort: function() {
            var e = window,
                a = 'inner';
            if (!('innerWidth' in window)) {
                a = 'client';
                e = document.documentElement || document.body;
            }

            return {
                width: e[a + 'Width'],
                height: e[a + 'Height']
            };
        },

		/**
         * Checks whether given device mode is currently activated.
         * @param {string} mode Responsive mode name(e.g: desktop,
         *     desktop-and-tablet, tablet, tablet-and-mobile, mobile)
         * @returns {boolean}
         */
        isBreakpointUp: function(mode) {
            var width = this.getViewPort().width;
			var breakpoint = this.getBreakpoint(mode);

			return (width >= breakpoint);
        },

		isBreakpointDown: function(mode) {
            var width = this.getViewPort().width;
			var breakpoint = this.getBreakpoint(mode);

			return (width < breakpoint);
        },

        getViewportWidth: function() {
            return this.getViewPort().width;
        },

        /**
         * Generates unique ID for give prefix.
         * @param {string} prefix Prefix for generated ID
         * @returns {boolean}
         */
        getUniqueId: function(prefix) {
            return prefix + Math.floor(Math.random() * (new Date()).getTime());
        },

        /**
         * Gets window width for give breakpoint mode.
         * @param {string} mode Responsive mode name(e.g: xl, lg, md, sm)
         * @returns {number}
         */
        getBreakpoint: function(breakpoint) {
            var value = this.getCssVariableValue('--bs-' + breakpoint);

            if ( value ) {
                value = parseInt(value.trim());
            } 

            return value;
        },

        /**
         * Checks whether object has property matchs given key path.
         * @param {object} obj Object contains values paired with given key path
         * @param {string} keys Keys path seperated with dots
         * @returns {object}
         */
        isset: function(obj, keys) {
            var stone;

            keys = keys || '';

            if (keys.indexOf('[') !== -1) {
                throw new Error('Unsupported object path notation.');
            }

            keys = keys.split('.');

            do {
                if (obj === undefined) {
                    return false;
                }

                stone = keys.shift();

                if (!obj.hasOwnProperty(stone)) {
                    return false;
                }

                obj = obj[stone];

            } while (keys.length);

            return true;
        },

        /**
         * Gets highest z-index of the given element parents
         * @param {object} el jQuery element object
         * @returns {number}
         */
        getHighestZindex: function(el) {
            var position, value;

            while (el && el !== document) {
                // Ignore z-index if position is set to a value where z-index is ignored by the browser
                // This makes behavior of this function consistent across browsers
                // WebKit always returns auto if the element is positioned
                position = KTUtil.css(el, 'position');

                if (position === "absolute" || position === "relative" || position === "fixed") {
                    // IE returns 0 when zIndex is not specified
                    // other browsers return a string
                    // we ignore the case of nested elements with an explicit value of 0
                    // <div style="z-index: -10;"><div style="z-index: 0;"></div></div>
                    value = parseInt(KTUtil.css(el, 'z-index'));

                    if (!isNaN(value) && value !== 0) {
                        return value;
                    }
                }

                el = el.parentNode;
            }

            return 1;
        },

        /**
         * Checks whether the element has any parent with fixed positionfreg
         * @param {object} el jQuery element object
         * @returns {boolean}
         */
        hasFixedPositionedParent: function(el) {
            var position;

            while (el && el !== document) {
                position = KTUtil.css(el, 'position');

                if (position === "fixed") {
                    return true;
                }

                el = el.parentNode;
            }

            return false;
        },

        /**
         * Simulates delay
         */
        sleep: function(milliseconds) {
            var start = new Date().getTime();
            for (var i = 0; i < 1e7; i++) {
                if ((new Date().getTime() - start) > milliseconds) {
                    break;
                }
            }
        },

        /**
         * Gets randomly generated integer value within given min and max range
         * @param {number} min Range start value
         * @param {number} max Range end value
         * @returns {number}
         */
        getRandomInt: function(min, max) {
            return Math.floor(Math.random() * (max - min + 1)) + min;
        },

        /**
         * Checks whether Angular library is included
         * @returns {boolean}
         */
        isAngularVersion: function() {
            return window.Zone !== undefined ? true : false;
        },

        // Deep extend:  $.extend(true, {}, objA, objB);
        deepExtend: function(out) {
            out = out || {};

            for (var i = 1; i < arguments.length; i++) {
                var obj = arguments[i];
                if (!obj) continue;

                for (var key in obj) {
                    if (!obj.hasOwnProperty(key)) {
                        continue;
                    }

                    // based on https://javascriptweblog.wordpress.com/2011/08/08/fixing-the-javascript-typeof-operator/
                    if ( Object.prototype.toString.call(obj[key]) === '[object Object]' ) {
                        out[key] = KTUtil.deepExtend(out[key], obj[key]);
                        continue;
                    }

                    out[key] = obj[key];
                }
            }

            return out;
        },

        // extend:  $.extend({}, objA, objB);
        extend: function(out) {
            out = out || {};

            for (var i = 1; i < arguments.length; i++) {
                if (!arguments[i])
                    continue;

                for (var key in arguments[i]) {
                    if (arguments[i].hasOwnProperty(key))
                        out[key] = arguments[i][key];
                }
            }

            return out;
        },

        get: function (query) {
            var el;

            if (query === document) {
                return document;
            }

            if (!!(query && query.nodeType === 1)) {
                return query;
            }

            if (el = document.getElementById(query)) {
                return el;
            } else if (el = document.getElementsByTagName(query)) {
                return el[0];
            } else if (el = document.getElementsByClassName(query)) {
                return el[0];
            } else {
                return null;
            }
        },

        getBody: function() {
            return document.getElementsByTagName('body')[0];
        },

        /**
         * Checks whether the element has given classes
         * @param {object} el jQuery element object
         * @param {string} Classes string
         * @returns {boolean}
         */
        hasClasses: function(el, classes) {
            if (!el) {
                return;
            }

            var classesArr = classes.split(" ");

            for (var i = 0; i < classesArr.length; i++) {
                if (KTUtil.hasClass(el, KTUtil.trim(classesArr[i])) == false) {
                    return false;
                }
            }

            return true;
        },

        hasClass: function(el, className) {
            if (!el) {
                return;
            }

            return el.classList ? el.classList.contains(className) : new RegExp('\\b' + className + '\\b').test(el.className);
        },

        addClass: function(el, className) {
            if (!el || typeof className === 'undefined') {
                return;
            }

            var classNames = className.split(' ');

            if (el.classList) {
                for (var i = 0; i < classNames.length; i++) {
                    if (classNames[i] && classNames[i].length > 0) {
                        el.classList.add(KTUtil.trim(classNames[i]));
                    }
                }
            } else if (!KTUtil.hasClass(el, className)) {
                for (var x = 0; x < classNames.length; x++) {
                    el.className += ' ' + KTUtil.trim(classNames[x]);
                }
            }
        },

        removeClass: function(el, className) {
          if (!el || typeof className === 'undefined') {
                return;
            }

            var classNames = className.split(' ');

            if (el.classList) {
                for (var i = 0; i < classNames.length; i++) {
                    el.classList.remove(KTUtil.trim(classNames[i]));
                }
            } else if (KTUtil.hasClass(el, className)) {
                for (var x = 0; x < classNames.length; x++) {
                    el.className = el.className.replace(new RegExp('\\b' + KTUtil.trim(classNames[x]) + '\\b', 'g'), '');
                }
            }
        },

        triggerCustomEvent: function(el, eventName, data) {
            var event;
            if (window.CustomEvent) {
                event = new CustomEvent(eventName, {
                    detail: data
                });
            } else {
                event = document.createEvent('CustomEvent');
                event.initCustomEvent(eventName, true, true, data);
            }

            el.dispatchEvent(event);
        },

        triggerEvent: function(node, eventName) {
            // Make sure we use the ownerDocument from the provided node to avoid cross-window problems
            var doc;

            if (node.ownerDocument) {
                doc = node.ownerDocument;
            } else if (node.nodeType == 9) {
                // the node may be the document itself, nodeType 9 = DOCUMENT_NODE
                doc = node;
            } else {
                throw new Error("Invalid node passed to fireEvent: " + node.id);
            }

            if (node.dispatchEvent) {
                // Gecko-style approach (now the standard) takes more work
                var eventClass = "";

                // Different events have different event classes.
                // If this switch statement can't map an eventName to an eventClass,
                // the event firing is going to fail.
                switch (eventName) {
                case "click": // Dispatching of 'click' appears to not work correctly in Safari. Use 'mousedown' or 'mouseup' instead.
                case "mouseenter":
                case "mouseleave":
                case "mousedown":
                case "mouseup":
                    eventClass = "MouseEvents";
                    break;

                case "focus":
                case "change":
                case "blur":
                case "select":
                    eventClass = "HTMLEvents";
                    break;

                default:
                    throw "fireEvent: Couldn't find an event class for event '" + eventName + "'.";
                    break;
                }
                var event = doc.createEvent(eventClass);

                var bubbles = eventName == "change" ? false : true;
                event.initEvent(eventName, bubbles, true); // All events created as bubbling and cancelable.

                event.synthetic = true; // allow detection of synthetic events
                // The second parameter says go ahead with the default action
                node.dispatchEvent(event, true);
            } else if (node.fireEvent) {
                // IE-old school style
                var event = doc.createEventObject();
                event.synthetic = true; // allow detection of synthetic events
                node.fireEvent("on" + eventName, event);
            }
        },

        index: function( el ){
            var c = el.parentNode.children, i = 0;
            for(; i < c.length; i++ )
                if( c[i] == el ) return i;
        },

        trim: function(string) {
            return string.trim();
        },

        eventTriggered: function(e) {
            if (e.currentTarget.dataset.triggered) {
                return true;
            } else {
                e.currentTarget.dataset.triggered = true;

                return false;
            }
        },

        remove: function(el) {
            if (el && el.parentNode) {
                el.parentNode.removeChild(el);
            }
        },

        find: function(parent, query) {
            if ( parent !== null) {
                return parent.querySelector(query);
            } else {
                return null;
            }
        },

        findAll: function(parent, query) {
            if ( parent !== null ) {
                return parent.querySelectorAll(query);
            } else {
                return null;
            }
        },

        insertAfter: function(el, referenceNode) {
            return referenceNode.parentNode.insertBefore(el, referenceNode.nextSibling);
        },

        parents: function(elem, selector) {
            // Set up a parent array
            var parents = [];

            // Push each parent element to the array
            for ( ; elem && elem !== document; elem = elem.parentNode ) {
                if (selector) {
                    if (elem.matches(selector)) {
                        parents.push(elem);
                    }
                    continue;
                }
                parents.push(elem);
            }

            // Return our parent array
            return parents;
        },

        children: function(el, selector, log) {
            if (!el || !el.childNodes) {
                return null;
            }

            var result = [],
                i = 0,
                l = el.childNodes.length;

            for (var i; i < l; ++i) {
                if (el.childNodes[i].nodeType == 1 && KTUtil.matches(el.childNodes[i], selector, log)) {
                    result.push(el.childNodes[i]);
                }
            }

            return result;
        },

        child: function(el, selector, log) {
            var children = KTUtil.children(el, selector, log);

            return children ? children[0] : null;
        },

        matches: function(el, selector, log) {
            var p = Element.prototype;
            var f = p.matches || p.webkitMatchesSelector || p.mozMatchesSelector || p.msMatchesSelector || function(s) {
                return [].indexOf.call(document.querySelectorAll(s), this) !== -1;
            };

            if (el && el.tagName) {
                return f.call(el, selector);
            } else {
                return false;
            }
        },

        data: function(el) {
            return {
                set: function(name, data) {
                    if (!el) {
                        return;
                    }

                    if (el.customDataTag === undefined) {
                        window.KTUtilElementDataStoreID++;
                        el.customDataTag = window.KTUtilElementDataStoreID;
                    }

                    if (window.KTUtilElementDataStore[el.customDataTag] === undefined) {
                        window.KTUtilElementDataStore[el.customDataTag] = {};
                    }

                    window.KTUtilElementDataStore[el.customDataTag][name] = data;
                },

                get: function(name) {
                    if (!el) {
                        return;
                    }

                    if (el.customDataTag === undefined) {
                        return null;
                    }

                    return this.has(name) ? window.KTUtilElementDataStore[el.customDataTag][name] : null;
                },

                has: function(name) {
                    if (!el) {
                        return false;
                    }

                    if (el.customDataTag === undefined) {
                        return false;
                    }

                    return (window.KTUtilElementDataStore[el.customDataTag] && window.KTUtilElementDataStore[el.customDataTag][name]) ? true : false;
                },

                remove: function(name) {
                    if (el && this.has(name)) {
                        delete window.KTUtilElementDataStore[el.customDataTag][name];
                    }
                }
            };
        },

        outerWidth: function(el, margin) {
            var width;

            if (margin === true) {
                width = parseFloat(el.offsetWidth);
                width += parseFloat(KTUtil.css(el, 'margin-left')) + parseFloat(KTUtil.css(el, 'margin-right'));

                return parseFloat(width);
            } else {
                width = parseFloat(el.offsetWidth);

                return width;
            }
        },

        offset: function(el) {
            var rect, win;

            if ( !el ) {
                return;
            }

            // Return zeros for disconnected and hidden (display: none) elements (gh-2310)
            // Support: IE <=11 only
            // Running getBoundingClientRect on a
            // disconnected node in IE throws an error

            if ( !el.getClientRects().length ) {
                return { top: 0, left: 0 };
            }

            // Get document-relative position by adding viewport scroll to viewport-relative gBCR
            rect = el.getBoundingClientRect();
            win = el.ownerDocument.defaultView;

            return {
                top: rect.top + win.pageYOffset,
                left: rect.left + win.pageXOffset,
                right: window.innerWidth - (el.offsetLeft + el.offsetWidth)
            };
        },

        height: function(el) {
            return KTUtil.css(el, 'height');
        },

        outerHeight: function(el, withMargin) {
            var height = el.offsetHeight;
            var style;

            if (typeof withMargin !== 'undefined' && withMargin === true) {
                style = getComputedStyle(el);
                height += parseInt(style.marginTop) + parseInt(style.marginBottom);

                return height;
            } else {
                return height;
            }
        },

        visible: function(el) {
            return !(el.offsetWidth === 0 && el.offsetHeight === 0);
        },

        attr: function(el, name, value) {
            if (el == undefined) {
                return;
            }

            if (value !== undefined) {
                el.setAttribute(name, value);
            } else {
                return el.getAttribute(name);
            }
        },

        hasAttr: function(el, name) {
            if (el == undefined) {
                return;
            }

            return el.getAttribute(name) ? true : false;
        },

        removeAttr: function(el, name) {
            if (el == undefined) {
                return;
            }

            el.removeAttribute(name);
        },

        animate: function(from, to, duration, update, easing, done) {
            /**
             * TinyAnimate.easings
             *  Adapted from jQuery Easing
             */
            var easings = {};
            var easing;

            easings.linear = function(t, b, c, d) {
                return c * t / d + b;
            };

            easing = easings.linear;

            // Early bail out if called incorrectly
            if (typeof from !== 'number' ||
                typeof to !== 'number' ||
                typeof duration !== 'number' ||
                typeof update !== 'function') {
                return;
            }

            // Create mock done() function if necessary
            if (typeof done !== 'function') {
                done = function() {};
            }

            // Pick implementation (requestAnimationFrame | setTimeout)
            var rAF = window.requestAnimationFrame || function(callback) {
                window.setTimeout(callback, 1000 / 50);
            };

            // Animation loop
            var canceled = false;
            var change = to - from;

            function loop(timestamp) {
                var time = (timestamp || +new Date()) - start;

                if (time >= 0) {
                    update(easing(time, from, change, duration));
                }
                if (time >= 0 && time >= duration) {
                    update(to);
                    done();
                } else {
                    rAF(loop);
                }
            }

            update(from);

            // Start animation loop
            var start = window.performance && window.performance.now ? window.performance.now() : +new Date();

            rAF(loop);
        },

        actualCss: function(el, prop, cache) {
            var css = '';

            if (el instanceof HTMLElement === false) {
                return;
            }

            if (!el.getAttribute('kt-hidden-' + prop) || cache === false) {
                var value;

                // the element is hidden so:
                // making the el block so we can meassure its height but still be hidden
                css = el.style.cssText;
                el.style.cssText = 'position: absolute; visibility: hidden; display: block;';

                if (prop == 'width') {
                    value = el.offsetWidth;
                } else if (prop == 'height') {
                    value = el.offsetHeight;
                }

                el.style.cssText = css;

                // store it in cache
                el.setAttribute('kt-hidden-' + prop, value);

                return parseFloat(value);
            } else {
                // store it in cache
                return parseFloat(el.getAttribute('kt-hidden-' + prop));
            }
        },

        actualHeight: function(el, cache) {
            return KTUtil.actualCss(el, 'height', cache);
        },

        actualWidth: function(el, cache) {
            return KTUtil.actualCss(el, 'width', cache);
        },

        getScroll: function(element, method) {
            // The passed in `method` value should be 'Top' or 'Left'
            method = 'scroll' + method;
            return (element == window || element == document) ? (
                self[(method == 'scrollTop') ? 'pageYOffset' : 'pageXOffset'] ||
                (browserSupportsBoxModel && document.documentElement[method]) ||
                document.body[method]
            ) : element[method];
        },

        css: function(el, styleProp, value, important) {
            if (!el) {
                return;
            }

            if (value !== undefined) {
                if ( important === true ) {
                    el.style.setProperty(styleProp, value, 'important');
                } else {
                    el.style[styleProp] = value;
                }
            } else {
                var defaultView = (el.ownerDocument || document).defaultView;

                // W3C standard way:
                if (defaultView && defaultView.getComputedStyle) {
                    // sanitize property name to css notation
                    // (hyphen separated words eg. font-Size)
                    styleProp = styleProp.replace(/([A-Z])/g, "-$1").toLowerCase();

                    return defaultView.getComputedStyle(el, null).getPropertyValue(styleProp);
                } else if (el.currentStyle) { // IE
                    // sanitize property name to camelCase
                    styleProp = styleProp.replace(/\-(\w)/g, function(str, letter) {
                        return letter.toUpperCase();
                    });

                    value = el.currentStyle[styleProp];

                    // convert other units to pixels on IE
                    if (/^\d+(em|pt|%|ex)?$/i.test(value)) {
                        return (function(value) {
                            var oldLeft = el.style.left, oldRsLeft = el.runtimeStyle.left;

                            el.runtimeStyle.left = el.currentStyle.left;
                            el.style.left = value || 0;
                            value = el.style.pixelLeft + "px";
                            el.style.left = oldLeft;
                            el.runtimeStyle.left = oldRsLeft;

                            return value;
                        })(value);
                    }

                    return value;
                }
            }
        },

        slide: function(el, dir, speed, callback, recalcMaxHeight) {
            if (!el || (dir == 'up' && KTUtil.visible(el) === false) || (dir == 'down' && KTUtil.visible(el) === true)) {
                return;
            }

            speed = (speed ? speed : 600);
            var calcHeight = KTUtil.actualHeight(el);
            var calcPaddingTop = false;
            var calcPaddingBottom = false;

            if (KTUtil.css(el, 'padding-top') && KTUtil.data(el).has('slide-padding-top') !== true) {
                KTUtil.data(el).set('slide-padding-top', KTUtil.css(el, 'padding-top'));
            }

            if (KTUtil.css(el, 'padding-bottom') && KTUtil.data(el).has('slide-padding-bottom') !== true) {
                KTUtil.data(el).set('slide-padding-bottom', KTUtil.css(el, 'padding-bottom'));
            }

            if (KTUtil.data(el).has('slide-padding-top')) {
                calcPaddingTop = parseInt(KTUtil.data(el).get('slide-padding-top'));
            }

            if (KTUtil.data(el).has('slide-padding-bottom')) {
                calcPaddingBottom = parseInt(KTUtil.data(el).get('slide-padding-bottom'));
            }

            if (dir == 'up') { // up
                el.style.cssText = 'display: block; overflow: hidden;';

                if (calcPaddingTop) {
                    KTUtil.animate(0, calcPaddingTop, speed, function(value) {
                        el.style.paddingTop = (calcPaddingTop - value) + 'px';
                    }, 'linear');
                }

                if (calcPaddingBottom) {
                    KTUtil.animate(0, calcPaddingBottom, speed, function(value) {
                        el.style.paddingBottom = (calcPaddingBottom - value) + 'px';
                    }, 'linear');
                }

                KTUtil.animate(0, calcHeight, speed, function(value) {
                    el.style.height = (calcHeight - value) + 'px';
                }, 'linear', function() {
                    el.style.height = '';
                    el.style.display = 'none';

                    if (typeof callback === 'function') {
                        callback();
                    }
                });


            } else if (dir == 'down') { // down
                el.style.cssText = 'display: block; overflow: hidden;';

                if (calcPaddingTop) {
                    KTUtil.animate(0, calcPaddingTop, speed, function(value) {//
                        el.style.paddingTop = value + 'px';
                    }, 'linear', function() {
                        el.style.paddingTop = '';
                    });
                }

                if (calcPaddingBottom) {
                    KTUtil.animate(0, calcPaddingBottom, speed, function(value) {
                        el.style.paddingBottom = value + 'px';
                    }, 'linear', function() {
                        el.style.paddingBottom = '';
                    });
                }

                KTUtil.animate(0, calcHeight, speed, function(value) {
                    el.style.height = value + 'px';
                }, 'linear', function() {
                    el.style.height = '';
                    el.style.display = '';
                    el.style.overflow = '';

                    if (typeof callback === 'function') {
                        callback();
                    }
                });
            }
        },

        slideUp: function(el, speed, callback) {
            KTUtil.slide(el, 'up', speed, callback);
        },

        slideDown: function(el, speed, callback) {
            KTUtil.slide(el, 'down', speed, callback);
        },

        show: function(el, display) {
            if (typeof el !== 'undefined') {
                el.style.display = (display ? display : 'block');
            }
        },

        hide: function(el) {
            if (typeof el !== 'undefined') {
                el.style.display = 'none';
            }
        },

        addEvent: function(el, type, handler, one) {
            if (typeof el !== 'undefined' && el !== null) {
                el.addEventListener(type, handler);
            }
        },

        removeEvent: function(el, type, handler) {
            if (el !== null) {
                el.removeEventListener(type, handler);
            }
        },

        on: function(element, selector, event, handler) {
            if ( element === null ) {
                return;
            }

            var eventId = KTUtil.getUniqueId('event');

            window.KTUtilDelegatedEventHandlers[eventId] = function(e) {
                var targets = element.querySelectorAll(selector);
                var target = e.target;

                while ( target && target !== element ) {
                    for ( var i = 0, j = targets.length; i < j; i++ ) {
                        if ( target === targets[i] ) {
                            handler.call(target, e);
                        }
                    }

                    target = target.parentNode;
                }
            }

            KTUtil.addEvent(element, event, window.KTUtilDelegatedEventHandlers[eventId]);

            return eventId;
        },

        off: function(element, event, eventId) {
            if (!element || !window.KTUtilDelegatedEventHandlers[eventId]) {
                return;
            }

            KTUtil.removeEvent(element, event, window.KTUtilDelegatedEventHandlers[eventId]);

            delete window.KTUtilDelegatedEventHandlers[eventId];
        },

        one: function onetime(el, type, callback) {
            el.addEventListener(type, function callee(e) {
                // remove event
                if (e.target && e.target.removeEventListener) {
                    e.target.removeEventListener(e.type, callee);
                }

                // need to verify from https://themeforest.net/author_dashboard#comment_23615588
                if (el && el.removeEventListener) {
				    e.currentTarget.removeEventListener(e.type, callee);
			    }

                // call handler
                return callback(e);
            });
        },

        hash: function(str) {
            var hash = 0,
                i, chr;

            if (str.length === 0) return hash;
            for (i = 0; i < str.length; i++) {
                chr = str.charCodeAt(i);
                hash = ((hash << 5) - hash) + chr;
                hash |= 0; // Convert to 32bit integer
            }

            return hash;
        },

        animateClass: function(el, animationName, callback) {
            var animation;
            var animations = {
                animation: 'animationend',
                OAnimation: 'oAnimationEnd',
                MozAnimation: 'mozAnimationEnd',
                WebkitAnimation: 'webkitAnimationEnd',
                msAnimation: 'msAnimationEnd',
            };

            for (var t in animations) {
                if (el.style[t] !== undefined) {
                    animation = animations[t];
                }
            }
            
            KTUtil.addClass(el, animationName);

            KTUtil.one(el, animation, function() {
                KTUtil.removeClass(el, animationName);
            });

            if (callback) {
                KTUtil.one(el, animation, callback);
            }
        },

        transitionEnd: function(el, callback) {
            var transition;
            var transitions = {
                transition: 'transitionend',
                OTransition: 'oTransitionEnd',
                MozTransition: 'mozTransitionEnd',
                WebkitTransition: 'webkitTransitionEnd',
                msTransition: 'msTransitionEnd'
            };

            for (var t in transitions) {
                if (el.style[t] !== undefined) {
                    transition = transitions[t];
                }
            }

            KTUtil.one(el, transition, callback);
        },

        animationEnd: function(el, callback) {
            var animation;
            var animations = {
                animation: 'animationend',
                OAnimation: 'oAnimationEnd',
                MozAnimation: 'mozAnimationEnd',
                WebkitAnimation: 'webkitAnimationEnd',
                msAnimation: 'msAnimationEnd'
            };

            for (var t in animations) {
                if (el.style[t] !== undefined) {
                    animation = animations[t];
                }
            }

            KTUtil.one(el, animation, callback);
        },

        animateDelay: function(el, value) {
            var vendors = ['webkit-', 'moz-', 'ms-', 'o-', ''];
            for (var i = 0; i < vendors.length; i++) {
                KTUtil.css(el, vendors[i] + 'animation-delay', value);
            }
        },

        animateDuration: function(el, value) {
            var vendors = ['webkit-', 'moz-', 'ms-', 'o-', ''];
            for (var i = 0; i < vendors.length; i++) {
                KTUtil.css(el, vendors[i] + 'animation-duration', value);
            }
        },

        scrollTo: function(target, offset, duration) {
            var duration = duration ? duration : 500;
            var targetPos = target ? KTUtil.offset(target).top : 0;
            var scrollPos = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
            var from, to;

            if (offset) {
                targetPos = targetPos - offset;
            }

            from = scrollPos;
            to = targetPos;

            KTUtil.animate(from, to, duration, function(value) {
                document.documentElement.scrollTop = value;
                document.body.parentNode.scrollTop = value;
                document.body.scrollTop = value;
            }); //, easing, done
        },

        scrollTop: function(offset, duration) {
            KTUtil.scrollTo(null, offset, duration);
        },

        isArray: function(obj) {
            return obj && Array.isArray(obj);
        },

        isEmpty: function(obj) {
            for (var prop in obj) {
                if (obj.hasOwnProperty(prop)) {
                    return false;
                }
            }

            return true;
        },

        numberString: function(nStr) {
            nStr += '';
            var x = nStr.split('.');
            var x1 = x[0];
            var x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        },

        isRTL: function() {
            return (document.querySelector('html').getAttribute("direction") === 'rtl');
        },

        snakeToCamel: function(s){
            return s.replace(/(\-\w)/g, function(m){return m[1].toUpperCase();});
        },

        filterBoolean: function(val) {
            // Convert string boolean
			if (val === true || val === 'true') {
				return true;
			}

			if (val === false || val === 'false') {
				return false;
			}

            return val;
        },

        setHTML: function(el, html) {
            el.innerHTML = html;
        },

        getHTML: function(el) {
            if (el) {
                return el.innerHTML;
            }
        },

        getDocumentHeight: function() {
            var body = document.body;
            var html = document.documentElement;

            return Math.max( body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight );
        },

        getScrollTop: function() {
            return  (document.scrollingElement || document.documentElement).scrollTop;
        },

        colorLighten: function(color, amount) {
            const addLight = function(color, amount){
                let cc = parseInt(color,16) + amount;
                let c = (cc > 255) ? 255 : (cc);
                c = (c.toString(16).length > 1 ) ? c.toString(16) : `0${c.toString(16)}`;
                return c;
            }

            color = (color.indexOf("#")>=0) ? color.substring(1,color.length) : color;
            amount = parseInt((255*amount)/100);
            
            return color = `#${addLight(color.substring(0,2), amount)}${addLight(color.substring(2,4), amount)}${addLight(color.substring(4,6), amount)}`;
        },

        colorDarken: function(color, amount) {
            const subtractLight = function(color, amount){
                let cc = parseInt(color,16) - amount;
                let c = (cc < 0) ? 0 : (cc);
                c = (c.toString(16).length > 1 ) ? c.toString(16) : `0${c.toString(16)}`;

                return c;
            }
              
            color = (color.indexOf("#")>=0) ? color.substring(1,color.length) : color;
            amount = parseInt((255*amount)/100);

            return color = `#${subtractLight(color.substring(0,2), amount)}${subtractLight(color.substring(2,4), amount)}${subtractLight(color.substring(4,6), amount)}`;
        },

        // Throttle function: Input as function which needs to be throttled and delay is the time interval in milliseconds
        throttle:  function (timer, func, delay) {
        	// If setTimeout is already scheduled, no need to do anything
        	if (timer) {
        		return;
        	}

        	// Schedule a setTimeout after delay seconds
        	timer  =  setTimeout(function () {
        		func();

        		// Once setTimeout function execution is finished, timerId = undefined so that in <br>
        		// the next scroll event function execution can be scheduled by the setTimeout
        		timer  =  undefined;
        	}, delay);
        },

        // Debounce function: Input as function which needs to be debounced and delay is the debounced time in milliseconds
        debounce: function (timer, func, delay) {
        	// Cancels the setTimeout method execution
        	clearTimeout(timer)

        	// Executes the func after delay time.
        	timer  =  setTimeout(func, delay);
        },

        parseJson: function(value) {
            if (typeof value === 'string') {
                value = value.replace(/'/g, "\"");

                var jsonStr = value.replace(/(\w+:)|(\w+ :)/g, function(matched) {
                    return '"' + matched.substring(0, matched.length - 1) + '":';
                });

                try {
                    value = JSON.parse(jsonStr);
                } catch(e) { }
            }

            return value;
        },

        getResponsiveValue: function(value, defaultValue) {
            var width = this.getViewPort().width;
            var result;

            value = KTUtil.parseJson(value);

            if (typeof value === 'object') {
                var resultKey;
                var resultBreakpoint = -1;
                var breakpoint;

                for (var key in value) {
                    if (key === 'default') {
                        breakpoint = 0;
                    } else {
                        breakpoint = this.getBreakpoint(key) ? this.getBreakpoint(key) : parseInt(key);
                    }

                    if (breakpoint <= width && breakpoint > resultBreakpoint) {
                        resultKey = key;
                        resultBreakpoint = breakpoint;
                    }
                }

                if (resultKey) {
                    result = value[resultKey];
                } else {
                    result = value;
                }
            } else {
                result = value;
            }

            return result;
        },

        each: function(array, callback) {
            return [].slice.call(array).map(callback);
        },

        getSelectorMatchValue: function(value) {
            var result = null;
            value = KTUtil.parseJson(value);

            if ( typeof value === 'object' ) {
                // Match condition
                if ( value['match'] !== undefined ) {
                    var selector = Object.keys(value['match'])[0];
                    value = Object.values(value['match'])[0];

                    if ( document.querySelector(selector) !== null ) {
                        result = value;
                    }
                }
            } else {
                result = value;
            }

            return result;
        },

        getConditionalValue: function(value) {
            var value = KTUtil.parseJson(value);
            var result = KTUtil.getResponsiveValue(value);

            if ( result !== null && result['match'] !== undefined ) {
                result = KTUtil.getSelectorMatchValue(result);
            }

            if ( result === null && value !== null && value['default'] !== undefined ) {
                result = value['default'];
            }

            return result;
        },

        getCssVariableValue: function(variableName) {
            var hex = getComputedStyle(document.documentElement).getPropertyValue(variableName);
            if ( hex && hex.length > 0 ) {
                hex = hex.trim();
            }

            return hex;
        },

        isInViewport: function(element) {        
            var rect = element.getBoundingClientRect();

            return (
                rect.top >= 0 &&
                rect.left >= 0 &&
                rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
                rect.right <= (window.innerWidth || document.documentElement.clientWidth)
            );
        },

        onDOMContentLoaded: function(callback) {
            if (document.readyState === 'loading') {
                document.addEventListener('DOMContentLoaded', callback);
            } else {
                callback();
            }
        },

        inIframe: function() {
            try {
                return window.self !== window.top;
            } catch (e) {
                return true;
            }
        }
    }
}();

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTUtil;
}
"use strict";

// Class definition
var KTApp = function() {
    var initPageLoader =  function() {
        // CSS3 Transitions only after page load(.page-loading class added to body tag and remove with JS on page load)
        KTUtil.removeClass(document.body, 'page-loading');
    }

    var initBootstrapTooltip = function(el, options) {
        var delay = {};

        // Handle delay options
        if (el.hasAttribute('data-bs-delay-hide')) {
            delay['hide'] = el.getAttribute('data-bs-delay-hide');
        }

        if (el.hasAttribute('data-bs-delay-show')) {
            delay['show'] = el.getAttribute('data-bs-delay-show');
        }

        if (delay) {
            options['delay'] = delay;
        }

        // Check dismiss options
        if (el.hasAttribute('data-bs-dismiss') && el.getAttribute('data-bs-dismiss') == 'click') {
            options['dismiss'] = 'click';
        }            

        // Initialize popover
        var tp = new bootstrap.Tooltip(el, options);

        // Handle dismiss
        if (options['dismiss'] && options['dismiss'] === 'click') {
            // Hide popover on element click
            el.addEventListener("click", function(e) {
                tp.hide();
            });
        }

        return tp;
    }

    var initBootstrapTooltips = function(el, options) {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));

        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            initBootstrapTooltip(tooltipTriggerEl, {});
        });
    }

    var initBootstrapPopover = function(el, options) {
        var delay = {};

        // Handle delay options
        if (el.hasAttribute('data-bs-delay-hide')) {
            delay['hide'] = el.getAttribute('data-bs-delay-hide');
        }

        if (el.hasAttribute('data-bs-delay-show')) {
            delay['show'] = el.getAttribute('data-bs-delay-show');
        }

        if (delay) {
            options['delay'] = delay;
        }

        // Handle dismiss option
        if (el.getAttribute('data-bs-dismiss') == 'true') {
            options['dismiss'] = true;
        }

        if (options['dismiss'] === true) {
            options['template'] = '<div class="popover" role="tooltip"><div class="popover-arrow"></div><span class="popover-dismiss btn btn-icon"><i class="bi bi-x fs-2"></i></span><h3 class="popover-header"></h3><div class="popover-body"></div></div>'
        }

        // Initialize popover
        var popover = new bootstrap.Popover(el, options);

        // Handle dismiss click
        if (options['dismiss'] === true) {
            var dismissHandler = function (e) {
                popover.hide();
            }

            el.addEventListener('shown.bs.popover', function() {
                var dismissEl = document.getElementById(el.getAttribute('aria-describedby'));
                dismissEl.addEventListener('click', dismissHandler);
            });

            el.addEventListener('hide.bs.popover', function() {
                var dismissEl = document.getElementById(el.getAttribute('aria-describedby'));
                dismissEl.removeEventListener('click', dismissHandler);
            });
        }

        return popover;
    }

    var initBootstrapPopovers = function() {
        var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));

        var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
            initBootstrapPopover(popoverTriggerEl, {});
        });
    }

    var initScrollSpy = function() {
        var elements = [].slice.call(document.querySelectorAll('[data-bs-spy="scroll"]'));

        elements.map(function (element) {
            var sel = element.getAttribute('data-bs-target');            
            var scrollContent = document.querySelector(element.getAttribute('data-bs-target'));
            var scrollSpy = bootstrap.ScrollSpy.getInstance(scrollContent);
            if (scrollSpy) {
                scrollSpy.refresh();
            }
        });
    }

    var initButtons = function() {
        var buttonsGroup = [].slice.call(document.querySelectorAll('[data-kt-buttons="true"]'));

        buttonsGroup.map(function (group) {
            var selector = group.hasAttribute('data-kt-buttons-target') ? group.getAttribute('data-kt-buttons-target') : '.btn';

            // Toggle Handler
            KTUtil.on(group, selector, 'click', function(e) {
                var buttons = [].slice.call(group.querySelectorAll(selector + '.active'));

                buttons.map(function (button) {
                    button.classList.remove('active');
                });

                this.classList.add('active');
            });
        });
    }   

    var initCheck = function() {
        // Toggle Handler
        KTUtil.on(document.body,  '[data-kt-check="true"]', 'change', function(e) {
            var check = this;
            var targets = document.querySelectorAll(check.getAttribute('data-kt-check-target'));

            KTUtil.each(targets, function (target) {
                if (target.type == 'checkbox') {
                    target.checked = check.checked;
                } else {
                    target.classList.toggle('active');
                }                
            });
        });
    }

    var initSelect2 = function() {
        var elements = [].slice.call(document.querySelectorAll('[data-control="select2"], [data-kt-select2="true"]'));
       
        elements.map(function (element) {
            var options = {
                dir: document.body.getAttribute('direction')
            };

            if ( element.getAttribute('data-hide-search') == 'true') {
                options.minimumResultsForSearch = Infinity;
            }
            
            $(element).select2(options);
        });
    }

    var initAutosize = function() {
        var inputs = [].slice.call(document.querySelectorAll('[data-kt-autosize="true"]'));
       
        inputs.map(function (input) {
            autosize(input);
        });
    }

    var initCountUp = function() {
        var elements = [].slice.call(document.querySelectorAll('[data-kt-countup="true"]:not(.counted)'));

        elements.map(function (element) {
            if (KTUtil.isInViewport(element) && KTUtil.visible(element) ) {
                var options = {};

                var value = element.getAttribute('data-kt-countup-value');
                value = parseFloat(value.replace(/,/g,""));

                if (element.hasAttribute('data-kt-countup-start-val')) {
                    options.startVal = parseFloat(element.getAttribute('data-kt-countup-start-val'));
                }

                if (element.hasAttribute('data-kt-countup-duration')) {
                    options.duration = parseInt(element.getAttribute('data-kt-countup-duration'));
                }

                if (element.hasAttribute('data-kt-countup-decimal-places')) {
                    options.decimalPlaces = parseInt(element.getAttribute('data-kt-countup-decimal-places'));
                }

                if (element.hasAttribute('data-kt-countup-prefix')) {
                    options.prefix = element.getAttribute('data-kt-countup-prefix');
                }

                if (element.hasAttribute('data-kt-countup-suffix')) {
                    options.suffix = element.getAttribute('data-kt-countup-suffix');
                }

                var count = new countUp.CountUp(element, value, options);

                count.start();
                
                element.classList.add('counted');
            }                
        });
    }

    var initCountUpTabs = function() {
        // Initial call
        initCountUp();

        // Window scroll event handler
        window.addEventListener('scroll', initCountUp);

        // Tabs shown event handler
        var tabs = [].slice.call(document.querySelectorAll('[data-kt-countup-tabs="true"][data-bs-toggle="tab"]'));
        tabs.map(function (tab) {
            tab.addEventListener('shown.bs.tab', initCountUp);
        });        
    }

    var initTinySliders = function() {
        // Init Slider
        var initSlider = function(el) {
            if (!el) {
                return;
            }

            const tnsOptions = {};

            // Convert string boolean
            const checkBool = function(val) {
                if (val === 'true') {
                    return true;
                }
                if (val === 'false') {
                    return false;
                }
                return val;
            };

            // get extra options via data attributes
            el.getAttributeNames().forEach(function(attrName) {
                // more options; https://github.com/ganlanyuan/tiny-slider#options
                if ((/^data-tns-.*/g).test(attrName)) {
                    let optionName = attrName.replace('data-tns-', '').toLowerCase().replace(/(?:[\s-])\w/g, function(match) {
                        return match.replace('-', '').toUpperCase();
                    });
                    
                    if (attrName === 'data-tns-responsive') {
                        // fix string with a valid json
                        const jsonStr = el.getAttribute(attrName).replace(/(\w+:)|(\w+ :)/g, function(matched) {
                            return '"' + matched.substring(0, matched.length - 1) + '":';
                        });
                        try {
                            // convert json string to object
                            tnsOptions[optionName] = JSON.parse(jsonStr);
                        }
                        catch (e) {
                        }
                    }
                    else {
                        tnsOptions[optionName] = checkBool(el.getAttribute(attrName));
                    }
                }
            });

            const opt = Object.assign({}, {
                container: el,
                slideBy: 'page',
                autoplay: true,
                autoplayButtonOutput: false,
            }, tnsOptions);

            if (el.closest('.tns')) {
                KTUtil.addClass(el.closest('.tns'), 'tns-initiazlied');
            }

            return tns(opt);
        }

        // Sliders
        const elements = Array.prototype.slice.call(document.querySelectorAll('[data-tns="true"]'), 0);

        if (!elements && elements.length === 0) {
            return;
        }

        elements.forEach(function(el) {
            initSlider(el);
        });
    }

    var initSmoothScroll = function() {
        if (SmoothScroll) {

            new SmoothScroll('a[data-kt-scroll-toggle][href*="#"]', {
                speed: 900,
                offset: function (anchor, toggle) {
                    // Integer or Function returning an integer. How far to offset the scrolling anchor location in pixels
                    // This example is a function, but you could do something as simple as `offset: 25`

                    // An example returning different values based on whether the clicked link was in the header nav or not
                    if (anchor.hasAttribute('data-kt-scroll-offset')) {
                        var val = KTUtil.getResponsiveValue(anchor.getAttribute('data-kt-scroll-offset'));

                        return val;
                    } else {
                        return 0;
                    }
                }
            });
        }        
    }

    return {
        init: function() {
            this.initPageLoader();

            this.initBootstrapTooltips();
            
            this.initBootstrapPopovers();
            
            this.initScrollSpy();
            
            this.initButtons();
            
            this.initCheck();
            
            this.initSelect2();
            
            this.initCountUp();

            this.initCountUpTabs();

            this.initAutosize();

            this.initTinySliders();

            this.initSmoothScroll();
        },

        initPageLoader: function() {
            initPageLoader();
        },

        initBootstrapTooltip: function(el, options) {
            return initBootstrapTooltip(el, options);
        },

        initBootstrapTooltips: function() {
            initBootstrapTooltips();
        },

        initBootstrapPopovers: function() {
            initBootstrapPopovers();
        },

        initBootstrapPopover: function(el, options) {
            return initBootstrapPopover(el, options);
        },

        initScrollSpy: function() {
            initScrollSpy();
        },

        initButtons: function() {
            initButtons();
        },

        initCheck: function() {
            initCheck();
        },

        initSelect2: function() {
            initSelect2();
        },

        initCountUp: function() {
            initCountUp();
        },

        initCountUpTabs: function() {
            initCountUpTabs();
        },

        initAutosize: function() {
            initAutosize();
        },

        initTinySliders: function() {
            initTinySliders();
        },

        initSmoothScroll: function() {
            initSmoothScroll();
        },

        isDarkMode: function() {
            return document.body.classList.contains('dark-mode');
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function() {
    KTApp.init();
});

// On window load
window.addEventListener("load", function() {
	KTApp.initPageLoader();
});

// Webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTApp;
}
"use strict";

// Class definition
var KTLayoutAside = function () {
    // Private variables
    var toggle;
    var aside;

    // Private functions
    var handleToggle = function () {
       var toggleObj = KTToggle.getInstance(toggle);

       // Add a class to prevent aside hover effect after toggle click
       toggleObj.on('kt.toggle.change', function() {
           aside.classList.add('animating');

           setTimeout(function() {
                aside.classList.remove('animating');
           }, 300);
       })
    }

    // Public methods
    return {
        init: function () {
            // Elements
            aside = document.querySelector('#kt_aside');
            toggle = document.querySelector('#kt_aside_toggle');

            if (!aside || !toggle) {
                return;
            }

            handleToggle();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTLayoutAside.init();
});
"use strict";

// Class definition
var KTLayoutExplore = function() {
    // Private variables    
    var explore;

    // Private functions

    // Public methods
	return {
		init: function() {
            // Elements
            explore = document.querySelector('#kt_explore');

            if (!explore) {
                return;
            }      
		}
	};
}();

// On document ready
KTUtil.onDOMContentLoaded(function() {
    KTLayoutExplore.init();
});
"use strict";

// Class definition
var KTLayoutSearch = function() {
    // Private variables
    var element;
    var formElement;
    var mainElement;
    var resultsElement;
    var wrapperElement;
    var emptyElement;

    var preferencesElement;
    var preferencesShowElement;
    var preferencesDismissElement;
    
    var advancedOptionsFormElement;
    var advancedOptionsFormShowElement;
    var advancedOptionsFormCancelElement;
    var advancedOptionsFormSearchElement;
    
    var searchObject;

    // Private functions
    var processs = function(search) {
        var timeout = setTimeout(function() {
            var number = KTUtil.getRandomInt(1, 3);

            // Hide recently viewed
            mainElement.classList.add('d-none');

            if (number === 3) {
                // Hide results
                resultsElement.classList.add('d-none');
                // Show empty message 
                emptyElement.classList.remove('d-none');
            } else {
                // Show results
                resultsElement.classList.remove('d-none');
                // Hide empty message 
                emptyElement.classList.add('d-none');
            }                  

            // Complete search
            search.complete();
        }, 1500);
    }

    var clear = function(search) {
        // Show recently viewed
        mainElement.classList.remove('d-none');
        // Hide results
        resultsElement.classList.add('d-none');
        // Hide empty message 
        emptyElement.classList.add('d-none');
    }    

    var handlePreferences = function() {
        // Preference show handler
        preferencesShowElement.addEventListener('click', function() {
            wrapperElement.classList.add('d-none');
            preferencesElement.classList.remove('d-none');
        });

        // Preference dismiss handler
        preferencesDismissElement.addEventListener('click', function() {
            wrapperElement.classList.remove('d-none');
            preferencesElement.classList.add('d-none');
        });
    }

    var handleAdvancedOptionsForm = function() {
        // Show
        advancedOptionsFormShowElement.addEventListener('click', function() {
            wrapperElement.classList.add('d-none');
            advancedOptionsFormElement.classList.remove('d-none');
        });

        // Cancel
        advancedOptionsFormCancelElement.addEventListener('click', function() {
            wrapperElement.classList.remove('d-none');
            advancedOptionsFormElement.classList.add('d-none');
        });

        // Search
        advancedOptionsFormSearchElement.addEventListener('click', function() {
            
        });
    }

    // Public methods
	return {
		init: function() {
            // Elements
            element = document.querySelector('#kt_header_search');

            if (!element) {
                return;
            }

            wrapperElement = element.querySelector('[data-kt-search-element="wrapper"]');
            formElement = element.querySelector('[data-kt-search-element="form"]');
            mainElement = element.querySelector('[data-kt-search-element="main"]');
            resultsElement = element.querySelector('[data-kt-search-element="results"]');
            emptyElement = element.querySelector('[data-kt-search-element="empty"]');

            preferencesElement = element.querySelector('[data-kt-search-element="preferences"]');
            preferencesShowElement = element.querySelector('[data-kt-search-element="preferences-show"]');
            preferencesDismissElement = element.querySelector('[data-kt-search-element="preferences-dismiss"]');

            advancedOptionsFormElement = element.querySelector('[data-kt-search-element="advanced-options-form"]');
            advancedOptionsFormShowElement = element.querySelector('[data-kt-search-element="advanced-options-form-show"]');
            advancedOptionsFormCancelElement = element.querySelector('[data-kt-search-element="advanced-options-form-cancel"]');
            advancedOptionsFormSearchElement = element.querySelector('[data-kt-search-element="advanced-options-form-search"]');
            
            // Initialize search handler
            searchObject = new KTSearch(element);

            // Search handler
            searchObject.on('kt.search.process', processs);

            // Clear handler
            searchObject.on('kt.search.clear', clear);

            // Custom handlers
            handlePreferences();
            handleAdvancedOptionsForm();            
		}
	};
}();

// On document ready
KTUtil.onDOMContentLoaded(function() {
    KTLayoutSearch.init();
});
"use strict";

// Class definition
var KTLayoutToolbar = function () {
    // Private variables
    var toolbar;

    // Private functions
    var initForm = function () {
        var rangeSlider = document.querySelector("#kt_toolbar_slider");
        var rangeSliderValueElement = document.querySelector("#kt_toolbar_slider_value");

        if (!rangeSlider) {
            return;
        }

        noUiSlider.create(rangeSlider, {
            start: [5],
            connect: [true, false],
            step: 1,
            format: wNumb({
                decimals: 1
            }),
            range: {
                min: [1],
                max: [10]
            }
        });

        rangeSlider.noUiSlider.on("update", function (values, handle) {
            rangeSliderValueElement.innerHTML = values[handle];
        });

        var handle = rangeSlider.querySelector(".noUi-handle");

        handle.setAttribute("tabindex", 0);

        handle.addEventListener("click", function () {
            this.focus();
        });

        handle.addEventListener("keydown", function (event) {
            var value = Number(rangeSlider.noUiSlider.get());

            switch (event.which) {
                case 37:
                    rangeSlider.noUiSlider.set(value - 1);
                    break;
                case 39:
                    rangeSlider.noUiSlider.set(value + 1);
                    break;
            }
        });
    }

    // Public methods
    return {
        init: function () {
            // Elements
            toolbar = document.querySelector('#kt_toolbar');

            if (!toolbar) {
                return;
            }

            initForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTLayoutToolbar.init();
});
// plugin setup
var KTWizard = function(elementId, options) {
    // Main object
    var the = this;
    var init = false;

    // Get element object
    var element = KTUtil.get(elementId);
    var body = KTUtil.get('body');

    if (!element) {
        return; 
    }

    // Default options
    var defaultOptions = {
        startStep: 1
    };

    ////////////////////////////
    // ** Private Methods  ** //
    ////////////////////////////

    var Plugin = {
        /**
         * Construct
         */

        construct: function(options) {
            if (KTUtil.data(element).has('wizard')) {
                the = KTUtil.data(element).get('wizard');
            } else {
                // reset menu
                Plugin.init(options);

                // build menu
                Plugin.build();

                KTUtil.data(element).set('wizard', the);
            }

            return the;
        },

        /**
         * Init wizard
         */
        init: function(options) {
            the.element = element;
            the.events = [];

            // merge default and user defined options
            the.options = KTUtil.deepExtend({}, defaultOptions, options);

            // Elements
            the.steps = KTUtil.findAll(element, '[data-ktwizard-type="step"]');

            the.btnSubmit = KTUtil.find(element, '[data-ktwizard-type="action-submit"]');
            the.btnNext = KTUtil.find(element, '[data-ktwizard-type="action-next"]');
            the.btnPrev = KTUtil.find(element, '[data-ktwizard-type="action-prev"]');
            the.btnLast = KTUtil.find(element, '[data-ktwizard-type="action-last"]');
            the.btnFirst = KTUtil.find(element, '[data-ktwizard-type="action-first"]');

            // Variables
            the.events = [];
            the.currentStep = 1;
            the.stopped = false;
            the.totalSteps = the.steps.length;

            // Init current step
            if (the.options.startStep > 1) {
                Plugin.goTo(the.options.startStep);
            }

            // Init UI
            Plugin.updateUI();
        },

        /**
         * Build Form Wizard
         */
        build: function() {
            // Next button event handler
            KTUtil.addEvent(the.btnNext, 'click', function(e) {
                e.preventDefault();
                Plugin.goTo(Plugin.getNextStep(), true);
            });

            // Prev button event handler
            KTUtil.addEvent(the.btnPrev, 'click', function(e) {
                e.preventDefault();
                Plugin.goTo(Plugin.getPrevStep(), true);
            });

            // First button event handler
            KTUtil.addEvent(the.btnFirst, 'click', function(e) {
                e.preventDefault();
                Plugin.goTo(Plugin.getFirstStep(), true);
            });

            // Last button event handler
            KTUtil.addEvent(the.btnLast, 'click', function(e) {
                e.preventDefault();
                Plugin.goTo(Plugin.getLastStep(), true);
            });

            KTUtil.on(element, 'a[data-ktwizard-type="step"]', 'click', function() {
                var index = KTUtil.index(this) + 1;
                if (index !== the.currentStep) {
                    Plugin.goTo(index, true);
                }                
            });
        },

        /**
         * Handles wizard click wizard
         */
        goTo: function(number, eventHandle) {
            // Skip if this step is already shown
            if (number === the.currentStep || number > the.totalSteps || number < 0) {
                return;
            }

            // Validate step number
            if (number) {
                number = parseInt(number);
            } else {
                number = Plugin.getNextStep();
            }

            // Before next and prev events
            var callback;

            if (eventHandle === true) {
                if (number > the.currentStep) {
                    callback = Plugin.eventTrigger('beforeNext');
                } else {
                    callback = Plugin.eventTrigger('beforePrev');
                }
            }
            
            // Skip if stopped
            if (the.stopped === true) {
                the.stopped = false;
                return;
            }

            // Continue if no exit
            if (callback !== false) {
                // Before change
                if (eventHandle === true) {
                    Plugin.eventTrigger('beforeChange');
                }

                // Set current step 
                the.currentStep = number;

                Plugin.updateUI();

                // Trigger change event
                if (eventHandle === true) {
                    Plugin.eventTrigger('change');
                }
            }

            // After next and prev events
            if (eventHandle === true) {
                if (number > the.startStep) {
                    Plugin.eventTrigger('afterNext');
                } else {
                    Plugin.eventTrigger('afterPrev');
                }
            }

            return the;
        },

        /**
         * Cancel
         */
        stop: function() {
            the.stopped = true;
        },

        /**
         * Resume
         */
        start: function() {
            the.stopped = false;
        },

        /**
         * Check last step
         */
        isLastStep: function() {
            return the.currentStep === the.totalSteps;
        },

        /**
         * Check first step
         */
        isFirstStep: function() {
            return the.currentStep === 1;
        },

        /**
         * Check between step
         */
        isBetweenStep: function() {
            return Plugin.isLastStep() === false && Plugin.isFirstStep() === false;
        },

        /**
         * Go to the first step
         */
        updateUI: function() {
            var stepType = '';
            var index = the.currentStep - 1;

            if (Plugin.isLastStep()) {
                stepType = 'last';
            } else if (Plugin.isFirstStep()) {
                stepType = 'first';
            } else {
                stepType = 'between';
            }

            KTUtil.attr(the.element, 'data-ktwizard-state', stepType);

            // Steps
            var steps = KTUtil.findAll(the.element, '[data-ktwizard-type="step"]');

            if (steps && steps.length > 0) {
                for (var i = 0, len = steps.length; i < len; i++) {
                    if (i == index) {
                        KTUtil.attr(steps[i], 'data-ktwizard-state', 'current');
                    } else {
                        if (i < index) {
                            KTUtil.attr(steps[i], 'data-ktwizard-state', 'done');
                        } else {
                            KTUtil.attr(steps[i], 'data-ktwizard-state', 'pending');
                        }
                    }
                }
            }

            // Steps Info
            var stepsInfo = KTUtil.findAll(the.element, '[data-ktwizard-type="step-info"]');
            if (stepsInfo &&stepsInfo.length > 0) {
                for (var i = 0, len = stepsInfo.length; i < len; i++) {
                    if (i == index) {
                        KTUtil.attr(stepsInfo[i], 'data-ktwizard-state', 'current');
                    } else {
                        KTUtil.removeAttr(stepsInfo[i], 'data-ktwizard-state');
                    }
                }
            }  

            // Steps Content
            var stepsContent = KTUtil.findAll(the.element, '[data-ktwizard-type="step-content"]');
            if (stepsContent&& stepsContent.length > 0) {
                for (var i = 0, len = stepsContent.length; i < len; i++) {
                    if (i == index) {
                        KTUtil.attr(stepsContent[i], 'data-ktwizard-state', 'current');
                    } else {
                        KTUtil.removeAttr(stepsContent[i], 'data-ktwizard-state');
                    }
                }
            }            
        },

        /**
         * Get next step
         */
        getNextStep: function() {
            if (the.totalSteps >= (the.currentStep + 1)) {
                return the.currentStep + 1;
            } else {
                return the.totalSteps;
            }
        },

        /**
         * Get prev step
         */
        getPrevStep: function() {
            if ((the.currentStep - 1) >= 1) {
                return the.currentStep - 1;
            } else {
                return 1;
            }
        },

        /**
         * Trigger events
         */
        eventTrigger: function(name, nested) {
            //KTUtil.triggerCustomEvent(name);
            for (var i = 0; i < the.events.length; i++) {
                var event = the.events[i];
                if (event.name == name) {
                    if (event.one == true) {
                        if (event.fired == false) {
                            the.events[i].fired = true;
                            return event.handler.call(this, the);
                        }
                    } else {
                        return event.handler.call(this, the);
                    }
                }
            }
        },

        addEvent: function(name, handler, one) {
            the.events.push({
                name: name,
                handler: handler,
                one: one,
                fired: false
            });

            return the;
        }
    };

    //////////////////////////
    // ** Public Methods ** //
    //////////////////////////

    /**
     * Set default options 
     */

    the.setDefaults = function(options) {
        defaultOptions = options;
    };

    /**
     * Go to the next step 
     */
    the.goNext = function(eventHandle) {
        return Plugin.goTo(Plugin.getNextStep(), eventHandle);
    };

    /**
     * Go to the prev step 
     */
    the.goPrev = function(eventHandle) {
        return Plugin.goTo(Plugin.getPrevStep(),eventHandle);
    };

    /**
     * Go to the last step 
     */
    the.goLast = function(eventHandle) {
        return Plugin.goTo(Plugin.getLastStep(), eventHandle);
    };

    /**
     * Go to the first step 
     */
    the.goFirst = function(eventHandle) {
        return Plugin.goTo(Plugin.getFirstStep(), eventHandle);
    };

    /**
     * Go to a step
     */
    the.goTo = function(number, eventHandle) {
        return Plugin.goTo(number, eventHandle);
    };

    /**
     * Cancel step 
     */
    the.stop = function() {
        return Plugin.stop();
    };

    /**
     * Resume step 
     */
    the.start = function() {
        return Plugin.start();
    };

    /**
     * Get current step number 
     */
    the.getStep = function() {
        return the.currentStep;
    };

    /**
     * Check last step 
     */
    the.isLastStep = function() {
        return Plugin.isLastStep();
    };

    /**
     * Check first step 
     */
    the.isFirstStep = function() {
        return Plugin.isFirstStep();
    };
    
    /**
     * Attach event
     */
    the.on = function(name, handler) {
        return Plugin.addEvent(name, handler);
    };

    /**
     * Attach event that will be fired once
     */
    the.one = function(name, handler) {
        return Plugin.addEvent(name, handler, true);
    };

    // Construct plugin
    Plugin.construct.apply(the, [options]);

    return the;
};

// webpack support
if (typeof module !== 'undefined' && typeof module.exports !== 'undefined') {
    module.exports = KTWizard;
}
'use strict';
(function($) {

	var pluginName = 'KTDatatable';
	var pfx = 'kt-';
	var util = KTUtil;
	var app = KTApp;

	if (typeof util === 'undefined') throw new Error('Util class is required and must be included before ' + pluginName);

	// plugin setup
	$.fn[pluginName] = function(options) {
		if ($(this).length === 0) {
			console.warn('No ' + pluginName + ' element exist.');
			return;
		}

		// global variables
		var datatable = this;

		// debug enabled?
		// 1) state will be cleared on each refresh
		// 2) enable some logs
		// 3) etc.
		datatable.debug = false;

		datatable.API = {
			record: null,
			value: null,
			params: null,
		};

		var Plugin = {
			/********************
			 ** PRIVATE METHODS
			 ********************/
			isInit: false,
			cellOffset: 110,
			iconOffset: 15,
			stateId: 'meta',
			ajaxParams: {},
			pagingObject: {},

			init: function(options) {
				var isHtmlTable = false;
				// data source option empty is normal table
				if (options.data.source === null) {
					Plugin.extractTable();
					isHtmlTable = true;
				}

				Plugin.setupBaseDOM.call();
				Plugin.setupDOM(datatable.table);

				// set custom query from options
				Plugin.setDataSourceQuery(Plugin.getOption('data.source.read.params.query'));

				// on event after layout had done setup, show datatable
				$(datatable).on(pfx + 'datatable--on-layout-updated', Plugin.afterRender);

				if (datatable.debug) {
					Plugin.stateRemove(Plugin.stateId);
				}

				// initialize extensions
				$.each(Plugin.getOption('extensions'), function(extName, extOptions) {
					if (typeof $.fn[pluginName][extName] === 'function') {
						new $.fn[pluginName][extName](datatable, extOptions);
					}
				});

				Plugin.spinnerCallback(true);
				// get data
				if (options.data.type === 'remote' || options.data.type === 'local') {
					if (options.data.saveState === false
						|| options.data.saveState.cookie === false
						&& options.data.saveState.webstorage === false) {
						Plugin.stateRemove(Plugin.stateId);
					}
					// get data for local datatable and local table
					if (options.data.type === 'local' && typeof options.data.source === 'object') {
						datatable.dataSet = datatable.originalDataSet = Plugin.dataMapCallback(options.data.source);
					}
					Plugin.dataRender();
				}

                // if html table, remove and setup a new header
                if (isHtmlTable) {
                  $(datatable.tableHead).find('tr').remove();
                  $(datatable.tableFoot).find('tr').remove();
                }

                Plugin.setHeadTitle();
                if (Plugin.getOption('layout.footer')) {
                  Plugin.setHeadTitle(datatable.tableFoot);
                }

				// hide header
				if (typeof options.layout.header !== 'undefined' &&
					options.layout.header === false) {
					$(datatable.table).find('thead').remove();
				}

				// hide footer
				if (typeof options.layout.footer !== 'undefined' &&
					options.layout.footer === false) {
					$(datatable.table).find('tfoot').remove();
				}

				// for normal and local data type, run layoutUpdate
				if (options.data.type === null ||
					options.data.type === 'local') {
					Plugin.setupCellField.call();
					Plugin.setupTemplateCell.call();

					// setup nested datatable, if option enabled
					Plugin.setupSubDatatable.call();

					// setup extra system column properties
					Plugin.setupSystemColumn.call();
					Plugin.redraw();
				}

				var width;
				var initialWidth = false;
				$(window).resize(function() {
					// get initial width
					if (!initialWidth) {
						width = $(this).width();
						initialWidth = true;
					}
					// issue: URL Bar Resizing on mobile, https://developers.google.com/web/updates/2016/12/url-bar-resizing
					// trigger datatable resize on width change only
					if ($(this).width() !== width) {
						width = $(this).width();
						Plugin.fullRender();
					}
				});

				$(datatable).height('');

				$(Plugin.getOption('search.input')).on('keyup', function(e) {
					if (Plugin.getOption('search.onEnter') && e.which !== 13) return;
					Plugin.search($(this).val());
				});

				return datatable;
			},

			/**
			 * Extract static HTML table content into datasource
			 */
			extractTable: function() {
				var columns = [];
				var headers = $(datatable).find('tr:first-child th').get().map(function(cell, i) {
					var field = $(cell).data('field');
					if (typeof field === 'undefined') {
						field = $(cell).text().trim();
					}
					var column = {field: field, title: field};
					for (var ii in options.columns) {
						if (options.columns[ii].field === field) {
							column = $.extend(true, {}, options.columns[ii], column);
						}
					}
					columns.push(column);
					return field;
				});
				// auto create columns config
				options.columns = columns;

				var rowProp = [];
				var source = [];

				$(datatable).find('tr').each(function() {
					if ($(this).find('td').length) {
						rowProp.push($(this).prop('attributes'));
					}
					var td = {};
					$(this).find('td').each(function(i, cell) {
						td[headers[i]] = cell.innerHTML.trim();
					});
					if (!util.isEmpty(td)) {
						source.push(td);
					}
				});

				options.data.attr.rowProps = rowProp;
				options.data.source = source;
			},

			/**
			 * One time layout update on init
			 */
			layoutUpdate: function() {
				// setup nested datatable, if option enabled
				Plugin.setupSubDatatable.call();

				// setup extra system column properties
				Plugin.setupSystemColumn.call();

				// setup cell hover event
				Plugin.setupHover.call();

				if (typeof options.detail === 'undefined'
					// temporary disable lock column in subtable
					&& Plugin.getDepth() === 1) {
					// lock columns handler
					Plugin.lockTable.call();
				}

				Plugin.resetScroll();

				// check if not is a locked column
				if (!Plugin.isLocked()) {
					Plugin.redraw.call();
					// check if its not a subtable and has autoHide option enabled
					if (!Plugin.isSubtable() && Plugin.getOption('rows.autoHide') === true) {
						Plugin.autoHide();
					}
					// reset row
					$(datatable.table).find('.' + pfx + 'datatable__row').css('height', '');
				}

				Plugin.columnHide.call();

				Plugin.rowEvenOdd.call();

				Plugin.sorting.call();

				Plugin.scrollbar.call();

				if (!Plugin.isInit) {
					// run once dropdown inside datatable
					Plugin.dropdownFix();
					$(datatable).trigger(pfx + 'datatable--on-init', {table: $(datatable.wrap).attr('id'), options: options});
					Plugin.isInit = true;
				}

				$(datatable).trigger(pfx + 'datatable--on-layout-updated', {table: $(datatable.wrap).attr('id')});
			},

			dropdownFix: function() {
				var dropdownMenu;
				$('body').on('show.bs.dropdown', '.' + pfx + 'datatable .' + pfx + 'datatable__body', function(e) {
					dropdownMenu = $(e.target).find('.dropdown-menu');
					$('body').append(dropdownMenu.detach());
					dropdownMenu.css('display', 'block');
					dropdownMenu.position({
						'my': 'right top',
						'at': 'right bottom',
						'of': $(e.relatedTarget),
					});
					// if datatable is inside modal
					if (datatable.closest('.modal').length) {
						// increase dropdown z-index
						dropdownMenu.css('z-index', '2000');
					}
				}).on('hide.bs.dropdown', '.' + pfx + 'datatable .' + pfx + 'datatable__body', function(e) {
					$(e.target).append(dropdownMenu.detach());
					dropdownMenu.hide();
				});
			},

			lockTable: function() {
				var lock = {
					lockEnabled: false,
					init: function() {
						// check if table should be locked columns
						lock.lockEnabled = Plugin.lockEnabledColumns();
						if (lock.lockEnabled.left.length === 0 &&
							lock.lockEnabled.right.length === 0) {
							return;
						}
						lock.enable();
					},
					enable: function() {
						var enableLock = function(tablePart) {
							// check if already has lock column
							if ($(tablePart).find('.' + pfx + 'datatable__lock').length > 0) {
								Plugin.log('Locked container already exist in: ', tablePart);
								return;
							}
							// check if no rows exists
							if ($(tablePart).find('.' + pfx + 'datatable__row').length === 0) {
								Plugin.log('No row exist in: ', tablePart);
								return;
							}

							// locked div container
							var lockLeft = $('<div/>').addClass(pfx + 'datatable__lock ' + pfx + 'datatable__lock--left');
							var lockScroll = $('<div/>').addClass(pfx + 'datatable__lock ' + pfx + 'datatable__lock--scroll');
							var lockRight = $('<div/>').addClass(pfx + 'datatable__lock ' + pfx + 'datatable__lock--right');

							$(tablePart).find('.' + pfx + 'datatable__row').each(function() {
								// create new row for lock columns and pass the data
								var rowLeft = $('<tr/>').addClass(pfx + 'datatable__row').data('obj', $(this).data('obj')).appendTo(lockLeft);
								var rowScroll = $('<tr/>').addClass(pfx + 'datatable__row').data('obj', $(this).data('obj')).appendTo(lockScroll);
								var rowRight = $('<tr/>').addClass(pfx + 'datatable__row').data('obj', $(this).data('obj')).appendTo(lockRight);
								$(this).find('.' + pfx + 'datatable__cell').each(function() {
									var locked = $(this).data('locked');
									if (typeof locked !== 'undefined') {
										if (typeof locked.left !== 'undefined' || locked === true) {
											// default locked to left
											$(this).appendTo(rowLeft);
										}
										if (typeof locked.right !== 'undefined') {
											$(this).appendTo(rowRight);
										}
									} else {
										$(this).appendTo(rowScroll);
									}
								});
								// remove old row
								$(this).remove();
							});

							if (lock.lockEnabled.left.length > 0) {
								$(datatable.wrap).addClass(pfx + 'datatable--lock');
								$(lockLeft).appendTo(tablePart);
							}
							if (lock.lockEnabled.left.length > 0 || lock.lockEnabled.right.length > 0) {
								$(lockScroll).appendTo(tablePart);
							}
							if (lock.lockEnabled.right.length > 0) {
								$(datatable.wrap).addClass(pfx + 'datatable--lock');
								$(lockRight).appendTo(tablePart);
							}
						};

						$(datatable.table).find('thead,tbody,tfoot').each(function() {
							var tablePart = this;
							if ($(this).find('.' + pfx + 'datatable__lock').length === 0) {
								$(this).ready(function() {
									enableLock(tablePart);
								});
							}
						});
					},
				};
				lock.init();
				return lock;
			},

			/**
			 * Render everything for resize
			 */
			fullRender: function() {
				$(datatable.tableHead).empty();
				Plugin.setHeadTitle();
				if (Plugin.getOption('layout.footer')) {
					$(datatable.tableFoot).empty();
					Plugin.setHeadTitle(datatable.tableFoot);
				}

				Plugin.spinnerCallback(true);
				$(datatable.wrap).removeClass(pfx + 'datatable--loaded');

				Plugin.insertData();
			},

			lockEnabledColumns: function() {
				var screen = $(window).width();
				var columns = options.columns;
				var enabled = {left: [], right: []};
				$.each(columns, function(i, column) {
					if (typeof column.locked !== 'undefined') {
						if (typeof column.locked.left !== 'undefined') {
							if (util.getBreakpoint(column.locked.left) <= screen) {
								enabled['left'].push(column.locked.left);
							}
						}
						if (typeof column.locked.right !== 'undefined') {
							if (util.getBreakpoint(column.locked.right) <= screen) {
								enabled['right'].push(column.locked.right);
							}
						}
					}
				});
				return enabled;
			},

			/**
			 * After render event, called by
			 * '+pfx+'-datatable--on-layout-updated
			 * @param e
			 * @param args
			 */
			afterRender: function(e, args) {
				$(datatable).ready(function() {
					// redraw locked columns table
					if (Plugin.isLocked()) {
						Plugin.redraw();
					}

					$(datatable.tableBody).css('visibility', '');
					$(datatable.wrap).addClass(pfx + 'datatable--loaded');

					Plugin.spinnerCallback(false);
				});
			},

			hoverTimer: 0,
			isScrolling: false,
			setupHover: function() {
				$(window).scroll(function(e) {
					// stop hover when scrolling
					clearTimeout(Plugin.hoverTimer);
					Plugin.isScrolling = true;
				});

				$(datatable.tableBody).find('.' + pfx + 'datatable__cell').off('mouseenter', 'mouseleave').on('mouseenter', function() {
					// reset scroll timer to hover class
					Plugin.hoverTimer = setTimeout(function() {
						Plugin.isScrolling = false;
					}, 200);
					if (Plugin.isScrolling) return;

					// normal table
					var row = $(this).closest('.' + pfx + 'datatable__row').addClass(pfx + 'datatable__row--hover');
					var index = $(row).index() + 1;

					// lock table
					$(row).closest('.' + pfx + 'datatable__lock').parent().find('.' + pfx + 'datatable__row:nth-child(' + index + ')').addClass(pfx + 'datatable__row--hover');
				}).on('mouseleave', function() {
					// normal table
					var row = $(this).closest('.' + pfx + 'datatable__row').removeClass(pfx + 'datatable__row--hover');
					var index = $(row).index() + 1;

					// look table
					$(row).closest('.' + pfx + 'datatable__lock').parent().find('.' + pfx + 'datatable__row:nth-child(' + index + ')').removeClass(pfx + 'datatable__row--hover');
				});
			},

			/**
			 * Adjust width of locked table containers by resize handler
			 * @returns {number}
			 */
			adjustLockContainer: function() {
				if (!Plugin.isLocked()) return 0;

				// refer to head dimension
				var containerWidth = $(datatable.tableHead).width();
				var lockLeft = $(datatable.tableHead).find('.' + pfx + 'datatable__lock--left').width();
				var lockRight = $(datatable.tableHead).find('.' + pfx + 'datatable__lock--right').width();

				if (typeof lockLeft === 'undefined') lockLeft = 0;
				if (typeof lockRight === 'undefined') lockRight = 0;

				var lockScroll = Math.floor(containerWidth - lockLeft - lockRight);
				$(datatable.table).find('.' + pfx + 'datatable__lock--scroll').css('width', lockScroll);

				return lockScroll;
			},

			/**
			 * todo; not in use
			 */
			dragResize: function() {
				var pressed = false;
				var start = undefined;
				var startX, startWidth;
				$(datatable.tableHead).find('.' + pfx + 'datatable__cell').mousedown(function(e) {
					start = $(this);
					pressed = true;
					startX = e.pageX;
					startWidth = $(this).width();
					$(start).addClass(pfx + 'datatable__cell--resizing');

				}).mousemove(function(e) {
					if (pressed) {
						var i = $(start).index();
						var tableBody = $(datatable.tableBody);
						var ifLocked = $(start).closest('.' + pfx + 'datatable__lock');

						if (ifLocked) {
							var lockedIndex = $(ifLocked).index();
							tableBody = $(datatable.tableBody).find('.' + pfx + 'datatable__lock').eq(lockedIndex);
						}

						$(tableBody).find('.' + pfx + 'datatable__row').each(function(tri, tr) {
							$(tr).find('.' + pfx + 'datatable__cell').eq(i).width(startWidth + (e.pageX - startX)).children().width(startWidth + (e.pageX - startX));
						});

						$(start).children().css('width', startWidth + (e.pageX - startX));
					}

				}).mouseup(function() {
					$(start).removeClass(pfx + 'datatable__cell--resizing');
					pressed = false;
				});

				$(document).mouseup(function() {
					$(start).removeClass(pfx + 'datatable__cell--resizing');
					pressed = false;
				});
			},

			/**
			 * To prepare placeholder for table before content is loading
			 */
			initHeight: function() {
				if (options.layout.height && options.layout.scroll) {
					var theadHeight = $(datatable.tableHead).find('.' + pfx + 'datatable__row').outerHeight();
					var tfootHeight = $(datatable.tableFoot).find('.' + pfx + 'datatable__row').outerHeight();
					var bodyHeight = options.layout.height;
					if (theadHeight > 0) {
						bodyHeight -= theadHeight;
					}
					if (tfootHeight > 0) {
						bodyHeight -= tfootHeight;
					}

					// scrollbar offset
					bodyHeight -= 2;

					$(datatable.tableBody).css('max-height', bodyHeight);

					// set scrollable area fixed height
					$(datatable.tableBody).find('.' + pfx + 'datatable__lock--scroll').css('height', bodyHeight);
				}
			},

			/**
			 * Setup base DOM (table, thead, tbody, tfoot) and create if not
			 * exist.
			 */
			setupBaseDOM: function() {
				// keep original state before datatable initialize
				datatable.initialDatatable = $(datatable).clone();

				// main element
				if ($(datatable).prop('tagName') === 'TABLE') {
					// if main init element is <table>, wrap with div
					datatable.table = $(datatable).removeClass(pfx + 'datatable').addClass(pfx + 'datatable__table');
					if ($(datatable.table).parents('.' + pfx + 'datatable').length === 0) {
						datatable.table.wrap($('<div/>').addClass(pfx + 'datatable').addClass(pfx + 'datatable--' + options.layout.theme));
						datatable.wrap = $(datatable.table).parent();
					}
				} else {
					// create table
					datatable.wrap = $(datatable).addClass(pfx + 'datatable').addClass(pfx + 'datatable--' + options.layout.theme);
					datatable.table = $('<table/>').addClass(pfx + 'datatable__table').appendTo(datatable);
				}

				if (typeof options.layout.class !== 'undefined') {
					$(datatable.wrap).addClass(options.layout.class);
				}

				$(datatable.table).removeClass(pfx + 'datatable--destroyed').css('display', 'block');

				// force disable save state
				if (typeof $(datatable).attr('id') === 'undefined') {
					Plugin.setOption('data.saveState', false);
					$(datatable.table).attr('id', util.getUniqueID(pfx + 'datatable--'));
				}

				// predefine table height
				if (Plugin.getOption('layout.minHeight'))
					$(datatable.table).css('min-height', Plugin.getOption('layout.minHeight'));

				if (Plugin.getOption('layout.height'))
					$(datatable.table).css('max-height', Plugin.getOption('layout.height'));

				// for normal table load
				if (options.data.type === null) {
					$(datatable.table).css('width', '').css('display', '');
				}

				// create table head element
				datatable.tableHead = $(datatable.table).find('thead');
				if ($(datatable.tableHead).length === 0) {
					datatable.tableHead = $('<thead/>').prependTo(datatable.table);
				}

				// create table head element
				datatable.tableBody = $(datatable.table).find('tbody');
				if ($(datatable.tableBody).length === 0) {
					datatable.tableBody = $('<tbody/>').appendTo(datatable.table);
				}

				if (typeof options.layout.footer !== 'undefined' &&
					options.layout.footer) {
					// create table foot element
					datatable.tableFoot = $(datatable.table).find('tfoot');
					if ($(datatable.tableFoot).length === 0) {
						datatable.tableFoot = $('<tfoot/>').appendTo(datatable.table);
					}
				}
			},

			/**
			 * Set column data before table manipulation.
			 */
			setupCellField: function(tableParts) {
				if (typeof tableParts === 'undefined') tableParts = $(datatable.table).children();
				var columns = options.columns;
				$.each(tableParts, function(part, tablePart) {
					$(tablePart).find('.' + pfx + 'datatable__row').each(function(tri, tr) {
						// prepare data
						$(tr).find('.' + pfx + 'datatable__cell').each(function(tdi, td) {
							if (typeof columns[tdi] !== 'undefined') {
								$(td).data(columns[tdi]);
							}
						});
					});
				});
			},

			/**
			 * Set column template callback
			 * @param tablePart
			 */
			setupTemplateCell: function(tablePart) {
				if (typeof tablePart === 'undefined') tablePart = datatable.tableBody;
				var columns = options.columns;
				$(tablePart).find('.' + pfx + 'datatable__row').each(function(tri, tr) {
					// row data object, if any
					var obj = $(tr).data('obj');
					if (typeof obj === 'undefined') {
						return;
					}

					// @deprecated in v5.0.6
					// obj['getIndex'] = function() {
					// 	return tri;
					// };
					// @deprecated in v5.0.6
					// obj['getDatatable'] = function() {
					// 	return datatable;
					// };

					// @deprecated in v5.0.6
					var rowCallback = Plugin.getOption('rows.callback');
					if (typeof rowCallback === 'function') {
						rowCallback($(tr), obj, tri);
					}
					// before template row callback
					var beforeTemplate = Plugin.getOption('rows.beforeTemplate');
					if (typeof beforeTemplate === 'function') {
						beforeTemplate($(tr), obj, tri);
					}
					// if data object is undefined, collect from table
					if (typeof obj === 'undefined') {
						obj = {};
						$(tr).find('.' + pfx + 'datatable__cell').each(function(tdi, td) {
							// get column settings by field
							var column = $.grep(columns, function(n, i) {
								return $(td).data('field') === n.field;
							})[0];
							if (typeof column !== 'undefined') {
								obj[column['field']] = $(td).text();
							}
						});
					}

					$(tr).find('.' + pfx + 'datatable__cell').each(function(tdi, td) {
						// get column settings by field
						var column = $.grep(columns, function(n, i) {
							return $(td).data('field') === n.field;
						})[0];
						if (typeof column !== 'undefined') {
							// column template
							if (typeof column.template !== 'undefined') {
								var finalValue = '';
								// template string
								if (typeof column.template === 'string') {
									finalValue = Plugin.dataPlaceholder(column.template, obj);
								}
								// template callback function
								if (typeof column.template === 'function') {
									finalValue = column.template(obj, tri, datatable);
								}

								// sanitize using DOMPurify if installed
								if (typeof DOMPurify !== 'undefined') {
									finalValue = DOMPurify.sanitize(finalValue);
								}

								var span = document.createElement('span');
								span.innerHTML = finalValue;

								// insert to cell, wrap with span
								$(td).html(span);

								// set span overflow
								if (typeof column.overflow !== 'undefined') {
									$(span).css('overflow', column.overflow);
									$(span).css('position', 'relative');
								}
							}
						}
					});

					// after template row callback
					var afterTemplate = Plugin.getOption('rows.afterTemplate');
					if (typeof afterTemplate === 'function') {
						afterTemplate($(tr), obj, tri);
					}
				});
			},

			/**
			 * Setup extra system column properties
			 * Note: selector checkbox, subtable toggle
			 */
			setupSystemColumn: function() {
				datatable.dataSet = datatable.dataSet || [];
				// no records available
				if (datatable.dataSet.length === 0) return;

				var columns = options.columns;
				$(datatable.tableBody).find('.' + pfx + 'datatable__row').each(function(tri, tr) {
					$(tr).find('.' + pfx + 'datatable__cell').each(function(tdi, td) {
						// get column settings by field
						var column = $.grep(columns, function(n, i) {
							return $(td).data('field') === n.field;
						})[0];
						if (typeof column !== 'undefined') {
							var value = $(td).text();

							// enable column selector
							if (typeof column.selector !== 'undefined' && column.selector !== false) {
								// check if checkbox exist
								if ($(td).find('.' + pfx + 'checkbox [type="checkbox"]').length > 0) return;

								$(td).addClass(pfx + 'datatable__cell--check');

								// append checkbox
								var chk = $('<label/>').
									addClass(pfx + 'checkbox ' + pfx + 'checkbox--single').
									append($('<input/>').attr('type', 'checkbox').attr('value', value).on('click', function() {
										if ($(this).is(':checked')) {
											// add checkbox active row class
											Plugin.setActive(this);
										} else {
											// add checkbox active row class
											Plugin.setInactive(this);
										}
									})).
									append('&nbsp;<span></span>');

								// checkbox selector has outline style
								if (typeof column.selector.class !== 'undefined') {
									$(chk).addClass(column.selector.class);
								}

								$(td).children().html(chk);
							}

							// enable column subtable toggle
							if (typeof column.subtable !== 'undefined' && column.subtable) {
								// check if subtable toggle exist
								if ($(td).find('.' + pfx + 'datatable__toggle-subtable').length > 0) return;
								// append subtable toggle
								$(td).
									children().
									html($('<a/>').
										addClass(pfx + 'datatable__toggle-subtable').
										attr('href', '#').
										attr('data-value', value).
										append($('<i/>').addClass(Plugin.getOption('layout.icons.rowDetail.collapse'))));
							}
						}
					});
				});

				// init checkbox for header/footer
				var initCheckbox = function(tr) {
					// get column settings by field
					var column = $.grep(columns, function(n, i) {
						return typeof n.selector !== 'undefined' && n.selector !== false;
					})[0];

					if (typeof column !== 'undefined') {
						// enable column selector
						if (typeof column.selector !== 'undefined' && column.selector !== false) {
							var td = $(tr).find('[data-field="' + column.field + '"]');
							// check if checkbox exist
							if ($(td).find('.' + pfx + 'checkbox [type="checkbox"]').length > 0) return;

							$(td).addClass(pfx + 'datatable__cell--check');

							// append checkbox
							var chk = $('<label/>').
								addClass(pfx + 'checkbox ' + pfx + 'checkbox--single ' + pfx + 'checkbox--all').
								append($('<input/>').attr('type', 'checkbox').on('click', function() {
									if ($(this).is(':checked')) {
										Plugin.setActiveAll(true);
									} else {
										Plugin.setActiveAll(false);
									}
								})).
								append('&nbsp;<span></span>');

							// checkbox selector has outline style
							if (typeof column.selector.class !== 'undefined') {
								$(chk).addClass(column.selector.class);
							}

							$(td).children().html(chk);
						}
					}
				};

				if (options.layout.header) {
					initCheckbox($(datatable.tableHead).find('.' + pfx + 'datatable__row').first());
				}
				if (options.layout.footer) {
					initCheckbox($(datatable.tableFoot).find('.' + pfx + 'datatable__row').first());
				}
			},

            maxWidthList: {},

			/**
			 * Adjust width to match container size
			 */
			adjustCellsWidth: function() {
				// get table width
				var containerWidth = $(datatable.tableBody).innerWidth() - Plugin.iconOffset;

				// get total number of columns
				var columns = $(datatable.tableBody).
					find('.' + pfx + 'datatable__row:first-child').
					find('.' + pfx + 'datatable__cell').
					// exclude expand icon
					not('.' + pfx + 'datatable__toggle-detail').
					not(':hidden').length;

				if (columns > 0) {
					//  remove reserved sort icon width
					containerWidth = containerWidth - (Plugin.iconOffset * columns);
					var minWidth = Math.floor(containerWidth / columns);

					// minimum width
					if (minWidth <= Plugin.cellOffset) {
						minWidth = Plugin.cellOffset;
					}

					$(datatable.table).find('.' + pfx + 'datatable__row').
							find('.' + pfx + 'datatable__cell').
							// exclude expand icon
							not('.' + pfx + 'datatable__toggle-detail').
							not(':hidden').each(function(tdi, td) {

						var width = minWidth;
						var dataWidth = $(td).data('width');

						if (typeof dataWidth !== 'undefined') {
							if (dataWidth === 'auto') {
								var field = $(td).data('field');
								if (Plugin.maxWidthList[field]) {
									width = Plugin.maxWidthList[field];
								}
								else {
									var cells = $(datatable.table).find('.' + pfx + 'datatable__cell[data-field="' + field + '"]');
									width = Plugin.maxWidthList[field] = Math.max.apply(null,
											$(cells).map(function() {
												return $(this).outerWidth();
											}).get());
								}
							}
							else {
								width = dataWidth;
							}
						}
						$(td).children().css('width', Math.ceil(width));
					});
				}

				return datatable;
			},

			/**
			 * Adjust height to match container size
			 */
			adjustCellsHeight: function() {
				$.each($(datatable.table).children(), function(part, tablePart) {
					var totalRows = $(tablePart).find('.' + pfx + 'datatable__row').first().parent().find('.' + pfx + 'datatable__row').length;
					for (var i = 1; i <= totalRows; i++) {
						var rows = $(tablePart).find('.' + pfx + 'datatable__row:nth-child(' + i + ')');
						if ($(rows).length > 0) {
							var maxHeight = Math.max.apply(null, $(rows).map(function() {
								return $(this).outerHeight();
							}).get());
							$(rows).css('height', Math.ceil(maxHeight));
						}
					}
				});
			},

			/**
			 * Setup table DOM and classes
			 */
			setupDOM: function(table) {
				// set table classes
				$(table).find('> thead').addClass(pfx + 'datatable__head');
				$(table).find('> tbody').addClass(pfx + 'datatable__body');
				$(table).find('> tfoot').addClass(pfx + 'datatable__foot');
				$(table).find('tr').addClass(pfx + 'datatable__row');
				$(table).find('tr > th, tr > td').addClass(pfx + 'datatable__cell');
				$(table).find('tr > th, tr > td').each(function(i, td) {
					if ($(td).find('span').length === 0) {
						$(td).wrapInner($('<span/>').css('width', Plugin.cellOffset));
					}
				});
			},

			/**
			 * Default scrollbar
			 * @returns {{tableLocked: null, init: init, onScrolling:
			 *     onScrolling}}
			 */
			scrollbar: function() {
				var scroll = {
					scrollable: null,
					tableLocked: null,
					initPosition: null,
					init: function() {
						var screen = util.getViewPort().width;
						// setup scrollable datatable
						if (options.layout.scroll) {
							// add scrollable datatable class
							$(datatable.wrap).addClass(pfx + 'datatable--scroll');

							var scrollable = $(datatable.tableBody).find('.' + pfx + 'datatable__lock--scroll');

							// check if scrollable area have rows
							if ($(scrollable).find('.' + pfx + 'datatable__row').length > 0 && $(scrollable).length > 0) {
								scroll.scrollHead = $(datatable.tableHead).find('> .' + pfx + 'datatable__lock--scroll > .' + pfx + 'datatable__row');
								scroll.scrollFoot = $(datatable.tableFoot).find('> .' + pfx + 'datatable__lock--scroll > .' + pfx + 'datatable__row');
								scroll.tableLocked = $(datatable.tableBody).find('.' + pfx + 'datatable__lock:not(.' + pfx + 'datatable__lock--scroll)');
								if (Plugin.getOption('layout.customScrollbar') && util.detectIE() != 10 && screen > util.getBreakpoint('lg')) {
									scroll.initCustomScrollbar(scrollable[0]);
								} else {
									scroll.initDefaultScrollbar(scrollable);
								}
							} else if ($(datatable.tableBody).find('.' + pfx + 'datatable__row').length > 0) {
								scroll.scrollHead = $(datatable.tableHead).find('> .' + pfx + 'datatable__row');
								scroll.scrollFoot = $(datatable.tableFoot).find('> .' + pfx + 'datatable__row');
								if (Plugin.getOption('layout.customScrollbar') && util.detectIE() != 10 && screen > util.getBreakpoint('lg')) {
									scroll.initCustomScrollbar(datatable.tableBody);
								} else {
									scroll.initDefaultScrollbar(datatable.tableBody);
								}
							}
						}
					},
					initDefaultScrollbar: function(scrollable) {
						// get initial scroll position
						scroll.initPosition = $(scrollable).scrollLeft();
						$(scrollable).css('overflow-y', 'auto').off().on('scroll', scroll.onScrolling);
						if (Plugin.getOption('rows.autoHide') !== true) {
							$(scrollable).css('overflow-x', 'auto');
						}
					},
					onScrolling: function(e) {
						var left = $(this).scrollLeft();
						var top = $(this).scrollTop();
						if (util.isRTL()) {
							// deduct initial position for RTL
							left = left - scroll.initPosition;
						}
						$(scroll.scrollHead).css('left', -left);
						$(scroll.scrollFoot).css('left', -left);
						$(scroll.tableLocked).each(function(i, table) {
							if (Plugin.isLocked()) {
								// scrollbar offset
								top -= 1;
							}
							$(table).css('top', -top);
						});
					},
					initCustomScrollbar: function(scrollable) {
						scroll.scrollable = scrollable;
						// create a new instance for table body with scrollbar
						Plugin.initScrollbar(scrollable);
						// get initial scroll position
						scroll.initPosition = $(scrollable).scrollLeft();
						$(scrollable).off().on('scroll', scroll.onScrolling);
					},
				};
				scroll.init();
				return scroll;
			},

			/**
			 * Init custom scrollbar and reset position
			 * @param element
			 * @param options
			 */
			initScrollbar: function(element, options) {
				if (!element || !element.nodeName) {
					return;
				}
				$(datatable.tableBody).css('overflow', '');
				if (util.hasClass(element, 'ps')) {
					$(element).data('ps').update();
				} else {
					var ps = new PerfectScrollbar(element, Object.assign({}, {
						wheelSpeed: 0.5,
						swipeEasing: true,
						// wheelPropagation: false,
						minScrollbarLength: 40,
						maxScrollbarLength: 300,
						suppressScrollX: Plugin.getOption('rows.autoHide') && !Plugin.isLocked()
					}, options));
					$(element).data('ps', ps);

					// reset perfect scrollbar on resize
					$(window).resize(function() {
						ps.update();
					});
				}
			},

			/**
			 * Set column title from options.columns settings
			 */
			setHeadTitle: function(tablePart) {
				if (typeof tablePart === 'undefined') tablePart = datatable.tableHead;
				tablePart = $(tablePart)[0];
				var columns = options.columns;
				var row = tablePart.getElementsByTagName('tr')[0];
				var ths = tablePart.getElementsByTagName('td');

				if (typeof row === 'undefined') {
					row = document.createElement('tr');
					tablePart.appendChild(row);
				}

				$.each(columns, function(i, column) {
					var th = ths[i];
					if (typeof th === 'undefined') {
						th = document.createElement('th');
						row.appendChild(th);
					}

					// set column title
					if (typeof column['title'] !== 'undefined') {
						th.innerHTML = column.title;
						th.setAttribute('data-field', column.field);
						util.addClass(th, column.class);
						// set disable autoHide or force enable
						if (typeof column.autoHide !== 'undefined') {
							if (column.autoHide !== true) {
								th.setAttribute('data-autohide-disabled', column.autoHide);
							} else {
								th.setAttribute('data-autohide-enabled', column.autoHide);
							}
						}
						$(th).data(column);
					}

					// set header attr option
					if (typeof column.attr !== 'undefined') {
						$.each(column.attr, function(key, val) {
							th.setAttribute(key, val);
						});
					}

					// apply text align to thead/tfoot
					if (typeof column.textAlign !== 'undefined') {
						var align = typeof datatable.textAlign[column.textAlign] !== 'undefined' ? datatable.textAlign[column.textAlign] : '';
						util.addClass(th, align);
					}
				});
				Plugin.setupDOM(tablePart);
			},

			/**
			 * Initiate to get remote or local data via ajax
			 */
			dataRender: function(action) {
				$(datatable.table).siblings('.' + pfx + 'datatable__pager').removeClass(pfx + 'datatable--paging-loaded');

				var buildMeta = function() {
					datatable.dataSet = datatable.dataSet || [];
					Plugin.localDataUpdate();
					// local pagination meta
					var meta = Plugin.getDataSourceParam('pagination');
					if (meta.perpage === 0) {
						meta.perpage = options.data.pageSize || 10;
					}
					meta.total = datatable.dataSet.length;
					var start = Math.max(meta.perpage * (meta.page - 1), 0);
					var end = Math.min(start + meta.perpage, meta.total);
					datatable.dataSet = $(datatable.dataSet).slice(start, end);
					return meta;
				};

				var afterGetData = function(result) {
					var localPagingCallback = function(ctx, meta) {
						if (!$(ctx.pager).hasClass(pfx + 'datatable--paging-loaded')) {
							$(ctx.pager).remove();
							ctx.init(meta);
						}
						$(ctx.pager).off().on(pfx + 'datatable--on-goto-page', function(e) {
							$(ctx.pager).remove();
							ctx.init(meta);
						});

						var start = Math.max(meta.perpage * (meta.page - 1), 0);
						var end = Math.min(start + meta.perpage, meta.total);

						Plugin.localDataUpdate();
						datatable.dataSet = $(datatable.dataSet).slice(start, end);

						// insert data into table content
						Plugin.insertData();
					};

					$(datatable.wrap).removeClass(pfx + 'datatable--error');
					// pagination enabled
					if (options.pagination) {
						if (options.data.serverPaging && options.data.type !== 'local') {
							// server pagination
							var serverMeta = Plugin.getObject('meta', result || null);
							if (serverMeta !== null) {
								Plugin.pagingObject = Plugin.paging(serverMeta);
							} else {
								// no meta object from server response, fallback to local pagination
								Plugin.pagingObject = Plugin.paging(buildMeta(), localPagingCallback);
							}
						} else {
							// local pagination can be used by remote data also
							Plugin.pagingObject = Plugin.paging(buildMeta(), localPagingCallback);
						}
					} else {
						// pagination is disabled
						Plugin.localDataUpdate();
					}
					// insert data into table content
					Plugin.insertData();
				};

				// get local datasource
				if (options.data.type === 'local'
					// for remote json datasource
					// || typeof options.data.source.read === 'undefined' && datatable.dataSet !== null
					// for remote datasource, server sorting is disabled and data already received from remote
					|| options.data.serverSorting === false && action === 'sort'
					|| options.data.serverFiltering === false && action === 'search'
				) {
					setTimeout(function() {
						afterGetData();
						Plugin.setAutoColumns();
					});
					return;
				}

				// getting data from remote only
				Plugin.getData().done(afterGetData);
			},

			/**
			 * Process ajax data
			 */
			insertData: function() {
				datatable.dataSet = datatable.dataSet || [];
				var params = Plugin.getDataSourceParam();

				// get row attributes
				var pagination = params.pagination;
				var start = (Math.max(pagination.page, 1) - 1) * pagination.perpage;
				var end = Math.min(pagination.page, pagination.pages) * pagination.perpage;
				var rowProps = {};
				if (typeof options.data.attr.rowProps !== 'undefined' && options.data.attr.rowProps.length) {
					rowProps = options.data.attr.rowProps.slice(start, end);
				}

				var tableBody = document.createElement('tbody');
				tableBody.style.visibility = 'hidden';
				var colLength = options.columns.length;

				$.each(datatable.dataSet, function(rowIndex, row) {
					var tr = document.createElement('tr');
					tr.setAttribute('data-row', rowIndex);
					// keep data object to row
					$(tr).data('obj', row);

					if (typeof rowProps[rowIndex] !== 'undefined') {
						$.each(rowProps[rowIndex], function() {
							tr.setAttribute(this.name, this.value);
						});
					}

					var cellIndex = 0;
					var tds = [];
					for (var a = 0; a < colLength; a += 1) {
						var column = options.columns[a];
						var classes = [];
						// add sorted class to cells
						if (Plugin.getObject('sort.field', params) === column.field) {
							classes.push(pfx + 'datatable__cell--sorted');
						}

						// apply text align
						if (typeof column.textAlign !== 'undefined') {
							var align = typeof datatable.textAlign[column.textAlign] !== 'undefined' ? datatable.textAlign[column.textAlign] : '';
							classes.push(align);
						}

						// var classAttr = '';
						if (typeof column.class !== 'undefined') {
							classes.push(column.class);
						}

						var td = document.createElement('td');
						util.addClass(td, classes.join(' '));
						td.setAttribute('data-field', column.field);
						// set disable autoHide or force enable
						if (typeof column.autoHide !== 'undefined') {
							if (column.autoHide !== true) {
								td.setAttribute('data-autohide-disabled', column.autoHide);
							} else {
								td.setAttribute('data-autohide-enabled', column.autoHide);
							}
						}
						td.innerHTML = Plugin.getObject(column.field, row);
						tr.appendChild(td);
					}

					tableBody.appendChild(tr);
				});

				// display no records message
				if (datatable.dataSet.length === 0) {
					var errorSpan = document.createElement('span');
					util.addClass(errorSpan, pfx + 'datatable--error');
					errorSpan.innerHTML = Plugin.getOption('translate.records.noRecords');
					tableBody.appendChild(errorSpan);
					$(datatable.wrap).addClass(pfx + 'datatable--error ' + pfx + 'datatable--loaded');
					Plugin.spinnerCallback(false);
				}

				// replace existing table body
				$(datatable.tableBody).replaceWith(tableBody);
				datatable.tableBody = tableBody;

				// layout update
				Plugin.setupDOM(datatable.table);
				Plugin.setupCellField([datatable.tableBody]);
				Plugin.setupTemplateCell(datatable.tableBody);
				Plugin.layoutUpdate();
			},

			updateTableComponents: function() {
				datatable.tableHead = $(datatable.table).children('thead');
				datatable.tableBody = $(datatable.table).children('tbody');
				datatable.tableFoot = $(datatable.table).children('tfoot');
			},

			/**
			 * Call ajax for raw JSON data
			 */
			getData: function() {
				// Plugin.spinnerCallback(true);

				var ajaxParams = {
					dataType: 'json',
					method: 'POST',
					data: {},
					timeout: Plugin.getOption('data.source.read.timeout') || 30000,
				};

				if (options.data.type === 'local') {
					ajaxParams.url = options.data.source;
				}

				if (options.data.type === 'remote') {
					var data = Plugin.getDataSourceParam();
					// remove if server params is not enabled
					if (!Plugin.getOption('data.serverPaging')) {
						delete data['pagination'];
					}
					if (!Plugin.getOption('data.serverSorting')) {
						delete data['sort'];
					}
					ajaxParams.data = $.extend({}, ajaxParams.data, Plugin.getOption('data.source.read.params'), data);
					ajaxParams = $.extend({}, ajaxParams, Plugin.getOption('data.source.read'));

					if (typeof ajaxParams.url !== 'string') ajaxParams.url = Plugin.getOption('data.source.read');
					if (typeof ajaxParams.url !== 'string') ajaxParams.url = Plugin.getOption('data.source');
					// ajaxParams.data = $.extend(ajaxParams.data, data.pagination);
				}

				return $.ajax(ajaxParams).done(function(response, textStatus, jqXHR) {
					datatable.lastResponse = response;
					// extendible data map callback for custom datasource
					datatable.dataSet = datatable.originalDataSet = Plugin.dataMapCallback(response);
					Plugin.setAutoColumns();
					$(datatable).trigger(pfx + 'datatable--on-ajax-done', [datatable.dataSet]);
				}).fail(function(jqXHR, textStatus, errorThrown) {
					$(datatable).trigger(pfx + 'datatable--on-ajax-fail', [jqXHR]);
					$(datatable.tableBody).html($('<span/>').addClass(pfx + 'datatable--error').html(Plugin.getOption('translate.records.noRecords')));
					$(datatable.wrap).addClass(pfx + 'datatable--error ' + pfx + 'datatable--loaded');
					Plugin.spinnerCallback(false);
				}).always(function() {
				});
			},

			/**
			 * Pagination object
			 * @param meta if null, local pagination, otherwise remote
			 *     pagination
			 * @param callback for update data when navigating page
			 */
			paging: function(meta, callback) {
				var pg = {
					meta: null,
					pager: null,
					paginateEvent: null,
					pagerLayout: {pagination: null, info: null},
					callback: null,
					init: function(meta) {
						pg.meta = meta;

						// parse pagination meta to integer
						pg.meta.page = parseInt(pg.meta.page);
						pg.meta.pages = parseInt(pg.meta.pages);
						pg.meta.perpage = parseInt(pg.meta.perpage);
						pg.meta.total = parseInt(pg.meta.total);

						// always recount total pages
						pg.meta.pages = Math.max(Math.ceil(pg.meta.total / pg.meta.perpage), 1);

						// current page must be not over than total pages
						if (pg.meta.page > pg.meta.pages) pg.meta.page = pg.meta.pages;

						// set unique event name between tables
						pg.paginateEvent = Plugin.getTablePrefix('paging');

						pg.pager = $(datatable.table).siblings('.' + pfx + 'datatable__pager');
						if ($(pg.pager).hasClass(pfx + 'datatable--paging-loaded')) return;

						// if class .'+pfx+'datatable--paging-loaded not exist, recreate pagination
						$(pg.pager).remove();

						// if no pages available
						if (pg.meta.pages === 0) return;

						// update datasource params
						Plugin.setDataSourceParam('pagination', {
							page: pg.meta.page,
							pages: pg.meta.pages,
							perpage: pg.meta.perpage,
							total: pg.meta.total,
						});

						// default callback function, contains remote pagination handler
						pg.callback = pg.serverCallback;
						// custom callback function
						if (typeof callback === 'function') pg.callback = callback;

						pg.addPaginateEvent();
						pg.populate();

						pg.meta.page = Math.max(pg.meta.page || 1, pg.meta.page);

						$(datatable).trigger(pg.paginateEvent, pg.meta);

						pg.pagingBreakpoint.call();
						$(window).resize(pg.pagingBreakpoint);
					},
					serverCallback: function(ctx, meta) {
						Plugin.dataRender();
					},
					populate: function() {
						var icons = Plugin.getOption('layout.icons.pagination');
						var title = Plugin.getOption('translate.toolbar.pagination.items.default');
						// pager root element
						pg.pager = $('<div/>').addClass(pfx + 'datatable__pager ' + pfx + 'datatable--paging-loaded');
						// numbering links
						var pagerNumber = $('<ul/>').addClass(pfx + 'datatable__pager-nav');
						pg.pagerLayout['pagination'] = pagerNumber;

						// pager first/previous button
						$('<li/>').
							append($('<a/>').
								attr('title', title.first).
								addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link--first').
								append($('<i/>').addClass(icons.first)).
								on('click', pg.gotoMorePage).
								attr('data-page', 1)).
							appendTo(pagerNumber);
						$('<li/>').
							append($('<a/>').
								attr('title', title.prev).
								addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link--prev').
								append($('<i/>').addClass(icons.prev)).
								on('click', pg.gotoMorePage)).
							appendTo(pagerNumber);

						// more previous pages
						$('<li/>').
							append($('<a/>').
								attr('title', title.more).
								addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link--more-prev').
								html($('<i/>').addClass(icons.more)).
								on('click', pg.gotoMorePage)).
							appendTo(pagerNumber);

						$('<li/>').append($('<input/>').attr('type', 'text').addClass(pfx + 'pager-input form-control').attr('title', title.input).on('keyup', function() {
							// on keyup update [data-page]
							$(this).attr('data-page', Math.abs($(this).val()));
						}).on('keypress', function(e) {
							// on keypressed enter button
							if (e.which === 13) pg.gotoMorePage(e);
						})).appendTo(pagerNumber);

						var pagesNumber = Plugin.getOption('toolbar.items.pagination.pages.desktop.pagesNumber');
						var end = Math.ceil(pg.meta.page / pagesNumber) * pagesNumber;
						var start = end - pagesNumber;
						if (end > pg.meta.pages) {
							end = pg.meta.pages;
						}
						for (var x = start; x < end; x++) {
							var pageNumber = x + 1;
							$('<li/>').
								append($('<a/>').
									addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link-number').
									text(pageNumber).
									attr('data-page', pageNumber).
									attr('title', pageNumber).
									on('click', pg.gotoPage)).
								appendTo(pagerNumber);
						}

						// more next pages
						$('<li/>').
							append($('<a/>').
								attr('title', title.more).
								addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link--more-next').
								html($('<i/>').addClass(icons.more)).
								on('click', pg.gotoMorePage)).
							appendTo(pagerNumber);

						// pager next/last button
						$('<li/>').
							append($('<a/>').
								attr('title', title.next).
								addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link--next').
								append($('<i/>').addClass(icons.next)).
								on('click', pg.gotoMorePage)).
							appendTo(pagerNumber);
						$('<li/>').
							append($('<a/>').
								attr('title', title.last).
								addClass(pfx + 'datatable__pager-link ' + pfx + 'datatable__pager-link--last').
								append($('<i/>').addClass(icons.last)).
								on('click', pg.gotoMorePage).
								attr('data-page', pg.meta.pages)).
							appendTo(pagerNumber);

						// page info
						if (Plugin.getOption('toolbar.items.info')) {
							pg.pagerLayout['info'] = $('<div/>').addClass(pfx + 'datatable__pager-info').append($('<span/>').addClass(pfx + 'datatable__pager-detail'));
						}

						$.each(Plugin.getOption('toolbar.layout'), function(i, layout) {
							$(pg.pagerLayout[layout]).appendTo(pg.pager);
						});

						// page size select
						var pageSizeSelect = $('<select/>').
							addClass('selectpicker ' + pfx + 'datatable__pager-size').
							attr('title', Plugin.getOption('translate.toolbar.pagination.items.default.select')).
							attr('data-width', '60px').
							val(pg.meta.perpage).
							on('change', pg.updatePerpage).
							prependTo(pg.pagerLayout['info']);

						var pageSizes = Plugin.getOption('toolbar.items.pagination.pageSizeSelect');
						// default value here, to fix override option by user
						if (pageSizes.length == 0) pageSizes = [10, 20, 30, 50, 100];
						$.each(pageSizes, function(i, size) {
							var display = size;
							if (size === -1) display = Plugin.getOption('translate.toolbar.pagination.items.default.all');
							$('<option/>').attr('value', size).html(display).appendTo(pageSizeSelect);
						});

						// init selectpicker to dropdown
						$(datatable).ready(function() {
							$('.selectpicker').
								selectpicker().
								on('hide.bs.select', function() {
									// fix dropup arrow icon on hide
									$(this).closest('.bootstrap-select').removeClass('dropup');
								}).
								siblings('.dropdown-toggle').
								attr('title', Plugin.getOption('translate.toolbar.pagination.items.default.select'));
						});

						pg.paste();
					},
					paste: function() {
						// insert pagination based on placement position, top|bottom
						$.each($.unique(Plugin.getOption('toolbar.placement')),
							function(i, position) {
								if (position === 'bottom') {
									$(pg.pager).clone(true).insertAfter(datatable.table);
								}
								if (position === 'top') {
									// pager top need some extra space
									$(pg.pager).clone(true).addClass(pfx + 'datatable__pager--top').insertBefore(datatable.table);
								}
							});
					},
					gotoMorePage: function(e) {
						e.preventDefault();
						// $(this) is a link of .'+pfx+'datatable__pager-link

						if ($(this).attr('disabled') === 'disabled') return false;

						var page = $(this).attr('data-page');

						// event from text input
						if (typeof page === 'undefined') {
							page = $(e.target).attr('data-page');
						}

						pg.openPage(parseInt(page));
						return false;
					},
					gotoPage: function(e) {
						e.preventDefault();
						// prevent from click same page number
						if ($(this).hasClass(pfx + 'datatable__pager-link--active')) return;

						pg.openPage(parseInt($(this).data('page')));
					},
					openPage: function(page) {
						// currentPage is 1-based index
						pg.meta.page = parseInt(page);

						$(datatable).trigger(pg.paginateEvent, pg.meta);
						pg.callback(pg, pg.meta);

						// update page callback function
						$(pg.pager).trigger(pfx + 'datatable--on-goto-page', pg.meta);
					},
					updatePerpage: function(e) {
						e.preventDefault();
						// if (Plugin.getOption('layout.height') === null) {
						// fix white space, when perpage is set from many records to less records
						// $('html, body').animate({scrollTop: $(datatable).position().top});
						// }

						pg.pager = $(datatable.table).siblings('.' + pfx + 'datatable__pager').removeClass(pfx + 'datatable--paging-loaded');

						// on change select page size
						if (e.originalEvent) {
							pg.meta.perpage = parseInt($(this).val());
						}

						$(pg.pager).find('select.' + pfx + 'datatable__pager-size').val(pg.meta.perpage).attr('data-selected', pg.meta.perpage);

						// update datasource params
						Plugin.setDataSourceParam('pagination', {
							page: pg.meta.page,
							pages: pg.meta.pages,
							perpage: pg.meta.perpage,
							total: pg.meta.total,
						});

						// update page callback function
						$(pg.pager).trigger(pfx + 'datatable--on-update-perpage', pg.meta);
						$(datatable).trigger(pg.paginateEvent, pg.meta);
						pg.callback(pg, pg.meta);

						// update pagination info
						pg.updateInfo.call();
					},
					addPaginateEvent: function(e) {
						// pagination event
						$(datatable).off(pg.paginateEvent).on(pg.paginateEvent, function(e, meta) {
							Plugin.spinnerCallback(true);

							pg.pager = $(datatable.table).siblings('.' + pfx + 'datatable__pager');
							var pagerNumber = $(pg.pager).find('.' + pfx + 'datatable__pager-nav');

							// set sync active page class
							$(pagerNumber).find('.' + pfx + 'datatable__pager-link--active').removeClass(pfx + 'datatable__pager-link--active');
							$(pagerNumber).find('.' + pfx + 'datatable__pager-link-number[data-page="' + meta.page + '"]').addClass(pfx + 'datatable__pager-link--active');

							// set next and previous link page number
							$(pagerNumber).find('.' + pfx + 'datatable__pager-link--prev').attr('data-page', Math.max(meta.page - 1, 1));
							$(pagerNumber).find('.' + pfx + 'datatable__pager-link--next').attr('data-page', Math.min(meta.page + 1, meta.pages));

							// current page input value sync
							$(pg.pager).each(function() {
								$(this).find('.' + pfx + 'pager-input[type="text"]').prop('value', meta.page);
							});

							$(pg.pager).find('.' + pfx + 'datatable__pager-nav').show();
							if (meta.pages <= 1) {
								// hide pager if has 1 page
								$(pg.pager).find('.' + pfx + 'datatable__pager-nav').hide();
							}

							// update datasource params
							Plugin.setDataSourceParam('pagination', {
								page: pg.meta.page,
								pages: pg.meta.pages,
								perpage: pg.meta.perpage,
								total: pg.meta.total,
							});

							$(pg.pager).find('select.' + pfx + 'datatable__pager-size').val(meta.perpage).attr('data-selected', meta.perpage);

							// clear active rows
							$(datatable.table).find('.' + pfx + 'checkbox > [type="checkbox"]').prop('checked', false);
							$(datatable.table).find('.' + pfx + 'datatable__row--active').removeClass(pfx + 'datatable__row--active');

							pg.updateInfo.call();
							pg.pagingBreakpoint.call();
							// Plugin.resetScroll();
						});
					},
					updateInfo: function() {
						var start = Math.max(pg.meta.perpage * (pg.meta.page - 1) + 1, 1);
						var end = Math.min(start + pg.meta.perpage - 1, pg.meta.total);
						// page info update
						$(pg.pager).find('.' + pfx + 'datatable__pager-info').find('.' + pfx + 'datatable__pager-detail').html(Plugin.dataPlaceholder(
							Plugin.getOption('translate.toolbar.pagination.items.info'), {
								start: start,
								end: pg.meta.perpage === -1 ? pg.meta.total : end,
								pageSize: pg.meta.perpage === -1 ||
								pg.meta.perpage >= pg.meta.total
									? pg.meta.total
									: pg.meta.perpage,
								total: pg.meta.total,
							}));
					},

					/**
					 * Update pagination layout breakpoint
					 */
					pagingBreakpoint: function() {
						// keep page links reference
						var pagerNumber = $(datatable.table).siblings('.' + pfx + 'datatable__pager').find('.' + pfx + 'datatable__pager-nav');
						if ($(pagerNumber).length === 0) return;

						var currentPage = Plugin.getCurrentPage();
						var pagerInput = $(pagerNumber).find('.' + pfx + 'pager-input').closest('li');

						// reset
						$(pagerNumber).find('li').show();

						// pagination update
						$.each(Plugin.getOption('toolbar.items.pagination.pages'),
							function(mode, option) {
								if (util.isInResponsiveRange(mode)) {
									switch (mode) {
										case 'desktop':
										case 'tablet':
											var end = Math.ceil(currentPage / option.pagesNumber) *
												option.pagesNumber;
											var start = end - option.pagesNumber;
											$(pagerInput).hide();
											pg.meta = Plugin.getDataSourceParam('pagination');
											pg.paginationUpdate();
											break;

										case 'mobile':
											$(pagerInput).show();
											$(pagerNumber).find('.' + pfx + 'datatable__pager-link--more-prev').closest('li').hide();
											$(pagerNumber).find('.' + pfx + 'datatable__pager-link--more-next').closest('li').hide();
											$(pagerNumber).find('.' + pfx + 'datatable__pager-link-number').closest('li').hide();
											break;
									}

									return false;
								}
							});
					},

					/**
					 * Update pagination number and button display
					 */
					paginationUpdate: function() {
						var pager = $(datatable.table).siblings('.' + pfx + 'datatable__pager').find('.' + pfx + 'datatable__pager-nav'),
							pagerMorePrev = $(pager).find('.' + pfx + 'datatable__pager-link--more-prev'),
							pagerMoreNext = $(pager).find('.' + pfx + 'datatable__pager-link--more-next'),
							pagerFirst = $(pager).find('.' + pfx + 'datatable__pager-link--first'),
							pagerPrev = $(pager).find('.' + pfx + 'datatable__pager-link--prev'),
							pagerNext = $(pager).find('.' + pfx + 'datatable__pager-link--next'),
							pagerLast = $(pager).find('.' + pfx + 'datatable__pager-link--last');

						// get visible page
						var pagerNumber = $(pager).find('.' + pfx + 'datatable__pager-link-number');
						// get page before of first visible
						var morePrevPage = Math.max($(pagerNumber).first().data('page') - 1, 1);
						$(pagerMorePrev).each(function(i, prev) {
							$(prev).attr('data-page', morePrevPage);
						});
						// show/hide <li>
						if (morePrevPage === 1) {
							$(pagerMorePrev).parent().hide();
						} else {
							$(pagerMorePrev).parent().show();
						}

						// get page after of last visible
						var moreNextPage = Math.min($(pagerNumber).last().data('page') + 1,
							pg.meta.pages);
						$(pagerMoreNext).each(function(i, prev) {
							$(pagerMoreNext).attr('data-page', moreNextPage).show();
						});

						// show/hide <li>
						if (moreNextPage === pg.meta.pages
							// missing dot fix when last hidden page is one left
							&& moreNextPage === $(pagerNumber).last().data('page')) {
							$(pagerMoreNext).parent().hide();
						} else {
							$(pagerMoreNext).parent().show();
						}

						// begin/end of pages
						if (pg.meta.page === 1) {
							$(pagerFirst).attr('disabled', true).addClass(pfx + 'datatable__pager-link--disabled');
							$(pagerPrev).attr('disabled', true).addClass(pfx + 'datatable__pager-link--disabled');
						} else {
							$(pagerFirst).removeAttr('disabled').removeClass(pfx + 'datatable__pager-link--disabled');
							$(pagerPrev).removeAttr('disabled').removeClass(pfx + 'datatable__pager-link--disabled');
						}
						if (pg.meta.page === pg.meta.pages) {
							$(pagerNext).attr('disabled', true).addClass(pfx + 'datatable__pager-link--disabled');
							$(pagerLast).attr('disabled', true).addClass(pfx + 'datatable__pager-link--disabled');
						} else {
							$(pagerNext).removeAttr('disabled').removeClass(pfx + 'datatable__pager-link--disabled');
							$(pagerLast).removeAttr('disabled').removeClass(pfx + 'datatable__pager-link--disabled');
						}

						// display more buttons
						var nav = Plugin.getOption('toolbar.items.pagination.navigation');
						if (!nav.first) $(pagerFirst).remove();
						if (!nav.prev) $(pagerPrev).remove();
						if (!nav.next) $(pagerNext).remove();
						if (!nav.last) $(pagerLast).remove();
						if (!nav.more) {
							$(pagerMorePrev).remove();
							$(pagerMoreNext).remove();
						}
					},
				};
				pg.init(meta);
				return pg;
			},

			/**
			 * Hide/show table cell defined by
			 * options[columns][i][responsive][visible/hidden]
			 */
			columnHide: function() {
				var screen = util.getViewPort().width;
				// foreach columns setting
				$.each(options.columns, function(i, column) {
					if (typeof column.responsive !== 'undefined' || typeof column.visible !== 'undefined') {
						var field = column.field;
						var tds = $.grep($(datatable.table).find('.' + pfx + 'datatable__cell'), function(n, i) {
							return field === $(n).data('field');
						});

						setTimeout(function () {
							// hide by force
							if (Plugin.getObject('visible', column) === false) {
								$(tds).hide();
							} else {
								// show/hide by responsive breakpoint
								if (util.getBreakpoint(Plugin.getObject('responsive.hidden', column)) >= screen) {
									$(tds).hide();
								} else {
									$(tds).show();
								}
								if (util.getBreakpoint(Plugin.getObject('responsive.visible', column)) <= screen) {
									$(tds).show();
								} else {
									$(tds).hide();
								}
							}
						});
					}
				});
			},

			/**
			 * Setup sub datatable
			 */
			setupSubDatatable: function() {
				var subTableCallback = Plugin.getOption('detail.content');
				if (typeof subTableCallback !== 'function') return;

				// subtable already exist
				if ($(datatable.table).find('.' + pfx + 'datatable__subtable').length > 0) return;

				$(datatable.wrap).addClass(pfx + 'datatable--subtable');

				options.columns[0]['subtable'] = true;

				// toggle on open sub table
				var toggleSubTable = function(e) {
					e.preventDefault();
					// get parent row of this subtable
					var parentRow = $(this).closest('.' + pfx + 'datatable__row');

					// get subtable row for sub table
					var subTableRow = $(parentRow).next('.' + pfx + 'datatable__row-subtable');
					if ($(subTableRow).length === 0) {
						// prepare DOM for sub table, each <tr> as parent and add <tr> as child table
						subTableRow = $('<tr/>').
							addClass(pfx + 'datatable__row-subtable ' + pfx + 'datatable__row-loading').
							hide().
							append($('<td/>').addClass(pfx + 'datatable__subtable').attr('colspan', Plugin.getTotalColumns()));
						$(parentRow).after(subTableRow);
						// add class to even row
						if ($(parentRow).hasClass(pfx + 'datatable__row--even')) {
							$(subTableRow).addClass(pfx + 'datatable__row-subtable--even');
						}
					}

					$(subTableRow).toggle();

					var subTable = $(subTableRow).find('.' + pfx + 'datatable__subtable');

					// get id from first column of parent row
					var primaryKey = $(this).closest('[data-field]:first-child').find('.' + pfx + 'datatable__toggle-subtable').data('value');

					var icon = $(this).find('i').removeAttr('class');

					// prevent duplicate datatable init
					if ($(parentRow).hasClass(pfx + 'datatable__row--subtable-expanded')) {
						$(icon).addClass(Plugin.getOption('layout.icons.rowDetail.collapse'));
						// remove expand class from parent row
						$(parentRow).removeClass(pfx + 'datatable__row--subtable-expanded');
						// trigger event on collapse
						$(datatable).trigger(pfx + 'datatable--on-collapse-subtable', [parentRow]);
					} else {
						// expand and run callback function
						$(icon).addClass(Plugin.getOption('layout.icons.rowDetail.expand'));
						// add expand class to parent row
						$(parentRow).addClass(pfx + 'datatable__row--subtable-expanded');
						// trigger event on expand
						$(datatable).trigger(pfx + 'datatable--on-expand-subtable', [parentRow]);
					}

					// prevent duplicate datatable init
					if ($(subTable).find('.' + pfx + 'datatable').length === 0) {
						// get data by primary id
						$.map(datatable.dataSet, function(n, i) {
							// primary id must be at the first column, otherwise e.data will be undefined
							if (primaryKey === n[options.columns[0].field]) {
								e.data = n;
								return true;
							}
							return false;
						});

						// deprecated in v5.0.6
						e.detailCell = subTable;

						e.parentRow = parentRow;
						e.subTable = subTable;

						// run callback with event
						subTableCallback(e);

						$(subTable).children('.' + pfx + 'datatable').on(pfx + 'datatable--on-init', function(e) {
							$(subTableRow).removeClass(pfx + 'datatable__row-loading');
						});
						if (Plugin.getOption('data.type') === 'local') {
							$(subTableRow).removeClass(pfx + 'datatable__row-loading');
						}
					}
				};

				var columns = options.columns;
				$(datatable.tableBody).find('.' + pfx + 'datatable__row').each(function(tri, tr) {
					$(tr).find('.' + pfx + 'datatable__cell').each(function(tdi, td) {
						// get column settings by field
						var column = $.grep(columns, function(n, i) {
							return $(td).data('field') === n.field;
						})[0];
						if (typeof column !== 'undefined') {
							var value = $(td).text();
							// enable column subtable toggle
							if (typeof column.subtable !== 'undefined' && column.subtable) {
								// check if subtable toggle exist
								if ($(td).find('.' + pfx + 'datatable__toggle-subtable').length > 0) return;
								// append subtable toggle
								$(td).
									html($('<a/>').
										addClass(pfx + 'datatable__toggle-subtable').
										attr('href', '#').
										attr('data-value', value).
										attr('title', Plugin.getOption('detail.title')).
										on('click', toggleSubTable).
										append($('<i/>').css('width', $(td).data('width')).addClass(Plugin.getOption('layout.icons.rowDetail.collapse'))));
							}
						}
					});
				});

				// $(datatable.tableHead).find('.'+pfx+'-datatable__row').first()
			},

			/**
			 * Datasource mapping callback
			 */
			dataMapCallback: function(raw) {
				// static dataset array
				var dataSet = raw;
				// dataset mapping callback
				if (typeof Plugin.getOption('data.source.read.map') === 'function') {
					return Plugin.getOption('data.source.read.map')(raw);
				} else {
					// default data mapping fallback
					if (typeof raw !== 'undefined' && typeof raw.data !== 'undefined') {
						dataSet = raw.data;
					}
				}
				return dataSet;
			},

			isSpinning: false,
			/**
			 * BlockUI spinner callback
			 * @param block
			 * @param target
			 */
			spinnerCallback: function(block, target) {
				if (typeof target === 'undefined') target = datatable;
				// get spinner options
				var spinnerOptions = Plugin.getOption('layout.spinner');
				// spinner is disabled
				if (typeof spinnerOptions === 'undefined' || !spinnerOptions) {
					return;
				}
				if (block) {
					if (!Plugin.isSpinning) {
						if (typeof spinnerOptions.message !== 'undefined' && spinnerOptions.message === true) {
							// use default spinner message from translation
							spinnerOptions.message = Plugin.getOption('translate.records.processing');
						}
						Plugin.isSpinning = true;
						if (typeof app !== 'undefined') {
							app.block(target, spinnerOptions);
						}
					}
				} else {
					Plugin.isSpinning = false;
					if (typeof app !== 'undefined') {
						app.unblock(target);
					}
				}
			},

			/**
			 * Default sort callback function
			 * @param data
			 * @param sort
			 * @param column
			 * @returns {*|Array.<T>|{sort, field}|{asc, desc}}
			 */
			sortCallback: function(data, sort, column) {
				var type = column['type'] || 'string';
				var format = column['format'] || '';
				var field = column['field'];

				return $(data).sort(function(a, b) {
					var aField = a[field];
					var bField = b[field];

					switch (type) {
						case 'date':
							if (typeof moment === 'undefined') {
								throw new Error('Moment.js is required.');
							}
							var diff = moment(aField, format).diff(moment(bField, format));
							if (sort === 'asc') {
								return diff > 0 ? 1 : diff < 0 ? -1 : 0;
							} else {
								return diff < 0 ? 1 : diff > 0 ? -1 : 0;
							}
							break;

						case 'number':
							if (isNaN(parseFloat(aField)) && aField != null) {
								aField = Number(aField.replace(/[^0-9\.-]+/g, ''));
							}
							if (isNaN(parseFloat(bField)) && bField != null) {
								bField = Number(bField.replace(/[^0-9\.-]+/g, ''));
							}
							aField = parseFloat(aField);
							bField = parseFloat(bField);
							if (sort === 'asc') {
								return aField > bField ? 1 : aField < bField ? -1 : 0;
							} else {
								return aField < bField ? 1 : aField > bField ? -1 : 0;
							}
							break;

						case 'string':
						default:
							if (sort === 'asc') {
								return aField > bField ? 1 : aField < bField ? -1 : 0;
							} else {
								return aField < bField ? 1 : aField > bField ? -1 : 0;
							}
							break;
					}
				});
			},

			/**
			 * Custom debug log
			 * @param text
			 * @param obj
			 */
			log: function(text, obj) {
				if (typeof obj === 'undefined') obj = '';
				if (datatable.debug) {
					console.log(text, obj);
				}
			},

			/**
			 * Auto hide columnds overflow in row
			 */
			autoHide: function() {
				var hiddenExist = false;
				// force hide enabled
				var hidDefault = $(datatable.table).find('[data-autohide-enabled]');
				if (hidDefault.length) {
					hiddenExist = true;
					hidDefault.hide();
				}

				var toggleHiddenColumns = function(e) {
					e.preventDefault();

					var row = $(this).closest('.' + pfx + 'datatable__row');
					var detailRow = $(row).next();

					if (!$(detailRow).hasClass(pfx + 'datatable__row-detail')) {
						$(this).find('i').removeClass(Plugin.getOption('layout.icons.rowDetail.collapse')).addClass(Plugin.getOption('layout.icons.rowDetail.expand'));

						var hiddenCells = $(row).find('.' + pfx + 'datatable__cell:hidden');
						var clonedCells = hiddenCells.clone().show();

						detailRow = $('<tr/>').addClass(pfx + 'datatable__row-detail').insertAfter(row);
						var detailRowTd = $('<td/>').addClass(pfx + 'datatable__detail').attr('colspan', Plugin.getTotalColumns()).appendTo(detailRow);

						var detailSubTable = $('<table/>');
						$(clonedCells).each(function() {
							var field = $(this).data('field');
							var column = $.grep(options.columns, function(n, i) {
								return field === n.field;
							})[0];
							if (typeof column === 'undefined' || column.visible !== false) {
								$(detailSubTable).
										append($('<tr class="' + pfx + 'datatable__row"></tr>').
												append($('<td class="' + pfx + 'datatable__cell"></td>').append($('<span/>').append(column.title))).
												append(this));
							}
						});
						$(detailRowTd).append(detailSubTable);

					} else {
						$(this).find('i').removeClass(Plugin.getOption('layout.icons.rowDetail.expand')).addClass(Plugin.getOption('layout.icons.rowDetail.collapse'));
						$(detailRow).remove();
					}
				};

				setTimeout(function () {
					$(datatable.table).find('.' + pfx + 'datatable__cell').show();
					$(datatable.tableBody).each(function() {
						var recursive = 0;
						while ($(this)[0].offsetWidth < $(this)[0].scrollWidth && recursive < options.columns.length) {
							$(datatable.table).find('.' + pfx + 'datatable__row').each(function(i) {
								var cell = $(this).find('.' + pfx + 'datatable__cell:not(:hidden):not([data-autohide-disabled])').last();
								$(cell).hide();
								hiddenExist = true;
							});
							recursive++;
						}
					});

					if (hiddenExist) {
						// toggle show hidden columns
						$(datatable.tableBody).find('.' + pfx + 'datatable__row').each(function() {
							// if no toggle yet
							if($(this).find('.' + pfx + 'datatable__toggle-detail').length === 0) {
								// add toggle
								$(this).prepend($('<td/>').
										addClass(pfx + 'datatable__cell ' + pfx + 'datatable__toggle-detail').
										append($('<a/>').
											addClass(pfx + 'datatable__toggle-detail').
											attr('href', '').
											on('click', toggleHiddenColumns).
											append('<i class="' + Plugin.getOption('layout.icons.rowDetail.collapse') + '"></i>')));
							}

							// check if subtable toggle exist
							if ($(datatable.tableHead).find('.' + pfx + 'datatable__toggle-detail').length === 0) {
								// add empty column to the header and footer
								$(datatable.tableHead).
									find('.' + pfx + 'datatable__row').
									first().
									prepend('<th class="' + pfx + 'datatable__cell ' + pfx + 'datatable__toggle-detail"><span></span></th>');
								$(datatable.tableFoot).
									find('.' + pfx + 'datatable__row').
									first().
									prepend('<th class="' + pfx + 'datatable__cell ' + pfx + 'datatable__toggle-detail"><span></span></th>');
							} else {
								$(datatable.tableHead).find('.' + pfx + 'datatable__toggle-detail').find('span');
							}
						});
					}
				});

				Plugin.adjustCellsWidth.call();
			},

			/**
			 * To enable auto columns features for remote data source
			 */
			setAutoColumns: function() {
				if (Plugin.getOption('data.autoColumns')) {
					$.each(datatable.dataSet[0], function(k, v) {
						var found = $.grep(options.columns, function(n, i) {
							return k === n.field;
						});
						if (found.length === 0) {
							options.columns.push({field: k, title: k});
						}
					});
					$(datatable.tableHead).find('.' + pfx + 'datatable__row').remove();
					Plugin.setHeadTitle();
					if (Plugin.getOption('layout.footer')) {
						$(datatable.tableFoot).find('.' + pfx + 'datatable__row').remove();
						Plugin.setHeadTitle(datatable.tableFoot);
					}
				}
			},

			/********************
			 ** HELPERS
			 ********************/

			/**
			 * Check if table is a locked colums table
			 */
			isLocked: function() {
				var isLocked = Plugin.lockEnabledColumns();
				return isLocked.left.length > 0 || isLocked.right.length > 0;
			},

			isSubtable: function() {
				return util.hasClass(datatable.wrap[0], pfx + 'datatable--subtable') || false;
			},

			/**
			 * Get total extra space of an element for width calculation,
			 * including padding, margin, border
			 * @param element
			 * @returns {number}
			 */
			getExtraSpace: function(element) {
				var padding = parseInt($(element).css('paddingRight')) +
					parseInt($(element).css('paddingLeft'));
				var margin = parseInt($(element).css('marginRight')) +
					parseInt($(element).css('marginLeft'));
				var border = Math.ceil(
					$(element).css('border-right-width').replace('px', ''));
				return padding + margin + border;
			},

			/**
			 * Insert data of array into {{ }} template placeholder
			 * @param template
			 * @param data
			 * @returns {*}
			 */
			dataPlaceholder: function(template, data) {
				var result = template;
				$.each(data, function(key, val) {
					result = result.replace('{{' + key + '}}', val);
				});
				return result;
			},

			/**
			 * Get table unique ID
			 * Note: table unique change each time refreshed
			 * @param suffix
			 * @returns {*}
			 */
			getTableId: function(suffix) {
				if (typeof suffix === 'undefined') suffix = '';
				var id = $(datatable).attr('id');
				if (typeof id === 'undefined') {
					id = $(datatable).attr('class').split(' ')[0];
				}
				return id + suffix;
			},

			/**
			 * Get table prefix with depth number
			 */
			getTablePrefix: function(suffix) {
				if (typeof suffix !== 'undefined') suffix = '-' + suffix;
				return Plugin.getTableId() + '-' + Plugin.getDepth() + suffix;
			},

			/**
			 * Get current table depth of sub table
			 * @returns {number}
			 */
			getDepth: function() {
				var depth = 0;
				var table = datatable.table;
				do {
					table = $(table).parents('.' + pfx + 'datatable__table');
					depth++;
				} while ($(table).length > 0);
				return depth;
			},

			/**
			 * Keep state item
			 * @param key
			 * @param value
			 */
			stateKeep: function(key, value) {
				key = Plugin.getTablePrefix(key);
				if (Plugin.getOption('data.saveState') === false) return;
				if (Plugin.getOption('data.saveState.webstorage') && localStorage) {
					localStorage.setItem(key, JSON.stringify(value));
				}
				if (Plugin.getOption('data.saveState.cookie')) {
					Cookies.set(key, JSON.stringify(value));
				}
			},

			/**
			 * Get state item
			 * @param key
			 * @param defValue
			 */
			stateGet: function(key, defValue) {
				key = Plugin.getTablePrefix(key);
				if (Plugin.getOption('data.saveState') === false) return;
				var value = null;
				if (Plugin.getOption('data.saveState.webstorage') && localStorage) {
					value = localStorage.getItem(key);
				} else {
					value = Cookies.get(key);
				}
				if (typeof value !== 'undefined' && value !== null) {
					return JSON.parse(value);
				}
			},

			/**
			 * Update data in state without clear existing
			 * @param key
			 * @param value
			 */
			stateUpdate: function(key, value) {
				var ori = Plugin.stateGet(key);
				if (typeof ori === 'undefined' || ori === null) ori = {};
				Plugin.stateKeep(key, $.extend({}, ori, value));
			},

			/**
			 * Remove state item
			 * @param key
			 */
			stateRemove: function(key) {
				key = Plugin.getTablePrefix(key);
				if (localStorage) {
					localStorage.removeItem(key);
				}
				Cookies.remove(key);
			},

			/**
			 * Get total columns.
			 */
			getTotalColumns: function(tablePart) {
				if (typeof tablePart === 'undefined') tablePart = datatable.tableBody;
				return $(tablePart).find('.' + pfx + 'datatable__row').first().find('.' + pfx + 'datatable__cell').length;
			},

			/**
			 * Get table row. Useful to get row when current table is in lock
			 * mode. Can be used for both lock and normal table mode. By
			 * default, returning result will be in a list of <td>.
			 * @param tablePart
			 * @param row 1-based index
			 * @param tdOnly Optional. Default true
			 * @returns {*}
			 */
			getOneRow: function(tablePart, row, tdOnly) {
				if (typeof tdOnly === 'undefined') tdOnly = true;
				// get list of <tr>
				var result = $(tablePart).find('.' + pfx + 'datatable__row:not(.' + pfx + 'datatable__row-detail):nth-child(' + row + ')');
				if (tdOnly) {
					// get list of <td> or <th>
					result = result.find('.' + pfx + 'datatable__cell');
				}
				return result;
			},

			/**
			 * Sort table row at HTML level by column index.
			 * todo; Not in use.
			 * @param header Header sort clicked
			 * @param sort asc|desc. Optional. Default asc
			 * @param int Boolean. Optional. Comparison value parse to integer.
			 *     Default false
			 */
			sortColumn: function(header, sort, int) {
				if (typeof sort === 'undefined') sort = 'asc'; // desc
				if (typeof int === 'undefined') int = false;

				var column = $(header).index();
				var rows = $(datatable.tableBody).find('.' + pfx + 'datatable__row');
				var hIndex = $(header).closest('.' + pfx + 'datatable__lock').index();
				if (hIndex !== -1) {
					rows = $(datatable.tableBody).find('.' + pfx + 'datatable__lock:nth-child(' + (hIndex + 1) + ')').find('.' + pfx + 'datatable__row');
				}

				var container = $(rows).parent();
				$(rows).sort(function(a, b) {
					var tda = $(a).find('td:nth-child(' + column + ')').text();
					var tdb = $(b).find('td:nth-child(' + column + ')').text();

					if (int) {
						// useful for integer type sorting
						tda = parseInt(tda);
						tdb = parseInt(tdb);
					}

					if (sort === 'asc') {
						return tda > tdb ? 1 : tda < tdb ? -1 : 0;
					} else {
						return tda < tdb ? 1 : tda > tdb ? -1 : 0;
					}
				}).appendTo(container);
			},

			/**
			 * Perform sort remote and local
			 */
			sorting: function() {
				var sortObj = {
					init: function() {
						if (options.sortable) {
							$(datatable.tableHead).
								find('.' + pfx + 'datatable__cell:not(.' + pfx + 'datatable__cell--check)').
								addClass(pfx + 'datatable__cell--sort').
								off('click').
								on('click', sortObj.sortClick);
							// first init
							sortObj.setIcon();
						}
					},
					setIcon: function() {
						var meta = Plugin.getDataSourceParam('sort');
						if ($.isEmptyObject(meta)) return;

						var column = Plugin.getColumnByField(meta.field);
						// sort is disabled for this column
						if (typeof column !== 'undefined' && typeof column.sortable !== 'undefined' && column.sortable === false) return;

						// sort icon beside column header
						var td = $(datatable.tableHead).find('.' + pfx + 'datatable__cell[data-field="' + meta.field + '"]').attr('data-sort', meta.sort);
						var sorting = $(td).find('span');
						var icon = $(sorting).find('i');

						var icons = Plugin.getOption('layout.icons.sort');
						// update sort icon; desc & asc
						if ($(icon).length > 0) {
							$(icon).removeAttr('class').addClass(icons[meta.sort]);
						} else {
							$(sorting).append($('<i/>').addClass(icons[meta.sort]));
						}

						// set sorted class to header on init
						$(td).addClass(pfx + 'datatable__cell--sorted');
					},
					sortClick: function(e) {
						var meta = Plugin.getDataSourceParam('sort');
						var field = $(this).data('field');
						var column = Plugin.getColumnByField(field);
						// sort is disabled for this column
						if (typeof column.sortable !== 'undefined' && column.sortable === false) return;

						// set sorted class to header
						$(datatable.tableHead).find('th').removeClass(pfx + 'datatable__cell--sorted');
						util.addClass(this, pfx + 'datatable__cell--sorted');

						$(datatable.tableHead).find('.' + pfx + 'datatable__cell > span > i').remove();

						if (options.sortable) {
							Plugin.spinnerCallback(true);

							var sort = 'desc';
							if (Plugin.getObject('field', meta) === field) {
								sort = Plugin.getObject('sort', meta);
							}

							// toggle sort
							sort = typeof sort === 'undefined' || sort === 'desc'
								? 'asc'
								: 'desc';

							// update field and sort params
							meta = {field: field, sort: sort};
							Plugin.setDataSourceParam('sort', meta);

							sortObj.setIcon();

							setTimeout(function() {
								Plugin.dataRender('sort');
								$(datatable).trigger(pfx + 'datatable--on-sort', meta);
							}, 300);
						}
					},
				};
				sortObj.init();
			},

			/**
			 * Update JSON data list linked with sort, filter and pagination.
			 * Call this method, before using dataSet variable.
			 * @returns {*|null}
			 */
			localDataUpdate: function() {
				var params = Plugin.getDataSourceParam();
				if (typeof datatable.originalDataSet === 'undefined') {
					datatable.originalDataSet = datatable.dataSet;
				}

				var field = Plugin.getObject('sort.field', params);
				var sort = Plugin.getObject('sort.sort', params);
				var column = Plugin.getColumnByField(field);
				if (typeof column !== 'undefined' && Plugin.getOption('data.serverSorting') !== true) {
					if (typeof column.sortCallback === 'function') {
						datatable.dataSet = column.sortCallback(datatable.originalDataSet, sort, column);
					} else {
						datatable.dataSet = Plugin.sortCallback(datatable.originalDataSet, sort, column);
					}
				} else {
					datatable.dataSet = datatable.originalDataSet;
				}

				// if server filter enable, don't pass local filter
				if (typeof params.query === 'object' && !Plugin.getOption('data.serverFiltering')) {
					params.query = params.query || {};

					var nestedSearch = function(obj) {
						for (var field in obj) {
							if (!obj.hasOwnProperty(field)) continue;
							if (typeof obj[field] === 'string') {
								if (obj[field].toLowerCase() == search || obj[field].toLowerCase().indexOf(search) !== -1) {
									return true;
								}
							} else if (typeof obj[field] === 'number') {
								if (obj[field] === search) {
									return true;
								}
							} else if (typeof obj[field] === 'object') {
								if (nestedSearch(obj[field])) {
									return true;
								}
							}
						}
						return false;
					};

					var search = $(Plugin.getOption('search.input')).val();
					if (typeof search !== 'undefined' && search !== '') {
						search = search.toLowerCase();
						datatable.dataSet = $.grep(datatable.dataSet, nestedSearch);
						// remove generalSearch as we don't need this for next columns filter
						delete params.query[Plugin.getGeneralSearchKey()];
					}

					// remove empty element from array
					$.each(params.query, function(k, v) {
						if (v === '') {
							delete params.query[k];
						}
					});

					// filter array by query
					datatable.dataSet = Plugin.filterArray(datatable.dataSet, params.query);

					// reset array index
					datatable.dataSet = datatable.dataSet.filter(function() {
						return true;
					});
				}

				return datatable.dataSet;
			},

			/**
			 * Utility helper to filter array by object pair of {key:value}
			 * @param list
			 * @param args
			 * @param operator
			 * @returns {*}
			 */
			filterArray: function(list, args, operator) {
				if (typeof list !== 'object') {
					return [];
				}

				if (typeof operator === 'undefined') operator = 'AND';

				if (typeof args !== 'object') {
					return list;
				}

				operator = operator.toUpperCase();

				if ($.inArray(operator, ['AND', 'OR', 'NOT']) === -1) {
					return [];
				}

				var count = Object.keys(args).length;
				var filtered = [];

				$.each(list, function(key, obj) {
					var to_match = obj;

					var matched = 0;
					$.each(args, function(m_key, m_value) {
						m_value = m_value instanceof Array ? m_value : [m_value];
						var match_property = Plugin.getObject(m_key, to_match);
						if (typeof match_property !== 'undefined' && match_property) {
							var lhs = match_property.toString().toLowerCase();
							m_value.forEach(function(item, index) {
								if (item.toString().toLowerCase() == lhs || lhs.indexOf(item.toString().toLowerCase()) !== -1) {
									matched++;
								}
							});
						}
					});

					if (('AND' == operator && matched == count) ||
						('OR' == operator && matched > 0) ||
						('NOT' == operator && 0 == matched)) {
						filtered[key] = obj;
					}
				});

				list = filtered;

				return list;
			},

			/**
			 * Reset lock column scroll to 0 when resize
			 */
			resetScroll: function() {
				if (typeof options.detail === 'undefined' && Plugin.getDepth() === 1) {
					$(datatable.table).find('.' + pfx + 'datatable__row').css('left', 0);
					$(datatable.table).find('.' + pfx + 'datatable__lock').css('top', 0);
					$(datatable.tableBody).scrollTop(0);
				}
			},

			/**
			 * Get column options by field
			 * @param field
			 * @returns {boolean}
			 */
			getColumnByField: function(field) {
				if (typeof field === 'undefined') return;
				var result;
				$.each(options.columns, function(i, column) {
					if (field === column.field) {
						result = column;
						return false;
					}
				});
				return result;
			},

			/**
			 * Get default sort column
			 */
			getDefaultSortColumn: function() {
				var result;
				$.each(options.columns, function(i, column) {
					if (typeof column.sortable !== 'undefined'
						&& $.inArray(column.sortable, ['asc', 'desc']) !== -1) {
						result = {sort: column.sortable, field: column.field};
						return false;
					}
				});
				return result;
			},

			/**
			 * Helper to get element dimensions, when the element is hidden
			 * @param element
			 * @param includeMargin
			 * @returns {{width: number, height: number, innerWidth: number,
			 *     innerHeight: number, outerWidth: number, outerHeight:
			 *     number}}
			 */
			getHiddenDimensions: function(element, includeMargin) {
				var props = {
						position: 'absolute',
						visibility: 'hidden',
						display: 'block',
					},
					dim = {
						width: 0,
						height: 0,
						innerWidth: 0,
						innerHeight: 0,
						outerWidth: 0,
						outerHeight: 0,
					},
					hiddenParents = $(element).parents().addBack().not(':visible');
				includeMargin = (typeof includeMargin === 'boolean')
					? includeMargin
					: false;

				var oldProps = [];
				hiddenParents.each(function() {
					var old = {};

					for (var name in props) {
						old[name] = this.style[name];
						this.style[name] = props[name];
					}

					oldProps.push(old);
				});

				dim.width = $(element).width();
				dim.outerWidth = $(element).outerWidth(includeMargin);
				dim.innerWidth = $(element).innerWidth();
				dim.height = $(element).height();
				dim.innerHeight = $(element).innerHeight();
				dim.outerHeight = $(element).outerHeight(includeMargin);

				hiddenParents.each(function(i) {
					var old = oldProps[i];
					for (var name in props) {
						this.style[name] = old[name];
					}
				});

				return dim;
			},

			getGeneralSearchKey: function() {
				var searchInput = $(Plugin.getOption('search.input'));
				return $(searchInput).prop('name') || $(searchInput).prop('id');
			},

			/**
			 * Get value by dot notation path string and to prevent undefined
			 * errors
			 * @param path String Dot notation path in string
			 * @param object Object to iterate
			 * @returns {*}
			 */
			getObject: function(path, object) {
				return path.split('.').reduce(function(obj, i) {
					return obj !== null && typeof obj[i] !== 'undefined' ? obj[i] : null;
				}, object);
			},

			/**
			 * Extend object
			 * @param obj
			 * @param path
			 * @param value
			 * @returns {*}
			 */
			extendObj: function(obj, path, value) {
				var levels = path.split('.'),
					i = 0;

				function createLevel(child) {
					var name = levels[i++];
					if (typeof child[name] !== 'undefined' && child[name] !== null) {
						if (typeof child[name] !== 'object' &&
							typeof child[name] !== 'function') {
							child[name] = {};
						}
					} else {
						child[name] = {};
					}
					if (i === levels.length) {
						child[name] = value;
					} else {
						createLevel(child[name]);
					}
				}

				createLevel(obj);
				return obj;
			},

			rowEvenOdd: function() {
				// row even class
				$(datatable.tableBody).find('.' + pfx + 'datatable__row').removeClass(pfx + 'datatable__row--even');
				if ($(datatable.wrap).hasClass(pfx + 'datatable--subtable')) {
					$(datatable.tableBody).find('.' + pfx + 'datatable__row:not(.' + pfx + 'datatable__row-detail):even').addClass(pfx + 'datatable__row--even');
				} else {
					$(datatable.tableBody).find('.' + pfx + 'datatable__row:nth-child(even)').addClass(pfx + 'datatable__row--even');
				}
			},

			/********************
			 ** PUBLIC API METHODS
			 ********************/

			// delay timer
			timer: 0,

			/**
			 * Redraw datatable by recalculating its DOM elements, etc.
			 * @returns {jQuery}
			 */
			redraw: function() {
				Plugin.adjustCellsWidth.call();
				if (Plugin.isLocked()) {
					// fix hiding cell width issue
					Plugin.scrollbar();
					Plugin.resetScroll();
					Plugin.adjustCellsHeight.call();
				}
				Plugin.adjustLockContainer.call();
				Plugin.initHeight.call();
				return datatable;
			},

			/**
			 * Shortcode to reload
			 * @returns {jQuery}
			 */
			load: function() {
				Plugin.reload();
				return datatable;
			},

			/**
			 * Datasource reload
			 * @returns {jQuery}
			 */
			reload: function() {
				var delay = (function() {
					return function(callback, ms) {
						clearTimeout(Plugin.timer);
						Plugin.timer = setTimeout(callback, ms);
					};
				})();
				delay(function() {
					// local only. remote pagination will skip this block
					if (!options.data.serverFiltering) {
						Plugin.localDataUpdate();
					}
					Plugin.dataRender();
					$(datatable).trigger(pfx + 'datatable--on-reloaded');
				}, Plugin.getOption('search.delay'));
				return datatable;
			},

			/**
			 * Get record by record ID
			 * @param id
			 * @returns {jQuery}
			 */
			getRecord: function(id) {
				if (typeof datatable.tableBody === 'undefined') datatable.tableBody = $(datatable.table).children('tbody');
				$(datatable.tableBody).find('.' + pfx + 'datatable__cell:first-child').each(function(i, cell) {
					if (id == $(cell).text()) {
						var rowNumber = $(cell).closest('.' + pfx + 'datatable__row').index() + 1;
						datatable.API.record = datatable.API.value = Plugin.getOneRow(datatable.tableBody, rowNumber);
						return datatable;
					}
				});
				return datatable;
			},

			/**
			 * @deprecated in v5.0.6
			 * Get column of current record ID
			 * @param columnName
			 * @returns {jQuery}
			 */
			getColumn: function(columnName) {
				Plugin.setSelectedRecords();
				datatable.API.value = $(datatable.API.record).find('[data-field="' + columnName + '"]');
				return datatable;
			},

			/**
			 * Destroy datatable to original DOM state before datatable was
			 * initialized
			 * @returns {jQuery}
			 */
			destroy: function() {
				$(datatable).parent().find('.' + pfx + 'datatable__pager').remove();
				var initialDatatable = $(datatable.initialDatatable).addClass(pfx + 'datatable--destroyed').show();
				$(datatable).replaceWith(initialDatatable);
				datatable = initialDatatable;
				$(datatable).trigger(pfx + 'datatable--on-destroy');
				Plugin.isInit = false;
				initialDatatable = null;
				return initialDatatable;
			},

			/**
			 * Sort by column field
			 * @param field
			 * @param sort
			 */
			sort: function(field, sort) {
				// toggle sort
				sort = typeof sort === 'undefined' ? 'asc' : sort;

				Plugin.spinnerCallback(true);

				// update field and sort params
				var meta = {field: field, sort: sort};
				Plugin.setDataSourceParam('sort', meta);

				setTimeout(function() {
					Plugin.dataRender('sort');
					$(datatable).trigger(pfx + 'datatable--on-sort', meta);
					$(datatable.tableHead).find('.' + pfx + 'datatable__cell > span > i').remove();
				}, 300);

				return datatable;
			},

			/**
			 * @deprecated in v5.0.6
			 * Get current selected column value
			 * @returns {jQuery}
			 */
			getValue: function() {
				return $(datatable.API.value).text();
			},

			/**
			 * Set checkbox active
			 * @param cell JQuery selector or checkbox ID
			 */
			setActive: function(cell) {
				if (typeof cell === 'string') {
					// set by checkbox id
					cell = $(datatable.tableBody).find('.' + pfx + 'checkbox--single > [type="checkbox"][value="' + cell + '"]');
				}

				$(cell).prop('checked', true);

				var ids = [];
				$(cell).each(function(i, td) {
					// normal table
					var row = $(td).closest('tr').addClass(pfx + 'datatable__row--active');
					var colIndex = $(row).index() + 1;

					// lock table
					$(row).closest('tbody').find('tr:nth-child(' + colIndex + ')').not('.' + pfx + 'datatable__row-subtable').addClass(pfx + 'datatable__row--active');

					var id = $(td).attr('value');
					if (typeof id !== 'undefined') {
						ids.push(id);
					}
				});

				$(datatable).trigger(pfx + 'datatable--on-check', [ids]);
			},

			/**
			 * Set checkbox inactive
			 * @param cell JQuery selector or checkbox ID
			 */
			setInactive: function(cell) {
				if (typeof cell === 'string') {
					// set by checkbox id
					cell = $(datatable.tableBody).find('.' + pfx + 'checkbox--single > [type="checkbox"][value="' + cell + '"]');
				}

				$(cell).prop('checked', false);

				var ids = [];
				$(cell).each(function(i, td) {
					// normal table
					var row = $(td).closest('tr').removeClass(pfx + 'datatable__row--active');
					var colIndex = $(row).index() + 1;

					// lock table
					$(row).closest('tbody').find('tr:nth-child(' + colIndex + ')').not('.' + pfx + 'datatable__row-subtable').removeClass(pfx + 'datatable__row--active');

					var id = $(td).attr('value');
					if (typeof id !== 'undefined') {
						ids.push(id);
					}
				});

				$(datatable).trigger(pfx + 'datatable--on-uncheck', [ids]);
			},

			/**
			 * Set all checkboxes active or inactive
			 * @param active
			 */
			setActiveAll: function(active) {
				var checkboxes = $(datatable.table).
					find('> tbody, > thead').
					find('tr').not('.' + pfx + 'datatable__row-subtable').
					find('.' + pfx + 'datatable__cell--check [type="checkbox"]');
				if (active) {
					Plugin.setActive(checkboxes);
				} else {
					Plugin.setInactive(checkboxes);
				}
			},

			/**
			 * @deprecated in v5.0.6
			 * Get selected rows which are active
			 * @returns {jQuery}
			 */
			setSelectedRecords: function() {
				datatable.API.record = $(datatable.tableBody).find('.' + pfx + 'datatable__row--active');
				return datatable;
			},

			/**
			 * Get selected records
			 * @returns {null}
			 */
			getSelectedRecords: function() {
				// support old method
				Plugin.setSelectedRecords();
				datatable.API.record = datatable.rows('.' + pfx + 'datatable__row--active').nodes();
				return datatable.API.record;
			},

			/**
			 * Get options by dots notation path
			 * @param path String Dot notation path in string
			 * @returns {*}
			 */
			getOption: function(path) {
				return Plugin.getObject(path, options);
			},

			/**
			 * Set global options nodes by dots notation path
			 * @param path
			 * @param object
			 */
			setOption: function(path, object) {
				options = Plugin.extendObj(options, path, object);
			},

			/**
			 * Search filter for local & remote
			 * @param value
			 * @param columns. Optional list of columns to be filtered.
			 */
			search: function(value, columns) {
				if (typeof columns !== 'undefined') columns = $.makeArray(columns);
				var delay = (function() {
					return function(callback, ms) {
						clearTimeout(Plugin.timer);
						Plugin.timer = setTimeout(callback, ms);
					};
				})();

				delay(function() {
					// get query parameters
					var query = Plugin.getDataSourceQuery();

					// search not by columns
					if (typeof columns === 'undefined' && typeof value !== 'undefined') {
						var key = Plugin.getGeneralSearchKey();
						query[key] = value;
					}

					// search by columns, support multiple columns
					if (typeof columns === 'object') {
						$.each(columns, function(k, column) {
							query[column] = value;
						});
						// remove empty element from arrays
						$.each(query, function(k, v) {
							if (v === '' || $.isEmptyObject(v)) {
								delete query[k];
							}
						});
					}

					Plugin.setDataSourceQuery(query);

					// reset pagination to 1 when doing seearching
					datatable.setDataSourceParam('pagination', Object.assign({}, datatable.getDataSourceParam('pagination'), {page: 1}));

					// local filter only. remote pagination will skip this block
					if (!options.data.serverFiltering) {
						Plugin.localDataUpdate();
					}
					Plugin.dataRender('search');
				}, Plugin.getOption('search.delay'));
			},

			/**
			 * Set datasource params extract
			 * @param param
			 * @param value
			 */
			setDataSourceParam: function(param, value) {
				datatable.API.params = $.extend({}, {
					pagination: {page: 1, perpage: Plugin.getOption('data.pageSize')},
					sort: Plugin.getDefaultSortColumn(),
					query: {},
				}, datatable.API.params, Plugin.stateGet(Plugin.stateId));

				datatable.API.params = Plugin.extendObj(datatable.API.params, param, value);

				Plugin.stateKeep(Plugin.stateId, datatable.API.params);
			},

			/**
			 * Get datasource params
			 * @param param
			 */
			getDataSourceParam: function(param) {
				datatable.API.params = $.extend({}, {
					pagination: {page: 1, perpage: Plugin.getOption('data.pageSize')},
					sort: Plugin.getDefaultSortColumn(),
					query: {},
				}, datatable.API.params, Plugin.stateGet(Plugin.stateId));

				if (typeof param === 'string') {
					return Plugin.getObject(param, datatable.API.params);
				}

				return datatable.API.params;
			},

			/**
			 * Shortcode to datatable.getDataSourceParam('query');
			 * @returns {*}
			 */
			getDataSourceQuery: function() {
				return Plugin.getDataSourceParam('query') || {};
			},

			/**
			 * Shortcode to datatable.setDataSourceParam('query', query);
			 * @param query
			 */
			setDataSourceQuery: function(query) {
				Plugin.setDataSourceParam('query', query);
			},

			/**
			 * Get current page number
			 * @returns {number}
			 */
			getCurrentPage: function() {
				return $(datatable.table).
					siblings('.' + pfx + 'datatable__pager').
					last().
					find('.' + pfx + 'datatable__pager-nav').
					find('.' + pfx + 'datatable__pager-link.' + pfx + 'datatable__pager-link--active').
					data('page') || 1;
			},

			/**
			 * Get selected dropdown page size
			 * @returns {*|number}
			 */
			getPageSize: function() {
				return $(datatable.table).siblings('.' + pfx + 'datatable__pager').last().find('select.' + pfx + 'datatable__pager-size').val() || 10;
			},

			/**
			 * Get total rows
			 */
			getTotalRows: function() {
				return datatable.API.params.pagination.total;
			},

			/**
			 * Get full dataset in grid
			 * @returns {*|null|Array}
			 */
			getDataSet: function() {
				return datatable.originalDataSet;
			},

			nodeTr: [],
			nodeTd: [],
			nodeCols: [],
			recentNode: [],

			table: function() {
				if (typeof datatable.table !== 'undefined') {
					return datatable.table;
				}
			},

			/**
			 * Select a single row from the table
			 * @param selector
			 * @returns {jQuery}
			 */
			row: function(selector) {
				Plugin.rows(selector);
				Plugin.nodeTr = Plugin.recentNode = $(Plugin.nodeTr).first();
				return datatable;
			},

			/**
			 * Select multiple rows from the table
			 * @param selector
			 * @returns {jQuery}
			 */
			rows: function(selector) {
				if (Plugin.isLocked()) {
					Plugin.nodeTr = Plugin.recentNode = $(datatable.tableBody).find(selector).filter('.' + pfx + 'datatable__lock--scroll > .' + pfx + 'datatable__row');
				} else {
					Plugin.nodeTr = Plugin.recentNode = $(datatable.tableBody).find(selector).filter('.' + pfx + 'datatable__row');
				}
				return datatable;
			},

			/**
			 * Select a single column from the table
			 * @param index zero-based index
			 * @returns {jQuery}
			 */
			column: function(index) {
				Plugin.nodeCols = Plugin.recentNode = $(datatable.tableBody).find('.' + pfx + 'datatable__cell:nth-child(' + (index + 1) + ')');
				return datatable;
			},

			/**
			 * Select multiple columns from the table
			 * @param selector
			 * @returns {jQuery}
			 */
			columns: function(selector) {
				var context = datatable.table;
				if (Plugin.nodeTr === Plugin.recentNode) {
					context = Plugin.nodeTr;
				}
				var columns = $(context).find('.' + pfx + 'datatable__cell[data-field="' + selector + '"]');
				if (columns.length > 0) {
					Plugin.nodeCols = Plugin.recentNode = columns;
				} else {
					Plugin.nodeCols = Plugin.recentNode = $(context).find(selector).filter('.' + pfx + 'datatable__cell');
				}
				return datatable;
			},

			cell: function(selector) {
				Plugin.cells(selector);
				Plugin.nodeTd = Plugin.recentNode = $(Plugin.nodeTd).first();
				return datatable;
			},

			cells: function(selector) {
				var cells = $(datatable.tableBody).find('.' + pfx + 'datatable__cell');
				if (typeof selector !== 'undefined') {
					cells = $(cells).filter(selector);
				}
				Plugin.nodeTd = Plugin.recentNode = cells;
				return datatable;
			},

			/**
			 * Delete the selected row from the table
			 * @returns {jQuery}
			 */
			remove: function() {
				if ($(Plugin.nodeTr.length) && Plugin.nodeTr === Plugin.recentNode) {
					$(Plugin.nodeTr).remove();
				}
				Plugin.layoutUpdate();
				return datatable;
			},

			/**
			 * Show or hide the columns or rows
			 */
			visible: function(bool) {
				if ($(Plugin.recentNode.length)) {
					var locked = Plugin.lockEnabledColumns();
					if (Plugin.recentNode === Plugin.nodeCols) {
						var index = Plugin.recentNode.index();

						if (Plugin.isLocked()) {
							var scrollColumns = $(Plugin.recentNode).closest('.' + pfx + 'datatable__lock--scroll').length;
							if (scrollColumns) {
								// is at center of scrollable area
								index += locked.left.length + 1;
							} else if ($(Plugin.recentNode).closest('.' + pfx + 'datatable__lock--right').length) {
								// is at the right locked table
								index += locked.left.length + scrollColumns + 1;
							}
						}
					}

					if (bool) {
						if (Plugin.recentNode === Plugin.nodeCols) {
							delete options.columns[index-1].visible;
						}
						$(Plugin.recentNode).show();
					} else {
						if (Plugin.recentNode === Plugin.nodeCols) {
							Plugin.setOption('columns.' + (index-1) + '.visible', false);
						}
						$(Plugin.recentNode).hide();
					}
					Plugin.columnHide();
					Plugin.redraw();
				}
			},

			/**
			 * Get the the DOM element for the selected rows or columns
			 * @returns {Array}
			 */
			nodes: function() {
				return Plugin.recentNode;
			},

			/**
			 * will be implemented soon
			 * @returns {jQuery}
			 */
			dataset: function() {
				return datatable;
			},

			/**
			 * Open page by number
			 * @param page number
			 */
			gotoPage: function (page) {
				if (typeof Plugin.pagingObject !== 'undefined') {
					Plugin.isInit = true;
					Plugin.pagingObject.openPage(page);
				}
			},

		};

		/**
		 * Public API methods can be used directly by datatable
		 */
		$.each(Plugin, function(funcName, func) {
			datatable[funcName] = func;
		});

		// initialize main datatable plugin
		if (typeof options !== 'undefined') {
			if (typeof options === 'string') {
				var method = options;
				datatable = $(this).data(pluginName);
				if (typeof datatable !== 'undefined') {
					options = datatable.options;
					Plugin[method].apply(this, Array.prototype.slice.call(arguments, 1));
				}
			} else {
				if (!datatable.data(pluginName) && !$(this).hasClass(pfx + 'datatable--loaded')) {
					datatable.dataSet = null;
					datatable.textAlign = {
						left: pfx + 'datatable__cell--left',
						center: pfx + 'datatable__cell--center',
						right: pfx + 'datatable__cell--right',
					};

					// merge default and user defined options
					options = $.extend(true, {}, $.fn[pluginName].defaults, options);

					datatable.options = options;

					// init plugin process
					Plugin.init.apply(this, [options]);

					$(datatable.wrap).data(pluginName, datatable);
				}
			}
		} else {
			// get existing instance datatable
			datatable = $(this).data(pluginName);
			if (typeof datatable === 'undefined') {
				$.error(pluginName + ' not initialized');
			}
			options = datatable.options;
		}

		return datatable;
	};

	// default options
	$.fn[pluginName].defaults = {
		// datasource definition
		data: {
			type: 'local',
			source: null,
			pageSize: 10, // display records per page
			saveState: {
				// save datatable state(pagination, filtering, sorting, etc) in cookie or browser webstorage
				cookie: false,
				webstorage: true,
			},

			serverPaging: false,
			serverFiltering: false,
			serverSorting: false,

			autoColumns: false,
			attr: {
				rowProps: [],
			},
		},

		// layout definition
		layout: {
			theme: 'default', // datatable will support multiple themes and designs
			class: pfx + 'datatable--brand', // custom wrapper class
			scroll: false, // enable/disable datatable scroll both horizontal and vertical when needed.
			height: null, // datatable's body's fixed height
			minHeight: null,
			footer: false, // display/hide footer
			header: true, // display/hide header
			customScrollbar: true, // set false to disable custom scrollbar

			// datatable spinner
			spinner: {
				overlayColor: '#000000',
				opacity: 0,
				type: 'loader',
				state: 'brand',
				message: true,
			},

			// datatable UI icons
			icons: {
				sort: {asc: 'flaticon2-arrow-up', desc: 'flaticon2-arrow-down'},
				pagination: {
					next: 'flaticon2-next',
					prev: 'flaticon2-back',
					first: 'flaticon2-fast-back',
					last: 'flaticon2-fast-next',
					more: 'flaticon-more-1',
				},
				rowDetail: {expand: 'fa fa-caret-down', collapse: 'fa fa-caret-right'},
			},
		},

		// column sorting
		sortable: true,

		// resize column size with mouse drag coming soon)
		resizable: false,

		// column based filtering (coming soon)
		filterable: false,

		pagination: true,

		// inline and bactch editing (cooming soon)
		editable: false,

		// columns definition
		columns: [],

		search: {
			// enable trigger search by keyup enter
			onEnter: false,
			// input text for search
			input: null,
			// search delay in milliseconds
			delay: 400,
		},

		rows: {
			// deprecated
			callback: function() {
			},
			// call before row template
			beforeTemplate: function() {
			},
			// call after row template
			afterTemplate: function() {
			},
			autoHide: true,
		},

		// toolbar
		toolbar: {
			// place pagination and displayInfo blocks according to the array order
			layout: ['pagination', 'info'],

			// toolbar placement can be at top or bottom or both top and bottom repeated
			placement: ['bottom'],  //'top', 'bottom'

			// toolbar items
			items: {
				// pagination
				pagination: {
					// pagination type(default or scroll)
					type: 'default',

					// number of pages to display by breakpoints
					pages: {
						desktop: {
							layout: 'default',
							pagesNumber: 5,
						},
						tablet: {
							layout: 'default',
							pagesNumber: 3,
						},
						mobile: {
							layout: 'compact',
						},
					},

					// navigation buttons
					navigation: {
						prev: true, // display prev button
						next: true, // display next button
						first: true, // display first button
						last: true, // display last button
						more: false // display more button
					},

					// page size select
					pageSizeSelect: [], // display dropdown to select pagination size. -1 is used for "ALl" option
				},

				// records info
				info: true,
			},
		},

		// here we will keep all strings and message used by datatable UI so developer can easiliy translate to any language.
		// By default the stirngs will be in the plugin source and here can override it
		translate: {
			records: {
				processing: 'Please wait...',
				noRecords: 'No records found',
			},
			toolbar: {
				pagination: {
					items: {
						default: {
							first: 'First',
							prev: 'Previous',
							next: 'Next',
							last: 'Last',
							more: 'More pages',
							input: 'Page number',
							select: 'Select page size',
							all: 'all',
						},
						info: 'Showing {{start}} - {{end}} of {{total}}',
					},
				},
			},
		},

		extensions: {},
	};

}(jQuery));