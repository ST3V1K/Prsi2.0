using Npgsql;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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

                Values.Game.CreateGame(seed, opponentName);
                Switcher.Switch(Values.Game);
            }
        }
    }
}