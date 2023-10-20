using FluentAssertions;
using NetArchTest.Rules;
using Domain = BlazorHero.CleanArchitecture.Domain;
using Application = BlazorHero.CleanArchitecture.Application;
using Infrastructure = BlazorHero.CleanArchitecture.Infrastructure;
using InfrastructureShared = BlazorHero.CleanArchitecture.Infrastructure.Shared;
using Shared = BlazorHero.CleanArchitecture.Shared;
using Server = BlazorHero.CleanArchitecture.Server;
using Client = BlazorHero.CleanArchitecture.Client;
using ClientInfrastructure = BlazorHero.CleanArchitecture.Client.Infrastructure;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private const string NamespacePrefix = "BlazorHero.CleanArchitecture";
    private const string DomainNamespace = $"{NamespacePrefix}.Domain";
    private const string ApplicationNamespace = $"{NamespacePrefix}.Application";
    private const string InfrastructureNamespace = $"{NamespacePrefix}.Infrastucture";
    private const string InfrastructureSharedNamespace = $"{NamespacePrefix}.Infrastucture.Shared";
    private const string ServerNamespace = $"{NamespacePrefix}.Server";
    private const string ClientNamespace = $"{NamespacePrefix}.Client";
    private const string ClientInfrastructureNamespace = $"{NamespacePrefix}.Client.Infrastructure";

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Domain.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            InfrastructureSharedNamespace,
            ServerNamespace,
            ClientNamespace,
            ClientInfrastructureNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Shared_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Shared.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            ApplicationNamespace,
            InfrastructureNamespace,
            InfrastructureSharedNamespace,
            ServerNamespace,
            ClientNamespace,
            ClientInfrastructureNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Application.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            InfrastructureSharedNamespace,
            ServerNamespace,
            ClientNamespace,
            ClientInfrastructureNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Infrastructure.AssemblyReference).Assembly;

        var otherProjects = new[] { ServerNamespace, ClientNamespace, ClientInfrastructureNamespace };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureShared_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(InfrastructureShared.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            InfrastructureNamespace,
            ServerNamespace,
            ClientNamespace,
            ClientInfrastructureNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Client_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(Client.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            DomainNamespace,
            ApplicationNamespace,
            InfrastructureNamespace,
            InfrastructureSharedNamespace,
            ServerNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ClientInfrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        var assembly = typeof(ClientInfrastructure.AssemblyReference).Assembly;

        var otherProjects = new[]
        {
            DomainNamespace,
            InfrastructureNamespace,
            InfrastructureSharedNamespace,
            ServerNamespace
        };

        // Act
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProjects)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_Have_DependencyOnDomainProject()
    {
        // Arrange
        var assembly = typeof(Application.AssemblyReference).Assembly;

        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOnMediatR()
    {
        // Arrange
        var assembly = typeof(Server.AssemblyReference).Assembly;
        const string controllersNamespace = $"{ServerNamespace}.Controllers";

        // TODO: Fix controllers that don't have dependency on MediatR
        // Act
        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .And()
            .DoNotResideInNamespaceStartingWith($"{controllersNamespace}.Utilities")
            .And()
            .DoNotResideInNamespaceStartingWith($"{controllersNamespace}.Identity")
            .And()
            .DoNotResideInNamespaceStartingWith($"{controllersNamespace}.Communication")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Utilities, Identity, Communication

        // Assert
        result.IsSuccessful.Should().BeTrue();
    }
}
