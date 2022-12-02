using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        public char? ChangeTo { get; set; }

        public Card? LastPlayed { get; private set; }

        public Values.Players? Winner { get; set; }

        private Random? r = null;

        private readonly List<Card> thrownOut = new();
        private readonly List<Card> cards = new();
        private List<Card> deck = new();

        private int enemyCardCount = 0;

        public int CardsCount { get => cards.Count; }

        private Values.Players? Playing = null;

        private string? opponentName;

        public int StackedCards { get; set; }

        public Game()
        {
            InitializeComponent();
        }

        private async void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Opravdu chcete opustit hru a tím se vzdát?", "Konec?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                await Surrender();
        }

        public void StartGame(int seed, string opponentName, bool host)
        {
            r = new(seed);
            this.opponentName = opponentName;
            Dispatcher.Invoke(() => txtNames.Text = $"{Values.PlayerName}\n×\n{opponentName}");

            ClearData();
            deck = ((Card[])Values.Cards.Clone()).ToList();

            LastPlayed = deck[r.Next(deck.Count)];
            deck.Remove(LastPlayed);
            thrownOut.Add(LastPlayed);

            for (int i = 0; i < 4; i++)
            {
                if (host)
                {
                    GiveCard();
                    GiveCardToOpponent();
                }
                else
                {
                    GiveCardToOpponent();
                    GiveCard();
                }
            }

            if (host) Playing = Values.Players.Player;
            else Playing = Values.Players.Opponent;

            VisualizePlayerCards();
            VisualizeOpponentCards();
        }

        private void ChangeColorPlaying()
        {
            Dispatcher.Invoke(() =>
            {
                var color = Playing switch
                {
                    Values.Players.Player => Colors.Green,
                    Values.Players.Opponent => Colors.Red,
                    _ => Colors.Black,
                };
                ellipse.Fill = new SolidColorBrush(color);
            });
        }

        private void VisualizeLastPlayedCard(char? centerCardColorCode)
        {
            if (LastPlayed == null) return;

            lastPlayedImage.Source = LastPlayed.BitmapImage;
            if (LastPlayed.Number == 12)
            {
                changeColorImage.Source = centerCardColorCode switch
                {
                    's' => (DrawingImage)FindResource("ImageS"),
                    'k' => (DrawingImage)FindResource("ImageK"),
                    'l' => (DrawingImage)FindResource("ImageL"),
                    'z' => (DrawingImage)FindResource("ImageZ"),
                    _ => null
                };
            }
        }

        private void VisualizePlayerCards()
        {
            Dispatcher.Invoke(() =>
            {
                if (LastPlayed == null) return;

                cardsGrid.Children.Clear();
                VisualizeLastPlayedCard(ChangeTo);
                ChangeColorPlaying();

                for (int i = 0; i < cards.Count; i++)
                {
                    double angle = i * 180 / cards.Count + 90 / cards.Count - 90;

                    var transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform(angle));

                    CardControl cardControl = new(cards[i], i + 1, angle)
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransformOrigin = new Point(.5, 1),
                        RenderTransform = transformGroup
                    };

                    cardsGrid.Children.Add(cardControl);
                    Panel.SetZIndex(cardControl, cardControl.Index);
                }
            });
        }

        private void VisualizeOpponentCards(char? colorCode = null)
        {
            Dispatcher.Invoke(() =>
            {
                if (LastPlayed == null) return;

                opponentGrid.Children.Clear();
                VisualizeLastPlayedCard(colorCode);
                ChangeColorPlaying();

                for (int i = 0; i < enemyCardCount; i++)
                {
                    double angle = i * 180 / enemyCardCount + 90 / enemyCardCount - 90;

                    var transformGroup = new TransformGroup();

                    transformGroup.Children.Add(new RotateTransform(angle));

                    opponentGrid.Children.Add(new Image()
                    {
                        RenderTransformOrigin = new Point(.5, 0),
                        RenderTransform = transformGroup,
                        Source = Values.CardBack.BitmapImage
                    });
                }
            });
        }

        private void GiveCard()
        {
            if (r == null) return;

            int index = r.Next(deck.Count);
            cards.Add(deck[index]);
            deck.RemoveAt(index);
        }

        private void GiveCardToOpponent()
        {
            if (r == null) return;

            int index = r.Next(deck.Count);
            deck.RemoveAt(index);
            enemyCardCount++;
        }

        public void DrawCard(int amount = 1)
        {
            for (int i = 0; i < amount; i++)
            {
                if (r == null || Playing == Values.Players.Opponent)
                    return;

                int index = r.Next(deck.Count);
                Card card = deck[index];

                if (deck.Count == 0)
                    NewDeck();
                deck.RemoveAt(index);

                cards.Add(card);

                Playing = Values.Players.Opponent;
            }
            VisualizePlayerCards();
        }

        public void DrawCardForEnemy(int amount = 1)
        {
            if (r == null || Playing == Values.Players.Player)
                return;

            for (int i = 0; i < amount; i++)
            {
                int index = r.Next(deck.Count);

                if (deck.Count == 0)
                    NewDeck();
                deck.RemoveAt(index);
                enemyCardCount++;

                Playing = Values.Players.Player;
            }
            VisualizeOpponentCards();
        }

        public async void PlayCard(Card card)
        {
            if (Playing == Values.Players.Opponent) return;
            if (!card.CanBePlayed(LastPlayed)) return;

            if (card.Name.Contains("12"))
            {
                ChangeColor changeColor = new() { Owner = Switcher.PageSwitcher };
                changeColor.ShowDialog();
            }
            else ChangeTo = null;

            cards.Remove(card);
            thrownOut.Add(card);
            LastPlayed = card;
            Playing = Values.Players.Opponent;

            if (card.Number == 7)
                StackedCards += 2;

            using NpgsqlCommand cmd = new("select tahni(@jmenoin, @hesloin, @tahin)", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            cmd.Parameters.AddWithValue("@tahin", card.Name + ChangeTo);
            await cmd.ExecuteNonQueryAsync();

            ChangeTo = null;
            Dispatcher.Invoke(() => Values.Game.changeColorImage.Source = null);

            VisualizePlayerCards();

            if (cards.Count == 0)
            {
                using NpgsqlCommand cmdKonec = new("select tahni(@jmenoin, @hesloin, 'X')", Values.Connection);
                cmdKonec.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
                cmdKonec.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
                await cmdKonec.ExecuteNonQueryAsync();

                Win();

                return;
            }
        }

        public void RemoveCardFromDeck(string name, char? colorCode)
        {
            if (Playing == Values.Players.Player) return;

            if (deck.Count == 0)
                NewDeck();

            Card? card = Values.Cards.Where(c => c.Name == name).FirstOrDefault();

            if (card == null) return;

            thrownOut.Add(card);
            LastPlayed = card;
            LastPlayed.ChangeToColor = colorCode;
            enemyCardCount--;

            Playing = Values.Players.Player;
            VisualizeOpponentCards(colorCode);
        }

        private void NewDeck() => deck = thrownOut.GetRange(0, thrownOut.Count);

        private void ClearData()
        {
            Playing = null;
            thrownOut.Clear();
            cards.Clear();
        }

        public void SetPlaying()
        {
            Playing = Values.Players.Player;
            ChangeColorPlaying();
        }

        private async Task Surrender()
        {
            using NpgsqlCommand cmd = new($"select tahni(@jmenoin, @hesloin, 'X')", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            cmd.Parameters.AddWithValue("@tahin", "X");
            await cmd.ExecuteNonQueryAsync(Values.FormClosedToken);
        }

        private void QuitGame()
        {
            ClearData();
            deck.Clear();
            Switcher.Switch(Values.GetMainMenu());
        }

        private async void BtnCannotPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Playing == Values.Players.Opponent) return;

            int num = 0;

            if (LastPlayed?.Name.Contains("14") == false)
                DrawCard();

            else if (LastPlayed?.Number == 7)
            {
                num = StackedCards;
                DrawCard(StackedCards);
                StackedCards = 0;
            }

            Listener.CannotPlay = true;
            ChangeColorPlaying();

            using NpgsqlCommand cmd = new($"select tahni(@jmenoin, @hesloin, '_{num}')", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            await cmd.ExecuteNonQueryAsync(Values.FormClosedToken);
        }

        public void EnlargeCardsGrid()
        {
            cardsGrid.Height = 240;
        }

        public void ShrinkCardsGrid()
        {
            cardsGrid.Height = 120;
        }

        public void Win()
        {
            Winner = Values.Players.Player;
            MessageBox.Show($"Vyhrál {Values.PlayerName}");
            QuitGame();
        }

        public void Lose()
        {
            VisualizeOpponentCards();
            Winner = Values.Players.Opponent;
            MessageBox.Show($"Vyhrál {Values.Game.opponentName}");
            QuitGame();
        }
    }
}
