using ContaCorrente.Application.Commands;
using ContaCorrente.Application.Handlers;
using ContaCorrente.Domain.Entities;
using ContaCorrente.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace ContaCorrente.UnitTests.Handlers;

public class SimpleHandlerTests
{
    [Fact]
    public void CriarContaCommand_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var command = new CriarContaCommand(12345, "João Silva", "12345678901", "123456");

        // Assert
        command.Numero.Should().Be(12345);
        command.Nome.Should().Be("João Silva");
        command.Cpf.Should().Be("12345678901");
        command.Senha.Should().Be("123456");
    }

    [Fact]
    public void LancarMovimentoCommand_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var command = new LancarMovimentoCommand("conta-id", "27/09/2024", 'C', 100.00m);

        // Assert
        command.IdConta.Should().Be("conta-id");
        command.Data.Should().Be("27/09/2024");
        command.Tipo.Should().Be('C');
        command.Valor.Should().Be(100.00m);
    }

    [Fact]
    public void Conta_ShouldCreateWithCorrectProperties()
    {
        // Arrange & Act
        var conta = new Conta
        {
            IdContaCorrente = "test-id",
            Numero = 12345,
            Nome = "João Silva",
            Cpf = "12345678901",
            Ativo = true
        };

        // Assert
        conta.IdContaCorrente.Should().Be("test-id");
        conta.Numero.Should().Be(12345);
        conta.Nome.Should().Be("João Silva");
        conta.Cpf.Should().Be("12345678901");
        conta.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Movimento_ShouldCreateWithCorrectProperties()
    {
        // Arrange & Act
        var movimento = new Movimento("conta-id", DateTime.Today, 'C', 100.00m);

        // Assert
        movimento.IdContaCorrente.Should().Be("conta-id");
        movimento.DataMovimento.Should().Be(DateTime.Today);
        movimento.TipoMovimento.Should().Be('C');
        movimento.Valor.Should().Be(100.00m);
        movimento.IsCredito.Should().BeTrue();
        movimento.IsDebito.Should().BeFalse();
    }

    [Fact]
    public void Movimento_Debit_ShouldHaveCorrectProperties()
    {
        // Arrange & Act
        var movimento = new Movimento("conta-id", DateTime.Today, 'D', 50.00m);

        // Assert
        movimento.TipoMovimento.Should().Be('D');
        movimento.Valor.Should().Be(50.00m);
        movimento.IsCredito.Should().BeFalse();
        movimento.IsDebito.Should().BeTrue();
    }
}
