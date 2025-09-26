namespace ContaCorrente.Application.Constants
{
    public static class ErrorMessages
    {
        // Documentos
        public const string INVALID_DOCUMENT = "CPF inválido";
        
        // Autenticação
        public const string USER_UNAUTHORIZED = "Credenciais inválidas";
        
        // Contas
        public const string INVALID_ACCOUNT = "Conta não encontrada";
        public const string INACTIVE_ACCOUNT = "Conta inativa";
        public const string CONTA_NAO_ENCONTRADA = "Conta não encontrada";
        public const string CONTA_JA_INATIVA = "Conta já está inativa";
        public const string CONTA_EXISTENTE = "Já existe uma conta com este número";
        
        // Valores e tipos
        public const string INVALID_VALUE = "Valor deve ser maior que zero";
        public const string INVALID_TYPE = "Tipo deve ser 'C' (Crédito) ou 'D' (Débito)";
        
        // Dados gerais
        public const string DADOS_INVALIDOS = "Dados inválidos";
        public const string OPERACAO_INVALIDA = "Operação inválida";
        
        // Saldo
        public const string SALDO_INSUFICIENTE = "Saldo insuficiente";
    }
}
