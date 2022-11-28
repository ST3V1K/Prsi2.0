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
        public static bool CannotPlay { get; set; }

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
                char? colorCode = null;

                if (payload is [_, '1', '2', _])
                {
                    cardName = payload[..^1];
                    colorCode = payload.LastOrDefault();
                }

                if (payload is [_, '7'])
                    Values.Game.StackedCards += 2;

                if (Values.Game.LastPlayed?.Name != cardName)
                    Values.Game.RemoveCardFromDeck(cardName, colorCode);
            }

            if (payload is ['_', ..])
            {
                if (CannotPlay)
                {
                    CannotPlay = false;
                    return; 
                }

                int amount = int.Parse(payload[1..]);

                if (amount == 0)
                    Values.Game.StackedCards = 0;

                if (Values.Game.LastPlayed?.Number != 14)
                    Values.Game.DrawCardForEnemy(amount == 0 ? 1 : amount);

                Values.Game.SetPlaying();

                CannotPlay = false;
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