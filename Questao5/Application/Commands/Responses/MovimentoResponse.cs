using Questao5.Domain.Entities;

namespace Questao5.Application.Commands.Responses
{
    public class MovimentoResponse
    {
        public string IdMovimento { get; set; }
        public string IdContaCorrente { get; set; }
        public string DataMovimento { get; set; }
        public string TipoMovimento { get; set; }
        public decimal Valor { get; set; }

        public static explicit operator MovimentoResponse(Movimento movimento)
        {
            return new MovimentoResponse()
            {
                IdMovimento = movimento.IdMovimento,
                IdContaCorrente = movimento.IdContaCorrente,
                DataMovimento = movimento.DataMovimento,
                TipoMovimento = movimento.TipoMovimento,
                Valor = movimento.Valor
            };
        }
    }
}
