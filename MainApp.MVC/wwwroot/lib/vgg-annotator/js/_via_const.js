'use strict'

// definition of contants that are used by all modules
const _VIA_USER_ROLE = { 'ADMIN': 1, 'ANNOTATOR': 2, 'REVIEWER': 3 };
const _VIA_STORE_TYPE = { 'LOCALSTORAGE': 1, 'COUCHDB': 2 };

var _VIA_SVG_NS = "http://www.w3.org/2000/svg";

var _VIA_PROJECT_ID_MARKER = '__VIA_PROJECT_ID__';
var _VIA_PROJECT_REV_ID_MARKER = '__VIA_PROJECT_REV_ID__';
var _VIA_PROJECT_REV_TIMESTAMP_MARKER = '__VIA_PROJECT_REV_TIMESTAMP__';

var _VIA_FILE_SELECT_TYPE = { 'JSON': 2, 'CSV': 4, 'TEXT': 8, 'IMAGE': 16, 'VIDEO': 32, 'AUDIO': 64, 'WEBVTT': 128 };
var _VIA_MERGE_STRATEGY = { 'THREE_WAY': 1 }

// zoom, used in _via_control_panel.js to populate a dropdown of zoom options
var _VIA_ZOOM_MODE = { 'FITHEIGHT': 1, 'FITWIDTH': 2, 'SCALE': 3 };
var _VIA_DEFAULT_ZOOM_MODE = _VIA_ZOOM_MODE.FITHEIGHT;
var _VIA_ZOOM_SCALE_VALUE_LIST = [0.01, 0.1, 0.2, 0.5, 0.8, 1.0, 2.0, 4.0, 8.0, 10.0];
var _VIA_ZOOM_SCALE_DEFAULT_INDEX = 5;
