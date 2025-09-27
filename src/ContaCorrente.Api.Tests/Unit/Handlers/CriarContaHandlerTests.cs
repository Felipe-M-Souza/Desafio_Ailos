using AutoFixture;
using AutoFixture.Xunit2;
using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Handlers;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContaCorrente.Api.Tests.Unit.Handlers;

public class CriarContaHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly CriarContaHandler _handler;
    private readonly Fixture _fixture;

    public CriarContaHandlerTests()
    {
        _contaRepositoryMock = new Mock<IContaCorrenteRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new CriarContaHandler(_contaRepositoryMock.Object, _passwordHasherMock.Object);
        _fixture = new Fixture();
    }

    [Theory, AutoData]
    public async Task Handle_ComDadosValidos_DeveCriarContaComSucesso(
        int numero, string nome, string cpf, string senha)
    {
        // Arrange
        var command = new CriarContaCommand(numero, nome, cpf, senha);
        var contaEsperada = _fixture.Create<Conta>();
        contaEsperada.Numero = numero;
        contaEsperada.Nome = nome;
        contaEsperada.Cpf = cpf;

        _contaRepositoryMock.Setup(x => x.ObterPorNumeroAsync(numero))
            .ReturnsAsync((Conta?)null);
        _contaRepositoryMock.Setup(x => x.ObterPorCpfAsync(cpf))
            .ReturnsAsync((Conta?)null);
        _passwordHasherMock.Setup(x => x.HashPassword(senha))
            .Returns("hashed_password");
        _contaRepositoryMock.Setup(x => x.CriarAsync(It.IsAny<Conta>()))
            .ReturnsAsync(contaEsperada);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Numero.Should().Be(numero);
        result.Nome.Should().Be(nome);
        result.Cpf.Should().Be(cpf);
        result.Ativo.Should().BeTrue();

        _contaRepositoryMock.Verify(x => x.CriarAsync(It.Is<Conta>(c => 
            c.Numero == numero && 
            c.Nome == nome && 
            c.Cpf == cpf)), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_ComNumeroJaExistente_DeveLancarExcecao(
        int numero, string nome, string cpf, string senha)
    {
        // Arrange
        var command = new CriarContaCommand(numero, nome, cpf, senha);
        var contaExistente = _fixture.Create<Conta>();

        _contaRepositoryMock.Setup(x => x.ObterPorNumeroAsync(numero))
            .ReturnsAsync(contaExistente);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Theory, AutoData]
    public async Task Handle_ComCpfJaExistente_DeveLancarExcecao(
        int numero, string nome, string cpf, string senha)
    {
        // Arrange
        var command = new CriarContaCommand(numero, nome, cpf, senha);
        var contaExistente = _fixture.Create<Conta>();

        _contaRepositoryMock.Setup(x => x.ObterPorNumeroAsync(numero))
            .ReturnsAsync((Conta?)null);
        _contaRepositoryMock.Setup(x => x.ObterPorCpfAsync(cpf))
            .ReturnsAsync(contaExistente);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }

    [Theory, AutoData]
    public async Task Handle_ComCpfInvalido_DeveLancarExcecao(
        int numero, string nome, string cpfInvalido, string senha)
    {
        // Arrange
        var command = new CriarContaCommand(numero, nome, cpfInvalido, senha);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _handler.Handle(command, CancellationToken.None));
    }
}
