using Grpc.Core;
using System.ComponentModel;

namespace Prsi
{
    static class Listener
    {
        static Listener()
        {
            backgroundWorker.DoWork += Listen;
        }

        public static bool CannotPlay { get; set; }
        public static bool Tie { get; set; }

        public static BackgroundWorker backgroundWorker = new();

        public static AsyncServerStreamingCall<Server.GameStream> call;

        public static async void Listen(object sender, DoWorkEventArgs e)
        {
            while (await call.ResponseStream.MoveNext())
            {
                var stream = call.ResponseStream.Current;

                if (stream.HasTie)
                {
                    if (Tie) return;
                    Tie = false;

                    if (Values.Game.CanPlaySomeCard)
                    {
                        Values.Game.RemoveDeck();
                        Values.Game.MustPlay = true;
                        Values.Game.SetPlaying();
                    }
                    else
                        Values.Game.Draw();
                }
                else if (stream.Card is not null) // Play
                {
                    if (Values.Game.LastPlayed == null) return;

                    if (Values.Game.LastPlayed.Name == stream.Card.GetCardName()) return;

                    if (stream.Card.Value == 12)
                        Values.Game.ChangeTo = stream.Card.ChangeTo.ToChar();
                    else
                        Values.Game.ChangeTo = null;

                    if (stream.Card.Value == 7)
                        Values.Game.StackedCards += 2;

                    if (Values.Game.LastPlayed?.Name != stream.Card.GetCardName())
                        Values.Game.RemoveCardFromDeck(stream.Card.GetCardName());

                    if (Values.Game.EnemyCardCount == 0)
                    {
                        Values.Game.Lose();
                    }
                }
                else if (stream.HasDraw) // Draw
                {
                    if (Values.Game.LastPlayed?.Number != 14 || Values.Game.LastPlayed?.CanPlay == true)
                        Values.Game.DrawCardForEnemy(stream.Draw);

                    if (Values.Game.LastPlayed == null) return;

                    Values.Game.LastPlayed.CanPlay = true;
                    Values.Game.StackedCards = 0;
                    Values.Game.SetPlaying();
                }
                else // Stand
                {
                    if (Values.Game.LastPlayed == null) return;

                    Values.Game.LastPlayed.CanPlay = true;
                    Values.Game.StackedCards = 0;
                    Values.Game.SetPlaying();
                }
            }
        }
    }
}