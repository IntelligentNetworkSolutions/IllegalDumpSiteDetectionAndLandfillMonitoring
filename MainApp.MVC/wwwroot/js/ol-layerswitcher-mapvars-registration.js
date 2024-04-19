$(function () {
    var checkExist = setInterval(function () {
        if (mapVars.map) {            

            mapVars.mapSidebar = new createTurboSidebar({ element: 'map-sidebar', position: 'right' });

            mapVars.map.addControl(mapVars.mapSidebar);

            //mapVars.refreshLayerSwitcher = function () {
            //    // Get out-of-the-map div element with the ID "layers" and renders layers to it.
            //    // NOTE: If the layers are changed outside of the layer switcher then you
            //    // will need to call ol.control.LayerSwitcher.renderPanel again to refesh
            //    // the layer tree. Style the tree via CSS.
            //    var toc = document.getElementById('sidebar-layers-container');
            //    ol.control.LayerSwitcher.renderPanel(mapVars.map, toc, { reverse: true });
            //};


            // Add a layer switcher outside the map
            /* OLD WAY */
            //mapVars.switcher = new ol.control.LayerSwitcher({
            //mapVars.switcher = new CustomLayerSwitcher({
            //    target: document.getElementById('sidebar-layers-container'),
            //    // displayInLayerSwitcher: function (l) { return false; },
            //    show_progress: true,
            //    show_legend: false,
            //    trash: true,
            //    //show_info_for_layers: mapVars.showInfoForLayers(false),
            //    //oninfo: function (l) { alert(l.get("title")); }
            //});

            //mapVars.map.addControl(mapVars.switcher);

            mapVars.switcher = createCustomLayerSwitcher({
                target: document.getElementById('sidebar-layers-container'),
                // displayInLayerSwitcher: function (l) { return false; },
                show_progress: true,
                show_legend: false,
                trash: true,
                //show_info_for_layers: mapVars.showInfoForLayers(false),
                //oninfo: function (l) { alert(l.get("title")); }
            });
            mapVars.map.addControl(mapVars.switcher);

            // Insert mapbox layer in layer switcher
            //function displayInLayerSwitcher(b) {
            //    mapbox.set('displayInLayerSwitcher', b);
            //}

            mapVars.refreshLayerSwitcher = function () {
                mapVars.switcher.drawPanel();
                //console.log("mapVars.refreshLayerSwitcher function is depricated!");
            };

            clearInterval(checkExist);
        }
    }, 100);
});