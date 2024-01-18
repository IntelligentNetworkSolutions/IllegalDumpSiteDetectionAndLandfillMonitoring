/**
 *
 * @class
 * @classdesc Manages import and export of annotations
 * @author Abhishek Dutta <adutta@robots.ox.ac.uk>
 * @date 26 May 2019
 *
 */

'use strict';

function _via_import_export(data) {
    this.d = data;

    // registers on_event(), emit_event(), ... methods from
    // _via_event to let this module listen and emit events
    this._EVENT_ID_PREFIX = '_via_import_export_';
    _via_event.call(this);
}

_via_import_export.prototype.import_from_file = function (data_format, file) {
    switch (data_format) {
        case 'coco':
            _via_util_load_text_file(file[0], this.import_from_coco.bind(this));
            break;
        case 'webvtt':
            _via_util_load_text_file(file[0], this.import_from_webvtt.bind(this));
        default:
            console.warn('Unknown data format: ' + data_format);
    }
}

_via_import_export.prototype.import_from_coco = function (json_str) {
    try {
        var d = JSON.parse(json_str);
        if (!(d.hasOwnProperty('annotations') &&
            d.hasOwnProperty('categories') &&
            d.hasOwnProperty('images')
        )
        ) {
            _via_util_msg_show('Cannot import annotations from malformed JSON!');
            return;
        }
        var p = this.d._init_default_project();

        // add files
        for (var i in d.images) {
            var fid = d.images[i].id;
            var fname = d.images[i].file_name;
            var src = d.images[i].coco_url;
            var type = _VIA_FILE_TYPE.IMAGE;
            var loc = _VIA_FILE_LOC.URIHTTP;
            p.file[fid] = new _via_file(fid, fname, type, loc, src);

            // add a view for each file
            var vid = fid;
            p.view[vid] = new _via_view([fid]); // view with single file
            p.project.vid_list.push(vid);
        }

        // add attributes
        for (var i in d.categories) {
            var aid = d.categories[i].id;
            var aname = d.categories[i].name;
            p.attribute[aid] = new _via_attribute(aname, 'FILE1_Z0_XY1', _VIA_ATTRIBUTE_TYPE.TEXT, {}, {});
            /*
                  var aid = d.categories[i].supercategory;
                  var option_id = d.categories[i].id;
                  var option_value = d.categories[i].name;
            
                  if ( p.attribute.hasOwnProperty(aid) ) {
                    // add as an option
                    p.attribute[aid].options[option_id] = option_value;
                  } else {
                    var options = {};
                    options[option_id] = option_value;
                    p.attribute[aid] = new _via_attribute(aid, 'FILE1_Z0_XY1', _VIA_ATTRIBUTE_TYPE.SELECT, options, {});
                  }
            */
        }

        // add metadata
        for (var i in d.annotations) {
            var mid = d.annotations[i].id;
            var vid = d.annotations[i].image_id;
            if (p.file.hasOwnProperty(vid)) {
                var aid = d.annotations[i].category_id;
                var shape = [_VIA_RSHAPE.POLYGON];
                var av = {}
                av[aid] = p.attribute[aid].aname;

                for (var j = 0; j < d.annotations[i].segmentation[0].length; ++j) {
                    shape.push(d.annotations[i].segmentation[0][j]);
                }

                p.metadata[mid] = new _via_metadata(vid, [], shape, av);
            }
        }

        this.d.project_load_json(p);
    }
    catch (e) {
        _via_util_msg_show('Failed to import annotations: ' + e);
        console.warn(e)
    }
}

_via_import_export.prototype.export_to_file = function (data_format) {
    console.log(data_format)
    switch (data_format) {
        case 'via3_csv':
            this.export_to_via3_csv();
            break;
        case 'temporal_segments_csv':
            this.export_to_temporal_segments_csv();
            break;
        case 'webvtt':
            this.export_to_webvtt();
            break;
        case 'coco':
            this.export_to_coco();
            break;

        default:
            console.warn('Unknown data format: ' + data_format);
    }
}

_via_import_export.prototype.export_to_coco = function () {
    return new Promise(function (ok_callback, err_callback) {
        var coco = {
            'info': {
                'year': '',
                'version': '',
                'description': '',
                'contributor': '',
                'url': '',
                'date_created': '',
            },
            'images': [],
            'annotations': [],
            'categories': [],
            'licenses': [{ 'id': 0, 'name': 'Unknown License', 'url': '' }] // indicates that license is unknown
        };

        coco['info']['description'] = this.d.store['project']['pname'];
        coco['info']['date_created'] = this.d.store['project']['created'];
        coco['info']['contributor'] = this.d.store['project']['creator'];
        coco['info']['version'] = this.d.store['project']['rev_timestamp'];
        var COCO_ALLOWED_RSHAPE = [_VIA_RSHAPE.RECTANGLE, _VIA_RSHAPE.POLYGON];

        var coco_categories = {};
        var coco_images_list = {};
        var annotation_id = 0;
        for (var mid in this.d.store['metadata']) {
            var vid = this.d.store['metadata'][mid]['vid'].toString();
            var fid = this.d.store['view'][vid]['fid_list'][0];
            var filename = this.d.store['file'][fid]['fname'];
            const fileloc = this.d.store['file'][fid]['loc'];
            const file_prefix = this.d.store['config']['file']['loc_prefix'][fileloc];
            for (var aid in this.d.store['metadata'][mid]['av']) {
                const aname = this.d.store['attribute'][aid]['aname'];
                var avalue = this.d.store['metadata'][mid]['av'][aid];
                const type = this.d.store['attribute'][aid]['type'];
                if (type === _VIA_ATTRIBUTE_TYPE.SELECT) {
                    const option_id = avalue;
                    avalue = this.d.store['attribute'][aid]['options'][option_id];
                }

                const anchor_id = this.d.store['attribute'][aid]['anchor_id'];
                if (anchor_id === 'FILE1_Z0_XY1' ||
                    anchor_id === 'FILE1_Z1_XY1') {
                    // check if the category exists in COCO
                    if (!coco_categories.hasOwnProperty(aname)) {
                        coco_categories[aname] = [];
                    }
                    if (!coco_categories[aname].includes(avalue)) {
                        coco_categories[aname].push(avalue);
                    }
                    var category_id = coco_categories[aname].indexOf(avalue);

                    // check if file has been added
                    if (!(filename in coco_images_list)) {
                        coco_images_list[filename] = fid;
                        coco['images'].push({
                            'id': parseInt(fid),
                            'license': 0,
                            'file_name': filename,
                            'coco_url': file_prefix + filename,
                            'width': -1,
                            'height': -1,
                            'date_captured': '',
                            'flickr_url': ''
                        });
                    }

                    // add this annotation
                    const region = this.d.store['metadata'][mid]['xy'];
                    const bbox = this.via3_region_to_bbox(region);
                    coco['annotations'].push({
                        'id': annotation_id,
                        'image_id': parseInt(fid),
                        'category_id': category_id,
                        'bbox': bbox,
                        'segmentation': this.via3_region_to_segmentation(region),
                        'iscrowd': false,
                        'area': Number.parseFloat(Number.parseFloat(bbox[2] * bbox[3]).toFixed(3))
                    });

                    annotation_id = annotation_id + 1;
                }
            }
        }

        // add categories to coco
        for (var aname in coco_categories) {
            for (var id in coco_categories[aname]) {
                coco['categories'].push({
                    'supercategory': aname,
                    'id': parseInt(id),
                    'name': coco_categories[aname][id]
                });
            }
        }

        // trigger download as JSON
        var filename = 'coco.json';
        if (this.d.store.project['pid'] === '__VIA_PROJECT_ID__') {
            filename = this.d.store.project['pname'].replace(' ', '-') + '-coco.json';
        } else {
            filename = this.d.store.project['pid'] + '-coco.json';
            coco['info']['url'] = _VIA_REMOTE_STORE + this.d.store.project['pid'];
            coco['info']['version'] = this.d.store.project['rev'];
        }
        var blob_attr = { type: 'application/json;charset=utf-9' }
        var coco_blob = new Blob([JSON.stringify(coco)], blob_attr);
        _via_util_download_as_file(coco_blob, filename);
    }.bind(this));
}

_via_import_export.prototype.export_to_via3_csv = function () {
    return new Promise(function (ok_callback, err_callback) {
        var csv = [];

        var attribute = {}
        for (var aid in this.d.store.attribute) {
            attribute[aid] = this.d.store.attribute[aid].aname;
        }

        csv.push('# Exported using VGG Image Annotator (http://www.robots.ox.ac.uk/~vgg/software/via)');
        csv.push('# Notes:');
        csv.push('# - spatial_coordinates of [2,10,20,50,80] denotes a rectangle (shape_id=2) of size 50x80 placed at (10,20)');
        csv.push('# - temporal coordinate of [1.349,2.741] denotes a temporal segment from time 1.346 sec. to 2.741 sec.');
        csv.push('# - temporal coordinate of [4.633] denotes a still video frame at 4.633 sec.');
        csv.push('# - metadata of {""1"":""3""} indicates attribute with id "1" set to an attribute option with id "3"');

        csv.push('# SHAPE_ID = ' + JSON.stringify(_VIA_RSHAPE));
        csv.push('# FLAG_ID = ' + JSON.stringify(_VIA_METADATA_FLAG));
        csv.push('# ATTRIBUTE = ' + JSON.stringify(this.d.store.attribute));
        csv.push('# CSV_HEADER = metadata_id,file_list,flags,temporal_coordinates,spatial_coordinates,metadata');
        // build file_list for each view_id
        var vid_filesrc_str_list = {};
        var vid, fid;
        for (var vindex in this.d.store.project.vid_list) {
            vid = this.d.store.project.vid_list[vindex];
            var vid_filesrc_list = [];
            for (var findex in this.d.store.view[vid].fid_list) {
                fid = this.d.store.view[vid].fid_list[findex];
                switch (this.d.store.file[fid].loc) {
                    case _VIA_FILE_LOC.LOCAL:
                        if (this.d.file_ref.hasOwnProperty(fid)) {
                            vid_filesrc_list.push(this.d.file_ref[fid].name);
                        } else {
                            vid_filesrc_list.push(this.d.store.file[fid].fname);
                        }
                        break;
                    case _VIA_FILE_LOC.INLINE:
                        vid_filesrc_list.push(this.d.store.file[fid].fname);
                        break;
                    default:
                        vid_filesrc_list.push(this.d.store.file[fid].src);
                }
            }
            vid_filesrc_str_list[vid] = _via_util_obj2csv(vid_filesrc_list);
        }

        for (var mid in this.d.store.metadata) {
            var line = [];
            line.push('"' + mid + '"');
            line.push(vid_filesrc_str_list[this.d.store.metadata[mid].vid]);
            line.push(this.d.store.metadata[mid].flg);
            line.push(_via_util_obj2csv(this.d.store.metadata[mid].z));
            line.push(_via_util_obj2csv(this.d.store.metadata[mid].xy));
            line.push(_via_util_obj2csv(this.d.store.metadata[mid].av));
            csv.push(line.join(','));
        }

        var data_blob = new Blob([csv.join('\n')],
            { type: 'text/csv;charset=utf-8' });
        var filename = [];
        filename.push(this.d.store.project.pname.replace(' ', '-'));
        filename.push(_via_util_date_to_filename_str(Date.now()));
        filename.push('_export.csv');
        _via_util_download_as_file(data_blob, filename.join(''));
    }.bind(this));
}

_via_import_export.prototype.export_to_temporal_segments_csv = function () {
    return new Promise(function (ok_callback, err_callback) {
        var csv = [];

        var attribute = {}
        for (var aid in this.d.store.attribute) {
            attribute[aid] = this.d.store.attribute[aid].aname;
        }

        csv.push('# Exported using VGG Image Annotator (http://www.robots.ox.ac.uk/~vgg/software/via)');
        csv.push('# CSV_HEADER = metadata_id,file_list,temporal_segment_start,temporal_segment_end,metadata');
        // build file_list for each view_id
        var vid_filesrc_str_list = {};
        var vid, fid;
        for (var vindex in this.d.store.project.vid_list) {
            vid = this.d.store.project.vid_list[vindex];
            var vid_filesrc_list = [];
            for (var findex in this.d.store.view[vid].fid_list) {
                fid = this.d.store.view[vid].fid_list[findex];
                switch (this.d.store.file[fid].loc) {
                    case _VIA_FILE_LOC.LOCAL:
                        if (this.d.file_ref.hasOwnProperty(fid)) {
                            vid_filesrc_list.push(this.d.file_ref[fid].name);
                        } else {
                            vid_filesrc_list.push(this.d.store.file[fid].fname);
                        }
                        break;
                    case _VIA_FILE_LOC.INLINE:
                        vid_filesrc_list.push(this.d.store.file[fid].fname);
                        break;
                    default:
                        vid_filesrc_list.push(this.d.store.file[fid].src);
                }
            }
            vid_filesrc_str_list[vid] = _via_util_obj2csv(vid_filesrc_list);
        }

        for (var mid in this.d.store.metadata) {
            var line = [];
            line.push('"' + mid + '"');
            line.push(vid_filesrc_str_list[this.d.store.metadata[mid].vid]);
            line.push(this.d.store.metadata[mid].z[0])
            line.push(this.d.store.metadata[mid].z[1])
            var descriptive_av = {};
            for (var aid in this.d.store.metadata[mid].av) {
                descriptive_av[this.d.store.attribute[aid].aname] = this.d.store.metadata[mid].av[aid];
            }
            line.push(_via_util_obj2csv(descriptive_av));
            csv.push(line.join(','));
        }

        var data_blob = new Blob([csv.join('\n')],
            { type: 'text/csv;charset=utf-8' });
        var filename = [];
        filename.push(this.d.store.project.pname.replace(' ', '-'));
        filename.push(_via_util_date_to_filename_str(Date.now()));
        filename.push('_export.csv');
        _via_util_download_as_file(data_blob, filename.join(''));
    }.bind(this));
}

_via_import_export.prototype.export_to_webvtt = function () {
    return new Promise(function (ok_callback, err_callback) {
        var csv = [];

        var attribute = {}
        for (var aid in this.d.store.attribute) {
            attribute[aid] = this.d.store.attribute[aid].aname;
        }

        csv.push('WEBVTT');
        csv.push('');
        // build file_list for each view_id
        var vid_filesrc_str_list = {};
        var vid, fid;
        for (var vindex in this.d.store.project.vid_list) {
            vid = this.d.store.project.vid_list[vindex];
            var vid_filesrc_list = [];
            for (var findex in this.d.store.view[vid].fid_list) {
                fid = this.d.store.view[vid].fid_list[findex];
                switch (this.d.store.file[fid].loc) {
                    case _VIA_FILE_LOC.LOCAL:
                        if (this.d.file_ref.hasOwnProperty(fid)) {
                            vid_filesrc_list.push(this.d.file_ref[fid].name);
                        } else {
                            vid_filesrc_list.push(this.d.store.file[fid].fname);
                        }
                        break;
                    case _VIA_FILE_LOC.INLINE:
                        vid_filesrc_list.push(this.d.store.file[fid].fname);
                        break;
                    default:
                        vid_filesrc_list.push(this.d.store.file[fid].src);
                }
            }
            vid_filesrc_str_list[vid] = _via_util_obj2csv(vid_filesrc_list);
        }

        for (var mid in this.d.store.metadata) {
            var tstart = _via_seconds_to_hh_mm_ss_ms(this.d.store.metadata[mid].z[0]);
            var tstart_str = tstart[0] + ':' + tstart[1] + ':' + tstart[2] + '.' + tstart[3];
            var tend = _via_seconds_to_hh_mm_ss_ms(this.d.store.metadata[mid].z[1]);
            var tend_str = tend[0] + ':' + tend[1] + ':' + tend[2] + '.' + tend[3];
            var subtitle = [];
            for (var aid in this.d.store.metadata[mid].av) {
                subtitle.push(this.d.store.metadata[mid].av[aid]);
            }
            var subtitle_str = subtitle.join(' ');
            csv.push(tstart_str + ' --> ' + tend_str);
            csv.push(subtitle.join(' '))
            csv.push('');
        }
        var data_blob = new Blob([csv.join('\n')],
            { type: 'text/vtt;charset=utf-8' });
        var filename = [];
        filename.push(this.d.store.project.pname);
        filename.push('.vtt');
        _via_util_download_as_file(data_blob, filename.join(''));
    }.bind(this));
}

_via_import_export.prototype.import_from_webvtt = function (webvtt_str, vid, subtitle_aid) {
    return new Promise(function (ok_callback, err_callback) {
        var line_split_regex = new RegExp('\n|\r|\r\n', 'g');
        var webvtt_lines = webvtt_str.split(line_split_regex);
        var metadata_list = [];
        var webvtt_lines_count = webvtt_lines.length;
        var current_line_index = 0;
        var timestamp_found = false;
        var timestamp_line_index = -1;
        var subtitles_parse_started = false;
        var subtitle_text = [];
        while (current_line_index < webvtt_lines_count) {
            //console.log('Processing line [' + current_line_index + '] : ' + webvtt_lines[current_line_index]);
            if (webvtt_lines[current_line_index] === '') {
                // ignore empty lines
                current_line_index += 1;
                //console.log('  ignored empty line');
                continue;
            }
            if (webvtt_lines[current_line_index].includes(' --> ')) {
                if (timestamp_found) {
                    // encountered timestamp for a new subtitle
                    // we now process previous subtitle
                    // we do not increment current_line_index so that the parser
                    // encounters this line again and begins parsing the new subtitle
                    var timestamp_tokens = webvtt_lines[timestamp_line_index].split(' --> ');
                    var starttime_sec = _via_hh_mm_ss_ms_to_seconds(timestamp_tokens[0]);
                    var endtime_sec = _via_hh_mm_ss_ms_to_seconds(timestamp_tokens[1]);
                    var metadata = {
                        'vid': vid,
                        'z': [starttime_sec, endtime_sec],
                        'xy': [],
                        'av': {}
                    }
                    metadata['av'][subtitle_aid] = subtitle_text.join(' ');
                    metadata_list.push(metadata);
                    //console.log('  added subtitle: ' + JSON.stringify(metadata))
                    timestamp_found = false; // to allow this new timestamp to be processed in next iteration
                    timestamp_line_index = -1;
                    subtitle_text = [];
                } else {
                    timestamp_found = true;
                    timestamp_line_index = current_line_index;
                    //console.log('  timestamp found at index: ' + timestamp_line_index);
                    current_line_index += 1;
                }
                continue;
            } else {
                if (timestamp_found) {
                    subtitle_text.push(webvtt_lines[current_line_index]);
                    //console.log('  pushed part of subtitle to subtitle text array');
                } else {
                    // not an empty line
                    // timestamp has not been encountered yet
                    // we ignore other irrelevant parts (e.g. header)
                    //console.log('  IGNORE: ' + webvtt_lines[current_line_index]);
                }
                current_line_index += 1;
                continue;
            }
        }

        //this.d.metadata_add_bulk(metadata_list, true); // appends to existing metadata

        // removes existing subtitles and adds new subtitles
        this.d.metadata_delete_all(vid, true).then(function (ok) {
            this.d.metadata_add_bulk(metadata_list, true);
        }.bind(this), function (err) {
            console.log(err);
        }.bind(this));
    }.bind(this));
}

_via_import_export.prototype.via3_region_to_segmentation = function (region) {
    switch (region[0]) {
        case _VIA_RSHAPE.RECTANGLE:
            var x = region[1];
            var y = region[2];
            var w = region[3];
            var h = region[4];
            return [[x, y, x + w, y, x + w, y + h, x, y + h]];
            break;
        case _VIA_RSHAPE.POLYGON:
            var pts = [];
            for (var i = 1; i < region.length; ++i) {
                pts.push(region[i]);
            }
            return pts;
            break;
        default:
            return [[]];
    }
}

_via_import_export.prototype.via3_region_to_bbox = function (region) {
    switch (region[0]) {
        case _VIA_RSHAPE.RECTANGLE:
            return [region[1], region[2], region[3], region[4]];
            break;
        case _VIA_RSHAPE.POLYGON:
            var extreme_pts = [-Infinity, -Infinity, -Infinity, -Infinity];
            for (var i = 1; i < region.length; i = i + 2) {
                if (region[i] < extreme_pts[0]) {
                    extreme_pts[0] = region[i];
                }
                if (region[i + 1] < extreme_pts[1]) {
                    extreme_pts[1] = region[i + 1];
                }
                if (region[i] > extreme_pts[2]) {
                    extreme_pts[2] = region[i];
                }
                if (region[i + 1] > extreme_pts[3]) {
                    extreme_pts[3] = region[i + 1];
                }
            }
            var bbox = [extreme_pts[0], extreme_pts[1]];
            bbox.push(extreme_pts[2] - extreme_pts[0]);
            bbox.push(extreme_pts[3] - extreme_pts[1]);
            return bbox;
            break;
        default:
            return [[]];
    }
}
