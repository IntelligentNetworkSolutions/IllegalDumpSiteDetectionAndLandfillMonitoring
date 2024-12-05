# Image Annotation  

Image annotation is a vital part of training computer vision models that process image data for object detection, classification, segmentation, and more. A dataset of images that have been labeled and annotated to identify and classify specific objects, for example, is required to train an object detection model.  

There are different types of data annotations, in our case we are using Bounding Boxes for data annotation. Bounding boxes are rectangular boxes drawn around objects in an image, used primarily for object detection tasks. These boxes are defined by their top-left and bottom-right coordinates.  

In our Annotations tab we have annotations toolbar. The annotation toolbar provides a set of tools designed to make the annotation process efficient and straightforward. Here’s an overview of the tools available and how they help during the annotation process.  

### Select Tool  

This is the tool used for interacting with existing annotations. It allows users to select an annotation, making it possible to adjust its size, move it to a different position, or update its label. The Select Tool is essential for refining and editing annotations after they’ve been created.  

### Draw Bounding Box (BBox) Tool  

The Draw BBox Tool is the primary tool for creating annotations. It enables users to draw rectangular boxes around objects of interest in the image. These bounding boxes are used to define the boundaries of waste items, ensuring each item is clearly marked and labeled.  

### Undo  

The Undo feature is there to reverse the most recent action. Whether it’s an annotation that was drawn incorrectly or an adjustment that didn’t go as planned, Undo helps quickly fix mistakes without disrupting the workflow.  

### Redo  

Redo complements the Undo feature by restoring an action that was previously undone. This tool ensures users can easily revisit steps they might have removed accidentally or wish to apply again.  

### Save  

The Save tool is crucial for preserving progress. It ensures all annotations are stored securely, either locally or in a database, depending on the app’s setup.  
The tools in the annotation toolbar are carefully designed to provide a balance of simplicity and functionality, enabling users to create, manage, and save annotations with ease.  

> [!IMPORTANT]  
> Classification is also a very important part during the annotation process  
>
> Annotation classification is a critical step in the annotation process, especially when working with datasets for training models like waste recognition systems. It involves assigning a category or label to the annotation, helping to define it.  


Including classification for annotations provides critical information to the machine learning model. By assigning specific categories to each annotated object, the model gains a clearer understanding of the relationships between different types of objects and their features. This enhanced context improves the model's ability to recognize patterns, differentiate between similar objects, and make more accurate predictions.  

> [!TIP]
> For more information, check the [Image Annotation Documentation](../../documentation/dataset-management/image-annotation.md) here.  
