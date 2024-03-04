# Model Config

## type  

- The name of detector  
- Maybe Constant  

## data_preprocessor  

- The config of data preprocessor, usually includes image normalization and padding  
`data_preprocessor=dict(`  

  - `type='DetDataPreprocessor',`  
    - The type of the data preprocessor, refer to [DetDataPreprocessor](https://mmdetection.readthedocs.io/en/latest/api.html#mmdet.models.data_preprocessors.DetDataPreprocessor)  
  - `mean=[123.675, 116.28, 103.53],`
    - Mean values used to pre-training the pre-trained backbone models, ordered in R, G, B  
  - `std=[58.395, 57.12, 57.375],`  
    - Standard variance used to pre-training the pre-trained backbone models, ordered in R, G, B  
  - `bgr_to_rgb=True,`  
    - whether to convert image from BGR to RGB  
  - `pad_mask=True,`  
    - whether to pad instance masks  
  - `pad_size_divisor=32),`  
    - The size of padded image should be divisible by "pad_size_divisor"  

## train_cfg  

## test_cfg  
