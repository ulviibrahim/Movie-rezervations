using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;
using System.Data.SqlClient;


namespace ReservationMovie
{
    public partial class Form1 : Form

    {
        string connectionstring = @"Data Source = .\SQLEXPRESS; Initial Catalog = MoveSeats;Integrated Security=True";
        SqlConnection conn;
         private Hall Hall = new Hall();
        private List<Button> SelectButton = new List<Button>();
        private List<Button> Booked = new List<Button>();
        Dictionary<string, Button> btns = new Dictionary<string, Button>();

        private int Price = 5;
        public Form1()
        {
            conn = new SqlConnection(connectionstring);

             InitializeComponent();
            AddtoHall();
            GenrateBtn();
        }
        private void AddtoHall()
        {
            Hall.SeatCount = 30;
            Hall.Column = 5;
        }

        private  void GenrateBtn()
        {
            int x = 5;
            int y = 20;
            for (int i = 0; i < Hall.SeatCount; i++)
            {
                Button btn = new Button();
                btn.Size = new Size(40, 40);
                btn.Text = (i + 1).ToString();
                btns[btn.Text] = btn;
                btn.Click += new EventHandler( this.selectBtn_click);
                btn.Cursor = Cursors.Hand;
                btn.BackColor = Color.Yellow;
                conn.Open();
                string dbSelectquery = $"select * from  Seats  where ChooseBool=1 and id='{btn.Text}'";
                var Selectcommand = new SqlCommand(dbSelectquery, conn);
                var Selectres = Selectcommand.ExecuteReader();

                Selectres.Read();
                //btns[Selectres.GetValue(0).ToString()].te

                    if(Selectres.HasRows && btn.Text == Selectres.GetValue(0).ToString())
                    {
                        this.Booked.Add(btn);
                        foreach (var item in Booked)
                        {
                            item.BackColor = Color.Red;
                            item.Cursor = Cursors.No;
                        }
                    }
                    else
                    {
                        btn.BackColor = Color.Yellow;
                    }
                
                conn.Close();


                btn.Location = new Point(x, y);
                x += 60;
                PnlSeats.Controls.Add(btn);
                if ((i+1) % Hall.Column == 0)
                {
                    x = 5;
                    y += 60;
                }

            }
            
        }
      
       
        private void selectBtn_click(object sender,EventArgs e)
        {
            var btn = sender as Button;
            if (btn.BackColor==Color.Yellow)
            {
                btn.BackColor = Color.Lime;
                
                this.SelectButton.Add(btn);

                

            }
            else if (btn.BackColor == Color.Lime)
            {
                btn.BackColor = Color.Yellow;
                this.SelectButton.Remove(btn);
            }
            else
            {
                btn.Click -= new EventHandler( selectBtn_click);
            }
            SelectPlaces();
            TotalPrice();
        }
        private void TotalPrice()
        {
            LblSum.Text = (this.SelectButton.Count*this.Price) + " azn";
            if (LblSum.Text=="0 azn")
            {
                LblSum.Text = "";
            }
            
        }

        private void SelectPlaces()
        {
            List<string> Places = new List<string>();
            foreach (var item in this.SelectButton)
            {
                Places.Add(item.Text);
            }

            LblSelectedPl.Text = string.Join(",", Places.ToArray());
        }

        private void BtnBooking_Click(object sender, EventArgs e)
        {
            if (SelectButton.Count<1)
            {
                MessageBox.Show("Evvelce Yerinizi secin");
            }
            else
            {
                DialogResult result = MessageBox.Show("Seçilmiş yerləri təsdiqləməyə əminsiniz?", "Bilet satışı", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    foreach (Button btn in SelectButton)
                    {
                        conn.Open();
                        string dbquery = $"update  Seats  set ChooseBool=1 where SelectedSeats= {btn.Text}";
                        var command = new SqlCommand(dbquery, conn);
                        var res = command.ExecuteNonQuery();
                        conn.Close();

                        btn.Cursor = Cursors.No;
                        btn.BackColor = Color.Red;
                    }

                    SelectButton.Clear();
                    LblSelectedPl.Text = "";
                    LblSum.Text = "";
                }
            }
        }

        private void BtnBooking_MouseHover(object sender, EventArgs e)
        {
            BtnBooking.BackColor = Color.Red;
        }
       
        private void BtnBooking_MouseLeave(object sender, EventArgs e)
        {
            BtnBooking.BackColor = Color.Green;
        }
    }
}
