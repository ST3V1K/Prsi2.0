using Npgsql;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Prsi
{
    static class Listener
    {
        public static async Task HandleListen(object o, NpgsqlNotificationEventArgs e)
        {
            string payload = e.Payload;
            if (payload.StartsWith("p"))
            {
                string[] info = payload.Split('$');

                int seed = int.Parse(info[1]);
                string opponentName = info[2];

                Values.Game.StartGame(seed, opponentName, true);
                Switcher.Switch(Values.Game);
            }

            if ((new char[] { 's', 'z', 'l', 'k' }).Contains(payload.First()))
            {
                string cardName = payload;
                if (payload.Contains("12"))
                    cardName = payload[..^1];
                if (Values.Game.LastPlayed?.Name != cardName)
                {
                    Values.Game.RemoveCardFromDeck(cardName);
                }
            }

            if (payload == "_")
            {
                if (Values.Game.LastPlayed?.Name.Contains("14") == true)
                    Values.Game.SetPlaying();
                else
                    Values.Game.DrawCardForEnemy();
            }

            if (payload == "konec" && Values.Game.Winner == null) 
            {
                if (Values.Game.CardsCount == 0)
                    await Values.Game.QuitGame(Values.Players.Player);
                else
                    await Values.Game.QuitGame(Values.Players.Opponent);
            } 
        }
    }
}