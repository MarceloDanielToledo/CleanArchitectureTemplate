using Application.Abstractions.Messaging;
using Domain.Entities;
using FluentValidation;
using NetArchTest.Rules;
using Repository.Contexts;
using WebAPI.Controllers;

namespace Application.UnitTests;

public class ArchitectureTests
{
    private const string DomainNamespace = "Domain";
    private const string ApplicationNamespace = "Application";
    private const string InfrastructureNamespace = "Repository";
    private const string PresentationNamespace = "WebAPI";

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Application()
    {
        var result = Types.InAssembly(typeof(Order).Assembly)
            .ShouldNot()
            .HaveDependencyOn(ApplicationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(Order).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void Domain_Should_Not_Have_Dependency_On_Presentation()
    {
        var result = Types.InAssembly(typeof(Order).Assembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Infrastructure()
    {
        var result = Types.InAssembly(typeof(ICommandHandler<,>).Assembly)
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void Application_Should_Not_Have_Dependency_On_Presentation()
    {
        var result = Types.InAssembly(typeof(ICommandHandler<,>).Assembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void Infrastructure_Should_Not_Have_Dependency_On_Presentation()
    {
        var result = Types.InAssembly(typeof(ApplicationDbContext).Assembly)
            .ShouldNot()
            .HaveDependencyOn(PresentationNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void CommandHandlers_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(ICommandHandler<,>).Assembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void QueryHandlers_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(IQueryHandler<,>).Assembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    [Fact]
    public void Validators_Should_Be_Sealed()
    {
        var result = Types.InAssembly(typeof(ICommandHandler<,>).Assembly)
            .That()
            .Inherit(typeof(AbstractValidator<>))
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(result.IsSuccessful, GetFailureMessage(result));
    }

    private static string GetFailureMessage(TestResult result)
    {
        if (result.FailingTypes is null || result.FailingTypes.Count == 0)
            return "Architecture test failed with no details.";

        return $"Failing types: {string.Join(", ", result.FailingTypes.Select(t => t.FullName))}";
    }
}
