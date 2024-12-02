using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Services.DatasetServices;
using Moq;
using SD;
using System.Linq.Expressions;

namespace Tests.MainAppBLTests.Services
{
    public class DatasetClassesServiceTests
    {
        [Fact]
        public async Task GetAllDatasetClasses_ShouldReturnListOfDatasetClassDTO()
        {
            // Arrange
            var datasetsRepositoryMock = new Mock<IDatasetsRepository>();
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            var mapperMock = new Mock<IMapper>();

            // Mock data
            var datasetClasses = new List<DatasetClass>();
            datasetClasses.Add(new DatasetClass
            {
                Id = Guid.NewGuid(),
                ClassName = "Class 1",
                CreatedById = "user1",
                CreatedOn = DateTime.UtcNow,
                Datasets = new List<Dataset_DatasetClass>()
            });

            // Setup repository mock
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(
                null,
                It.IsAny<Func<IQueryable<DatasetClass>, IOrderedQueryable<DatasetClass>>>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<int?>()
            )).ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(datasetClasses));

            // Setup mapper mock
            mapperMock.Setup(mapper => mapper.Map<List<DatasetClassDTO>>(datasetClasses)).Returns(new List<DatasetClassDTO>
            {
                new DatasetClassDTO
                {
                    Id = datasetClasses[0].Id,
                    ClassName = datasetClasses[0].ClassName,
                    CreatedById = datasetClasses[0].CreatedById,
                    CreatedOn = datasetClasses[0].CreatedOn,
                    Datasets = new List<Dataset_DatasetClassDTO>()
                }
            });

            // Create instance of service
            var service = new DatasetClassesService(
                datasetsRepositoryMock.Object,
                datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepositoryMock.Object,
                mapperMock.Object
            );

            // Act
            var result = await service.GetAllDatasetClasses();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultDTO<List<DatasetClassDTO>>>(result);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetAllDatasetClasses_WhenRepositoryReturnsNull_ReturnsFailResult()
        {
            // Arrange
            var datasetsRepositoryMock = new Mock<IDatasetsRepository>();
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            var mapperMock = new Mock<IMapper>();

            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(
                null,
                It.IsAny<Func<IQueryable<DatasetClass>, IOrderedQueryable<DatasetClass>>>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<int?>()
            )).ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Fail("Repository failed"));

            // Create instance of service
            var service = new DatasetClassesService(
                datasetsRepositoryMock.Object,
                datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepositoryMock.Object,
                mapperMock.Object
            );

            // Act
            var result = await service.GetAllDatasetClasses();

            // Assert
            Assert.True(!result.IsSuccess);
            Assert.Equal("Repository failed", result.ErrMsg);
        }

        [Fact]
        public async Task GetAllDatasetClassesByDatasetId_WhenRepositoryReturnsData_ShouldReturnListOfDatasetClassDTO()
        {
            // Arrange
            var datasetsRepositoryMock = new Mock<IDatasetsRepository>();
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            var mapperMock = new Mock<IMapper>();

            // Mock data
            var datasetId = Guid.NewGuid();
            var datasetClassEntities = new List<Dataset_DatasetClass>
            {
                new Dataset_DatasetClass { DatasetId = datasetId, DatasetClass = new DatasetClass { Id = Guid.NewGuid(), ClassName = "Class 1" } },
                new Dataset_DatasetClass { DatasetId = datasetId, DatasetClass = new DatasetClass { Id = Guid.NewGuid(), ClassName = "Class 2" } }
            };

            // Setup repository mock
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(
                It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(),
                It.IsAny<Func<IQueryable<Dataset_DatasetClass>, IOrderedQueryable<Dataset_DatasetClass>>>(),
                It.IsAny<bool>(),
                It.IsAny<string>(),
                It.IsAny<int?>()
            )).ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClassEntities));

            // Setup mapper mock
            mapperMock.Setup(mapper => mapper.Map<List<DatasetClassDTO>>(It.IsAny<IEnumerable<DatasetClass>>())).Returns((IEnumerable<DatasetClass> source) =>
            {
                return source.Select(x => new DatasetClassDTO
                {
                    Id = x.Id,
                    ClassName = x.ClassName,
                    CreatedById = x.CreatedById,
                    CreatedOn = x.CreatedOn
                }).ToList();
            });

            // Create instance of service
            var service = new DatasetClassesService(
                datasetsRepositoryMock.Object,
                datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepositoryMock.Object,
                mapperMock.Object
            );

            // Act
            var result = await service.GetAllDatasetClassesByDatasetId(datasetId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ResultDTO<List<DatasetClassDTO>>>(result);
            Assert.Equal(2, result.Data.Count);
        }

        [Fact]
        public async Task GetDatasetClassById_Returns_Correct_DatasetClassDTO()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass { Id = classId, ClassName = "TestClass" };
            var expectedDto = new DatasetClassDTO { Id = classId, ClassName = "TestClass" };

            // Mocking repositories and mapper
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()))
                                         .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<DatasetClassDTO>(datasetClass))
                      .Returns(expectedDto);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.GetDatasetClassById(classId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Data.Id);
            Assert.Equal(expectedDto.ClassName, result.Data.ClassName);
        }

        [Fact]
        public async Task GetDatasetClassById_Returns_DatasetClassDTO_When_Class_Found()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass
            {
                Id = classId,
                ClassName = "TestClass",
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            var expectedDto = new DatasetClassDTO
            {
                Id = classId,
                ClassName = "TestClass",
                CreatedById = "user123",
                CreatedOn = DateTime.UtcNow
            };

            // Mocking repositories and mapper
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<string>()))
                                         .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<DatasetClassDTO>(datasetClass))
                      .Returns(expectedDto);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.GetDatasetClassById(classId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Id, result.Data.Id);
            Assert.Equal(expectedDto.ClassName, result.Data.ClassName);
        }

        [Fact]
        public async Task AddDatasetClass_With_No_ParentClass_Returns_Success_Result()
        {
            // Arrange
            var dto = new CreateDatasetClassDTO
            {
                ClassName = "NewClass"
            };

            var newClass = new DatasetClass
            {
                Id = Guid.NewGuid(),
                ClassName = "NewClass"
            };

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.Create(It.IsAny<DatasetClass>(), It.IsAny<bool>(), default))
                                         .ReturnsAsync(ResultDTO.Ok());

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<DatasetClass>(dto))
                      .Returns(newClass);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.AddDatasetClass(dto);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task AddDatasetClass_With_Creation_Failure_Returns_Error_Result()
        {
            // Arrange
            var dto = new CreateDatasetClassDTO
            {
                ClassName = "NewClass"
            };

            var newClass = new DatasetClass
            {
                Id = Guid.NewGuid(),
                ClassName = "NewClass"
            };

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.Create(It.IsAny<DatasetClass>(), It.IsAny<bool>(), default))
                                         .ReturnsAsync(ResultDTO.Fail("Failed to create dataset class."));

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<DatasetClass>(dto))
                      .Returns(newClass);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.AddDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to create dataset class.", result.ErrMsg);
        }

        [Fact]
        public async Task AddDatasetClass_With_ValidParentClassId_WithSubclass_Returns_Error_Result()
        {
            // Arrange
            var dto = new CreateDatasetClassDTO
            {
                ClassName = "NewClass",
                ParentClassId = Guid.NewGuid() // Set a valid ParentClassId
            };

            var parentClass = new DatasetClass
            {
                Id = dto.ParentClassId.Value,
                ClassName = "ParentClass",
                ParentClassId = Guid.NewGuid() // Parent has its own parent, indicating it's a subclass
            };

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(dto.ParentClassId.Value, false, It.IsAny<string>()))
                                         .ReturnsAsync(ResultDTO<DatasetClass?>.Ok(parentClass));

            var mapperMock = new Mock<IMapper>();
            var newClass = new DatasetClass { Id = Guid.NewGuid(), ClassName = "NewClass" };
            mapperMock.Setup(mapper => mapper.Map<DatasetClass>(dto)).Returns(newClass);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.AddDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("You cannot add this class as a subclass because the selected parent class is already set as a subclass!", result.ErrMsg);
        }

        [Fact]
        public async Task AddDatasetClass_With_NonExistentParentClassId_ReturnsFailResult()
        {
            // Arrange
            var dto = new CreateDatasetClassDTO
            {
                ClassName = "NewClass",
                ParentClassId = Guid.NewGuid()
            };

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(dto.ParentClassId.Value, false, It.IsAny<string>()))
                                         .ReturnsAsync(ResultDTO<DatasetClass>.Fail("Parent class not found"));

            var mapperMock = new Mock<IMapper>();
            var newClass = new DatasetClass { Id = Guid.NewGuid(), ClassName = "NewClass" };
            mapperMock.Setup(mapper => mapper.Map<DatasetClass>(dto)).Returns(newClass);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act 
            var result = await service.AddDatasetClass(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Parent class not found", result.ErrMsg);

        }


        [Fact]
        public async Task DeleteDatasetClass_Should_Return_Success_When_No_Child_Classes_And_No_Dataset_Usage()
        {
            // Arrange
            var classId = Guid.NewGuid();

            var mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockDatasetRepository = new Mock<IDatasetsRepository>();
            var mockMapper = new Mock<IMapper>();

            mockDatasetClassesRepository.Setup(repo => repo.GetById(classId, true, "CreatedBy,ParentClass"))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(new DatasetClass()));

            mockDatasetClassesRepository.Setup(repo => repo.GetAll(null, null, false, "CreatedBy,ParentClass,Datasets", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(x => x.DatasetClassId == classId, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(null, null, false, null, null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            mockDatasetClassesRepository.Setup(repo => repo.Delete(It.IsAny<DatasetClass>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var service = new DatasetClassesService(
                mockDatasetRepository.Object,
                mockDatasetClassesRepository.Object,
                mockDatasetDatasetClassRepository.Object,
                mockMapper.Object
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Dataset class deleted successfully.", result.ErrMsg);
        }


        [Fact]
        public async Task EditDatasetClass_Should_Return_Success_When_No_Child_Classes_And_No_Dataset_Usage()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "Dto class"
            };

            var datasetClassDb = new DatasetClass();

            var mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockDatasetRepository = new Mock<IDatasetsRepository>();
            var mockMapper = new Mock<IMapper>();

            mockDatasetClassesRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), true, "CreatedBy,ParentClass"))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClassDb));

            mockDatasetClassesRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, false, "CreatedBy,ParentClass,Datasets", null))
               .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            mockMapper.Setup(mapper => mapper.Map<EditDatasetClassDTO, DatasetClass>(dto, It.IsAny<DatasetClass>()))
                     .Returns(datasetClassDb);

            var updatedResult = new ResultDTO<int>(true, 1, null, null);
            mockDatasetClassesRepository.Setup(repo => repo.Update(It.IsAny<DatasetClass>(), true, default))
                .ReturnsAsync(ResultDTO.Ok());

            var service = new DatasetClassesService(
                mockDatasetRepository.Object,
                mockDatasetClassesRepository.Object,
                mockDatasetDatasetClassRepository.Object,
                mockMapper.Object
            );

            // Act
            var result = await service.EditDatasetClass(dto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetClass_Returns_Error_When_Child_Classes_Exist()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "EditedClass",
                ParentClassId = Guid.NewGuid(),
            };
            var datasetClass = new DatasetClass
            {
                Id = dto.Id,
                ClassName = "TestClass"
            };
            var childClasses = new List<DatasetClass>
            {
                new DatasetClass { Id = Guid.NewGuid(), ClassName = "ChildClass", ParentClassId = dto.Id }
            };

            var datasetClassResult = ResultDTO<DatasetClass>.Ok(datasetClass);

            var allClassesResult = ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>
            {
                datasetClass,
                new DatasetClass { Id = Guid.NewGuid(), ClassName = "AnotherClass", ParentClassId = datasetClass.Id }
            });

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                        .ReturnsAsync(allClassesResult);
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(dto.Id, true, "CreatedBy,ParentClass"))
                                        .ReturnsAsync(datasetClassResult);

            var mapperMock = new Mock<IMapper>();

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.EditDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data);
            Assert.Equal("This dataset class has subclasses and cannot be set as a subclass too!", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetClass_Should_Return_Error_When_Child_Classes_Exist_And_ParentClassId_Is_Set()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "EditedClass",
                ParentClassId = Guid.NewGuid() // Setting a parent class ID
            };

            var childClasses = new List<DatasetClass>
    {
        new DatasetClass { Id = Guid.NewGuid(), ClassName = "ChildClass", ParentClassId = dto.Id }
    };

            var datasetClassDb = new DatasetClass { Id = dto.Id }; // Simulating existing dataset class
            var datasetDatasetClass = new List<Dataset_DatasetClass>();

            var mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockMapper = new Mock<IMapper>();

            mockDatasetClassesRepository.Setup(repo => repo.GetById(dto.Id, true, "CreatedBy,ParentClass"))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClassDb));

            mockDatasetClassesRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(childClasses));
            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetDatasetClass));

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: mockDatasetClassesRepository.Object,
                datasetDatasetClassRepository: mockDatasetDatasetClassRepository.Object,
                mapper: mockMapper.Object
            );

            // Act
            var result = await service.EditDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data); // Assuming 2 is the error code for this scenario
            Assert.Equal("This dataset class has subclasses and cannot be set as a subclass too!", result.ErrMsg);
        }

        [Fact]
        public async Task EditDatasetClass_Should_Return_Error_When_DatasetClass_Is_Already_In_Use()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "EditedClass",
                ParentClassId = null // No parent class
            };

            var datasetClassDb = new DatasetClass { Id = dto.Id };

            var datasetClassesInUse = new List<Dataset_DatasetClass>
    {
        new Dataset_DatasetClass { DatasetClassId = dto.Id }
    };

            var mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockMapper = new Mock<IMapper>();

            mockDatasetClassesRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), true, "CreatedBy,ParentClass"))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClassDb));

            mockDatasetClassesRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, false, "CreatedBy,ParentClass,Datasets", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetClassesInUse));

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: mockDatasetClassesRepository.Object,
                datasetDatasetClassRepository: mockDatasetDatasetClassRepository.Object,
                mapper: mockMapper.Object
            );

            // Act
            var result = await service.EditDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(3, result.Data); // Error code for dataset usage
            Assert.Equal("The selected class is already in use in dataset(s). You can only change the class name!", result.ErrMsg);
        }


        [Fact]
        public async Task EditDatasetClass_Should_Return_Error_When_ParentClassId_Is_Set_To_Subclass()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "EditedClass",
                ParentClassId = Guid.NewGuid() // Setting a parent class ID
            };

            var parentClassDb = new DatasetClass { Id = dto.ParentClassId.Value, ParentClassId = Guid.NewGuid() }; // Parent class is a subclass
            var datasetClassDb = new DatasetClass { Id = dto.Id };

            var mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockMapper = new Mock<IMapper>();

            mockDatasetClassesRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), true, "CreatedBy,ParentClass"))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClassDb));

            mockDatasetClassesRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, false, "CreatedBy,ParentClass,Datasets", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            mockDatasetClassesRepository.Setup(repo => repo.GetById(dto.ParentClassId.Value, It.IsAny<bool>(), It.IsAny<string>()))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(parentClassDb));

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: mockDatasetClassesRepository.Object,
                datasetDatasetClassRepository: mockDatasetDatasetClassRepository.Object,
                mapper: mockMapper.Object
            );

            // Act
            var result = await service.EditDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(4, result.Data); // Error code for parent class being a subclass
            Assert.Equal("You cannot add this class as a subclass because the selected parent class is already set as a subclass!", result.ErrMsg);
        }


        [Fact]
        public async Task EditDatasetClass_Should_Return_Error_When_Update_Fails()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "EditedClass"
            };

            var datasetClassDb = new DatasetClass { Id = dto.Id };

            var mockDatasetClassesRepository = new Mock<IDatasetClassesRepository>();
            var mockDatasetDatasetClassRepository = new Mock<IDataset_DatasetClassRepository>();
            var mockMapper = new Mock<IMapper>();

            mockDatasetClassesRepository.Setup(repo => repo.GetById(It.IsAny<Guid>(), true, "CreatedBy,ParentClass"))
                .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClassDb));

            mockDatasetClassesRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, false, "CreatedBy,ParentClass,Datasets", null))
                .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            mockDatasetDatasetClassRepository.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, false, "DatasetClass,Dataset", null))
                .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            mockMapper.Setup(mapper => mapper.Map<EditDatasetClassDTO, DatasetClass>(dto, It.IsAny<DatasetClass>()))
                .Returns(datasetClassDb);

            mockDatasetClassesRepository.Setup(repo => repo.Update(It.IsAny<DatasetClass>(), true, default)).ReturnsAsync(ResultDTO.Fail("Update failed"));

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: mockDatasetClassesRepository.Object,
                datasetDatasetClassRepository: mockDatasetDatasetClassRepository.Object,
                mapper: mockMapper.Object
            );

            // Act
            var result = await service.EditDatasetClass(dto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(5, result.Data); // Error code for update failure
            Assert.Equal("Update failed", result.ErrMsg);
        }



        [Fact]
        public async Task DeleteDatasetClass_ReturnsErrorMessage_When_Child_Classes_Exist()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass { Id = classId };
            var datasetClassesList = new List<DatasetClass>();
            var childClasses = new List<DatasetClass>
            {
                new DatasetClass { Id = Guid.NewGuid(), ParentClassId = classId }
            };

            var childClassesResult = ResultDTO<IEnumerable<DatasetClass>>.Ok(childClasses);

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(classId, true, "CreatedBy,ParentClass"))
                            .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "CreatedBy,ParentClass,Datasets", null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(datasetClassesList));
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                         .ReturnsAsync(childClassesResult);


            var mapperMock = new Mock<IMapper>();

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("This dataset class cannot be deleted because there are subclasses. Delete the subclasses first.", result.ErrMsg);
            Assert.Equal(2, result.Data);
        }

        [Fact]
        public async Task DeleteDatasetClass_ReturnsErrorMessage_When_Class_In_Use_By_Datasets()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass { Id = classId };
            var datasetClassesList = new List<DatasetClass>();
            var datasetDatasetClasses = new List<Dataset_DatasetClass>
            {
                new Dataset_DatasetClass { DatasetClassId = classId }
            };

            var datasetDatasetClassesResult = ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetDatasetClasses);

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(classId, true, "CreatedBy,ParentClass"))
                                        .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(null, null, false, "CreatedBy,ParentClass,Datasets", null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(datasetClassesList));
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                             .ReturnsAsync(datasetDatasetClassesResult);

            var mapperMock = new Mock<IMapper>();

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: mapperMock.Object
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("This dataset class cannot be deleted because it is in use in datasets.", result.ErrMsg);
            Assert.Equal(3, result.Data);
        }

        [Fact]
        public async Task DeleteDatasetClass_Returns_Error_When_Child_Classes_Exist()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var childClasses = new List<DatasetClass>
            {
                {
                    new DatasetClass { Id = Guid.NewGuid(), ParentClassId = classId }
                }
            };
            var childClassesResult = ResultDTO<IEnumerable<DatasetClass>>.Ok(childClasses);
            var datasetClass = new DatasetClass { Id = classId };
            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();

            datasetClassesRepositoryMock.Setup(repo => repo.GetById(classId, true, It.IsAny<string>()))
                                        .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));

            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                         .ReturnsAsync(childClassesResult);

            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(null, null, false, It.IsAny<string>(), null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(childClasses));

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                             .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));


            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: null // No mapper needed for this test
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(2, result.Data); // Assuming 2 is the error code for child classes
            Assert.Equal("This dataset class cannot be deleted because there are subclasses. Delete the subclasses first.", result.ErrMsg);
        }

        [Fact]
        public async Task DeleteDatasetClass_Returns_Error_When_Class_In_Use_By_Datasets()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass { Id = classId }; // Create a mock dataset class object

            var datasetDatasetClasses = new List<Dataset_DatasetClass>
    {
        new Dataset_DatasetClass { DatasetClassId = classId }
    };
            var datasetDatasetClassesResult = ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetDatasetClasses);

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(classId, true, It.IsAny<string>()))
                                        .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass)); // Mock the GetById method

            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(null, null, false, It.IsAny<string>(), null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                             .ReturnsAsync(datasetDatasetClassesResult);

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: null // No mapper needed for this test
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(3, result.Data); // Assuming 3 is the error code for dataset usage
            Assert.Equal("This dataset class cannot be deleted because it is in use in datasets.", result.ErrMsg);
        }


        [Fact]
        public async Task DeleteDatasetClass_Deletes_Class_When_No_Children_Or_Usage()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass { Id = classId };

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(classId, true, It.IsAny<string>()))
                                        .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));

            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(null, null, false, It.IsAny<string>(), null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                             .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            datasetDatasetClassRepositoryMock.Setup(repo => repo.DeleteRange(It.IsAny<IEnumerable<Dataset_DatasetClass>>(), true, default))
                                             .ReturnsAsync(ResultDTO.Ok());

            datasetClassesRepositoryMock.Setup(repo => repo.Delete(datasetClass, true, default))
                                        .ReturnsAsync(ResultDTO.Ok());

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: null // No mapper needed for this test
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Data); // Assuming 1 means successful deletion
        }

        [Fact]
        public async Task DeleteDatasetClass_Returns_Error_When_DeleteFails()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetClass = new DatasetClass { Id = classId };

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetById(classId, true, It.IsAny<string>()))
                                        .ReturnsAsync(ResultDTO<DatasetClass>.Ok(datasetClass));

            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(null, null, false, It.IsAny<string>(), null))
                                        .ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(new List<DatasetClass>()));

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                             .ReturnsAsync(ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(new List<Dataset_DatasetClass>()));

            datasetClassesRepositoryMock.Setup(repo => repo.Delete(datasetClass, true, default))
                                        .ReturnsAsync(ResultDTO.Fail("Delete failed")); // Simulate deletion failure

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: null // No mapper needed for this test
            );

            // Act
            var result = await service.DeleteDatasetClass(classId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(4, result.Data); // Assuming 4 is the error code for deletion failure
            Assert.Equal("Delete failed", result.ErrMsg);
        }

    }
}
