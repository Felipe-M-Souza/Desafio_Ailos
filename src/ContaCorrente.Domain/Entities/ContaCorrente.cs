using System;

namespace ContaCorrente.Domain.Entities
{
    public class Conta
    {
        public string IdContaCorrente { get; set; } = string.Empty;
        public int Numero { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public string Senha { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;

        public Conta()
        {
            IdContaCorrente = Guid.NewGuid().ToString();
        }

        public Conta(int numero, string nome, string cpf, string senha, string salt) : this()
        {
            Numero = numero;
            Nome = nome;
            Cpf = cpf;
            Senha = senha;
            Salt = salt;
            Ativo = false; // Conta criada inativa por padrão
        }

        public void Ativar()
        {
            Ativo = true;
        }

        public void Desativar()
        {
            Ativo = false;
        }

        public bool PodeRealizarOperacoes()
        {
            return Ativo;
        }
    }
}
