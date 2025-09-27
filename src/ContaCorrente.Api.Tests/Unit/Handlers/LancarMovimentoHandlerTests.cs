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

public class LancarMovimentoHandlerTests
{
    private readonly Mock<IContaCorrenteRepository> _contaRepositoryMock;
    private readonly Mock<IMovimentoRepository> _movimentoRepositoryMock;
    private readonly Mock<ContaCorrente.Domain.Interfaces.ITarifaService> _tarifaServiceMock;
    private readonly Mock<ContaCorrente.Infrastructure.Services.IEventPublisher> _eventPublisherMock;
    private readonly LancarMovimentoHandler _handler;
    private readonly Fixture _fixture;

    public LancarMovimentoHandlerTests()
    {
        _contaRepositoryMock = new Mock<IContaCorrenteRepository>();
        _movimentoRepositoryMock = new Mock<IMovimentoRepository>();
        _tarifaServiceMock = new Mock<ContaCorrente.Domain.Interfaces.ITarifaService>();
        _eventPublisherMock = new Mock<ContaCorrente.Infrastructure.Services.IEventPublisher>();
        _handler = new LancarMovimentoHandler(
            _contaRepositoryMock.Object, 
            _movimentoRepositoryMock.Object, 
            _tarifaServiceMock.Object,
            _eventPublisherMock.Object);
        _fixture = new Fixture();
    }

    [Theory, AutoData]
    public async Task Handle_ComCreditoValido_DeveProcessarComSucesso(
        string contaId, decimal valor, string descricao)
    {
        // Arrange
        var command = new LancarMovimentoCommand(contaId, valor, "C", descricao);
        var conta = _fixture.Create<Conta>();
        conta.Id = Guid.Parse(contaId);
        conta.Ativo = true;

        var movimentoEsperado = _fixture.Create<Movimento>();
        movimentoEsperado.IdConta = conta.Id;
        movimentoEsperado.Valor = valor;
        movimentoEsperado.Tipo = "C";

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(Guid.Parse(contaId)))
            .ReturnsAsync(conta);
        _movimentoRepositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimento>()))
            .ReturnsAsync(movimentoEsperado);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
        result.IdMovimento.Should().Be(movimentoEsperado.Id);

        _movimentoRepositoryMock.Verify(x => x.CriarAsync(It.Is<Movimento>(m => 
            m.IdConta == conta.Id && 
            m.Valor == valor && 
            m.Tipo == "C")), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_ComDebitoValido_DeveProcessarComSucesso(
        string contaId, decimal valor, string descricao)
    {
        // Arrange
        var command = new LancarMovimentoCommand(contaId, valor, "D", descricao);
        var conta = _fixture.Create<Conta>();
        conta.Id = Guid.Parse(contaId);
        conta.Ativo = true;

        var movimentoEsperado = _fixture.Create<Movimento>();
        movimentoEsperado.IdConta = conta.Id;
        movimentoEsperado.Valor = valor;
        movimentoEsperado.Tipo = "D";

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(Guid.Parse(contaId)))
            .ReturnsAsync(conta);
        _movimentoRepositoryMock.Setup(x => x.CriarAsync(It.IsAny<Movimento>()))
            .ReturnsAsync(movimentoEsperado);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
        result.IdMovimento.Should().Be(movimentoEsperado.Id);
    }

    [Theory, AutoData]
    public async Task Handle_ComContaNaoEncontrada_DeveRetornarErro(
        string contaId, decimal valor, string descricao)
    {
        // Arrange
        var command = new LancarMovimentoCommand(contaId, valor, "C", descricao);

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(Guid.Parse(contaId)))
            .ReturnsAsync((Conta?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Conta não encontrada");
    }

    [Theory, AutoData]
    public async Task Handle_ComContaInativa_DeveRetornarErro(
        string contaId, decimal valor, string descricao)
    {
        // Arrange
        var command = new LancarMovimentoCommand(contaId, valor, "C", descricao);
        var conta = _fixture.Create<Conta>();
        conta.Id = Guid.Parse(contaId);
        conta.Ativo = false;

        _contaRepositoryMock.Setup(x => x.ObterPorIdAsync(Guid.Parse(contaId)))
            .ReturnsAsync(conta);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Conta inativa");
    }

    [Theory, AutoData]
    public async Task Handle_ComValorNegativo_DeveRetornarErro(
        string contaId, decimal valorNegativo, string descricao)
    {
        // Arrange
        var command = new LancarMovimentoCommand(contaId, Math.Abs(valorNegativo) * -1, "C", descricao);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Valor deve ser positivo");
    }

    [Theory, AutoData]
    public async Task Handle_ComTipoInvalido_DeveRetornarErro(
        string contaId, decimal valor, string descricao, string tipoInvalido)
    {
        // Arrange
        var command = new LancarMovimentoCommand(contaId, valor, tipoInvalido, descricao);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Tipo de movimento inválido");
    }
}
