using Microsoft.AspNetCore.Mvc;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Interfaces;

namespace Questao5.Infrastructure.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaBancariaController : ControllerBase
    {
        private readonly IContaBancariaService _contaBancariaService;

        public ContaBancariaController(IContaBancariaService contaBancariaService)
        {
            _contaBancariaService = contaBancariaService;
        }

        [HttpGet("SaldoContaCorrente/Extrato")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> SaldoContaCorrente([FromQuery]SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            // Validar dados de entrada
            if (!await ContaCorrenteExiste(saldoContaCorrenteRequest.Numero))
            {
                return BadRequest("Conta corrente inválida. TIPO: INVALID_ACCOUNT.");
            }

            if (!await ContaCorrenteAtiva(saldoContaCorrenteRequest.Numero))
            {
                return BadRequest("Conta corrente inativa. TIPO: INACTIVE_ACCOUNT.");
            }

            // Calcular saldo da conta corrente

            //Idempotencia
            if (await ResquestSaldoExiste(saldoContaCorrenteRequest))
            {
                var respostaExiste = await ObterRespostaSaldoIdempotencia(saldoContaCorrenteRequest);
                return Ok(respostaExiste);
            }

            decimal saldo = await CalcularSaldo(saldoContaCorrenteRequest.Numero);
            await _contaBancariaService.InsertIdempotenciaSaldoCorrente(saldoContaCorrenteRequest, saldo);

            return Ok(new
            {
                NumeroContaCorrente = saldoContaCorrenteRequest.Numero,
                DataHoraConsulta = DateTime.Now,
                Saldo = saldo.ToString("0.00")
            });
        }

        private async Task<bool> ContaCorrenteExiste(int numero)
        {
            return await _contaBancariaService.ContaCorrenteExiste(numero);
        }

        private async Task<bool> ContaCorrenteAtiva(int numero)
        {
            return await _contaBancariaService.ContaCorrenteAtiva(numero);
        }

        private async Task<decimal> CalcularSaldo(int numero)
        {
            return await _contaBancariaService.CalcularSaldo(numero);
        }

        [HttpPut("SaldoContaCorrente/Movimentacao")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
        public async Task<IActionResult> MovimentacaoContaCorrente(MovimentoRequest movimentoRequest)
        {
            // Validar dados de entrada
            if (!await ContaCorrenteExiste(movimentoRequest.Numero))
            {
                return BadRequest("Conta corrente inválida. TIPO: INVALID_ACCOUNT.");
            }

            if (!await ContaCorrenteAtiva(movimentoRequest.Numero))
            {
                return BadRequest("Conta corrente inativa. TIPO: INACTIVE_ACCOUNT.");
            }

            if (movimentoRequest.Valor <= 0)
            {
                return BadRequest("Valor inválido. TIPO: INVALID_VALUE.");
            }

            if (movimentoRequest.TipoMovimento.ToUpper() != "C" && movimentoRequest.TipoMovimento.ToUpper() != "D")
            {
                return BadRequest("Tipo de movimento inválido. TIPO: INVALID_TYPE.");
            }

            //Idempotencia
            if (await ResquestMovimentoExiste(movimentoRequest))
            {
                var respostaExiste = await ObterRespostaMovimentoIdempotencia(movimentoRequest);
                return Ok(respostaExiste);
            }

            var resposta = await _contaBancariaService.InsertMovimento(movimentoRequest);

            return StatusCode(200, resposta);
        }

        private async Task<bool> ResquestMovimentoExiste(MovimentoRequest movimentoRequest)
        {
            return await _contaBancariaService.ResquestMovimentoExiste(movimentoRequest);
        }

        private async Task<object> ObterRespostaMovimentoIdempotencia(MovimentoRequest movimentoRequest)
        {
            return await _contaBancariaService.ObterRespostaMovimentoIdempotencia(movimentoRequest);
        }

        private async Task<bool> ResquestSaldoExiste(SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            return await _contaBancariaService.ResquestSaldoExiste(saldoContaCorrenteRequest);
        }

        private async Task<object> ObterRespostaSaldoIdempotencia(SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            return await _contaBancariaService.ObterRespostaSaldoIdempotencia(saldoContaCorrenteRequest);
        }
    }
}
