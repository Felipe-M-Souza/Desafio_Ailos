using ContaCorrente.Domain.Entities;
using System;
using Xunit;

namespace ContaCorrente.UnitTests.Entities
{
    public class MovimentoTests
    {
        [Fact]
        public void CriarMovimentoCredito_DeveCriarMovimentoCorreto()
        {
            // Arrange
            var idConta = Guid.NewGuid().ToString();
            var data = DateTime.Now;
            var tipo = 'C';
            var valor = 100.50m;

            // Act
            var movimento = new Movimento(idConta, data, tipo, valor);

            // Assert
            Assert.Equal(idConta, movimento.IdContaCorrente);
            Assert.Equal(data, movimento.DataMovimento);
            Assert.Equal(tipo, movimento.TipoMovimento);
            Assert.Equal(valor, movimento.Valor);
            Assert.True(movimento.IsCredito);
            Assert.False(movimento.IsDebito);
            Assert.Equal(valor, movimento.ValorComSinal);
        }

        [Fact]
        public void CriarMovimentoDebito_DeveCriarMovimentoCorreto()
        {
            // Arrange
            var idConta = Guid.NewGuid().ToString();
            var data = DateTime.Now;
            var tipo = 'D';
            var valor = 50.25m;

            // Act
            var movimento = new Movimento(idConta, data, tipo, valor);

            // Assert
            Assert.Equal(idConta, movimento.IdContaCorrente);
            Assert.Equal(data, movimento.DataMovimento);
            Assert.Equal(tipo, movimento.TipoMovimento);
            Assert.Equal(valor, movimento.Valor);
            Assert.False(movimento.IsCredito);
            Assert.True(movimento.IsDebito);
            Assert.Equal(-valor, movimento.ValorComSinal);
        }

        [Theory]
        [InlineData('C', true)]
        [InlineData('D', true)]
        [InlineData('X', false)]
        [InlineData('c', false)]
        [InlineData('d', false)]
        public void IsTipoValido_DeveValidarTiposCorretamente(char tipo, bool esperado)
        {
            // Act
            var resultado = Movimento.IsTipoValido(tipo);

            // Assert
            Assert.Equal(esperado, resultado);
        }
    }
}


