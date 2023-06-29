using System;
using System.Globalization;

namespace Questao1
{
    class ContaBancaria
    {
        private int Numero { get; set; }
        private string Titular { get; set; }
        private double? SaldoConta { get; set; }
        public string DadosConta { get; set; }
        private const double TaxaSaque = 3.5;

        public ContaBancaria(int numero, string titular, double? depositoInicial = 0)
        {
            Numero = numero;
            Titular = titular;
            SaldoConta = depositoInicial;
            MontaMensagem();
        }

        public void Deposito(double quantiaDeposito)
        {
            SaldoConta = SaldoConta + quantiaDeposito;
            MontaMensagem();
        }

        public void Saque(double quantiaSaque)
        {
            SaldoConta = (SaldoConta - quantiaSaque) - TaxaSaque;
            MontaMensagem();
        }

        private void MontaMensagem()
        {
            DadosConta = $"Conta {Numero}, Titular: {Titular}, Saldo: ${SaldoConta}";
        }
    }
}
