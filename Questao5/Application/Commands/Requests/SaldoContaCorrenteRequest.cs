using System.Drawing;

namespace Questao5.Application.Commands.Requests
{
    public class SaldoContaCorrenteRequest
    {
        public int Numero { get; set; }

        public override string ToString()
        {
            return $"{{ \n\"Numero\": \"{Numero}\", \n\"Horário Extrato\": \"{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}\" \n}}";
        }
    }
}
