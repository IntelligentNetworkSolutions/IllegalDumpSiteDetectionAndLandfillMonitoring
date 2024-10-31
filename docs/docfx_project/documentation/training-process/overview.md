# Training Process Overview

## Introduction

The training process is a core component of the Illegal Dump Site Detection system, handling the training of machine learning models for waste detection. The system uses MMDetection as its underlying framework and implements a robust workflow for model training management.

## Key Components

### [Core Entities](./entities.md)

- **TrainingRun**: Represents a single training session
  - Tracks the training progress and status
  - Links to the dataset and model being trained
  - Maintains training parameters and configuration
- **TrainedModel**: Represents a trained model artifact
  - Stores model files and configurations
  - Tracks model lineage through base model relationships
  - Manages model publication status

### [Services](./services.md)

- **TrainingRunService**: Manages training execution and lifecycle
- **TrainedModelService**: Handles model management and metadata

### [Controllers](./controllers.md)

- **TrainingRunsController**: Exposes training operations via API
- **TrainedModelsController**: Handles model management operations

## High-Level Process Flow

1. **Training Initialization**
   - User requests new training run
   - System validates parameters and creates TrainingRun entity
   - Training configuration is generated

2. **Execution**
   - Training job is queued via Hangfire
   - Dataset is prepared in COCO format
   - MMDetection training script is executed
   - Progress is monitored and status updated

3. **Completion**
   - Results are processed and validated
   - TrainedModel entity is created
   - Training artifacts are stored
   - Metrics are recorded

4. **Post-Training**
   - Model can be published for production use
   - Results can be reviewed and analyzed
   - Model can be used as base for further training

## Key Features

- Async training execution via background jobs
- Training progress monitoring and status tracking
- Error handling and logging
- Model versioning and lineage tracking
- Access control via authorization claims
- Integration with MMDetection framework

## System Requirements

- .NET 8.0
- PostgreSQL 16 with PostGIS
- Python environment with MMDetection
- Hangfire for background job processing
