using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PW_Prom
{
    class MyTimer
    {
        public int start = 0;
        public int current;
        public Timer oTimer;

        public void StartTimer()
        {
            start = Environment.TickCount;
            oTimer = new Timer(
                FunkcjaWatka,                   //Funkcja wątka typu TimerCallback(void(object))
                null,                           //parametr przekazywany do funkcji jako argument (state)
                0,                              //opóźnienie (czas do pierwszego uruchomienia metody)
                1000);                          //co ile milisekund będzie uruchamiana metoda (FunkcjaWatka)
        }

        public void StopTimer()
        {
            oTimer.Dispose();
        }

        private void FunkcjaWatka(object state)
        {
            current = (Environment.TickCount - start)/1000;
        }
    }
}
