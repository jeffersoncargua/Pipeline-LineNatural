using AutoMapper;
using LineNatural.DTOs.Category;
using LineNatural.DTOs.LocalUser;
using LineNatural.DTOs.Product;
using LineNatural.Entities;
using LineNatural.Mapping;
using System.Runtime.Serialization;


namespace LineNatural.UnitTest.Mapping
{
    public class MappingTest
    {
        private readonly IConfigurationProvider _configuration; //Es una interfaz que permite configurar AutoMapper
        private readonly IMapper _mapper; //Es la interfaz que permite utilizar el archivo MappingConfig

        public MappingTest()
        {
            //permite simular la interfaz que permite utilizar el archivo MappingConfig que se encuentra en el 
            //proyecto LineNatural, por lo tanto se observa que se agrega en el AddProflie
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfig>();
            });

            _mapper = _configuration.CreateMapper(); //permite generar la interfaz virtual para utilizar los mapeos de MappingConfig
        }

        /// <summary>
        /// Permite verificar que se ha realizado correctamente la configuracion para utilizar
        /// Mapper
        /// </summary>
        [Fact]
        public void ShouldBeValidConfiguration()
        {
            //Arrange

            //Act

            //Assert

            _configuration.AssertConfigurationIsValid();
        }

        [Theory]
        [InlineData(typeof(Category), typeof(CategoryDto))]
        [InlineData(typeof(CategoryDto), typeof(Category))]
        [InlineData(typeof(Category), typeof(CategoryCreateDto))]
        [InlineData(typeof(Category), typeof(CategoryUpdateDto))]
        [InlineData(typeof(Producto), typeof(ProductoDto))]
        [InlineData(typeof(ProductoDto), typeof(Producto))]
        [InlineData(typeof(Producto), typeof(ProductoCreateDto))]
        [InlineData(typeof(Producto), typeof(ProductoUpdateDto))]
        [InlineData(typeof(UserDto), typeof(ApplicationUser))]
        [InlineData(typeof(ApplicationUser), typeof(UserDto))]
        public void Mapping_SourceToDestination_ExistsConfiguration(Type source, Type destination)
        {
            //Arrange


            //Act
            var instance = FormatterServices.GetUninitializedObject(source);

            //Assert
            _mapper.Map(instance, source, destination);
        }


    }
}
