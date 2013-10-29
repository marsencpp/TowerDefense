using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TowerDefense;

namespace TowerDefense
{
    public partial class Form1 : Form
    {
        private Level _lvl;
        public Form1()
        {
            InitializeComponent();
            _lvl = new Level(1300, 650);
            //Level.Restart();
            KeyDown+=Form1_KeyDown;
            KeyUp+=Form1_KeyUp;
            KeyPress+=Form1_KeyPress;
            //Cursor.Dispose();
            pictureBox1.Paint += Level.Paint;
        }

        
        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (e.KeyChar == 'r') Level.Restart();
        }

        void Form1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
