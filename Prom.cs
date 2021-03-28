using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PW_Prom
{
    class Prom
    {
        public int x;
        public int y;
        public int pojemnosc; 
        
        //stan: 0 - lewy brzeg, 1 - plynie, 2 - prawy brzeg
        public int stan;
        public int wolneMiejsca; 
        
        //lista miejsc na promie
        public List<Miejsce> miejsca = new List<Miejsce>();
        
        //prog cierpliwosci
        public int timeout; 
        public MyTimer timer = new MyTimer();
        
        //semafor odpowiedzialny za liczbe wolnych miejsc na promie
        SemaphoreSlim liczbaMiejsc = new SemaphoreSlim(0);

        //semafor odpowiedzialny za pojedynczy wjazd na prom/zjazd z promu
        SemaphoreSlim dostepDoPromu = new SemaphoreSlim(1);

        public Prom(int x, int y, int pojemnosc, int timeout, List<Miejsce> miejsca)
        {
            this.x = x;
            this.y = y;
            this.pojemnosc = pojemnosc;
            this.wolneMiejsca = pojemnosc;
            this.stan = 0;
            this.timeout = timeout;
            timer.StartTimer();
            this.miejsca.AddRange(miejsca);
            liczbaMiejsc.Release(miejsca.Count);
        }
        //wjazd na prom
        public Miejsce obslugaWjazdu()
        {
            //zajmowane miejsce
            Miejsce miejsce = null;
            //zmniejszenie liczby wolnych miejsc
            liczbaMiejsc.Wait();
            //blokada dostepu do wjazdu
            dostepDoPromu.Wait();
            //zajęcie miejsca
            miejsce = this.miejsca[0];
            this.miejsca.RemoveAt(0);
            wolneMiejsca--;
            //odblokowanie dostepu do wjazdu
            dostepDoPromu.Release();

            naDrugiBrzeg();

            return miejsce;
        }

        //wyjazd z promu
        public void obslugaWyjazdu(Miejsce miejsce)
        {
            //blokada dostepu do wyjazdu
            dostepDoPromu.Wait();
            //zwolnienie miejsca
            miejsca.Add(miejsce);
            //zwiekszenie liczby wolnych miejsc
            liczbaMiejsc.Release();
            wolneMiejsca++;
            //odblokowanie dostepu do wyjazdu
            dostepDoPromu.Release();
        }

        public void naDrugiBrzeg()
        {
            if((stan == 0 && timeout - timer.current < 1 && wolneMiejsca < pojemnosc) || (stan == 0 && wolneMiejsca == 0))
            {
                timer.StopTimer();
                stan = 1;
                Thread.Sleep(500);
                ruchX(550);
                if(this.x == 550)
                {
                    stan = 2;
                }
            }
            else if(stan == 0 && timeout - timer.current < 1 && wolneMiejsca == pojemnosc)
            {
                timer.StopTimer();
                timer.StartTimer();
            }
            else if(stan == 2 && wolneMiejsca == pojemnosc)
            {
                stan = 1;
                ruchX(160);
                if (this.x == 160)
                {
                    stan = 0;
                    timer.StartTimer();
                }
            }
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
            int krok = 5;

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
            //g.DrawString("stan: " + stan, new Font("Arial", 12), new SolidBrush(Color.Black), 0, 420);
            int temp = 0;
            g.FillRectangle(new SolidBrush(Color.Gray), x, y, 140, 80);
            g.DrawRectangle(new Pen(Color.Black, 2), x, y, 140, 80);

            g.DrawString(pojemnosc - wolneMiejsca + "/" + pojemnosc, new Font("Arial", 12), new SolidBrush(Color.Black), x + 55, y + 28);
            
            if (timeout - timer.current <= 0) temp = 0;
            else temp = timeout - timer.current;

            g.DrawString("Odpływa za: " + temp, new Font("Arial", 12), new SolidBrush(Color.Black), x + 10, y + 40);

        }
    }
}