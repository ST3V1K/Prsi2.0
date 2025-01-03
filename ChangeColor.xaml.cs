﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Prsi
{
    /// <summary>
    /// Interakční logika pro ChangeColor.xaml
    /// </summary>
    public partial class ChangeColor : Window
    {
        public ChangeColor()
        {
            InitializeComponent();
        }

        private void Img_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            char? changeTo = ((string)img.Tag).FirstOrDefault();
            Values.Game.ChangeTo = changeTo;
            Close();
        }
    }
}
