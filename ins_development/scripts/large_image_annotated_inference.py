# Copyright (c) OpenMMLab. All rights reserved.
"""Perform MMDET inference on large images (as satellite imagery) as:

```shell
wget -P checkpoint https://download.openmmlab.com/mmdetection/v2.0/faster_rcnn/faster_rcnn_r101_fpn_2x_coco/faster_rcnn_r101_fpn_2x_coco_bbox_mAP-0.398_20200504_210455-1d2dac9c.pth # noqa: E501, E261.

python demo/large_image_demo.py \
    demo/large_image.jpg \
    configs/faster_rcnn/faster-rcnn_r101_fpn_2x_coco.py \
    checkpoint/faster_rcnn_r101_fpn_2x_coco_bbox_mAP-0.398_20200504_210455-1d2dac9c.pth
```

Run Command (VSCode Terminal openmmlab env):
python .\demo\large_image_demo.py .\ins_development\resources\test-detection-images\odm_orthophoto_lisiche.tif  .\ins_development\outputs\training\ins_annotated_v9\faster\r101_fpn_2x_coco\default\ins_annotated_v9_faster_rcnn_r101_fpn_2x_coco_with_tests.py .\ins_development\outputs\training\ins_annotated_v9\faster\r101_fpn_2x_coco\default\epoch_10.pth --out-dir .\ins_development\outputs\detection\ins_annotated_v9\faster\r101_fpn_2x_coco\default\e30\ --patch-size 1280
"""

import os
import random
from argparse import ArgumentParser
from pathlib import Path

import mmcv
import numpy as np
from mmengine.config import Config, ConfigDict
from mmengine.logging import print_log
from mmengine.utils import ProgressBar

from mmdet.apis import inference_detector, init_detector

try:
    from sahi.slicing import slice_image
except ImportError:
    raise ImportError('Please run "pip install -U sahi" '
                      'to install sahi first for large image inference.')

from mmdet.registry import VISUALIZERS
from mmdet.utils.large_image import merge_results_by_nms, shift_predictions
from mmdet.utils.misc import get_file_list

# INS added

# import pprint
# def print_full_content(obj):
#     """
#     Print the full content of an object without truncation.
#     """
#     pp = pprint.PrettyPrinter(indent=4, width=200, depth=None)
#     return pp.pformat(obj)

import json
import torch
import cv2

def tensor_to_list(tensor):
    if isinstance(tensor, torch.Tensor):
        return tensor.tolist()
    return tensor

def detdatasample_to_dict(sample):
    result = {}
    
    # Process metadata
    metadata = {
        'batch_input_shape': tensor_to_list(sample.batch_input_shape),
        'scale_factor': tensor_to_list(sample.scale_factor),
        'pad_shape': tensor_to_list(sample.pad_shape),
        'img_shape': tensor_to_list(sample.img_shape),
        'img_id': sample.img_id,
        'ori_shape': tensor_to_list(sample.ori_shape),
        'img_path': sample.img_path
    }
    result['metadata'] = metadata

    # Process pred_instances
    if hasattr(sample, 'pred_instances'):
        pred_instances = sample.pred_instances
        detections = []
        for bbox, score, label in zip(
            tensor_to_list(pred_instances.bboxes),
            tensor_to_list(pred_instances.scores),
            tensor_to_list(pred_instances.labels)
        ):
            detections.append({
                "bbox": bbox,
                "score": score,
                "label": int(label)
            })
        result['detections'] = detections

    return result

def draw_bboxes_from_json(json_file, image_file, output_file):
    # Load JSON data
    with open(json_file, 'r') as f:
        data = json.load(f)
    
    # Read the image
    img = cv2.imread(image_file)
    
    # Draw bounding boxes
    for detection in data['detections']:
        bbox = detection['bbox']
        score = detection['score']
        label = detection['label']
        
        if score > 0.5:
            xmin, ymin, xmax, ymax = map(int, bbox)
        
            # Draw rectangle
            cv2.rectangle(img, (xmin, ymin), (xmax, ymax), (55, 55, 255), 2)
            
            # Draw label and score
            label_text = f"waste {score:.2f}"
            cv2.putText(img, label_text, (xmin, ymin - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (55, 55, 255), 2)
    
    # Save the image with bounding boxes
    cv2.imwrite(output_file, img)
    print(f"Image with bounding boxes saved to {output_file}")

## MMDetection
def parse_args():
    parser = ArgumentParser(
        description='Perform MMDET inference on large images.')
    parser.add_argument(
        'img', help='Image path, include image file, dir and URL.')
    parser.add_argument('config', help='Config file')
    parser.add_argument('checkpoint', help='Checkpoint file')
    parser.add_argument(
        '--out-dir', default='./output', help='Path to output file')
    parser.add_argument(
        '--device', default='cuda:0', help='Device used for inference')
    parser.add_argument(
        '--show', action='store_true', help='Show the detection results')
    parser.add_argument(
        '--tta',
        action='store_true',
        help='Whether to use test time augmentation')
    parser.add_argument(
        '--score-thr', type=float, default=0.15, help='Bbox score threshold')
    parser.add_argument(
        '--patch-size', type=int, default=640, help='The size of patches')
    parser.add_argument(
        '--patch-overlap-ratio',
        type=float,
        default=0.25,
        help='Ratio of overlap between two patches')
    parser.add_argument(
        '--merge-iou-thr',
        type=float,
        default=0.25,
        help='IoU threshould for merging results')
    parser.add_argument(
        '--merge-nms-type',
        type=str,
        default='nms',
        help='NMS type for merging results')
    parser.add_argument(
        '--batch-size',
        type=int,
        default=1,
        help='Batch size, must greater than or equal to 1')
    parser.add_argument(
        '--debug',
        action='store_true',
        help='Export debug results before merging')
    parser.add_argument(
        '--save-patch',
        action='store_true',
        help='Save the results of each patch. '
        'The `--debug` must be enabled.')
    args = parser.parse_args()
    return args


def main():
    args = parse_args()

    config = args.config

    if isinstance(config, (str, Path)):
        config = Config.fromfile(config)
    elif not isinstance(config, Config):
        raise TypeError('config must be a filename or Config object, '
                        f'but got {type(config)}')
    if 'init_cfg' in config.model.backbone:
        config.model.backbone.init_cfg = None

    if args.tta:
        assert 'tta_model' in config, 'Cannot find ``tta_model`` in config.' \
                                      " Can't use tta !"
        assert 'tta_pipeline' in config, 'Cannot find ``tta_pipeline`` ' \
                                         "in config. Can't use tta !"
        config.model = ConfigDict(**config.tta_model, module=config.model)
        test_data_cfg = config.test_dataloader.dataset
        while 'dataset' in test_data_cfg:
            test_data_cfg = test_data_cfg['dataset']

        test_data_cfg.pipeline = config.tta_pipeline

    # TODO: TTA mode will error if cfg_options is not set.
    #  This is an mmdet issue and needs to be fixed later.
    # build the model from a config file and a checkpoint file
    model = init_detector(
        config, args.checkpoint, device=args.device, cfg_options={})

    if not os.path.exists(args.out_dir) and not args.show:
        os.mkdir(args.out_dir)

    # init visualizer
    visualizer = VISUALIZERS.build(model.cfg.visualizer)
    visualizer.dataset_meta = model.dataset_meta

    # get file list
    files, source_type = get_file_list(args.img)

    # start detector inference
    print(f'Performing inference on {len(files)} images.... '
          'This may take a while.')
    progress_bar = ProgressBar(len(files))
    for file in files:
        # read image
        img = mmcv.imread(file)

        # arrange slices
        height, width = img.shape[:2]
        sliced_image_object = slice_image(
            img,
            slice_height=args.patch_size,
            slice_width=args.patch_size,
            auto_slice_resolution=False,
            overlap_height_ratio=args.patch_overlap_ratio,
            overlap_width_ratio=args.patch_overlap_ratio,
        )
        # perform sliced inference
        slice_results = []
        start = 0
        while True:
            # prepare batch slices
            end = min(start + args.batch_size, len(sliced_image_object))
            images = []
            for sliced_image in sliced_image_object.images[start:end]:
                images.append(sliced_image)

            # forward the model
            slice_results.extend(inference_detector(model, images))

            if end >= len(sliced_image_object):
                break
            start += args.batch_size

        if source_type['is_dir']:
            filename = os.path.relpath(file, args.img).replace('/', '_')
        else:
            filename = os.path.basename(file)

        img = mmcv.imconvert(img, 'bgr', 'rgb')
        out_file = None if args.show else os.path.join(args.out_dir, filename)

        # export debug images
        if args.debug:
            # export sliced image results
            name, suffix = os.path.splitext(filename)

            shifted_instances = shift_predictions(
                slice_results,
                sliced_image_object.starting_pixels,
                src_image_shape=(height, width))
            merged_result = slice_results[0].clone()
            print(shifted_instances)
            merged_result.pred_instances = shifted_instances

            debug_file_name = name + '_debug' + suffix
            debug_out_file = None if args.show else os.path.join(
                args.out_dir, debug_file_name)
            visualizer.set_image(img.copy())

            debug_grids = []
            for starting_point in sliced_image_object.starting_pixels:
                start_point_x = starting_point[0]
                start_point_y = starting_point[1]
                end_point_x = start_point_x + args.patch_size
                end_point_y = start_point_y + args.patch_size
                debug_grids.append(
                    [start_point_x, start_point_y, end_point_x, end_point_y])
            debug_grids = np.array(debug_grids)
            debug_grids[:, 0::2] = np.clip(debug_grids[:, 0::2], 1,
                                           img.shape[1] - 1)
            debug_grids[:, 1::2] = np.clip(debug_grids[:, 1::2], 1,
                                           img.shape[0] - 1)

            palette = np.random.randint(0, 256, size=(len(debug_grids), 3))
            palette = [tuple(c) for c in palette]
            line_styles = random.choices(['-', '-.', ':'], k=len(debug_grids))
            visualizer.draw_bboxes(
                debug_grids,
                edge_colors=palette,
                alpha=1,
                line_styles=line_styles)
            visualizer.draw_bboxes(
                debug_grids, face_colors=palette, alpha=0.15)

            visualizer.draw_texts(
                list(range(len(debug_grids))),
                debug_grids[:, :2] + 5,
                colors='w')

            visualizer.add_datasample(
                debug_file_name,
                visualizer.get_image(),
                data_sample=merged_result,
                draw_gt=False,
                show=args.show,
                wait_time=0,
                out_file=debug_out_file,
                pred_score_thr=args.score_thr,
            )

            if args.save_patch:
                debug_patch_out_dir = os.path.join(args.out_dir,
                                                   f'{name}_patch')
                for i, slice_result in enumerate(slice_results):
                    patch_out_file = os.path.join(
                        debug_patch_out_dir,
                        f'{filename}_slice_{i}_result.jpg')
                    image = mmcv.imconvert(sliced_image_object.images[i],
                                           'bgr', 'rgb')

                    visualizer.add_datasample(
                        'patch_result',
                        image,
                        data_sample=slice_result,
                        draw_gt=False,
                        show=False,
                        wait_time=0,
                        out_file=patch_out_file,
                        pred_score_thr=args.score_thr,
                    )

        image_result = merge_results_by_nms(
            slice_results,
            sliced_image_object.starting_pixels,
            src_image_shape=(height, width),
            nms_cfg={
                'type': args.merge_nms_type,
                'iou_threshold': args.merge_iou_thr
            })
        
        visualizer.add_datasample(
            filename,
            img,
            data_sample=image_result,
            draw_gt=False,
            show=args.show,
            wait_time=0,
            out_file=out_file,
            pred_score_thr=args.score_thr,
        )
        progress_bar.update()

        tensor_bboxes_out_file_json = os.path.join(args.out_dir, "detection_bboxes.json")
        json_drawn_out_file_img = os.path.join(args.out_dir, "detection_bboxes_printed_" + os.path.basename(out_file))

        # Convert image_result to JSON
        json_data = detdatasample_to_dict(image_result)

        # Save as JSON
        with open(tensor_bboxes_out_file_json, 'w') as f:
            json.dump(json_data, f, indent=2)

        print(f"JSON data saved to {tensor_bboxes_out_file_json}")

        draw_bboxes_from_json(tensor_bboxes_out_file_json, file, json_drawn_out_file_img)

    if not args.show or (args.debug and args.save_patch):
        print_log(f'\nResults have been saved at {os.path.abspath(args.out_dir)}')


if __name__ == '__main__':
    main()
