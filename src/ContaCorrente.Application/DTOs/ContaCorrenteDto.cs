namespace ContaCorrente.Application.DTOs
{
    public record CriarContaRequest(int Numero, string Nome, string Cpf, string Senha);
    
    public record ContaResponse(string Id, int Numero, string Nome, bool Ativo);
    
    public record AtivarContaRequest(bool Ativo);
    
    public record InativarContaRequest(string Senha);
    
    public record LoginRequest(int? Numero, string? Cpf, string Senha);
    
    public record LoginResponse(string Token, string Id);
}
