# Waste Detection Usefull Links

## Contents  

- [Models](#models)
- [Datasets](#datasets)

## Models  

### Detr

- Link:
  - [Hugging Face COCO](https://huggingface.co/facebook/detr-resnet-101-dc5)

### ConvNeXt V2  

- Link:  
  - <https://github.com/facebookresearch/ConvNeXt-V2>  

### Detectron2

- Description:
  - Detectron2 is Facebook AI Research's next generation library that provides state-of-the-art detection and segmentation algorithms. It is the successor of Detectron and maskrcnn-benchmark. It supports a number of computer vision research projects and production applications in Facebook.  

- Link:  
  - <https://github.com/facebookresearch/detectron2>  
  - <https://stackoverflow.com/questions/60631933/install-detectron2-on-windows-10>  

- Cons:
  - Linux only _(officially)_

### MMDetection

- MMDetection is an open source object detection toolbox based on PyTorch. It is a part of the OpenMMLab project.

- Link:  
  - <https://github.com/open-mmlab/mmdetection>  

- Cons:
  - Linux only _(officially)_

### Segment Anything  

- Description:  
  - The Segment Anything Model (SAM) produces high quality object masks from input prompts such as points or boxes, and it can be used to generate masks for all objects in an image. It has been trained on a dataset of 11 million images and 1.1 billion masks, and has strong zero-shot performance on a variety of segmentation tasks.

- License:  
  - Apache-2.0 license  

- Link:  
  - [GitHub](https://github.com/facebookresearch/segment-anything)
  - [Do not train](https://medium.com/@rustemgal/fine-tune-segment-anything-model-9877993d9db9)

- Cons:  
  - No classification, only segmentation  

### 🦕 Grounding DINO  

- Grounding DINO: Marrying DINO with Grounded Pre-Training for Open-Set Object Detection  

- Description:  
  - In this paper, we present an open-set object detector, called Grounding DINO, by marrying Transformer-based detector DINO with grounded pre-training, which can detect arbitrary objects with human inputs such as category names or referring expressions. The key solution of open-set object detection is introducing language to a closed-set detector for open-set concept generalization. To effectively fuse language and vision modalities, we conceptually divide a closed-set detector into three phases and propose a tight fusion solution, which includes a feature enhancer, a language-guided query selection, and a cross-modality decoder for cross-modality fusion. While previous works mainly evaluate open-set object detection on novel categories, we propose to also perform evaluations on referring expression comprehension for objects specified with attributes. Grounding DINO performs remarkably well on all three settings, including benchmarks on COCO, LVIS, ODinW, and RefCOCO/+/g. Grounding DINO achieves a 52.5 AP on the COCO detection zero-shot transfer benchmark, i.e., without any training data from COCO. It sets a new record on the ODinW zero-shot benchmark with a mean 26.1 AP. Code will be available at \url{this https URL}.  

- Link:  
  - [GitHub](https://github.com/IDEA-Research/GroundingDINO)  

- Cons:
  - Object detection is not good enough

### lang-segment-anything

- Link:  
  - [GitHub](https://github.com/luca-medeiros/lang-segment-anything)  

- Cons:  
  - Not needed  

### YOLO v6  

- Train custom data example

- Description:  
  - YOLOv6: a single-stage object detection framework dedicated to industrial applications.

- Link :
  - [GitHub](https://github.com/meituan/YOLOv6)

- Cons:
  - Not YOLO v8  

### YOLO v8

- Description:  
  - Ultralytics YOLOv8 is a cutting-edge, state-of-the-art (SOTA) model that builds upon the success of previous YOLO versions and introduces new features and improvements to further boost performance and flexibility. YOLOv8 is designed to be fast, accurate, and easy to use, making it an excellent choice for a wide range of object detection and tracking, instance segmentation, image classification and pose estimation tasks.  

- Link:  
  - [GitHub](https://github.com/ultralytics/ultralytics)  
  - [Tutorial](https://medium.com/@beyzaakyildiz/what-is-yolov8-how-to-use-it-b3807d13c5ce)
  - [Tips for Best Training Results](https://docs.ultralytics.com/yolov5/tutorials/tips_for_best_training_results/)
  - [Hot it works Explanation](https://www.datacamp.com/blog/yolo-object-detection-explained)
  - [How to use it](https://medium.com/@beyzaakyildiz/what-is-yolov8-how-to-use-it-b3807d13c5ce)
  - [Explanation](https://medium.com/cord-tech/yolov8-for-object-detection-explained-practical-example-23920f77f66a)
  - [YOLOYOLOYOLO](https://medium.com/@rustemgal/yolov8-efficientdet-faster-r-cnn-or-yolov5-for-remote-sensing-12487c40ef68)

- Cons:
  - Might need an enterprise license  

### CEASC: Adaptive Sparse Convolutional Networks with Global Context Enhancement for Faster Object Detection on Drone Images

- Description:
  - Object detection on drone images with low-latency is an important but challenging task on the resource-constrained unmanned aerial vehicle (UAV) platform. This paper investigates optimizing the detection head based on the sparse convolution, which proves effective in balancing the accuracy and efficiency. Nevertheless, it suffers from inadequate integration of contextual information of tiny objects as well as clumsy control of the mask ratio in the presence of foreground with varying scales. To address the issues above, we propose a novel global context-enhanced adaptive sparse convolutional network (CEASC). It first develops a context-enhanced group normalization (CE-GN) layer, by replacing the statistics based on sparsely sampled features with the global contextual ones, and then designs an adaptive multi-layer masking strategy to generate optimal mask ratios at distinct scales for compact foreground coverage, promoting both the accuracy and efficiency. Extensive experimental results on two major benchmarks, i.e. VisDrone and UAVDT, demonstrate that CEASC remarkably reduces the GFLOPs and accelerates the inference procedure when plugging into the typical state-of-the-art detection frameworks (e.g. RetinaNet and GFL V1) with competitive performance. Code is available at <https://github.com/Cuogeihong/CEASC>.

- Link:  
  - <https://github.com/cuogeihong/ceasc>  
  - <https://paperswithcode.com/paper/adaptive-sparse-convolutional-networks-with>

### Autodistill  

- Description:  
  - Autodistill uses big, slower foundation models to train small, faster supervised models. Using autodistill, you can go from unlabeled images to inference on a custom model running at the edge with no human intervention in between.  

- Link:  
  - <https://github.com/autodistill/autodistill>  

### ArcGIS Pro 3.1  

- Description:  
  - Segmentation and classification tools provide an approach to extracting features from imagery based on objects. These objects are created via an image segmentation process where pixels in close proximity and having similar spectral characteristics are grouped together into a segment. Segments exhibiting certain shapes, spectral, and spatial characteristics can be further grouped into objects. The objects can then be grouped into classes that represent real-world features on the ground. Image classification can also be performed on pixel imagery, for example, traditional unsegmented imagery.

- Link:  
  - <https://pro.arcgis.com/en/pro-app/latest/tool-reference/spatial-analyst/understanding-segmentation-and-classification.htm#:~:text=Segmentation%20and%20classification%20tools%20provide%20an%20approach%20to,spectral%20characteristics%20are%20grouped%20together%20into%20a%20segment>.  

#### More Unexplored Links

- [Grounded Segment Anything](https://console.paperspace.com/github/gradient-ai/Grounded-Segment-Anything/?machine=Free-GPU&ref=blog.paperspace.com&_gl=1*1c29uo7*_gcl_au*MjIwNzcwMDA2LjE2OTYzMjMwMjM.)  

- [List of Object Detection Models](https://paperswithcode.com/methods/category/object-detection-models)  

- <https://www.mdpi.com/2504-446X/5/4/125>

- <https://www.mdpi.com/2072-4292/13/5/965?fbclid=IwAR1CDEjjLufvYE2UNth_-XhLwXm_q_HjFbXe6I6Ka6GYsFR-YFb04WOKn2c>  

- [Stock Drone Images / Videos](https://www.storyblocks.com/video/stock/aerial-drone-view-over-garbages-along-an-asphalt-road-illegal-waste-dump-france-bd-bpezmwhjxvuckaa)  

- [A Recipe for Training Neural Networks](http://karpathy.github.io/2019/04/25/recipe/)  

- [Pre-Trained-Model vs From-Scratch-Model](https://heartbeat.comet.ml/pre-trained-machine-learning-models-vs-models-trained-from-scratch-63e079ed648f)  

## Datasets

### UDD - Urban Drone Dataset(UDD)

- Drone Imagery
- Link:
  - [GitHub](https://github.com/MarcWong/UDD/tree/master)

### VisDrone-Dataset

- Drone Imagery
- Link:
  - [Git Hub](https://github.com/VisDrone/VisDrone-Dataset)  
  - (<https://huggingface.co/mshamrai/yolov8x-visdrone>)

### add303train Computer Vision Project

- Sattelite Imagery
- Link:
  - [Roboflow] (<https://universe.roboflow.com/add300train/add303train>)

## DPG Digital Public Goods

## Unit Testing

## Integration Testing

## GitHubActions

## Open Source Security - openssf

    - [Course for Open Source Security] (https://openssf.org/training/courses/)
    - [Developing Secure Software](https://training.linuxfoundation.org/training/developing-secure-software-lfd121/)
    - security

dhis2

spdh

agora.unicef.com

<https://digitalpublicgoods.net/>

public cloud
    -
    - forks
    - GitHub actions
    - sub repo
    - real the doc python

Not in solution:
    Optional for processing drone images :
        Open Drone Map

UNICEF: AI Mentorship:  
    - The plan for the data  
        - Gathering  
        - Storing  
        - Updating  

### Ivan Links  

- [](https://htmx.org/docs/#3rd-party)  
- [](https://digitalpublicgoods.net/blog/exploring-a-gradient-approach-to-the-openness-of-ai-system-components/)  
- [](https://dpg-website.s3.amazonaws.com/wp-content/uploads/2023/10/27134510/AI-DPGs-gradient-openness3-1.jpg)  
- [](https://blog.opensource.org/dpga-members-engage-in-open-source-ai-definition-workshop/)  
- [](https://docs.astral.sh/ruff/)  
- pytest --coverage  
- [Open Street Map Attribution for Use](https://osmfoundation.org/wiki/Licence/Attribution_Guidelines)
- about.codecov.io

What is being tested in JS ?  
Pure function ?
DOM changes ?

If endpoints return HTML is the HTML generation tested ?  

How are .NET Components and Partial Views supposed to be tested ?

How is code coverage coputed for integration tests ?

Does code coverage have to be automatic ?
