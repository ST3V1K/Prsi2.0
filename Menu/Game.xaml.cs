using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Prsi.Values;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro Game.xaml
    /// </summary>
    public partial class Game : UserControl
    {
        public char? ChangeTo { get; set; }

        public Card? LastPlayed { get; private set; }

        public Players? Winner { get; set; }

        private Random? r = null;

        private readonly List<Card> thrownOut = new();
        private readonly List<Card> cards = new();
        private List<Card> deck = new();

        public int EnemyCardCount = 0;

        public int CardsCount { get => cards.Count; }

        private Players? Playing = null;

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

        public void StartGame(string opponentName, bool host, int seed)
        {
            r = new(seed);
            this.opponentName = opponentName;
            Dispatcher.Invoke(() => txtNames.Text = $"{Values.PlayerName}\n×\n{opponentName}");

            ClearData();
            deck = ((Card[])Cards.Clone()).ToList();

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
            else Playing = Players.Opponent;

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
                    Players.Player => Colors.Green,
                    Players.Opponent => Colors.Red,
                    _ => Colors.Black,
                };
                ellipse.Fill = new SolidColorBrush(color);
            });
        }

        private void VisualizeLastPlayedCard()
        {
            if (LastPlayed == null) return;

            lastPlayedImage.Source = LastPlayed.BitmapImage;
            if (LastPlayed.Number == 12)
            {
                changeColorImage.Source = ChangeTo switch
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
                VisualizeLastPlayedCard();
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
                VisualizeLastPlayedCard();
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
            if (r == null || Playing == Players.Opponent)
                return;

            await CardClient.DrawAsync(new()
            {
                Player = ServerPlayer,
                Game = ServerGame,
                Draw = amount
            }, deadline: Deadline);

            for (int i = 0; i < amount; i++)
            {
                if (deck.Count == 0)
                    await NewDeck();

                int index = r.Next(deck.Count);
                Card card = deck[index];

                deck.RemoveAt(index);
                cards.Add(card);
            }
            Playing = Players.Opponent;
            VisualizeDeck();
            VisualizePlayerCards();
        }

        public async void DrawCardForEnemy(int amount = 1)
        {
            if (r == null || Playing == Players.Player)
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
            if (Playing == Players.Opponent) return;
            if (!card.CanBePlayed(LastPlayed)) return;

            var _card = Cards.Where(c => c.Name == card.Name).First();

            cards.RemoveAll(c => c.Name == card.Name);
            thrownOut.Add(_card);
            LastPlayed = _card;

            if (LastPlayed.Number == 12)
            {
                ChangeColor changeColor = new()
                {
                    Owner = Switcher.PageSwitcher
                };
                changeColor.ShowDialog();
            }
            else
            {
                ChangeTo = null;
                Dispatcher.Invoke(() => Values.Game.changeColorImage.Source = null);
            }

            if (card.Number == 7)
                StackedCards += 2;

            Playing = Players.Opponent;

            Server.Card serverCard = ChangeTo is null ? new()
            {
                Color = card.Color,
                Value = card.Number
            } : new()
            {
                Color = card.Color,
                Value = card.Number,
                ChangeTo = ((char)ChangeTo).ToColor()
            };

            await CardClient.PlayAsync(new()
            {
                Player = ServerPlayer,
                Game = ServerGame,
                Card = serverCard
            }, deadline: Deadline);

            MustPlay = false;
            VisualizePlayerCards();

            if (cards.Count == 0)
            {
                Win();
            }
        }

        public void RemoveCardFromDeck(string name)
        {
            if (Playing == Players.Player) return;

            Card? card = Cards.Where(c => c.Name == name).FirstOrDefault();
            if (card == null) return;

            EnemyCardCount--;
            deck.RemoveAll(c => c.Name == name);
            thrownOut.Add(card);
            LastPlayed = card;
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
            LastPlayed = null;
        }

        public void SetPlaying()
        {
            Playing = Players.Player;
            ChangeColorPlaying();
            using SoundPlayer sound = new(Properties.Resources.beep);
            sound.Play();
        }

        public async Task Surrender()
        {
            Winner = Players.Opponent;

            await GameClient.LeaveAsync(new()
            {
                Game = ServerGame,
                Player = ServerPlayer
            });
        }

        private async void QuitGame()
        {
            await GameClient.LeaveAsync(new()
            {
                Game = ServerGame,
                Player = ServerPlayer
            });

            Listener.call?.Dispose();

            IsRunning = false;

            ClearData();
            deck.Clear();
            Switcher.Switch(GetMainMenu());
        }

        private async void BtnCannotPlay_Click(object sender, RoutedEventArgs e)
        {
            if (Playing == Players.Opponent) return;

            if (MustPlay)
            {
                MessageBox.Show("Musíte odehrát");
                return;
            }

            bool noTie = false;

            if (LastPlayed?.Number == 7)
            {
                DrawCard(StackedCards == 0 ? 1 : StackedCards);
                StackedCards = 0;
            }
            else if (LastPlayed?.Number == 14 && LastPlayed?.CanPlay != true)
            {
                await CardClient.StandAsync(new()
                {
                    Player = ServerPlayer,
                    Game = ServerGame
                }, deadline: Deadline);

                noTie = true;
                Playing = Players.Opponent;
            }
            else
                DrawCard();

            if (LastPlayed != null)
                LastPlayed.CanPlay = true;

            ChangeColorPlaying();

            if (deck.Count == 0 && !noTie)
                await RequestTie();
        }

        private static async Task RequestTie()
        {
            Listener.Tie = true;
            await GameClient.RequestTieAsync(new()
            {
                Game = ServerGame,
                Player = ServerPlayer
            }, deadline: Deadline);
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
            Winner = Players.Player;
            MessageBox.Show($"Vyhrál {PlayerName}");
            QuitGame();
        }

        public void Lose()
        {
            VisualizeOpponentCards();
            Winner = Players.Opponent;
            MessageBox.Show($"Vyhrál {Values.Game.opponentName}");
            QuitGame();
        }

        public void Draw()
        {
            Winner = Players.None;
            MessageBox.Show($"Remíza");
            QuitGame();
        }
    }
}
