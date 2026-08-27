

using Application.Services;
using Domain.Models.Products;
using FluentAssertions;
using Infrastructure.Data;
using NetArchTest.Rules;
using StoreApi.Controllers;

namespace UnitTests.ArchitectureTests
{
    public class CleanArchitectureTests
    {
        private const string ApplicationNamespace = "Application";
        private const string InfrastructureNamespace = "Infrastructure";
        private const string StoreApiNamespace = "StoreApi";

        [Fact]
        public void Domain_ShouldNot_HaveDependencyOnOtherLayers()
        {
            var domainAssembly = typeof(Product).Assembly;


            var result = Types.InAssembly(domainAssembly)
                              .ShouldNot()
                              .HaveDependencyOn(ApplicationNamespace)
                              .And()
                              .HaveDependencyOn(InfrastructureNamespace)
                              .And()
                              .HaveDependencyOn(StoreApiNamespace)
                              .GetResult();


            result.IsSuccessful.Should().BeTrue(
                because: "Domain layer must remain isolated and independent of other layers."
            );
        }

        [Fact]
        public void Application_ShouldNotHaveDependencyOnInfrastructureOrStoreApi()
        {
            var applicationAssembly = typeof(ProductService).Assembly;


            var result = Types.InAssembly(applicationAssembly)
                              .ShouldNot()
                              .HaveDependencyOn(InfrastructureNamespace)
                              .And()
                              .HaveDependencyOn(StoreApiNamespace)
                              .GetResult();


            result.IsSuccessful.Should().BeTrue(
                because: "Application layer should only depend on Domain, not Infrastructure."
            );
        }

        [Fact]
        public void Infrastructure_ShouldNotHaveDependencyOnStoreApi()
        {
            var infrastructureAssembly = typeof(AppDbContext).Assembly;


            var result = Types.InAssembly(infrastructureAssembly)
                              .ShouldNot()
                              .HaveDependencyOn(StoreApiNamespace)
                              .GetResult();


            result.IsSuccessful.Should().BeTrue(
                because: "Infrastructure layer should only depend on Domain and Application."
            );
        }

        [Fact]
        public void Controllers_ShouldHaveNameEndingWithController()
        {
            var apiAssembly = typeof(ProductController).Assembly;


            var result = Types.InAssembly(apiAssembly)
                              .That()
                              .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase))
                              .And()
                              .AreNotAbstract()
                              .Should()
                              .HaveNameEndingWith("Controller")
                              .GetResult();


            result.IsSuccessful.Should().BeTrue(
                because: "All API controller classes must follow the standard 'XYZController' naming convention."
            );
        }
    }
}
