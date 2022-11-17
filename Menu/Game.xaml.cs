using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public Game()
        {
            InitializeComponent();
            lbName.Content = Values.PlayerName;
        }

        public List<Card> cards = new();

        public void DrawCards()
        {
            grid.Children.Clear();
            for (int i = 0; i < cards.Count; i++)
            {
                double angle = i * 180 / cards.Count + 90 / cards.Count - 90;

                var transformGroup = new TransformGroup();

                transformGroup.Children.Add(new RotateTransform(angle));

                CardControl cardControl = new(cards[i], i + 1, angle)
                {
                    Width = 120,
                    Height = 200,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    RenderTransformOrigin = new Point(.5, 1),
                    RenderTransform = transformGroup
                };
                
                grid.Children.Add(cardControl);
                Panel.SetZIndex(cardControl, cardControl.Index);
            }
        }

        public void AddCard(Card card)
        {
            cards.Add(card);
            DrawCards();
        }
    }
}
