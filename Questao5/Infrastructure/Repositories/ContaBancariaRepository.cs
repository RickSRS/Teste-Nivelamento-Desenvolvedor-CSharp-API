using Microsoft.Data.Sqlite;
using Questao5.Application.Commands.Requests;
using Questao5.Domain.Interfaces;
using Questao5.Infrastructure.Sqlite;

namespace Questao5.Infrastructure.Repositories
{
    public class ContaBancariaRepository : IContaBancariaRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly SqliteConnection _connection;

        public ContaBancariaRepository(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
            _connection = new SqliteConnection(_databaseConfig.Name);
        }

        public async Task<bool> ContaCorrenteExiste(int numero)
        {
            await _connection.OpenAsync();
            string query = @"SELECT COUNT(*) FROM contacorrente WHERE numero = @numero";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@numero", numero);
            int count = Convert.ToInt32(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();

            return count > 0;
        }

        public async Task<bool> ContaCorrenteAtiva(int numero)
        {
            await _connection.OpenAsync();
            string query = @"SELECT ativo FROM contacorrente WHERE numero = @numero";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@numero", numero);
            bool ativo = Convert.ToBoolean(await command.ExecuteScalarAsync());
            await _connection.CloseAsync();

            return ativo;
        }

        public async Task<decimal> CalcularSaldo(int numero)
        {
            await _connection.OpenAsync();
            string query = @"SELECT SUM(CASE WHEN mov.tipomovimento = 'C' THEN mov.valor ELSE -mov.valor END)
                            FROM contacorrente cc
                            LEFT JOIN movimento mov ON mov.idcontacorrente = cc.idcontacorrente
                            WHERE cc.numero = @numero";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@numero", numero);
            object result = await command.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        private async Task<string> ObterIdContaCorrente(int numero)
        {
            await _connection.OpenAsync();
            string query = @"SELECT idcontacorrente FROM contacorrente WHERE numero = @numero";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@numero", numero);
            object idcontacorrente = await command.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return idcontacorrente.ToString();
        }

        public async Task<int> InsertMovimento(MovimentoRequest movimentoRequest)
        {
            var idContaCorrente = ObterIdContaCorrente(movimentoRequest.Numero).Result;
            await _connection.OpenAsync();
            string query = "INSERT INTO movimento (idmovimento, idcontacorrente, datamovimento, tipomovimento, valor) VALUES (@IdMovimento, @IdContaCorrente, @DataMovimento, @TipoMovimento, @Valor)";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@IdMovimento", Guid.NewGuid().ToString());
            command.Parameters.AddWithValue("@IdContaCorrente", idContaCorrente);
            command.Parameters.AddWithValue("@DataMovimento", DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            command.Parameters.AddWithValue("@TipoMovimento", movimentoRequest.TipoMovimento.ToUpper());
            command.Parameters.AddWithValue("@Valor", movimentoRequest.Valor);
            var result = await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();

            return result;
        }

        public async Task<string> ResgatarIdMovimentacao(MovimentoRequest movimento)
        {
            await _connection.OpenAsync();
            string query = @"SELECT mov.idmovimento
                            FROM contacorrente cc
                            LEFT JOIN movimento mov ON mov.idcontacorrente = cc.idcontacorrente
                            WHERE cc.numero = @Numero
                            AND mov.valor = @Valor
                            AND mov.tipomovimento = @TipoMovimento
                            ORDER BY datamovimento DESC
                            LIMIT 1";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@Numero", movimento.Numero);
            command.Parameters.AddWithValue("@Valor", movimento.Valor);
            command.Parameters.AddWithValue("@TipoMovimento", movimento.TipoMovimento.ToUpper());
            object idMovimento = await command.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return idMovimento.ToString();
        }

        public async Task<bool> RequestMovimentoExiste(MovimentoRequest movimentoRequest)
        {
            await _connection.OpenAsync();
            string query = @"SELECT COUNT(*) FROM idempotencia WHERE requisicao = @Requisicao";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@Requisicao", movimentoRequest.ToString());
            var result = (long)command.ExecuteScalar();
            await _connection.CloseAsync();

            return result > 0;
        }

        public async Task InsertIdempotenciaMovimento(MovimentoRequest movimentoRequest)
        {
            string idMovimento = await ResgatarIdMovimentacao(movimentoRequest);
            await _connection.OpenAsync();
            string query = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@ChaveIdempotencia", Guid.NewGuid().ToString());
            command.Parameters.AddWithValue("@Requisicao", movimentoRequest.ToString());
            command.Parameters.AddWithValue("@Resultado", $"{{ \n\"Resultado\": \"Operação já realizada anteriormente\", \n\"Tipo Operação\": \"Movimentação\", \n\"idMovimento\": \"{idMovimento}\", \n\"Requisição\": \"{movimentoRequest.ToString()}\" \n}}");
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task InsertIdempotenciaSaldoCorrente(SaldoContaCorrenteRequest saldoRequest, decimal saldo)
        {
            await _connection.OpenAsync();
            string query = "INSERT INTO idempotencia (chave_idempotencia, requisicao, resultado) VALUES (@ChaveIdempotencia, @Requisicao, @Resultado)";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@ChaveIdempotencia", Guid.NewGuid().ToString());
            command.Parameters.AddWithValue("@Requisicao", saldoRequest.ToString());
            command.Parameters.AddWithValue("@Resultado", $"{{ \n\"Resultado\": \"Operação já realizada anteriormente\", \n\"Tipo Operação\": \"Extrato\", \n\"Saldo\": \"{saldo}\", \n\"Requisição\": \"{saldoRequest.ToString()}\" \n}}");
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task<object> ObterRespostaMovimentoIdempotencia(MovimentoRequest movimentoRequest)
        {
            await _connection.OpenAsync();
            string query = @"SELECT resultado
                            FROM idempotencia
                            WHERE requisicao = @Requisicao";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@Requisicao", movimentoRequest.ToString());
            object resultado = await command.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return resultado;
        }

        public async Task<object> ObterRespostaSaldoIdempotencia(SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            await _connection.OpenAsync();
            string query = @"SELECT resultado
                            FROM idempotencia
                            WHERE requisicao = @Requisicao";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@Requisicao", saldoContaCorrenteRequest.ToString());
            object resultado = await command.ExecuteScalarAsync();
            await _connection.CloseAsync();

            return resultado;
        }

        public async Task<bool> RequestSaldoExiste(SaldoContaCorrenteRequest saldoContaCorrenteRequest)
        {
            await _connection.OpenAsync();
            string query = @"SELECT COUNT(*) FROM idempotencia WHERE requisicao = @Requisicao";
            SqliteCommand command = new SqliteCommand(query, _connection);
            command.Parameters.AddWithValue("@Requisicao", saldoContaCorrenteRequest.ToString());
            var result = (long)command.ExecuteScalar();
            await _connection.CloseAsync();

            return result > 0;
        }
    }
}
