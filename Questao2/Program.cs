using Newtonsoft.Json;
using Questao2;
using System.Net;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        var requisicaoWeb = WebRequest.CreateHttp($"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}");
        requisicaoWeb.Method = "GET";
        requisicaoWeb.UserAgent = "RequisicaoWeb";

        using (var resposta = requisicaoWeb.GetResponse())
        {
            var streamDados = resposta.GetResponseStream();
            StreamReader reader = new StreamReader(streamDados);
            object objResponse = reader.ReadToEnd();

            var post = JsonConvert.DeserializeObject<Post>(objResponse.ToString());

            foreach (var item in post.data)
            {
                totalGoals += Convert.ToInt32(item.team1goals);
            }

            streamDados.Close();
            resposta.Close();
        }

        return totalGoals;
    }

}