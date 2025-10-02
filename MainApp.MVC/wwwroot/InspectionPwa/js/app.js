var permission;
if ('serviceWorker' in navigator) {
    window.addEventListener('DOMContentLoaded', async function () {
        navigator.serviceWorker
            .register(window.AppSettings.FullAppUrl + "/ServiceWorker")
            .then(async registration => {
                console.log('Service Worker registered with scope:', registration.scope);
            });
    });
}
// Add this to your map initialization script

class GeolocationControl {
    constructor(map) {
        this.map = map;
        this.geolocation = null;
        this.accuracyFeature = null;
        this.positionFeature = null;
        this.vectorLayer = null;
        this.isActive = false;

        this.init();
    }

    init() {
        // Create vector source and layer for location display
        const vectorSource = new ol.source.Vector();
        this.vectorLayer = new ol.layer.Vector({
            source: vectorSource,
            style: this.createLocationStyle(),
            zIndex: 1000
        });

        this.map.addLayer(this.vectorLayer);

        // Create geolocation instance
        this.geolocation = new ol.Geolocation({
            trackingOptions: {
                enableHighAccuracy: true,
                timeout: 10000,
                maximumAge: 60000
            },
            projection: this.map.getView().getProjection()
        });

        // Handle position changes
        this.geolocation.on('change:accuracyGeometry', () => {
            if (this.geolocation.getAccuracyGeometry()) {
                if (!this.accuracyFeature) {
                    this.accuracyFeature = new ol.Feature();
                    vectorSource.addFeature(this.accuracyFeature);
                }
                this.accuracyFeature.setGeometry(this.geolocation.getAccuracyGeometry());
            }
        });

        this.geolocation.on('change:position', () => {
            const coordinates = this.geolocation.getPosition();
            if (coordinates) {
                if (!this.positionFeature) {
                    this.positionFeature = new ol.Feature();
                    this.positionFeature.setStyle(this.createPositionStyle());
                    vectorSource.addFeature(this.positionFeature);
                }
                this.positionFeature.setGeometry(new ol.geom.Point(coordinates));

                // Zoom to location on first position
                if (!this.hasZoomedToLocation) {
                    this.map.getView().animate({
                        center: coordinates,
                        zoom: 18,
                        duration: 1000
                    });
                    this.hasZoomedToLocation = true;
                }
            }
        });

        // Handle errors
        this.geolocation.on('error', (error) => {
            console.error('Geolocation error:', error);
            this.showLocationError(error.message);
            this.stopTracking();
        });

        // Create control button
        this.createControl();
    }

    createLocationStyle() {
        return new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(0, 123, 255, 0.2)'
            }),
            stroke: new ol.style.Stroke({
                color: 'rgba(0, 123, 255, 0.8)',
                width: 2
            })
        });
    }

    createPositionStyle() {
        return new ol.style.Style({
            image: new ol.style.Circle({
                radius: 8,
                fill: new ol.style.Fill({
                    color: '#007bff'
                }),
                stroke: new ol.style.Stroke({
                    color: 'white',
                    width: 2
                })
            })
        });
    }

    createControl() {
        const button = document.createElement('button');
        button.innerHTML = '<i class="fas fa-location-arrow"></i>';
        button.className = 'ol-location-btn';
        button.type = 'button';
        button.title = 'Get Current Location';

        button.addEventListener('click', () => {
            this.toggleTracking();
        });

        const element = document.createElement('div');
        element.className = 'ol-location ol-unselectable ol-control';
        element.appendChild(button);

        const control = new ol.control.Control({
            element: element
        });

        this.map.addControl(control);

        // Add CSS styles
        this.addStyles();
    }

    addStyles() {
        const style = document.createElement('style');
        style.textContent = `
            .ol-location {
                top: 65px;
                right: 0.5em;
            }
            
            .ol-location-btn {
                width: 2.5em;
                height: 2.5em;
                background-color: rgba(255, 255, 255, 0.9);
                border: none;
                border-radius: 2px;
                color: #333;
                cursor: pointer;
                font-size: 14px;
                transition: all 0.2s;
                box-shadow: 0 1px 4px rgba(0,0,0,0.3);
            }
            
            .ol-location-btn:hover {
                background-color: rgba(255, 255, 255, 1);
                color: #007bff;
            }
            
            .ol-location-btn.active {
                background-color: #007bff;
                color: white;
            }
            
            .ol-location-btn:disabled {
                opacity: 0.6;
                cursor: not-allowed;
            }
        `;
        document.head.appendChild(style);
    }

    toggleTracking() {
        if (this.isActive) {
            this.stopTracking();
        } else {
            this.startTracking();
        }
    }

    startTracking() {
        if (!navigator.geolocation) {
            this.showLocationError('Geolocation is not supported by this browser');
            return;
        }

        this.isActive = true;
        this.hasZoomedToLocation = false;

        const button = document.querySelector('.ol-location-btn');
        button.classList.add('active');
        button.disabled = true;
        button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';

        this.geolocation.setTracking(true);

        // Re-enable button after a delay
        setTimeout(() => {
            button.disabled = false;
            button.innerHTML = '<i class="fas fa-location-arrow"></i>';
        }, 2000);
    }

    stopTracking() {
        this.isActive = false;
        this.geolocation.setTracking(false);

        const button = document.querySelector('.ol-location-btn');
        button.classList.remove('active');
        button.innerHTML = '<i class="fas fa-location-arrow"></i>';

        // Clear features
        const vectorSource = this.vectorLayer.getSource();
        vectorSource.clear();
        this.accuracyFeature = null;
        this.positionFeature = null;
    }

    showLocationError(message) {
        // Use your existing notification system
        if (typeof notifyAlerts === 'function') {
            notifyAlerts('error', `Location Error: ${message}`);
        } else {
            alert(`Location Error: ${message}`);
        }
    }

    getCurrentPosition() {
        return this.geolocation.getPosition();
    }

    getCurrentAccuracy() {
        return this.geolocation.getAccuracy();
    }
}

// Usage: Add this after your map is initialized
 window.geolocationControl = new GeolocationControl(mapVars.map);
