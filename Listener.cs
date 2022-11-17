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
        public static void HandleListen(object o, NpgsqlNotificationEventArgs e)
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
                if (Values.Game.LastPlayed != payload)
                {
                    Values.Game.RemoveCardFromDeck(payload);
                }
            }

            if (payload == "_")
            {
                // oppenent cannot play
            }
        }
    }
}