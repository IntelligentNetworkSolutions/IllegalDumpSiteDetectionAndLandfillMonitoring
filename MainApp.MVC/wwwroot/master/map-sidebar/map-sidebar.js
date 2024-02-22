// TEMP map-sidebar requires ol.inherits which was removed in ol v6.0
if (window.ol && !window.ol.inherits) {
    ol.inherits = function inherits(childCtor, parentCtor) {
        childCtor.prototype = Object.create(parentCtor.prototype);
        childCtor.prototype.constructor = childCtor;
    };
}

function createNexusSidebar(opt_options) {
    class NexusSidebar extends ol.control.Control {
        /**
         * @param {Object} [opt_options] Control options.
         */
        constructor(opt_options) {
            var defaults = {
                element: null,
                position: 'left'
            }, i, child;

            const options = Object.assign({}, defaults, opt_options);
            const element = document.getElementById(options.element);
            super({
                element: element,
                target: options.target,
            });

            element.classList.add('sidebar-' + options.position);

            // Find sidebar > div.sidebar-content
            for (i = element.children.length - 1; i >= 0; i--) {
                child = element.children[i];
                if (child.tagName === 'DIV' && child.classList.contains('turbosidebar-content')) {
                    this._container = child;
                }
            }

            // Find sidebar ul.sidebar-tabs > li, sidebar .sidebar-tabs > ul > li
            this._tabitems = this.element.querySelectorAll('ul.turbosidebar-tabs > li, .turbosidebar-tabs > ul > li');
            for (i = this._tabitems.length - 1; i >= 0; i--) {
                this._tabitems[i]._sidebar = this;
            }

            // Find sidebar > div.sidebar-content > div.sidebar-pane
            this._panes = [];
            this._closeButtons = [];
            for (i = this._container.children.length - 1; i >= 0; i--) {
                child = this._container.children[i];
                if (child.tagName == 'DIV' &&
                    child.classList.contains('turbosidebar-pane')) {
                    this._panes.push(child);

                    var closeButtons = child.querySelectorAll('.turbosidebar-close');
                    for (var j = 0, len = closeButtons.length; j < len; j++) {
                        this._closeButtons.push(closeButtons[j]);
                    }
                }
            }
        }

        setMap(map) {
            var i, child;

            for (i = this._tabitems.length - 1; i >= 0; i--) {
                child = this._tabitems[i];
                var sub = child.querySelector('a');
                if (sub.hasAttribute('href') && sub.getAttribute('href').slice(0, 1) == '#') {
                    sub.onclick = this._onClick.bind(child);
                }
            }

            for (i = this._closeButtons.length - 1; i >= 0; i--) {
                child = this._closeButtons[i];
                child.onclick = this._onCloseClick.bind(this);
            }
        };

        open(id) {
            var i, child;

            // hide old active contents and show new content
            for (i = this._panes.length - 1; i >= 0; i--) {
                child = this._panes[i];
                if (child.id == id)
                    child.classList.add('active');
                else if (child.classList.contains('active'))
                    child.classList.remove('active');
            }

            // remove old active highlights and set new highlight
            for (i = this._tabitems.length - 1; i >= 0; i--) {
                child = this._tabitems[i];
                if (child.querySelector('a').hash == '#' + id)
                    child.classList.add('active');
                else if (child.classList.contains('active'))
                    child.classList.remove('active');
            }

            // open sidebar (if necessary)
            if (this.element.classList.contains('collapsed')) {
                this.element.classList.remove('collapsed');
            }

            return this;
        };

        close() {
            // remove old active highlights
            for (var i = this._tabitems.length - 1; i >= 0; i--) {
                var child = this._tabitems[i];
                if (child.classList.contains('active'))
                    child.classList.remove('active');
            }

            // close sidebar
            if (!this.element.classList.contains('collapsed')) {
                this.element.classList.add('collapsed');
            }

            return this;
        };

        _onClick(evt) {
            evt.preventDefault();
            if (this.classList.contains('active')) {
                this._sidebar.close();
            } else if (!this.classList.contains('disabled')) {
                this._sidebar.open(this.querySelector('a').hash.slice(1));
            }
        };

        _onCloseClick() {
            this.close();
        };

    }
    const nexusSideBar = new NexusSidebar(opt_options);
    return nexusSideBar;
}