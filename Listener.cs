using System.Globalization;
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
        public static bool Tie { get; set; }

        //public static void HandleListen(object o, NpgsqlNotificationEventArgs e)
        //{
        //    string payload = e.Payload;
        //    if (payload is ['p', ..])
        //    {
        //        string[] info = payload.Split('$');

        //        int seed = int.Parse(info[1]);
        //        string opponentName = info[2];

        //        Values.Game.StartGame(seed, opponentName, true);
        //        Switcher.Switch(Values.Game);
        //    }

        //    if (payload is ['s' or 'z' or 'l' or 'k', ..])
        //    {
        //        if (Values.Game.LastPlayed == null) return;


        //        string cardName = payload is [_, '1', '2', _] ? payload[..^1] : payload;

        //        if (Values.Game.LastPlayed.Name == cardName) return;

        //        if (payload is [_, '1', '2', char color])
        //            Values.Game.ChangeTo = color;
        //        else
        //            Values.Game.ChangeTo = null;

        //        if (payload is [_, '7'])
        //            Values.Game.StackedCards += 2;

        //        if (Values.Game.LastPlayed?.Name != cardName)
        //            Values.Game.RemoveCardFromDeck(cardName);
        //    }

        //    if (payload is ['_', .. string num])
        //    {
        //        if (CannotPlay)
        //        {
        //            CannotPlay = false;
        //            return; 
        //        }

        //        int amount = int.Parse(num);

        //        if (Values.Game.LastPlayed?.Number != 14 || Values.Game.LastPlayed?.CanPlay == true) 
        //            Values.Game.DrawCardForEnemy(amount == 0 ? 1 : amount);

        //        if (Values.Game.LastPlayed == null) return;

        //        Values.Game.LastPlayed.CanPlay = true;
        //        Values.Game.StackedCards = 0;
        //        Values.Game.SetPlaying();

        //        CannotPlay = false;
        //    }

        //    if (payload == "konec") 
        //    {
        //        if (Values.Game.EnemyCardCount == 0 || Values.Game.Winner == Values.Players.Opponent)
        //            Values.Game.Lose();
        //        else
        //            Values.Game.Win();
        //    }

        //    if (payload == "remiza")
        //    {
        //        if (Tie) return;
        //        Tie = false;

        //        if (Values.Game.CanPlaySomeCard)
        //        {
        //            Values.Game.RemoveDeck();
        //            Values.Game.MustPlay = true;
        //            Values.Game.SetPlaying();
        //        }
        //        else
        //            Values.Game.Draw();
        //    }
        //}
    }
}