using Questao5.Application.Commands.Requests;

namespace Questao5.Domain.Interfaces
{
    public interface IContaBancariaService
    {
        public Task<bool> ContaCorrenteExiste(int numero);
        public Task<bool> ContaCorrenteAtiva(int numero);
        public Task<decimal> CalcularSaldo(int numero);
        public Task<string> InsertMovimento(MovimentoRequest movimento);
        public Task<bool> ResquestMovimentoExiste(MovimentoRequest movimentoRequest);
        public Task<object> ObterRespostaMovimentoIdempotencia(MovimentoRequest movimentoRequest);
        public Task InsertIdempotenciaMovimento(MovimentoRequest movimentoRequest);
        public Task InsertIdempotenciaSaldoCorrente(SaldoContaCorrenteRequest saldoRequest, decimal saldo);
        public Task<bool> ResquestSaldoExiste(SaldoContaCorrenteRequest saldoContaCorrenteRequest);
        public Task<object> ObterRespostaSaldoIdempotencia(SaldoContaCorrenteRequest saldoContaCorrenteRequest);
    }
}
