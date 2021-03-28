using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PW_Prom
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        //lista samochodow i lista miejsc
        List<Samochod> samochody = new List<Samochod>();
        List<Miejsce> miejsca = new List<Miejsce>();
        Prom prom;

        //kolejka do promu i kolejka do wyjazdu z promu
        List<SemaphoreSlim> kolejkaSamochodow = new List<SemaphoreSlim>();
        List<SemaphoreSlim> samochodyNaPromie = new List<SemaphoreSlim>();
        Random random = new Random();



        public Form1()
        {
            InitializeComponent();

            //tworzenie miejsc na promie
            for(int i = 0; i < 10; i++)
            {
                miejsca.Add(new Miejsce(prom, i));
                samochodyNaPromie.Add(new SemaphoreSlim(1));
            }

            //tworzenie kolejki samochodow
            for (int i = 0; i < 8; i++)
            {
                kolejkaSamochodow.Add(new SemaphoreSlim(1));
            }
            //tworzenie promu
            prom = new Prom(160, 220, 10, 15, miejsca);

            timer.Interval = 40; // przerysowanie co 40 ms
            timer.Tick += new EventHandler(repaintTimer);
            timer.Start();
        }

        private void repaintTimer(object sender, EventArgs e)
        {
            //generowanie samochodow do przeprawy przez rzeke
            if (random.Next(300) < 5 && samochody.Count < 25)
            {
                samochody.Add(new Samochod(40, -20, random, kolejkaSamochodow, samochodyNaPromie, prom));
            }

            //usuwanie samochodow, ktore juz przejechaly przez rzeke
            samochody.RemoveAll(x => x.watek.IsAlive == false);

            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //rysowanie symulacji
            e.Graphics.FillRectangle(new SolidBrush(Color.SkyBlue), 160, 0, 530, 600);
            prom.Paint(e.Graphics);
            e.Graphics.DrawString("Oczekujacy: " + (samochody.Count - prom.pojemnosc + prom.wolneMiejsca), new Font("Arial", 12), new SolidBrush(Color.Black), 0, 500);
            samochody.ForEach(x => x.Paint(e.Graphics));
        }
    }
}
