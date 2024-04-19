/** Inherit the prototype methods from one constructor into another.
 * replace deprecated ol method
 *
 * @param {!Function} childCtor Child constructor.
 * @param {!Function} parentCtor Parent constructor.
 * @function module:ol.inherits
 * @api
 */
var ol_ext_inherits = function (child, parent) {
    child.prototype = Object.create(parent.prototype);
    child.prototype.constructor = child;
};

// Compatibilty with ol > 5 to be removed when v6 is out
if (window.ol) {
    if (!ol.inherits) ol.inherits = ol_ext_inherits;
}

/* IE Polyfill */
// NodeList.forEach
if (window.NodeList && !NodeList.prototype.forEach) {
    NodeList.prototype.forEach = Array.prototype.forEach;
}
// Element.remove
if (window.Element && !Element.prototype.remove) {
    Element.prototype.remove = function () {
        if (this.parentNode) this.parentNode.removeChild(this);
    }
}




//BASE BEGIN

//if (window.ol) {
//    ol.ext.input = {};
//}

/** Abstract base class; normally only used for creating subclasses and not instantiated in apps.    
 * @constructor
 * @extends {ol.Object}
 * @param {*} options
 *  @param {Element} [options.input] input element, if none create one
 *  @param {string} [options.type] input type, if no input
 *  @param {number} [options.min] input min, if no input
 *  @param {number} [options.max] input max, if no input
 *  @param {number} [options.step] input step, if no input
 *  @param {string|number} [options.val] input value
 *  @param {boolean} [options.checked] check input
 *  @param {boolean} [options.hidden] the input is display:none
 *  @param {boolean} [options.disabled] disable input
 *  @param {Element} [options.parent] parent element, if no input
 */
var ol_ext_input_Base = function (options) {
    options = options || {};

    ol.Object.call(this);

    var input = this.input = options.input;
    if (!input) {
        input = this.input = document.createElement('INPUT');
        if (options.type) input.setAttribute('type', options.type);
        if (options.min !== undefined) input.setAttribute('min', options.min);
        if (options.max !== undefined) input.setAttribute('max', options.max);
        if (options.step !== undefined) input.setAttribute('step', options.step);
        if (options.parent) options.parent.appendChild(input);
    }
    if (options.disabled) input.disabled = true;
    if (options.checked !== undefined) input.checked = !!options.checked;
    if (options.val !== undefined) input.value = options.val;
    if (options.hidden) input.style.display = 'none';
    input.addEventListener('focus', function () {
        if (this.element) this.element.classList.add('ol-focus');
    }.bind(this))
    var tout;
    input.addEventListener('focusout', function () {
        if (this.element) {
            if (tout) clearTimeout(tout);
            tout = setTimeout(function () {
                this.element.classList.remove('ol-focus');
            }.bind(this), 0);
        }
    }.bind(this))
};
ol_ext_inherits(ol_ext_input_Base, ol.Object);

/** Listen to drag event
 * @param {Element} elt 
 * @param {function} cback when draggin on the element
 * @private
 */
ol_ext_input_Base.prototype._listenDrag = function (elt, cback) {
    var handle = function (e) {
        this.moving = true;
        var listen = function (e) {
            if (e.type === 'pointerup') {
                document.removeEventListener('pointermove', listen);
                document.removeEventListener('pointerup', listen);
                document.removeEventListener('pointercancel', listen);
                setTimeout(function () {
                    this.moving = false;
                }.bind(this));
            }
            if (e.target === elt) cback(e);
            e.stopPropagation();
            e.preventDefault();
        }.bind(this);
        document.addEventListener('pointermove', listen, false);
        document.addEventListener('pointerup', listen, false);
        document.addEventListener('pointercancel', listen, false);
        e.stopPropagation();
        e.preventDefault();
    }.bind(this)
    elt.addEventListener('mousedown', handle, false);
    elt.addEventListener('touchstart', handle, false);
};

/** Set the current value
 */
ol_ext_input_Base.prototype.setValue = function (v) {
    if (v !== undefined) this.input.value = v;
    this.input.dispatchEvent(new Event('change'));
};

/** Get the current getValue
 * @returns {string}
 */
ol_ext_input_Base.prototype.getValue = function () {
    return this.input.value;
};

/** Get the input element
 * @returns {Element}
 */
ol_ext_input_Base.prototype.getInputElement = function () {
    return this.input;
};

//BASE END



// CHECKBOX BEGIN

/** Checkbox input
 * @constructor
 * @extends {ol_ext_input_Base}
 * @fires check
 * @param {*} options
 *  @param {string} [options.className]
 *  @param {Element|string} [options.html] label content
 *  @param {string} [options.after] label garnish (placed after)
 *  @param {Element} [options.input] input element, if non create one
 *  @param {Element} [options.parent] parent element, if create an input
 *  @param {boolean} [options.autoClose=true]
 *  @param {boolean} [options.visible=false] display the input
 */
var ol_ext_input_Checkbox = function (options) {
    options = options || {};

    ol_ext_input_Base.call(this, options);

    var label = this.element = document.createElement('LABEL');
    if (options.html instanceof Element) label.appendChild(options.html)
    else if (options.html !== undefined) label.innerHTML = options.html;
    label.className = ('ol-ext-check ol-ext-checkbox ' + (options.className || '')).trim();

    if (this.input.parentNode) this.input.parentNode.insertBefore(label, this.input);
    label.appendChild(this.input);
    label.appendChild(document.createElement('SPAN'));
    if (options.after) {
        label.appendChild(document.createTextNode(options.after));
    }

    // Handle change
    this.input.addEventListener('change', function () {
        this.dispatchEvent({ type: 'check', checked: this.input.checked, value: this.input.value });
    }.bind(this));

};
ol_ext_inherits(ol_ext_input_Checkbox, ol_ext_input_Base);

ol_ext_input_Checkbox.prototype.isChecked = function () {
    return this.input.checked;
};

// CHECKBOX END






//SWITCH BEGIN

/** Switch input
 * @constructor
 * @extends {ol_ext_input_Checkbox}
 * @fires check
 * @param {*} options
 *  @param {string} [options.className]
 *  @param {Element} [options.input] input element, if non create one
 *  @param {Element} [options.parent] parent element, if create an input
 */
var ol_ext_input_Switch = function (options) {
    options = options || {};

    ol_ext_input_Checkbox.call(this, options);

    this.element.className = ('ol-ext-toggle-switch ' + (options.className || '')).trim();
};
ol_ext_inherits(ol_ext_input_Switch, ol_ext_input_Checkbox);

//SWITCH END






//RADIO BEGIN

/** Switch input
 * @constructor
 * @extends {ol_ext_input_Checkbox}
 * @fires check
 * @param {*} options
 *  @param {string} [options.className]
 *  @param {Element} [options.input] input element, if non create one
 *  @param {Element} [options.parent] parent element, if create an input
 */
var ol_ext_input_Radio = function (options) {
    options = options || {};

    ol_ext_input_Checkbox.call(this, options);

    this.element.className = ('ol-ext-check ol-ext-radio ' + (options.className || '')).trim();
};
ol_ext_inherits(ol_ext_input_Radio, ol_ext_input_Checkbox);

//RADIO END







//ELEMENT BEGIN

/** @namespace ol.ext.element */
var ol_ext_element = {};

/**
 * Create an element
 * @param {string} tagName The element tag, use 'TEXT' to create a text node
 * @param {*} options
 *  @param {string} options.className className The element class name 
 *  @param {Element} options.parent Parent to append the element as child
 *  @param {Element|string} [options.html] Content of the element (if text is not set)
 *  @param {string} [options.text] Text content (if html is not set)
 *  @param {Element|string} [options.options] when tagName = SELECT a list of options as key:value to add to the select
 *  @param {string} options.* Any other attribut to add to the element
 */
ol_ext_element.create = function (tagName, options) {
    options = options || {};
    var elt;
    // Create text node
    if (tagName === 'TEXT') {
        elt = document.createTextNode(options.html || '');
        if (options.parent) options.parent.appendChild(elt);
    } else {
        // Other element
        elt = document.createElement(tagName);
        if (/button/i.test(tagName)) elt.setAttribute('type', 'button');
        for (var attr in options) {
            switch (attr) {
                case 'className': {
                    if (options.className && options.className.trim) elt.setAttribute('class', options.className.trim());
                    break;
                }
                case 'text': {
                    elt.innerText = options.text;
                    break;
                }
                case 'html': {
                    if (options.html instanceof Element) elt.appendChild(options.html)
                    else if (options.html !== undefined) elt.innerHTML = options.html;
                    break;
                }
                case 'parent': {
                    if (options.parent) options.parent.appendChild(elt);
                    break;
                }
                case 'options': {
                    if (/select/i.test(tagName)) {
                        for (var i in options.options) {
                            ol_ext_element.create('OPTION', {
                                html: i,
                                value: options.options[i],
                                parent: elt
                            })
                        }
                    }
                    break;
                }
                case 'style': {
                    this.setStyle(elt, options.style);
                    break;
                }
                case 'change':
                case 'click': {
                    ol_ext_element.addListener(elt, attr, options[attr]);
                    break;
                }
                case 'on': {
                    for (var e in options.on) {
                        ol_ext_element.addListener(elt, e, options.on[e]);
                    }
                    break;
                }
                case 'checked': {
                    elt.checked = !!options.checked;
                    break;
                }
                default: {
                    elt.setAttribute(attr, options[attr]);
                    break;
                }
            }
        }
    }
    return elt;
};

/** Create a toggle switch input
 * @param {*} options
 *  @param {string|Element} options.html
 *  @param {string|Element} options.after
 *  @param {boolean} options.checked
 *  @param {*} [options.on] a list of actions
 *  @param {function} [options.click]
 *  @param {function} [options.change]
 *  @param {Element} options.parent
 */
ol_ext_element.createSwitch = function (options) {
    var input = ol_ext_element.create('INPUT', {
        type: 'checkbox',
        on: options.on,
        click: options.click,
        change: options.change,
        parent: options.parent
    });
    var opt = Object.assign({ input: input }, options || {});
    new ol_ext_input_Switch(opt);
    return input;
};

/** Create a toggle switch input
 * @param {*} options
 *  @param {string|Element} options.html
 *  @param {string|Element} options.after
 *  @param {string} [options.name] input name
 *  @param {string} [options.type=checkbox] input type: radio or checkbox
 *  @param {string} options.value input value
 *  @param {*} [options.on] a list of actions
 *  @param {function} [options.click]
 *  @param {function} [options.change]
 *  @param {Element} options.parent
 */
ol_ext_element.createCheck = function (options) {
    var input = ol_ext_element.create('INPUT', {
        name: options.name,
        type: (options.type === 'radio' ? 'radio' : 'checkbox'),
        on: options.on,
        parent: options.parent
    });
    console.log(input)
    var opt = Object.assign({ input: input }, options || {});
    if (options.type === 'radio') {
        new ol_ext_input_Radio(opt);
    } else {
        new ol_ext_input_Checkbox(opt);
    }
    return input;
};

/** Set inner html or append a child element to an element
 * @param {Element} element
 * @param {Element|string} html Content of the element
 */
ol_ext_element.setHTML = function (element, html) {
    if (html instanceof Element) element.appendChild(html)
    else if (html !== undefined) element.innerHTML = html;
};

/** Append text into an elemnt
 * @param {Element} element
 * @param {string} text text content
 */
ol_ext_element.appendText = function (element, text) {
    element.appendChild(document.createTextNode(text || ''));
};

/**
 * Add a set of event listener to an element
 * @param {Element} element
 * @param {string|Array<string>} eventType
 * @param {function} fn
 */
ol_ext_element.addListener = function (element, eventType, fn, useCapture) {
    if (typeof eventType === 'string') eventType = eventType.split(' ');
    eventType.forEach(function (e) {
        element.addEventListener(e, fn, useCapture);
    });
};

/**
 * Add a set of event listener to an element
 * @param {Element} element
 * @param {string|Array<string>} eventType
 * @param {function} fn
 */
ol_ext_element.removeListener = function (element, eventType, fn) {
    if (typeof eventType === 'string') eventType = eventType.split(' ');
    eventType.forEach(function (e) {
        element.removeEventListener(e, fn);
    });
};

/**
 * Show an element
 * @param {Element} element
 */
ol_ext_element.show = function (element) {
    element.style.display = '';
};

/**
 * Hide an element
 * @param {Element} element
 */
ol_ext_element.hide = function (element) {
    element.style.display = 'none';
};

/**
 * Test if an element is hihdden
 * @param {Element} element
 * @return {boolean}
 */
ol_ext_element.hidden = function (element) {
    return ol_ext_element.getStyle(element, 'display') === 'none';
};

/**
 * Toggle an element
 * @param {Element} element
 */
ol_ext_element.toggle = function (element) {
    element.style.display = (element.style.display === 'none' ? '' : 'none');
};

/** Set style of an element
 * @param {DOMElement} el the element
 * @param {*} st list of style
 */
ol_ext_element.setStyle = function (el, st) {
    for (var s in st) {
        switch (s) {
            case 'top':
            case 'left':
            case 'bottom':
            case 'right':
            case 'minWidth':
            case 'maxWidth':
            case 'width':
            case 'height': {
                if (typeof (st[s]) === 'number') {
                    el.style[s] = st[s] + 'px';
                } else {
                    el.style[s] = st[s];
                }
                break;
            }
            default: {
                el.style[s] = st[s];
            }
        }
    }
};

/**
 * Get style propertie of an element
 * @param {DOMElement} el the element
 * @param {string} styleProp Propertie name
 * @return {*} style value
 */
ol_ext_element.getStyle = function (el, styleProp) {
    var value, defaultView = (el.ownerDocument || document).defaultView;
    // W3C standard way:
    if (defaultView && defaultView.getComputedStyle) {
        // sanitize property name to css notation
        // (hypen separated words eg. font-Size)
        styleProp = styleProp.replace(/([A-Z])/g, "-$1").toLowerCase();
        value = defaultView.getComputedStyle(el, null).getPropertyValue(styleProp);
    } else if (el.currentStyle) { // IE
        // sanitize property name to camelCase
        styleProp = styleProp.replace(/-(\w)/g, function (str, letter) {
            return letter.toUpperCase();
        });
        value = el.currentStyle[styleProp];
        // convert other units to pixels on IE
        if (/^\d+(em|pt|%|ex)?$/i.test(value)) {
            return (function (value) {
                var oldLeft = el.style.left, oldRsLeft = el.runtimeStyle.left;
                el.runtimeStyle.left = el.currentStyle.left;
                el.style.left = value || 0;
                value = el.style.pixelLeft + "px";
                el.style.left = oldLeft;
                el.runtimeStyle.left = oldRsLeft;
                return value;
            })(value);
        }
    }
    if (/px$/.test(value)) return parseInt(value);
    return value;
};

/** Get outerHeight of an elemen
 * @param {DOMElement} elt
 * @return {number}
 */
ol_ext_element.outerHeight = function (elt) {
    return elt.offsetHeight + ol_ext_element.getStyle(elt, 'marginBottom')
};

/** Get outerWidth of an elemen
 * @param {DOMElement} elt
 * @return {number}
 */
ol_ext_element.outerWidth = function (elt) {
    return elt.offsetWidth + ol_ext_element.getStyle(elt, 'marginLeft')
};

/** Get element offset rect
 * @param {DOMElement} elt
 * @return {*} 
 */
ol_ext_element.offsetRect = function (elt) {
    var rect = elt.getBoundingClientRect();
    return {
        top: rect.top + (window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0),
        left: rect.left + (window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft || 0),
        height: rect.height || (rect.bottom - rect.top),
        width: rect.width || (rect.right - rect.left)
    }
};

/** Get element offset 
 * @param {ELement} elt
 * @returns {Object} top/left offset
 */
ol_ext_element.getFixedOffset = function (elt) {
    var offset = {
        left: 0,
        top: 0
    };
    var getOffset = function (parent) {
        if (!parent) return offset;
        // Check position when transform
        if (ol_ext_element.getStyle(parent, 'position') === 'absolute'
            && ol_ext_element.getStyle(parent, 'transform') !== "none") {
            var r = parent.getBoundingClientRect();
            offset.left += r.left;
            offset.top += r.top;
            return offset;
        }
        return getOffset(parent.offsetParent)
    }
    return getOffset(elt.offsetParent)
};

/** Get element offset rect
 * @param {DOMElement} elt
 * @param {boolean} fixed get fixed position
 * @return {Object} 
 */
ol_ext_element.positionRect = function (elt, fixed) {
    var gleft = 0;
    var gtop = 0;

    var getRect = function (parent) {
        if (parent) {
            gleft += parent.offsetLeft;
            gtop += parent.offsetTop;
            return getRect(parent.offsetParent);
        } else {
            var r = {
                top: elt.offsetTop + gtop,
                left: elt.offsetLeft + gleft
            };
            if (fixed) {
                r.top -= (window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0);
                r.left -= (window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft || 0);
            }
            r.bottom = r.top + elt.offsetHeight;
            r.right = r.top + elt.offsetWidth;
            return r;
        }
    };
    return getRect(elt.offsetParent);
}

/** Make a div scrollable without scrollbar.
 * On touch devices the default behavior is preserved
 * @param {DOMElement} elt
 * @param {*} options
 *  @param {function} [options.onmove] a function that takes a boolean indicating that the div is scrolling
 *  @param {boolean} [options.vertical=false] 
 *  @param {boolean} [options.animate=true] add kinetic to scroll
 *  @param {boolean} [options.mousewheel=false] enable mousewheel to scroll
 *  @param {boolean} [options.minibar=false] add a mini scrollbar to the parent element (only vertical scrolling)
 * @returns {Object} an object with a refresh function
 */
ol_ext_element.scrollDiv = function (elt, options) {
    options = options || {};
    var pos = false;
    var speed = 0;
    var d, dt = 0;

    var onmove = (typeof (options.onmove) === 'function' ? options.onmove : function () { });
    //var page = options.vertical ? 'pageY' : 'pageX';
    var page = options.vertical ? 'screenY' : 'screenX';
    var scroll = options.vertical ? 'scrollTop' : 'scrollLeft';
    var moving = false;
    // Factor scale content / container
    var scale, isbar;

    // Update the minibar
    var updateCounter = 0;
    var updateMinibar = function () {
        if (scrollbar) {
            updateCounter++;
            setTimeout(updateMinibarDelay);
        }
    }
    var updateMinibarDelay = function () {
        if (scrollbar) {
            updateCounter--;
            // Prevent multi call
            if (updateCounter) return;
            // Container height
            var pheight = elt.clientHeight;
            // Content height
            var height = elt.scrollHeight;
            // Set scrollbar value
            scale = pheight / height;
            scrollbar.style.height = scale * 100 + '%';
            scrollbar.style.top = (elt.scrollTop / height * 100) + '%';
            scrollContainer.style.height = pheight + 'px';
            // No scroll
            if (pheight > height - .5) scrollContainer.classList.add('ol-100pc');
            else scrollContainer.classList.remove('ol-100pc');
        }
    }

    // Handle pointer down
    var onPointerDown = function (e) {
        // Prevent scroll
        if (e.target.classList.contains('ol-noscroll')) return;
        // Start scrolling
        moving = false;
        pos = e[page];
        dt = new Date();
        elt.classList.add('ol-move');
        // Prevent elt dragging
        e.preventDefault();
        // Listen scroll
        window.addEventListener('pointermove', onPointerMove);
        ol_ext_element.addListener(window, ['pointerup', 'pointercancel'], onPointerUp);
    }

    // Register scroll
    var onPointerMove = function (e) {
        moving = true;
        if (pos !== false) {
            var delta = (isbar ? -1 / scale : 1) * (pos - e[page]);
            elt[scroll] += delta;
            d = new Date();
            if (d - dt) {
                speed = (speed + delta / (d - dt)) / 2;
            }
            pos = e[page];
            dt = d;
            // Tell we are moving
            if (delta) onmove(true);
        }
    };

    // Animate scroll
    var animate = function (to) {
        var step = (to > 0) ? Math.min(100, to / 2) : Math.max(-100, to / 2);
        to -= step;
        elt[scroll] += step;
        if (-1 < to && to < 1) {
            if (moving) setTimeout(function () { elt.classList.remove('ol-move'); });
            else elt.classList.remove('ol-move');
            moving = false;
            onmove(false);
        } else {
            setTimeout(function () {
                animate(to);
            }, 40);
        }
    }

    // Initialize scroll container for minibar
    var scrollContainer, scrollbar;
    if (options.vertical && options.minibar) {
        var init = function (b) {
            // only once
            elt.removeEventListener('pointermove', init);
            elt.parentNode.classList.add('ol-miniscroll');
            scrollbar = ol_ext_element.create('DIV');
            scrollContainer = ol_ext_element.create('DIV', {
                className: 'ol-scroll',
                html: scrollbar
            });
            elt.parentNode.insertBefore(scrollContainer, elt);
            // Move scrollbar
            scrollbar.addEventListener('pointerdown', function (e) {
                isbar = true;
                onPointerDown(e)
            });
            // Handle mousewheel
            if (options.mousewheel) {
                ol_ext_element.addListener(scrollContainer,
                    ['mousewheel', 'DOMMouseScroll', 'onmousewheel'],
                    function (e) { onMouseWheel(e) }
                );
                ol_ext_element.addListener(scrollbar,
                    ['mousewheel', 'DOMMouseScroll', 'onmousewheel'],
                    function (e) { onMouseWheel(e) }
                );
            }
            // Update on enter
            elt.parentNode.addEventListener('pointerenter', updateMinibar);
            // Update on resize
            window.addEventListener('resize', updateMinibar);
            // Update
            if (b !== false) updateMinibar();
        };
        // Allready inserted in the DOM
        if (elt.parentNode) init(false);
        // or wait when ready
        else elt.addEventListener('pointermove', init);
        // Update on scroll
        elt.addEventListener('scroll', function () {
            updateMinibar();
        });
    }

    // Enable scroll
    elt.style['touch-action'] = 'none';
    elt.style['overflow'] = 'hidden';
    elt.classList.add('ol-scrolldiv');

    // Start scrolling
    ol_ext_element.addListener(elt, ['pointerdown'], function (e) {
        isbar = false;
        onPointerDown(e)
    });

    // Prevet click when moving...
    elt.addEventListener('click', function (e) {
        if (elt.classList.contains('ol-move')) {
            e.preventDefault();
            e.stopPropagation();
        }
    }, true);

    // Stop scrolling
    var onPointerUp = function (e) {
        dt = new Date() - dt;
        if (dt > 100 || isbar) {
            // User stop: no speed
            speed = 0;
        } else if (dt > 0) {
            // Calculate new speed
            speed = ((speed || 0) + (pos - e[page]) / dt) / 2;
        }
        animate(options.animate === false ? 0 : speed * 200);
        pos = false;
        speed = 0;
        dt = 0;
        // Add class to handle click (on iframe / double-click)
        if (!elt.classList.contains('ol-move')) {
            elt.classList.add('ol-hasClick')
            setTimeout(function () { elt.classList.remove('ol-hasClick'); }, 500);
        } else {
            elt.classList.remove('ol-hasClick');
        }
        isbar = false;
        window.removeEventListener('pointermove', onPointerMove)
        ol_ext_element.removeListener(window, ['pointerup', 'pointercancel'], onPointerUp);
    };

    // Handle mousewheel
    var onMouseWheel = function (e) {
        var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
        elt.classList.add('ol-move');
        elt[scroll] -= delta * 30;
        elt.classList.remove('ol-move');
        return false;
    }
    if (options.mousewheel) { // && !elt.classList.contains('ol-touch')) {
        ol_ext_element.addListener(elt,
            ['mousewheel', 'DOMMouseScroll', 'onmousewheel'],
            onMouseWheel
        );
    }

    return {
        refresh: updateMinibar
    }
};

/** Dispatch an event to an Element 
 * @param {string} eventName
 * @param {Element} element
 */
ol_ext_element.dispatchEvent = function (eventName, element) {
    var event;
    try {
        event = new CustomEvent(eventName);
    } catch (e) {
        // Try customevent on IE
        event = document.createEvent("CustomEvent");
        event.initCustomEvent(eventName, true, true, {});
    }
    element.dispatchEvent(event);
};


//ELEMENT END