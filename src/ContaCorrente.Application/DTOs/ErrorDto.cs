namespace ContaCorrente.Application.DTOs
{
    /// <summary>
    /// Resposta de erro da API
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Mensagem de erro descritiva
        /// </summary>
        /// <example>Número de conta já existe</example>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Código de erro interno para identificação
        /// </summary>
        /// <example>ACCOUNT_NUMBER_EXISTS</example>
        public string Code { get; set; } = string.Empty;
    }
}
