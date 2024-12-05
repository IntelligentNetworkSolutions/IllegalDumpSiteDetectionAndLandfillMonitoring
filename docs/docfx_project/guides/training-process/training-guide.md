# Training process  

Training a deep learning model entails providing it with data and fine-tuning its parameters to enable it to make precise predictions.  

### Introduction  

The Raven Scan Platform helps you train AI models to detect waste in images.  
This guide will walk you through the process of creating and managing training runs, and working with trained models.

### Getting Started

#### Prerequisites

- Valid system account with appropriate permissions
- Prepared dataset for training
- Base model for transfer learning (if performing fine-tuning)

#### Required Permissions

To use the training system, you need one or more of these permissions:

- Create Training Run
- Schedule Training Run
- View Training Runs
- Delete Training Run
- Publish Training Run Model

### Creating a New Training Run  

**Step 1: Initialize New Training**  

1. Navigate to the Training section  
2. Click "Create New Training Run"  
3. You'll be presented with the training setup form  

**Step 2: Configure Training**  

Fill in the required information:  

- **Name**: Give your training run a descriptive name  
- **Dataset**: Select the dataset you want to use  
- **Base Model**: Choose a pre-trained model to start from  

**Step 3: Start Training**  

1. Review your settings  
2. Click "Schedule Training Run"  
3. The system will validate your inputs and start the process  

> [!IMPORTANT]
>
> Once training starts, it may take several hours to complete depending on your dataset size and configuration.  

### Monitoring Training Progress  

**Checking Training Progress**  

1. Navigate to the Training Runs list  
2. Find your training run  
3. The status will update automatically  
4. Click on a training run to view detailed progress  

**Viewing Training Status**  

Training runs can have the following statuses  

- **Waiting**: Training is queued  
- **Processing**: Training is actively running  
- **Success**: Training completed successfully  
- **Error**: Training encountered an error  

#### Error Handling  

If your training run encounters an error:  

1. Click on the training run  
2. Look for the error message  
3. Review the error logs for details  
4. You may need to start a new training run with adjusted parameters  

### Managing Trained Models  

**Reviewing Results**  

After successful training:  

1. Navigate to the Trained Models section  
2. Find your newly created model  
3. Review the training metrics and performance  

### Publishing Models  

To make a model available for detection:  

1. Select the trained model  
2. Click "Publish Model"  
3. Confirm the publication  

> [!NOTE]
>
> Only publish models that have been properly validated and meet your performance requirements.  

### Model Management  

- View all trained models  
- Check model details and configurations  
- See which dataset was used  
- Track model lineage (base model relationships)  

### Common Issues  

#### Training Won't Start  

- Check if you have the necessary permissions  
- Verify dataset is properly prepared  
- Ensure base model is accessible  

#### Training Fails  

1. Check the error message  
2. Review the training logs  
3. Common causes  
   - Dataset format issues  
   - Resource constraints  
   - Configuration problems  

#### Model Won't Publish  

- Ensure training completed successfully  
- Check you have publish permissions  
- Verify model meets quality requirements  

### Training Tips  

- Use descriptive names for training runs  
- Start with smaller datasets to validate configuration  
- Monitor training progress regularly  
- Keep track of successful configurations  

> [!TIP]
> Need more help? Contact your system administrator or check the [**Training Documentation**](../../documentation/training-process) here.  
