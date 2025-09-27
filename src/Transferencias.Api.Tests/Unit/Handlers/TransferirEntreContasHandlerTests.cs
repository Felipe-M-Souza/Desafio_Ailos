using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Transferencias.Application.Handlers;
using Transferencias.Application.Services;
using Transferencias.Infrastructure.Messaging;
using Xunit;

namespace Transferencias.Api.Tests.Unit.Handlers;

public class TransferirEntreContasHandlerTests
{
    private readonly Mock<ITransferenciaService> _transferenciaServiceMock;
    private readonly Mock<IContaCorrenteService> _contaCorrenteServiceMock;
    private readonly Mock<IKafkaMessageProducer> _kafkaProducerMock;
    private readonly TransferirEntreContasHandler _handler;
    private readonly Fixture _fixture;

    public TransferirEntreContasHandlerTests()
    {
        _transferenciaServiceMock = new Mock<ITransferenciaService>();
        _contaCorrenteServiceMock = new Mock<IContaCorrenteService>();
        _kafkaProducerMock = new Mock<IKafkaMessageProducer>();
        _handler = new TransferirEntreContasHandler(
            _transferenciaServiceMock.Object,
            _contaCorrenteServiceMock.Object,
            _kafkaProducerMock.Object);
        _fixture = new Fixture();
    }

    [Theory, AutoData]
    public async Task Handle_ComDadosValidos_DeveProcessarTransferenciaComSucesso(
        int numeroContaOrigem, int numeroContaDestino, decimal valor, string descricao, string token)
    {
        // Arrange
        var command = new TransferirEntreContasCommand
        {
            NumeroContaOrigem = numeroContaOrigem,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            Token = token
        };

        var contaOrigem = _fixture.Create<ContaCorrenteResponse>();
        var contaDestino = _fixture.Create<ContaCorrenteResponse>();
        var debitoResult = _fixture.Create<MovimentoResponse>();
        var creditoResult = _fixture.Create<MovimentoResponse>();
        var transferencia = _fixture.Create<Transferencia>();

        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaOrigem))
            .ReturnsAsync(contaOrigem);
        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaDestino))
            .ReturnsAsync(contaDestino);
        _contaCorrenteServiceMock.Setup(x => x.RealizarDebito(numeroContaOrigem, valor, descricao, token))
            .ReturnsAsync(debitoResult);
        _contaCorrenteServiceMock.Setup(x => x.RealizarCredito(numeroContaDestino, valor, descricao, token))
            .ReturnsAsync(creditoResult);
        _transferenciaServiceMock.Setup(x => x.CriarTransferencia(
            numeroContaOrigem, numeroContaDestino, valor, descricao, 
            debitoResult.IdMovimento!.Value, creditoResult.IdMovimento!.Value))
            .ReturnsAsync(transferencia);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeTrue();
        result.IdTransferencia.Should().Be(transferencia.Id);

        _contaCorrenteServiceMock.Verify(x => x.RealizarDebito(numeroContaOrigem, valor, descricao, token), Times.Once);
        _contaCorrenteServiceMock.Verify(x => x.RealizarCredito(numeroContaDestino, valor, descricao, token), Times.Once);
        _transferenciaServiceMock.Verify(x => x.CriarTransferencia(
            numeroContaOrigem, numeroContaDestino, valor, descricao,
            debitoResult.IdMovimento!.Value, creditoResult.IdMovimento!.Value), Times.Once);
        _kafkaProducerMock.Verify(x => x.PublishAsync("transferencias.efetuadas", It.IsAny<object>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task Handle_ComContaOrigemNaoEncontrada_DeveRetornarErro(
        int numeroContaOrigem, int numeroContaDestino, decimal valor, string descricao, string token)
    {
        // Arrange
        var command = new TransferirEntreContasCommand
        {
            NumeroContaOrigem = numeroContaOrigem,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            Token = token
        };

        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaOrigem))
            .ReturnsAsync((ContaCorrenteResponse?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Conta de origem não encontrada");
        result.TipoErro.Should().Be("INVALID_ACCOUNT");
    }

    [Theory, AutoData]
    public async Task Handle_ComContaDestinoNaoEncontrada_DeveRetornarErro(
        int numeroContaOrigem, int numeroContaDestino, decimal valor, string descricao, string token)
    {
        // Arrange
        var command = new TransferirEntreContasCommand
        {
            NumeroContaOrigem = numeroContaOrigem,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            Token = token
        };

        var contaOrigem = _fixture.Create<ContaCorrenteResponse>();

        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaOrigem))
            .ReturnsAsync(contaOrigem);
        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaDestino))
            .ReturnsAsync((ContaCorrenteResponse?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Conta de destino não encontrada");
        result.TipoErro.Should().Be("INVALID_ACCOUNT");
    }

    [Theory, AutoData]
    public async Task Handle_ComMesmaConta_DeveRetornarErro(
        int numeroConta, decimal valor, string descricao, string token)
    {
        // Arrange
        var command = new TransferirEntreContasCommand
        {
            NumeroContaOrigem = numeroConta,
            NumeroContaDestino = numeroConta, // Mesma conta
            Valor = valor,
            Descricao = descricao,
            Token = token
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Contain("Não é possível transferir para a mesma conta");
        result.TipoErro.Should().Be("INVALID_ACCOUNT");
    }

    [Theory, AutoData]
    public async Task Handle_ComFalhaNoDebito_DeveRetornarErro(
        int numeroContaOrigem, int numeroContaDestino, decimal valor, string descricao, string token)
    {
        // Arrange
        var command = new TransferirEntreContasCommand
        {
            NumeroContaOrigem = numeroContaOrigem,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            Token = token
        };

        var contaOrigem = _fixture.Create<ContaCorrenteResponse>();
        var contaDestino = _fixture.Create<ContaCorrenteResponse>();
        var debitoResult = _fixture.Create<MovimentoResponse>();
        debitoResult.Sucesso = false;
        debitoResult.Erro = "Saldo insuficiente";

        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaOrigem))
            .ReturnsAsync(contaOrigem);
        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaDestino))
            .ReturnsAsync(contaDestino);
        _contaCorrenteServiceMock.Setup(x => x.RealizarDebito(numeroContaOrigem, valor, descricao, token))
            .ReturnsAsync(debitoResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Be("Saldo insuficiente");
    }

    [Theory, AutoData]
    public async Task Handle_ComFalhaNoCredito_DeveFazerEstorno(
        int numeroContaOrigem, int numeroContaDestino, decimal valor, string descricao, string token)
    {
        // Arrange
        var command = new TransferirEntreContasCommand
        {
            NumeroContaOrigem = numeroContaOrigem,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            Token = token
        };

        var contaOrigem = _fixture.Create<ContaCorrenteResponse>();
        var contaDestino = _fixture.Create<ContaCorrenteResponse>();
        var debitoResult = _fixture.Create<MovimentoResponse>();
        debitoResult.Sucesso = true;
        var creditoResult = _fixture.Create<MovimentoResponse>();
        creditoResult.Sucesso = false;
        creditoResult.Erro = "Erro no crédito";

        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaOrigem))
            .ReturnsAsync(contaOrigem);
        _contaCorrenteServiceMock.Setup(x => x.ObterContaPorNumero(numeroContaDestino))
            .ReturnsAsync(contaDestino);
        _contaCorrenteServiceMock.Setup(x => x.RealizarDebito(numeroContaOrigem, valor, descricao, token))
            .ReturnsAsync(debitoResult);
        _contaCorrenteServiceMock.Setup(x => x.RealizarCredito(numeroContaDestino, valor, descricao, token))
            .ReturnsAsync(creditoResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Sucesso.Should().BeFalse();
        result.Erro.Should().Be("Erro no crédito");

        // Verificar se foi feito estorno
        _contaCorrenteServiceMock.Verify(x => x.RealizarCredito(
            numeroContaOrigem, valor, "Estorno - Falha na transferência", token), Times.Once);
    }
}
