using Questao5.Application.Commands.Requests;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Services
{
    public class ContaBancariaService : IContaBancariaService
    {
        private readonly IContaBancariaRepository _contaBancariaRepository;

        public ContaBancariaService(IContaBancariaRepository contaBancariaRepository)
        {
            _contaBancariaRepository = contaBancariaRepository;
        }

        public async Task<bool> ContaCorrenteExiste(int numero)
        {
            return await _contaBancariaRepository.ContaCorrenteExiste(numero);
        }

        public async Task<bool> ContaCorrenteAtiva(int numero)
        {
            return await _contaBancariaRepository.ContaCorrenteAtiva(numero);
        }

        public async Task<decimal> CalcularSaldo(int numero)
        {
            return await _contaBancariaRepository.CalcularSaldo(numero);
        }

        public async Task<string> InsertMovimento(MovimentoRequest movimento)
        {
            bool salvou = Convert.ToBoolean(await _contaBancariaRepository.InsertMovimento(movimento));

            if (salvou)
            {
                return $"ID da Movimentação: { await _contaBancariaRepository.ResgatarIdMovimentacao(movimento) }";
            }

            return "Erro ao fazer a movimentação da Conta Corrente. Tente novamente.";
        }

        public async Task<bool> ResquestMovimentoExiste(MovimentoRequest movimentoRequest)
        {
            return await _contaBancariaRepository.RequestMovimentoExiste(movimentoRequest);
        }

        public async Task<object> ObterRespostaMovimentoIdempotencia(MovimentoRequest movimentoRequest)
        {
            return await _contaBancariaRepository.ObterRespostaMovimentoIdempotencia(movimentoRequest);
        }

        public async Task InsertIdempotenciaMovimento(MovimentoRequest movimentoRequest)
        {
            await _contaBancariaRepository.InsertIdempotenciaMovimento(movimentoRequest);
        }

        public async Task<bool> ResquestSaldoExiste(SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            return await _contaBancariaRepository.RequestSaldoExiste(saldoContaCorrenteRequest);
        }

        public async Task<object> ObterRespostaSaldoIdempotencia(SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            return await _contaBancariaRepository.ObterRespostaSaldoIdempotencia(saldoContaCorrenteRequest);
        }

        public async Task InsertIdempotenciaSaldoCorrente(SaldoContaCorrenteRequest saldoRequest, decimal saldo)
        {
            await _contaBancariaRepository.InsertIdempotenciaSaldoCorrente(saldoRequest, saldo);
        }
    }
}
