using FluentAssertions;
using StoreApi.DTOs;
using StoreApi.Mappers;
using StoreApi.Models.Identity;
using StoreApi.Models.Store;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.Tests.UnitTests
{
    public class ProductMappersTest
    {
        private RegisterDataDto CreateDefaultRegisterDataDto() => new RegisterDataDto(
                UserName: "SomeName",
                Email: "email@gmail.com",
                PhoneNumber: "+48100100100",
                Password: "KindaHashPassword"
                );
        private ApplicationUser CreateDefaultApplicationUser() => new ApplicationUser
        {
            UserName = "SomeName",
            Email = "email@gmail.com",
            PhoneNumber = "+48100100100",
            PasswordHash = "KindaHashPassword",
            Role = UserRoles.Customer
        };

        [Fact]
        public void ToEntity_MapAllProperties_CorrectlyMapping()
        {
            var registerDto = CreateDefaultRegisterDataDto();
            var userEntity = CreateDefaultApplicationUser();

            var resultDto = registerDto.ToEntity();

            Assert.NotNull(resultDto);
            resultDto.Should().BeEquivalentTo(userEntity, options => options
            .Excluding(usr => usr.PasswordHash)
            .Excluding(usr => usr.Id));
        }

        [Fact]
        public void ToReadDto_GivenTagsProperty_ShouldHaveListOfTagsName()
        {
            var product = new Product
            {
                Name = "",
                Tags = new List<Tag>
                {
                    new Tag{Name = "Electronics"},
                    new Tag{Name = "Accessories"}
                },
                CategoryId = Guid.CreateVersion7(),
                Category = null,
                Price = (decimal)10.0,
                ProductSeo = null
            };
          
            var resultDto = product.ToReadDto();

            Assert.NotNull(resultDto.TagNames);
            Assert.Contains("Electronics", resultDto.TagNames);
            Assert.Contains("Accessories", resultDto.TagNames);
            Assert.Equal(2, resultDto.TagNames.Count);
        }
    }
}
