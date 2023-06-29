namespace Questao5.Application.Commands.Requests
{
    public class MovimentoRequest
    {
        public int Numero { get; set; }
        public string TipoMovimento { get; set; }
        public decimal Valor { get; set; }

        public override string ToString()
        {
            return $"{{ \n\"Numero\": \"{Numero}\", \n\"Valor\": \"{Valor}\", \n\"TipoMovimento\": \"{TipoMovimento}\", \n\"Horário Movimentação\": \"{DateTime.Now.ToString("dd/MM/yyyy HH:mm")}\" \n}}";
        }
    }
}
