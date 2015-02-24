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
        Graphics gr;
        Bitmap bmp;
        Client client;
        ClientInterface interpace;
        List<Card> current_booner;
        // !!!
        string current_playmode;
        string image_folder = "D:/planchik/pics";
        delegate void ShowStringMessageCallBack(string str);
        delegate void ShowCardsCallBack(List<Card> cards);
        delegate void PromptPlayCallBack();                

        public Form1()
        {
            InitializeComponent();
            //gr = .CreateGraphics();                   
            //pictureBox1.CreateGraphics();
            bmp = new Bitmap(wonderBox.Width, wonderBox.Height);
            gr = Graphics.FromImage(bmp);
            wonderBox.BackgroundImage = bmp;
            current_booner = new List<Card>();
            card_boxes = new List<PictureBox>();
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
                //pickcardButton.Enabled = true;
                //sellcardButton.Enabled = false;
                
                
                ShowStringMessage("Cards in booster: " + cards.Count);
                //listBox1.Items.Add("Cards in booster: " + cards.Count);
                Image image;
                gr.Clear(Color.White);
                // !!!
                image = Image.FromFile(image_folder + "/" + "Rhodos.jpg");
                gr.DrawImage(image, 0, 0);
                //string str = String.Join(",", cards.Select(x => x.Name));
                //gr.DrawString(str, new Font(FontFamily.GenericSerif, 20), new SolidBrush(Color.Black), 5, 5);
                int i = 0;
                current_booner.Clear();
                foreach (Card card in cards)
                {
                    current_booner.Add(card);
                    
                    // !!!
                    string image_filename = image_folder + "/" + card.Name + ".jpg";
                    if (System.IO.File.Exists(image_filename))
                    {                        
                        image = Image.FromFile(image_filename);
                    }
                    else
                    {
                        image = Image.FromFile("D:/planchik/pics/missing_card.jpg");
                    }
                    gr.DrawImage(image, 208 * i, 100);
                    i++;
                }
                //pictureBox1.BackgroundImage = bmp;
                wonderBox.Invalidate();
            }
        }



        public void PromptPlayMode()
        {
            if (InvokeRequired)
            {
                Delegate d = new PromptPlayCallBack(PromptPlayMode);
                this.Invoke(d);
            }
            else
            {
                GameCommand command = new GameCommand("PlayMode", current_playmode);
                interpace.Send(command);
                //headlineLabel.Text = "PLAY THE CARD BITCH";
                //pickcardButton.Enabled = false;
                //sellcardButton.Enabled = true;                
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
                        card_boxes.Add(new PictureBox());
                    }
                }
                var groups = cards.OrderBy(card => card.Color).GroupBy(card => card.Color);                
                int j = 0;
                int k = 0;
                foreach (var group in groups)
                {
                    int i=0;
                    foreach(var card in group)
                    {                                                
                        PictureBox pbox = card_boxes[k];
                        pbox.Parent = boardContainer;
                        pbox.Left = wonderBox.Left - 44 * (i + 1) + 220 * j;
                        pbox.Top = wonderBox.Top - 80 * (i + 1);
                        pbox.Width = 208;
                        pbox.Height = 320;
                        string image_filename = image_folder + "/" + card.Name + ".jpg";
                        pbox.BackgroundImage = Image.FromFile(image_filename);
                        pbox.SendToBack();
                        i++;
                        k++;
                    }
                    j++;
                }            
            }
        }

        private void startclientButton_Click(object sender, EventArgs e)
        {
            client = new Client();
            interpace = new ClientInterface();
            client.client_interface = interpace;
            interpace.client = client;
            interpace.gui = this;
            client.StartClient();
            startclientButton.Enabled = false;
        }

        /*
        private void pickcardButton_Click(object sender, EventArgs e)
        {
            GameCommand command = new GameCommand("CardPick", current_booner.First().Id.ToString());
            interpace.Send(command);
        }
         */

        private void sellcardButton_Click(object sender, EventArgs e)
        {
            current_playmode = "Sell";
            GameCommand command = new GameCommand("CardPick", current_booner.First().Id.ToString());
            interpace.Send(command);
            //GameCommand command = new GameCommand("PlayMode", "Sell");
            //interpace.Send(command);
        }

        private void playcardButton_Click(object sender, EventArgs e)
        {
            current_playmode = "Play";
            GameCommand command = new GameCommand("CardPick", current_booner.First().Id.ToString());
            interpace.Send(command);
            //GameCommand command = new GameCommand("PlayMode", "Play");
            //interpace.Send(command);
        }               
    }
}
