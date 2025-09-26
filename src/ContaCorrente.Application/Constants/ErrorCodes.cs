namespace ContaCorrente.Application.Constants
{
    public static class ErrorCodes
    {
        // Documentos
        public const string INVALID_DOCUMENT = "INVALID_DOCUMENT";
        
        // Autenticação
        public const string USER_UNAUTHORIZED = "USER_UNAUTHORIZED";
        
        // Contas
        public const string INVALID_ACCOUNT = "INVALID_ACCOUNT";
        public const string INACTIVE_ACCOUNT = "INACTIVE_ACCOUNT";
        public const string CONTA_NAO_ENCONTRADA = "CONTA_NAO_ENCONTRADA";
        public const string CONTA_JA_INATIVA = "CONTA_JA_INATIVA";
        public const string CONTA_EXISTENTE = "CONTA_EXISTENTE";
        
        // Valores e tipos
        public const string INVALID_VALUE = "INVALID_VALUE";
        public const string INVALID_TYPE = "INVALID_TYPE";
        
        // Dados gerais
        public const string DADOS_INVALIDOS = "DADOS_INVALIDOS";
        public const string OPERACAO_INVALIDA = "OPERACAO_INVALIDA";
        
        // Saldo
        public const string SALDO_INSUFICIENTE = "SALDO_INSUFICIENTE";
    }
}
