using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PW_Prom
{
    class Samochod
    {
        int x, y;
        bool naPromie;
        public Thread watek;
        Random random;
        private List<SemaphoreSlim> kolejkaSamochodow;
        private List<SemaphoreSlim> samochodyNaPromie;
        private Prom prom;

        public Samochod(int x, int y, Random random, List<SemaphoreSlim> kolejkaSamochodow, List<SemaphoreSlim> samochodyNaPromie, Prom prom)
        {
            this.x = x;
            this.y = y;
            this.random = random;
            this.naPromie = false;
            this.kolejkaSamochodow = kolejkaSamochodow;
            this.samochodyNaPromie = samochodyNaPromie;
            this.prom = prom;

            watek = new Thread(this.skorzystajzPromu)
            {
                IsBackground = true
            };
            watek.Start();
        }

        public void skorzystajzPromu()
        {
            //zajmowanie ostatniego miejsca w kolejce
            kolejkaSamochodow[0].Wait();
            //pzesuwanie kolejki
            for (int i = 1; i < kolejkaSamochodow.Count; i++)
            {
                kolejkaSamochodow[i].Wait();
                kolejkaSamochodow[i - 1].Release();
                ruchY(this.y + 30);
            }
            
            //jeśli prom odpłynął lub upływa czas oczekiwania, samochód ma czekać
            while (prom.stan != 0 || prom.timeout - prom.timer.current < 1)
            {
                prom.naDrugiBrzeg();
                Thread.Sleep(100);
            }

            //podjazd do brzegu i wjazd na prom
            prom.naDrugiBrzeg();
            ruchY(prom.y + 30);
            ruchX(prom.x - 20); 
            naPromie = true;
            Miejsce miejsce = prom.obslugaWjazdu();
            kolejkaSamochodow[kolejkaSamochodow.Count - 1].Release();
            
            //jeśli samochod jest na promie, ma czekać aż dopłynie na drugi brzeg
            while (prom.stan != 2)
            {
                prom.naDrugiBrzeg();
                Thread.Sleep(100);
            }

            //przesuwanie kolejki do wyjazdu
            for (int i = 1; i < samochodyNaPromie.Count; i++)
            {
                samochodyNaPromie[i].Wait();
                samochodyNaPromie[i - 1].Release();
            }

            //podjazd do wyjazdu i wyjazd z promu
            x = prom.x + 145;
            y = prom.y + 30;
            Thread.Sleep(500);
            naPromie = false; 
            ruchX(x + 30);
            prom.obslugaWyjazdu(miejsce);
            samochodyNaPromie[samochodyNaPromie.Count - 1].Release();
            ruchY(-30);
        }

        public void ruchX(int cel)
        {
            while (cel != this.x)
            {
                this.x += kierunek(cel, this.x);
                Thread.Sleep(40);
            }
        }

        public void ruchY(int cel)
        {
            while (cel != this.y)
            {
                this.y += kierunek(cel, this.y);
                Thread.Sleep(40);
            }
        }

        public int kierunek(int cel, int start)
        {
            int krok = 10;

            if (cel > start)
            {
                if (cel > start + krok)
                {
                    return krok;
                }
                else
                {
                    return cel - start;
                }
            }
            else
            {
                if (cel < start - krok)
                {
                    return -krok;
                }
                else
                {
                    return cel - start;
                }
            }
        }

        public void Paint(Graphics g)
        {
            Brush brush = null;
            brush = new SolidBrush(Color.Red);
            if(naPromie == false)
            {
                g.FillRectangle(brush, x, y, 20, 20);
                g.DrawRectangle(new Pen(Color.Black, 2), x, y, 20, 20);
            }
        }

    }
}
