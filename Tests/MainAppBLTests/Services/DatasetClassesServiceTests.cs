using AutoMapper;
using DAL.Interfaces.Repositories.DatasetRepositories;
using DTOs.MainApp.BL.DatasetDTOs;
using Entities.DatasetEntities;
using MainApp.BL.Services.DatasetServices;
using Moq;
using SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
            Assert.IsType<List<DatasetClassDTO>>(result);
            Assert.Single(result); 
        }

        [Fact]
        public async Task GetAllDatasetClasses_WhenRepositoryReturnsNull_ShouldThrowException()
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
            )).ReturnsAsync(ResultDTO<IEnumerable<DatasetClass>>.Ok(null));

            // Create instance of service
            var service = new DatasetClassesService(
                datasetsRepositoryMock.Object,
                datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepositoryMock.Object,
                mapperMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAllDatasetClasses());
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
            Assert.IsType<List<DatasetClassDTO>>(result);
            Assert.Equal(2, result.Count); 
        }

        [Fact]
        public async Task GetAllDatasetClassesByDatasetId_WhenMapperReturnsNull_ShouldThrowException()
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

            // Setup mapper mock to return null
            mapperMock.Setup(mapper => mapper.Map<List<DatasetClassDTO>>(It.IsAny<IEnumerable<DatasetClass>>())).Returns((IEnumerable<DatasetClass> source) => null);

            // Create instance of service
            var service = new DatasetClassesService(
                datasetsRepositoryMock.Object,
                datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepositoryMock.Object,
                mapperMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAllDatasetClassesByDatasetId(datasetId));
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
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.ClassName, result.ClassName);
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
            Assert.Equal(expectedDto.Id, result.Id);
            Assert.Equal(expectedDto.ClassName, result.ClassName);
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

            mockDatasetClassesRepository.Setup(repo => repo.Delete(It.IsAny<DatasetClass>(),true,default))
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
            Assert.Null(result.ErrMsg);
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
        public async Task EditDatasetClass_Throws_Exception_When_Child_Classes_Exist()
        {
            // Arrange
            var dto = new EditDatasetClassDTO
            {
                Id = Guid.NewGuid(),
                ClassName = "EditedClass"
            };

            var childClasses = new List<DatasetClass>
            {
                new DatasetClass { Id = Guid.NewGuid(), ClassName = "ChildClass", ParentClassId = dto.Id }
            };

            var childClassesResult = ResultDTO<IEnumerable<DatasetClass>>.Ok(childClasses);

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                         .ReturnsAsync(childClassesResult);

            var mapperMock = new Mock<IMapper>();

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: null,
                mapper: mapperMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.EditDatasetClass(dto));
        }
             
        [Fact]
        public async Task DeleteDatasetClass_Throws_Exception_When_Child_Classes_Exist()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var childClasses = new List<DatasetClass>
            {
                new DatasetClass { Id = Guid.NewGuid(), ParentClassId = classId }
            };

            var childClassesResult = ResultDTO<IEnumerable<DatasetClass>>.Ok(childClasses);

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();
            datasetClassesRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                         .ReturnsAsync(childClassesResult);

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();

            var mapperMock = new Mock<IMapper>();

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: mapperMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeleteDatasetClass(classId));
        }

        [Fact]
        public async Task DeleteDatasetClass_Throws_Exception_When_Class_In_Use_By_Datasets()
        {
            // Arrange
            var classId = Guid.NewGuid();
            var datasetDatasetClasses = new List<Dataset_DatasetClass>
            {
                new Dataset_DatasetClass { DatasetClassId = classId }
            };

            var datasetDatasetClassesResult = ResultDTO<IEnumerable<Dataset_DatasetClass>>.Ok(datasetDatasetClasses);

            var datasetDatasetClassRepositoryMock = new Mock<IDataset_DatasetClassRepository>();
            datasetDatasetClassRepositoryMock.Setup(repo => repo.GetAll(It.IsAny<Expression<Func<Dataset_DatasetClass, bool>>>(), null, It.IsAny<bool>(), It.IsAny<string>(), null))
                                             .ReturnsAsync(datasetDatasetClassesResult);

            var datasetClassesRepositoryMock = new Mock<IDatasetClassesRepository>();

            var mapperMock = new Mock<IMapper>();

            var service = new DatasetClassesService(
                datasetsRepository: null,
                datasetClassesRepository: datasetClassesRepositoryMock.Object,
                datasetDatasetClassRepository: datasetDatasetClassRepositoryMock.Object,
                mapper: mapperMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.DeleteDatasetClass(classId));
        }
    }
}
