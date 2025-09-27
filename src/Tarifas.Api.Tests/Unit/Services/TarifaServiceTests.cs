using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Tarifas.Application.Services;
using Tarifas.Domain.Entities;
using Tarifas.Infrastructure.Messaging;
using Tarifas.Infrastructure.Repositories;
using Xunit;

namespace Tarifas.Api.Tests.Unit.Services;

public class TarifaServiceTests
{
    private readonly Mock<ITarifaRepository> _tarifaRepositoryMock;
    private readonly Mock<ITarifaCobradaRepository> _tarifaCobradaRepositoryMock;
    private readonly Mock<IKafkaMessageProducer> _kafkaProducerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<TarifaService>> _loggerMock;
    private readonly TarifaService _service;
    private readonly Fixture _fixture;

    public TarifaServiceTests()
    {
        _tarifaRepositoryMock = new Mock<ITarifaRepository>();
        _tarifaCobradaRepositoryMock = new Mock<ITarifaCobradaRepository>();
        _kafkaProducerMock = new Mock<IKafkaMessageProducer>();
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<TarifaService>>();
        _service = new TarifaService(
            _tarifaRepositoryMock.Object,
            _tarifaCobradaRepositoryMock.Object,
            _kafkaProducerMock.Object,
            _configurationMock.Object,
            _loggerMock.Object);
        _fixture = new Fixture();
    }

    [Theory, AutoData]
    public async Task ProcessarTarifaAsync_ComTransferenciaValida_DeveProcessarComSucesso(
        Guid idTransferencia, Guid idContaOrigem, int numeroContaOrigem, 
        Guid idContaDestino, int numeroContaDestino, decimal valor, string descricao)
    {
        // Arrange
        var evento = new TransferenciaRealizadaEvent
        {
            IdTransferencia = idTransferencia,
            IdContaOrigem = idContaOrigem,
            NumeroContaOrigem = numeroContaOrigem,
            IdContaDestino = idContaDestino,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            DataTransferencia = DateTime.Now
        };

        var tarifa = _fixture.Create<Tarifa>();
        tarifa.TipoOperacao = "TRANSFERENCIA";
        tarifa.Valor = 2.50m;

        var tarifaCobrada = _fixture.Create<TarifaCobrada>();
        tarifaCobrada.IdConta = idContaOrigem;
        tarifaCobrada.NumeroConta = numeroContaOrigem;
        tarifaCobrada.TipoOperacao = "TRANSFERENCIA";
        tarifaCobrada.Valor = 2.50m;

        _tarifaRepositoryMock.Setup(x => x.ObterPorTipoOperacaoAsync("TRANSFERENCIA"))
            .ReturnsAsync(tarifa);
        _tarifaCobradaRepositoryMock.Setup(x => x.CriarAsync(It.IsAny<TarifaCobrada>()))
            .ReturnsAsync(tarifaCobrada);
        _configurationMock.Setup(x => x.GetValue<decimal>("Tarifas:ValorTransferencia", 2.50m))
            .Returns(2.50m);

        // Act
        var result = await _service.ProcessarTarifaAsync(evento);

        // Assert
        result.Should().NotBeNull();
        result.IdConta.Should().Be(idContaOrigem);
        result.NumeroConta.Should().Be(numeroContaOrigem);
        result.TipoOperacao.Should().Be("TRANSFERENCIA");
        result.Valor.Should().Be(2.50m);

        _tarifaCobradaRepositoryMock.Verify(x => x.CriarAsync(It.Is<TarifaCobrada>(tc => 
            tc.IdConta == idContaOrigem && 
            tc.NumeroConta == numeroContaOrigem && 
            tc.TipoOperacao == "TRANSFERENCIA" && 
            tc.Valor == 2.50m)), Times.Once);
        _kafkaProducerMock.Verify(x => x.PublishAsync("tarifas.cobradas", It.IsAny<object>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task ProcessarTarifaAsync_ComTarifaNaoExistente_DeveCriarTarifaPadrao(
        Guid idTransferencia, Guid idContaOrigem, int numeroContaOrigem, 
        Guid idContaDestino, int numeroContaDestino, decimal valor, string descricao)
    {
        // Arrange
        var evento = new TransferenciaRealizadaEvent
        {
            IdTransferencia = idTransferencia,
            IdContaOrigem = idContaOrigem,
            NumeroContaOrigem = numeroContaOrigem,
            IdContaDestino = idContaDestino,
            NumeroContaDestino = numeroContaDestino,
            Valor = valor,
            Descricao = descricao,
            DataTransferencia = DateTime.Now
        };

        var tarifaCobrada = _fixture.Create<TarifaCobrada>();
        tarifaCobrada.IdConta = idContaOrigem;
        tarifaCobrada.NumeroConta = numeroContaOrigem;

        _tarifaRepositoryMock.Setup(x => x.ObterPorTipoOperacaoAsync("TRANSFERENCIA"))
            .ReturnsAsync((Tarifa?)null);
        _tarifaRepositoryMock.Setup(x => x.CriarAsync(It.IsAny<Tarifa>()))
            .ReturnsAsync(_fixture.Create<Tarifa>());
        _tarifaCobradaRepositoryMock.Setup(x => x.CriarAsync(It.IsAny<TarifaCobrada>()))
            .ReturnsAsync(tarifaCobrada);
        _configurationMock.Setup(x => x.GetValue<decimal>("Tarifas:ValorTransferencia", 2.50m))
            .Returns(2.50m);

        // Act
        var result = await _service.ProcessarTarifaAsync(evento);

        // Assert
        result.Should().NotBeNull();
        result.Valor.Should().Be(2.50m);

        _tarifaRepositoryMock.Verify(x => x.CriarAsync(It.Is<Tarifa>(t => 
            t.TipoOperacao == "TRANSFERENCIA" && 
            t.Valor == 2.50m)), Times.Once);
    }

    [Theory, AutoData]
    public async Task ObterTarifasCobradasAsync_ComContaValida_DeveRetornarTarifas(
        Guid idConta)
    {
        // Arrange
        var tarifasEsperadas = _fixture.CreateMany<TarifaCobrada>(3).ToList();
        tarifasEsperadas.ForEach(t => t.IdConta = idConta);

        _tarifaCobradaRepositoryMock.Setup(x => x.ObterPorContaAsync(idConta))
            .ReturnsAsync(tarifasEsperadas);

        // Act
        var result = await _service.ObterTarifasCobradasAsync(idConta);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(t => t.IdConta.Should().Be(idConta));

        _tarifaCobradaRepositoryMock.Verify(x => x.ObterPorContaAsync(idConta), Times.Once);
    }

    [Theory, AutoData]
    public async Task ObterValorTarifaAsync_ComTipoOperacaoValido_DeveRetornarValor(
        string tipoOperacao)
    {
        // Arrange
        var tarifa = _fixture.Create<Tarifa>();
        tarifa.TipoOperacao = tipoOperacao;
        tarifa.Valor = 5.00m;

        _tarifaRepositoryMock.Setup(x => x.ObterPorTipoOperacaoAsync(tipoOperacao))
            .ReturnsAsync(tarifa);

        // Act
        var result = await _service.ObterValorTarifaAsync(tipoOperacao);

        // Assert
        result.Should().Be(5.00m);
        _tarifaRepositoryMock.Verify(x => x.ObterPorTipoOperacaoAsync(tipoOperacao), Times.Once);
    }

    [Theory, AutoData]
    public async Task ObterValorTarifaAsync_ComTipoOperacaoNaoExistente_DeveRetornarValorPadrao(
        string tipoOperacao)
    {
        // Arrange
        _tarifaRepositoryMock.Setup(x => x.ObterPorTipoOperacaoAsync(tipoOperacao))
            .ReturnsAsync((Tarifa?)null);
        _configurationMock.Setup(x => x.GetValue<decimal>("Tarifas:ValorTransferencia", 2.50m))
            .Returns(2.50m);

        // Act
        var result = await _service.ObterValorTarifaAsync(tipoOperacao);

        // Assert
        result.Should().Be(2.50m);
    }
}
