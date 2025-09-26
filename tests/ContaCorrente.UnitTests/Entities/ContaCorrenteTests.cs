using ContaCorrente.Domain.Entities;
using Xunit;

namespace ContaCorrente.UnitTests.Entities
{
    public class ContaCorrenteTests
    {
        [Fact]
        public void CriarConta_DeveCriarContaInativa()
        {
            // Arrange
            var numero = 123;
            var nome = "Felipe";
            var senha = "Senha123";
            var salt = "salt123";

            // Act
            var conta = new Conta(numero, nome, "12345678901", senha, salt);

            // Assert
            Assert.Equal(numero, conta.Numero);
            Assert.Equal(nome, conta.Nome);
            Assert.Equal(senha, conta.Senha);
            Assert.Equal(salt, conta.Salt);
            Assert.False(conta.Ativo);
            Assert.False(conta.PodeRealizarOperacoes());
        }

        [Fact]
        public void AtivarConta_DeveTornarContaAtiva()
        {
            // Arrange
            var conta = new Conta(123, "Felipe", "12345678901", "Senha123", "salt123");

            // Act
            conta.Ativar();

            // Assert
            Assert.True(conta.Ativo);
            Assert.True(conta.PodeRealizarOperacoes());
        }

        [Fact]
        public void DesativarConta_DeveTornarContaInativa()
        {
            // Arrange
            var conta = new Conta(123, "Felipe", "12345678901", "Senha123", "salt123");
            conta.Ativar();

            // Act
            conta.Desativar();

            // Assert
            Assert.False(conta.Ativo);
            Assert.False(conta.PodeRealizarOperacoes());
        }
    }
}
