using System;
using System.Threading;

namespace hmTank.Classes
{
    static class Menu
    {
        // позиции от верхней границы консоли
        private static int iPosY, iMinY, iMaxY;

        // пункты меню
        private static readonly string strStart = "Начать игру";
        private static readonly string strExit = "Выход";

        // метка для выхода из асинхроного вызова
        private static bool isExit;

        // объект блокировки
        private static object locker = new object();



        // инициализация полей и запуск асинхронного метода
        static Menu()
        {
            // позиция курсора сверху в данный момент
            iPosY = Console.CursorTop + 2;
            
            // границы пунктов меню
            iMinY = iPosY;
            iMaxY = iPosY + 1;

            // запуск потока для мигания пунктов меню
            Action act = Blinking;
            act.BeginInvoke(null, null);
        }



        // выбор пункта меню
        public static void ShowMenu()
        {
            // данные о нажатой кнопки
            ConsoleKeyInfo cki = new ConsoleKeyInfo();

            // пока не нажмем 'ENTER'
            while (cki.Key != ConsoleKey.Enter)
            {
                cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    // стрелка вверх
                    case ConsoleKey.UpArrow:
                        lock(locker)
                        {
                            if (iPosY > iMinY)
                                iPosY--;
                        }
                        break;

                    // стрелка вниз
                    case ConsoleKey.DownArrow:
                        lock (locker)
                        {
                            if (iPosY < iMaxY)
                                iPosY++;
                        }
                        break;
                   
                    default:
                        continue;

                }
            }   // end while()

            // проверка на конкретный пункт меню
            if (iPosY == iMaxY)
                // выход
                throw new MyExcep("Спасибо за внимание!");

            // флаг завершения мигания пунктов меню и продолжение выполения программы
            isExit = true;
        }



        // мигание строк
        private static void Blinking()
        {
            while (true)
            {
                // вывод в центре консоли пунктов меню
                Console.SetCursorPosition(Game.iWidthMax / 2 - Menu.strStart.Length / 2, iMinY);
                Console.WriteLine(strStart);
                Console.SetCursorPosition(Game.iWidthMax / 2 - Menu.strExit.Length / 2, iMinY + 1);
                Console.WriteLine(strExit);

                Thread.Sleep(222);

                // затирание строки
                Console.SetCursorPosition(Game.iWidthMax / 2 - Menu.strStart.Length / 2, iPosY);
                Console.WriteLine(new string(' ', strStart.Length));

                Thread.Sleep(150);

                // флаг для выхода из метода
                if (isExit)
                    break;
            }
        }
    }
}