// All used projections(except 4326 and 3857) must be declared here
window.proj4custom_definitions =[
    ["EPSG:6316", "+proj=tmerc +lat_0=0 +lon_0=21 +k=0.9999 +x_0=7500000 +y_0=0 +ellps=bessel +towgs84=551.7,162.9,467.9,6.04,1.96,-11.38,-4.82 +units=m +no_defs"],
    ["EPSG:6870", "+proj=tmerc +lat_0=0 +lon_0=20 +k=1 +x_0=500000 +y_0=0 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs"],
    ["EPSG:32634", "+proj=utm +zone=34 +datum=WGS84 +units=m +no_defs"]    
]

if (window.proj4 != undefined) {
    proj4.defs(proj4custom_definitions);
}

//load default 4326 and 3857 OpenLayers projections, do not add projections to this list, add them UP instead
window.proj4custom_definitions.unshift(
    ["EPSG:3857", "+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext  +no_defs"],
    ["EPSG:4326", "+proj=longlat +datum=WGS84 +no_defs "]
);

if (window.ol != undefined) {
    ol.proj.proj4.register(proj4);
}