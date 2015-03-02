using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Omnitwork;
using _7WondersCore;

namespace SevenWondersClientGUI
{
    public partial class Form1 : Form, GUI
    {
        List<PictureBox> card_boxes;
        List<PictureBox> booster_boxes;
        List<PictureBox> tier_boxes;                        
        ClientInterface interpace;        
        // !!!        
        string image_folder = "D:/planchik/pics";
        // странное название для многофункциональных делегатов, есть смысл переделать в %signature%CallBack
        delegate void ShowGoldCallBack(int gold);
        delegate void ShowStringMessageCallBack(string str);
        delegate void ShowCardsCallBack(List<Card> cards);
        delegate void DisplayWonderCallBack(Wonder wonder);
        delegate void PromptPlayCallBack();
        public bool wonder = false;
        int current_tier = 0;

        public Form1()
        {
            InitializeComponent();
            InitializeContainers();
            StartClient();
        }

        public void InitializeContainers()
        {
            card_boxes = new List<PictureBox>();
            booster_boxes = new List<PictureBox>();
            tier_boxes = new List<PictureBox>();
            for (int i = 0; i < 7; i++)
            {
                PictureBox pbox = new PictureBox();
                pbox.Parent = boosterContainer;
                pbox.Top = 10;
                pbox.Left = 220 * i + 10;
                pbox.Width = 208;
                pbox.Height = 320;
                pbox.Enabled = false;
                pbox.MouseClick += pbox_MouseClick;
                booster_boxes.Add(pbox);
            }
        }
        
        void pbox_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox pbox = ((PictureBox)sender);
            if (e.Button == MouseButtons.Left)
            {
                interpace.PickCard((int)pbox.Tag, "Play");
            }
            else
            {
                interpace.PickCard((int)pbox.Tag, "Wonder");
            }
            boosterContainer.Hide();
        }

        public void ShowBooster(List<Card> cards)
        {
            if (InvokeRequired)
            {
                Delegate d = new ShowCardsCallBack(ShowBooster);
                this.Invoke(d, cards);
            }
            else
            {
                headlineLabel.Text = "PICK A CARD BITCH";
                // !!!        
                for (int i = 0; i < cards.Count; i++)
                {
                    // !!!
                    string image_filename = image_folder + "/" + cards[i].Name + ".jpg";
                    booster_boxes[i].BackgroundImage = Image.FromFile(image_filename);
                    booster_boxes[i].Tag = cards[i].Id;
                    booster_boxes[i].Enabled = true;                    
                }
                boosterContainer.Show();
                if (wonder)
                {
                    pbox_MouseClick(booster_boxes.First(), new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 0, 0, 0, 0));
                }
                else
                {
                    pbox_MouseClick(booster_boxes.First(), new MouseEventArgs(System.Windows.Forms.MouseButtons.Right, 0, 0, 0, 0));
                    wonder = true;
                }
            }
        }

        public void ShowStringMessage(string s)
        {
            if (InvokeRequired)
            {
                Delegate d = new ShowStringMessageCallBack(ShowStringMessage);
                this.Invoke(d, s);
            }
            else
            {
                listBox1.Items.Add(s);
            }
        }

        public void DisplayGold(string s)
        {
            if (InvokeRequired)
            {
                Delegate d = new ShowStringMessageCallBack(DisplayGold);
                this.Invoke(d, s);
            }
            else
            {
                moezolotoLabel.Text = "MOE ZOLOTO : " + s + " AZAZA";
            }
        }

        public void DisplayGameState(int gold)
        {            
            if (InvokeRequired)
            {
                Delegate d = new ShowGoldCallBack(DisplayGameState);
                this.Invoke(d, gold);
            }
            else
            {
                moezolotoLabel.Text = String.Format("GAME STATE : MNOGO ZOLOTA BITCH ({0})",gold);
            }
        }

        public void DisplayBoard(List<Card> cards)
        {
            if (InvokeRequired)
            {
                Delegate d = new ShowCardsCallBack(DisplayBoard);
                this.Invoke(d, cards);
            }
            else
            {
                // !!!                                
                if (cards.Count() > card_boxes.Count())
                {
                    for (int i = 0; i < cards.Count() - card_boxes.Count(); i++)
                    {
                        PictureBox pbox = new PictureBox();
                        pbox.MouseDown += pbox_MouseDown;
                        pbox.MouseUp += pbox_MouseUp;                        
                        card_boxes.Add(pbox);                        
                    }
                }
                var groups = cards.OrderBy(card => card.Color).GroupBy(card => card.Color);
                int j = 0;
                int k = 0;
                boardContainer.Controls.SetChildIndex(wonderBox, 0);
                foreach (var group in groups)
                {
                    int i = 0;
                    foreach (var card in group)
                    {
                        PictureBox pbox = card_boxes[k];
                        pbox.Parent = boardContainer;
                        pbox.Left = wonderBox.Left - 44 * (i + 1) + 220 * j;
                        pbox.Top = wonderBox.Top - 80 * (i + 1);
                        pbox.Width = 208;
                        pbox.Height = 320;
                        string image_filename = image_folder + "/" + card.Name + ".jpg";
                        pbox.BackgroundImage = Image.FromFile(image_filename);
                        pbox.Tag = i + j * 10 + 1;
                        boardContainer.Controls.SetChildIndex(pbox, i + j * 10 + 1);                        
                        i++;
                        k++;
                    }
                    j++;
                }
                DrawTierBoxes();
            }
        }

        public void DisplayWonder(Wonder wonder)
        {
            if (InvokeRequired)
            {
                Delegate d = new DisplayWonderCallBack(DisplayWonder);
                this.Invoke(d,wonder);
            }
            else
            {
                string image_path = String.Format("D:/planchik/pics/Wonders/{0}.jpg", wonder.Id);
                wonderBox.BackgroundImage = (Image)new Bitmap(Image.FromFile(image_path), new Size(wonderBox.Width, wonderBox.Height));
            }
        }

        public void DisplayNewTier()
        {
            if (InvokeRequired)
            {
                Delegate d = new PromptPlayCallBack(DisplayNewTier);
                this.Invoke(d);
            }
            else
            {
                current_tier++;
                DrawTierBoxes();
            }
        }

        public void DrawTierBoxes()
        {
            if (current_tier > tier_boxes.Count())
            {
                for (int i = 0; i < current_tier - tier_boxes.Count(); i++)
                {
                    PictureBox pbox = new PictureBox();
                    tier_boxes.Add(pbox);
                }
            }
            for (int i = 0; i < tier_boxes.Count; i++)
            {
                tier_boxes[i].Parent = boardContainer;
                tier_boxes[i].Width = 208;
                tier_boxes[i].Height = 320;
                tier_boxes[i].BackgroundImage = Image.FromFile("D:/planchik/pics/Ore Vein.jpg");
                tier_boxes[i].Top = wonderBox.Top + 100;
                tier_boxes[i].Left = wonderBox.Left + 100 * (i + 1);
                tier_boxes[i].BringToFront();
            }
        }

        void pbox_MouseDown(object sender, MouseEventArgs e)
        {            
            PictureBox pbox = (PictureBox)sender;            
            pbox.BringToFront();            
        }

        void pbox_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pbox = (PictureBox)sender;            
            RefreshLayout();            
        }

        public void RefreshLayout()
        {
            wonderBox.BringToFront();
            foreach (PictureBox pbox in card_boxes)
            {
                boardContainer.Controls.SetChildIndex(pbox, (int)pbox.Tag);
            }
        }

        public void StartClient()
        {
            interpace = new ClientInterface(this);
            startclientButton.Enabled = false;            
        }
        

        private void startclientButton_Click(object sender, EventArgs e)
        {
            StartClient();
        }                               
    }
}
