// TEMP map-sidebar requires ol.inherits which was removed in ol v6.0
if (window.ol && !window.ol.inherits) {
    ol.inherits = function inherits(childCtor, parentCtor) {
        childCtor.prototype = Object.create(parentCtor.prototype);
        childCtor.prototype.constructor = childCtor;
    };
}

class TurboSidebar extends ol.control.Control {
    constructor(opt_options) {
        const defaults = {
            element: null,
            position: 'left'
        };
        const options = Object.assign({}, defaults, opt_options);
        const element = document.getElementById(options.element);
        super({
            element: element,
            target: options.target,
        });

        element.classList.add('sidebar-' + options.position);

        this._container = Array.from(element.children).find(child => child.tagName === 'DIV' && child.classList.contains('turbosidebar-content'));

        this._tabitems = Array.from(this.element.querySelectorAll('ul.turbosidebar-tabs > li, .turbosidebar-tabs > ul > li')).map(child => {
            child._sidebar = this;
            return child;
        });

        this._panes = [];
        this._closeButtons = [];
        Array.from(this._container.children).forEach(child => {
            if (child.tagName === 'DIV' && child.classList.contains('turbosidebar-pane')) {
                this._panes.push(child);
                this._closeButtons.push(...Array.from(child.querySelectorAll('.turbosidebar-close')));
            }
        });
    }

    setMap() {
        this._tabitems.forEach(child => {
            const sub = child.querySelector('a');
            if (sub.hasAttribute('href') && sub.getAttribute('href').slice(0, 1) === '#') {
                sub.onclick = this._onClick.bind(child);
            }
        });

        this._closeButtons.forEach(child => {
            child.onclick = this._onCloseClick.bind(this);
        });
    }

    open(id) {
        this._panes.forEach(child => {
            if (child.id === id) {
                child.classList.add('active');
            } else if (child.classList.contains('active')) {
                child.classList.remove('active');
            }
        });

        this._tabitems.forEach(child => {
            if (child.querySelector('a').hash === `#${id}`) {
                child.classList.add('active');
            } else if (child.classList.contains('active')) {
                child.classList.remove('active');
            }
        });

        if (this.element.classList.contains('collapsed')) {
            this.element.classList.remove('collapsed');
        }

        return this;
    }

    close() {
        this._tabitems.forEach(child => {
            if (child.classList.contains('active')) {
                child.classList.remove('active');
            }
        });

        if (!this.element.classList.contains('collapsed')) {
            this.element.classList.add('collapsed');
        }

        return this;
    }

    _onClick(evt) {
        evt.preventDefault();
        if (this.classList.contains('active')) {
            this._sidebar.close();
        } else if (!this.classList.contains('disabled')) {
            this._sidebar.open(this.querySelector('a').hash.slice(1));
        }
    }

    _onCloseClick() {
        this.close();
    }
}

function createTurboSidebar(opt_options) {
    const customSideBar = new TurboSidebar(opt_options);
    return customSideBar;
}