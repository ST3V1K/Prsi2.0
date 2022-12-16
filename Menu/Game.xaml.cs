using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public int EnemyCardCount = 0;

        public int CardsCount { get => cards.Count; }

        private Values.Players? Playing = null;

        private string? opponentName;

        public int StackedCards { get; set; }

        public bool CanPlaySomeCard { get => cards.Any(c => c.CanBePlayed(LastPlayed)); }
        
        public bool MustPlay { get; set; }

        public bool IsRunning { get; private set; }

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

            Card firstCard;
            while ((firstCard = deck[r.Next(deck.Count)]).Number == 12) ;

            if (firstCard.Number == 7) StackedCards += 2;

            LastPlayed = firstCard;
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

            if (host) SetPlaying();
            else Playing = Values.Players.Opponent;

            VisualizeDeck();
            VisualizePlayerCards();
            VisualizeOpponentCards();

            IsRunning = true;
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
            else changeColorImage.Source = null;
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

        private void VisualizeOpponentCards()
        {
            Dispatcher.Invoke(() =>
            {
                if (LastPlayed == null) return;

                opponentGrid.Children.Clear();
                VisualizeLastPlayedCard(LastPlayed.ChangeToColor);
                ChangeColorPlaying();

                for (int i = 0; i < EnemyCardCount; i++)
                {
                    double angle = i * 180 / EnemyCardCount + 90 / EnemyCardCount - 90;

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

        public void VisualizeDeck()
        {
            Dispatcher.Invoke(() =>
            {
                deckGrid.Children.Clear();

                for (int i = 0; i < deck.Count; i++)
                {
                    Image img = new()
                    {
                        Source = Values.CardBack.BitmapImage,
                        Width = 120,
                        Height = 160,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new(0, 0, i, i)
                    };

                    deckGrid.Children.Add(img);
                }
                deckGrid.Children.Add(new TextBlock()
                {
                    Text = deck.Count.ToString(),
                    TextAlignment = TextAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = 20,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left,
                });
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
            EnemyCardCount++;
        }

        public async void DrawCard(int amount = 1)
        {
            if (r == null || Playing == Values.Players.Opponent)
                return;

            for (int i = 0; i < amount; i++)
            {
                if (deck.Count == 0)
                    await NewDeck();

                int index = r.Next(deck.Count);
                Card card = deck[index];

                deck.RemoveAt(index);
                cards.Add(card);
            }
            Playing = Values.Players.Opponent;
            VisualizeDeck();
            VisualizePlayerCards();
        }

        public async void DrawCardForEnemy(int amount = 1)
        {
            if (r == null || Playing == Values.Players.Player)
                return;

            for (int i = 0; i < amount; i++)
            {
                if (deck.Count == 0)
                    await NewDeck();

                int index = r.Next(deck.Count);
                deck.RemoveAt(index);
                EnemyCardCount++;
            }
            SetPlaying();
            VisualizeDeck();
            VisualizeOpponentCards();
        }

        public async void PlayCard(Card card)
        {
            if (Playing == Values.Players.Opponent) return;
            if (!card.CanBePlayed(LastPlayed)) return;

            if (card.Number == 12)
            {
                ChangeColor changeColor = new() { Owner = Switcher.PageSwitcher };
                changeColor.ShowDialog();
            }
            else
            {
                ChangeTo = null; 
                Dispatcher.Invoke(() => Values.Game.changeColorImage.Source = null);
            }

            var _card = Values.Cards.Where(c => c.Name == card.Name).First();

            cards.RemoveAll(c => c.Name == card.Name);
            thrownOut.Add(_card);
            LastPlayed = _card;
            Playing = Values.Players.Opponent;

            if (card.Number == 7)
                StackedCards += 2;

            using NpgsqlCommand cmd = new("select tahni(@jmenoin, @hesloin, @tahin)", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            cmd.Parameters.AddWithValue("@tahin", card.Name + ChangeTo);
            await cmd.ExecuteNonQueryAsync();

            MustPlay = false;
            VisualizePlayerCards();

            if (cards.Count == 0)
            {
                using NpgsqlCommand cmdKonec = new("select tahni(@jmenoin, @hesloin, 'X')", Values.Connection);
                cmdKonec.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
                cmdKonec.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
                await cmdKonec.ExecuteNonQueryAsync();
            }
        }

        public void RemoveCardFromDeck(string name)
        {
            if (Playing == Values.Players.Player) return;

            Card? card = Values.Cards.Where(c => c.Name == name).FirstOrDefault();
            if (card == null) return;

            char? changeTo = LastPlayed?.ChangeToColor;

            EnemyCardCount--;
            deck.RemoveAll(c => c.Name == name);
            thrownOut.Add(card);
            LastPlayed = card;
            LastPlayed.ChangeToColor = changeTo;
            SetPlaying();

            VisualizeOpponentCards();
        }

        public void RemoveDeck()
        {
            deck.Clear();
            VisualizeDeck();
        }

        private async Task NewDeck() 
        { 
            deck.AddRange(thrownOut.Where(c => c != LastPlayed));
            thrownOut.Clear();

            if (deck.Count == 0) await RequestTie();
        }

        private void ClearData()
        {
            Playing = null;
            thrownOut.Clear();
            cards.Clear();
            EnemyCardCount = 0;
            StackedCards = 0;
            MustPlay = false;
        }

        public void SetPlaying()
        {
            Playing = Values.Players.Player;
            ChangeColorPlaying();
            using SoundPlayer sound = new(Properties.Resources.beep);
            sound.Play();
        }

        public async Task Surrender()
        {
            Winner = Values.Players.Opponent;

            using NpgsqlCommand cmd = new($"select tahni(@jmenoin, @hesloin, 'X')", Values.Connection);
            cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            await cmd.ExecuteNonQueryAsync(Values.FormClosedToken).ContinueWith((t) => { });
        }

        private void QuitGame()
        {
            IsRunning = false;

            ClearData();
            deck.Clear();
            Switcher.Switch(Values.GetMainMenu());
        }

        private async void BtnCannotPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Playing == Values.Players.Opponent) return;

            if (MustPlay)
            {
                MessageBox.Show("Musíte odehrát");
                return;
            }

            int num = 0;
            bool noTie = false;

            if (LastPlayed?.Number == 7)
            {
                num = StackedCards;
                DrawCard(StackedCards == 0 ? 1 : StackedCards);
                StackedCards = 0;
            }
            else if (LastPlayed?.Number == 14 && LastPlayed?.CanPlay != true)
            {
                noTie = true;
                Playing = Values.Players.Opponent;
            }
            else
                DrawCard();

            if (LastPlayed != null)
                LastPlayed.CanPlay = true;
            Listener.CannotPlay = true;
            ChangeColorPlaying();

            if (deck.Count > 0 || noTie)
            {
                using NpgsqlCommand cmd = new($"select tahni(@jmenoin, @hesloin, '_{num}')", Values.Connection);
                cmd.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
                cmd.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
                await cmd.ExecuteNonQueryAsync(Values.FormClosedToken);
            }
            else await RequestTie();
        }

        private static async Task RequestTie()
        {
            Listener.Tie = true;
            using NpgsqlCommand cmdKonec = new("select tahni(@jmenoin, @hesloin, 'T')", Values.Connection);
            cmdKonec.Parameters.AddWithValue("@jmenoin", Values.PlayerName);
            cmdKonec.Parameters.AddWithValue("@hesloin", Values.PlayerPassword);
            await cmdKonec.ExecuteNonQueryAsync();
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

        public void Draw()
        {
            Winner = Values.Players.None;
            MessageBox.Show($"Remíza");
            QuitGame();
        }
    }
}
