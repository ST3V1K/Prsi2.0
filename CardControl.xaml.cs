using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
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
    /// Interakční logika pro CardControl.xaml
    /// </summary>
    public partial class CardControl : UserControl
    {
        private readonly Card Card;

        private bool Click;

        public bool Selected { get; private set; }

        public int Index { get; private set; }

        internal double Angle { get; set; }

        public CardControl(Card card, int index, double angle)
        {
            InitializeComponent();
            border.BorderBrush = null;
            image.Source = card.BitmapImage;
            Card = card;
            Index = index;
            Angle = angle;
        }

        private void Card_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Click = true;
        }

        private void Card_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Click)
            {
                if (Selected)
                {
                    Values.Game?.PlayCard(Card);
                }

                Selected = true;
                double offX = Values.MOVE_LENGTH * Math.Sin(Angle * (Math.PI / 180));
                double offY = -Values.MOVE_LENGTH * Math.Cos(Angle * (Math.PI / 180));
                ((TransformGroup)RenderTransform).Children.Add(new TranslateTransform(offX, offY));

                foreach (CardControl cc in Values.Game?.cardsGrid.Children.OfType<CardControl>().Where(c => c != this) ?? Array.Empty<CardControl>())
                {
                    cc.Selected = false;
                    if (((TransformGroup)cc.RenderTransform).Children.Count > 1)
                        ((TransformGroup)cc.RenderTransform).Children.RemoveAt(1);
                }
            }

            Click = false;
        }

        private void Card_MouseEnter(object sender, MouseEventArgs e)
        {
            Values.Game.EnlargeCardsGrid();
            border.BorderBrush = Brushes.Black;
            Panel.SetZIndex(this, 99);
        }

        private void Card_MouseLeave(object sender, MouseEventArgs e)
        {
            Values.Game.ShrinkCardsGrid();  
            border.BorderBrush = null;
            Panel.SetZIndex(this, Index);
            Selected = false;
            if (((TransformGroup)RenderTransform).Children.Count > 1)
                ((TransformGroup)RenderTransform).Children.RemoveAt(1);
        }
    }
}
