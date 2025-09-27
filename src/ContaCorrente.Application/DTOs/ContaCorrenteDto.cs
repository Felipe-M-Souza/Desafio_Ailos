using System.ComponentModel.DataAnnotations;

namespace ContaCorrente.Application.DTOs
{
    /// <summary>
    /// Dados para criação de uma nova conta corrente
    /// </summary>
    public record CriarContaRequest
    {
        /// <summary>
        /// Número da conta (deve ser único)
        /// </summary>
        /// <example>12345</example>
        [Required(ErrorMessage = "O número da conta é obrigatório")]
        [Range(1000, 99999, ErrorMessage = "O número da conta deve estar entre 1000 e 99999")]
        public int Numero { get; init; }

        /// <summary>
        /// Nome completo do titular da conta
        /// </summary>
        /// <example>João Silva Santos</example>
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string Nome { get; init; } = string.Empty;

        /// <summary>
        /// CPF do titular (apenas números)
        /// </summary>
        /// <example>12345678901</example>
        [Required(ErrorMessage = "O CPF é obrigatório")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter exatamente 11 dígitos")]
        public string Cpf { get; init; } = string.Empty;

        /// <summary>
        /// Senha da conta (mínimo 6 caracteres)
        /// </summary>
        /// <example>minhasenha123</example>
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres")]
        public string Senha { get; init; } = string.Empty;
    }
    
    /// <summary>
    /// Dados de resposta da conta criada
    /// </summary>
    /// <param name="Id">Identificador único da conta</param>
    /// <param name="Numero">Número da conta</param>
    /// <param name="Nome">Nome do titular</param>
    /// <param name="Ativo">Indica se a conta está ativa</param>
    public record ContaResponse(string Id, int Numero, string Nome, bool Ativo);
    
    /// <summary>
    /// Dados para ativação/desativação de conta
    /// </summary>
    public record AtivarContaRequest
    {
        /// <summary>
        /// Status da conta (true = ativa, false = inativa)
        /// </summary>
        /// <example>true</example>
        [Required(ErrorMessage = "O status da conta é obrigatório")]
        public bool Ativo { get; init; }
    }
    
    /// <summary>
    /// Dados para inativação de conta
    /// </summary>
    public record InativarContaRequest
    {
        /// <summary>
        /// Senha atual da conta para confirmação
        /// </summary>
        /// <example>minhasenha123</example>
        [Required(ErrorMessage = "A senha é obrigatória para inativação")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres")]
        public string Senha { get; init; } = string.Empty;
    }
    
    /// <summary>
    /// Dados para login na conta
    /// </summary>
    public record LoginRequest
    {
        /// <summary>
        /// Número da conta (opcional se CPF for informado)
        /// </summary>
        /// <example>12345</example>
        public int? Numero { get; init; }

        /// <summary>
        /// CPF da conta (opcional se número for informado)
        /// </summary>
        /// <example>12345678901</example>
        public string? Cpf { get; init; }

        /// <summary>
        /// Senha da conta
        /// </summary>
        /// <example>minhasenha123</example>
        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres")]
        public string Senha { get; init; } = string.Empty;
    }
    
    /// <summary>
    /// Resposta do login com token de autenticação
    /// </summary>
    /// <param name="Token">Token JWT para autenticação nas próximas requisições</param>
    /// <param name="Id">Identificador único da conta</param>
    public record LoginResponse(string Token, string Id);
}