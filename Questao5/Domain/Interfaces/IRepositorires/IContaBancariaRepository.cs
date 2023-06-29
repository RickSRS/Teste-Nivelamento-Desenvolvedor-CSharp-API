using Questao5.Application.Commands.Requests;

namespace Questao5.Domain.Interfaces
{
    public interface IContaBancariaRepository
    {
        public Task<bool> ContaCorrenteExiste(int numero);
        public Task<bool> ContaCorrenteAtiva(int numero);
        public Task<decimal> CalcularSaldo(int numero);
        public Task<int> InsertMovimento(MovimentoRequest movimento);
        public Task<string> ResgatarIdMovimentacao(MovimentoRequest movimento);
        public Task<bool> RequestMovimentoExiste(MovimentoRequest movimentoRequest);
        public Task<object> ObterRespostaMovimentoIdempotencia(MovimentoRequest movimentoRequest);
        public Task InsertIdempotenciaMovimento(MovimentoRequest movimentoRequest);
        public Task InsertIdempotenciaSaldoCorrente(SaldoContaCorrenteRequest saldoRequest, decimal saldo);
        public Task<object> ObterRespostaSaldoIdempotencia(SaldoContaCorrenteRequest saldoContaCorrenteRequest);
        Task<bool> RequestSaldoExiste(SaldoContaCorrenteRequest saldoContaCorrenteRequest);
    }
}
