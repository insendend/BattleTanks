using System;
using System.Threading;

namespace hmTank.Classes
{
    // движение влево-вправо
    delegate void DelTextMoving(string text, int pos);

    // игра компьютера
    delegate void DelCompPlay();

    class Game
    {
        // размеры консоли
        public static readonly int iWidthMax, iHeigthMax;

        // объект блокировки
        private static readonly object locker = new object();

        // два объекта танка пользователь и компьютер
        private TankUser tankUser = new TankUser();
        private TankComp tankComp = new TankComp();

        // для компьютера (частота выстрела)
        private static Random rnd = new Random();

        // событие, оповещающее о пропуске заставки
        private static ManualResetEvent mreAll = new ManualResetEvent(false);

        // событие для оповещение, что пропуск заставки уже не нужен
        private static ManualResetEvent mreSingle = new ManualResetEvent(false);

        // событие конца игры
        public static ManualResetEvent mreEnd = new ManualResetEvent(false);



        // инциализация размеров экрана
        static Game()
        {
            Console.Title = "BATTLE TANKS";

            iWidthMax = 90;    // длина
            iHeigthMax = 30;   // высота

            Console.SetWindowSize(iWidthMax + 1, iHeigthMax + 1);
        }




        // запуск игры
        public void Run()
        {
            // заставка
            this.PreviewScreen();
            
            // экран после заставки
            this.StartScreen();

            // переодическая очистка консоли от "мусора"
            Game.AsyncConsoleClear();

            // игра компютера
            this.AsyncCompGameplay();

            // игра пользователя
            this.UserGamePlay();
        }




        // начальная заставка игры
        private void PreviewScreen()
        {
            //ожидание пропуска заставки
            Game.AsyncSkipping();

            // движение танка
            Game.TankMove();

            // выстрел снаряда на заставке
            Game.TankShooting();

            // отображение названия игры
            Game.ShowTitle();

            // отображение информации об игре
            Game.AsyncShowHelp();

            // меню игры
            Menu.ShowMenu();
        }

        // стартовый экран после заставки (отрисовка двух танков)
        private void StartScreen()
        {
            Console.Clear();
            this.tankUser.DrawTank();
            this.tankComp.DrawTank();
        }

        // пользовательское управление
        private void UserGamePlay()
        {
            while (true)
            {

                if (Game.mreEnd.WaitOne(1))
                    return;

                // сохраняем данные о нажатай кнопке
                ConsoleKeyInfo cki = Console.ReadKey(true);

                switch (cki.Key)
                {
                    // движение влево
                    case ConsoleKey.LeftArrow:
                        this.tankUser.AsyncMoveLeft();
                        TankUser.iTankLeftBorder--;
                        break;

                    // движение вправо
                    case ConsoleKey.RightArrow:
                        this.tankUser.AsyncMoveRight();
                        TankUser.iTankLeftBorder++;
                        break;

                    // выстрел
                    case ConsoleKey.Spacebar:
                        this.tankUser.AsyncShooting();
                        break;

                    default:
                        continue;
                }
            }
        }

        // игра компьютера
        private void CompGameplay()
        {
            while (true)
            {

                if (Game.mreEnd.WaitOne(1))
                    return;
                

                // следование движениям пользователя
                if (tankUser.iBarrelPos < tankComp.iBarrelPos)
                {
                    this.tankComp.AsyncMoveLeft();
                    TankComp.iTankLeftBorder--;
                }

                else if (tankUser.iBarrelPos > tankComp.iBarrelPos)
                {
                    this.tankComp.AsyncMoveRight();
                    TankComp.iTankLeftBorder++;
                }

                // задержка реакции компьютера
                Thread.Sleep(333);

                // выстрелы
                if (Game.rnd.Next(100) > 88)
                    this.tankComp.AsyncShooting();

            }

        }

        // отдельный поток под игру компьютера
        private void AsyncCompGameplay()
        {
            // настраиваем метод
            DelCompPlay dcp = this.CompGameplay;
            IAsyncResult res = dcp.BeginInvoke(null, null);
        }




        // ************** первый экран (заставка) и всё что на нём происходит ***********
        // отрисовка танка
        private static void TankDraw(int iCount)
        {
            Console.WriteLine(new string(' ', iCount) + "      __|__");
            Console.WriteLine(new string(' ', iCount) + "___ / ***** \\=======#");
            Console.WriteLine(new string(' ', iCount) + "|HH* T-34 *HH:\\");
            Console.WriteLine(new string(' ', iCount) + "(@=@=@=@=@=@=@=@)");
        }

        // движение танка к центру экрана
        private static void TankMove()
        {
            // количество пробелов перед танком
            int iCount = 0;

            // длина танка
            int iSizeTank = 17;

            // двигаем танк, пока не середина танка не окажется в центре консоли
            while (iCount != Console.WindowWidth / 2 - iSizeTank / 2)
            {
                // условие выхода из метода, когда установлено событие "пропуска заставки" в сигнальное положение
                if (Game.mreAll.WaitOne(0))
                    break;

                // позиция от которой будут отрисовываться новое число пробелов
                Console.SetCursorPosition(0, 0);

                // задержка движения
                Thread.Sleep(55);

                // рисуем танк
                Game.TankDraw(iCount);

                // сдвигаем танк (увеличиваем количество пробелов перед рисунком танка)
                iCount++;
            }
        }

        // выстрел танка
        private static void TankShooting()
        {
            // стартовая позиция выстрела (дуло танка)
            int iShotPos = iWidthMax / 2 + 12;

            Thread.Sleep(420);

            while (iShotPos != iWidthMax - 4)
            {

                // условие выхода из метода, когда установлено событие "пропуска заставки" в сигнальное положение
                if (Game.mreAll.WaitOne(0))
                    break;

                // установка курсора в позицию старта вылета снаряда
                Console.SetCursorPosition(iShotPos, 1);

                // задержка движения
                Thread.Sleep(40);

                // изменение символа "снаряда" во время движения
                if (iShotPos < 55)
                    Console.WriteLine(" *");
                else if (iShotPos < 65)
                    Console.WriteLine(" o");
                else if (iShotPos < 75)
                    Console.WriteLine(" O");
                else if (iShotPos < 80)
                    Console.WriteLine(" @");
                else if (iShotPos < 85)
                    Console.WriteLine(" Ж");
                else
                    Console.WriteLine(" BOOM!");

                // полёт снаряда
                iShotPos++;
            }

            // возврат в исходное положение курсора
            Console.WriteLine('\n');
        }

        // отображение название игры по центру
        private static void ShowTitle()
        {
            // длина надписи
            int iSizeOfTitle = 24;

            Console.WriteLine("\n" + new string(' ', iWidthMax / 2 - iSizeOfTitle / 2) + "************************");
            Console.WriteLine(new string(' ', Console.WindowWidth / 2 - iSizeOfTitle / 2) + "***** BATTLE TANKS *****");
            Console.WriteLine(new string(' ', Console.WindowWidth / 2 - iSizeOfTitle / 2) + "************************\n");
        }

        // информации об игре (HELP)
        private static void AsyncShowHelp()
        {
            // текст, который будет выезжать
            string strHelp1 = "Игра БОЙ ТАНКОВ представляет из себя: ";
            string strHelp2 = "  - два танка, расположенных друг напротив друга ";
            string strHelp3 = "  - движения совершаются паралельно друг другу ";
            string strHelp4 = "  - каждый из танков имеет возможность стрелять перед собой ";
            string strHelp5 = "  - выигрывает тот, кто нанесет большее кол-во урона оппоненту ";

            // настраиваем метод
            DelTextMoving dtm = Game.TextMoving;

            // начальная позиция номера строки вывода текста
            int iStartPos = 9;

            // асинхронные вызовы строк HELP'a
            IAsyncResult text1 = dtm.BeginInvoke(strHelp1, iStartPos, Finish, dtm);
            IAsyncResult text2 = dtm.BeginInvoke(strHelp2, ++iStartPos, Finish, dtm);
            IAsyncResult text3 = dtm.BeginInvoke(strHelp3, ++iStartPos, Finish, dtm);
            IAsyncResult text4 = dtm.BeginInvoke(strHelp4, ++iStartPos, Finish, dtm);
            IAsyncResult text5 = dtm.BeginInvoke(strHelp5, ++iStartPos, Finish, dtm);

            // дожидаемся конца движения текста
            WaitHandle.WaitAll(
                new[] {
                text1.AsyncWaitHandle,
                text2.AsyncWaitHandle,
                text3.AsyncWaitHandle,
                text4.AsyncWaitHandle,
                text5.AsyncWaitHandle,

                });

            // пропуск заставки уже не актуален, и уже ожидание не требуется
            Game.mreSingle.Set();

            Console.WriteLine("\n\n\n\n");
        }

        // движение текста HELP
        private static void TextMoving(string text, int iYpos)
        {
            // количество пробелов перед текстом
            int iCount = iWidthMax;

            while (iCount != 0)
            {
                // критическая секция
                lock (locker)
                {
                    // условие выхода из метода, когда установлено событие "пропуска заставки" в сигнальное положение
                    if (Game.mreAll.WaitOne(0))
                        break;

                    // позиция от которой будут отрисовываться новое число пробелов
                    Console.SetCursorPosition(0, iYpos);

                    // задержка движения
                    Thread.Sleep(5);

                    // вывод строки с определенным кол-вом сдвига пробелами
                    Console.WriteLine(new string(' ', iCount) + text);

                    // сдвигаем текст
                    iCount--;
                }
            }
        }

        // экран после пропуска заставки
        private static void FullScreenText()
        {
            Console.Clear();

            // длина танка
            int iSizeTank = 17;
            int iCenter = Console.WindowWidth / 2 - iSizeTank / 2;

            // отображение танка
            Console.WriteLine(new string(' ', iCenter) + "      __|__");
            Console.Write(new string(' ', iCenter) + "___ / ***** \\=======#");
            Console.SetCursorPosition(iWidthMax - 4, 1);
            Console.WriteLine("BOOM!");
            Console.WriteLine(new string(' ', iCenter) + "|HH* T-34 *HH:\\");
            Console.WriteLine(new string(' ', iCenter) + "(@=@=@=@=@=@=@=@)");

            // длина надписи названия игры
            int iSizeOfTitle = 24;

            // название игры
            Console.WriteLine("\n" + new string(' ', iWidthMax / 2 - iSizeOfTitle / 2) + "************************");
            Console.WriteLine(new string(' ', Console.WindowWidth / 2 - iSizeOfTitle / 2) + "***** BATTLE TANKS *****");
            Console.WriteLine(new string(' ', Console.WindowWidth / 2 - iSizeOfTitle / 2) + "************************\n\n\n");

            // HELP
            Console.WriteLine("Игра БОЙ ТАНКОВ представляет из себя:\n" +
                "  - два танка, расположенных друг напротив друга\n" +
                "  - движения совершаются паралельно друг другу\n" +
                "  - каждый из танков имеет возможность стрелять перед собой\n" +
                "  - выигрывает тот, кто нанесет большее кол-во урона оппоненту "
                );
        }

        // ожидание пропуска заставки
        private static void WaitForSkip()
        {
            // информационное сообщение
            string strText = "Нажмите 'ANY_KEY', чтобы пропустить";

            // вывод в центре консоли снизу информационное сообщение
            Console.SetCursorPosition(Game.iWidthMax / 2 - strText.Length / 2, Game.iHeigthMax - 1);
            Console.Write(strText);
            Console.SetCursorPosition(0, 0);

            // проверка состояния "ожидается ли нажатие кнопки"
            while (!Console.KeyAvailable)
            {
                // проверка на установленное событие "конца заставки"
                if (Game.mreSingle.WaitOne(1))
                {
                    // заставка уже закончилась и пропуск не актуален
                    Game.mreSingle.Reset();
                    Game.mreSingle = null;
                    return;
                }
            }

            // кнопка нажата во время заставки
            // устанавливаем событие для всех потоков
            Game.mreAll.Set();

            // оторбажаем весь экран заставки
            Thread.Sleep(1000);
            Game.FullScreenText();

            // завершаем событие
            Game.mreAll.Reset();
            Game.mreAll = null;
        }

        // поток ожидания пропуска заставки
        private static void AsyncSkipping()
        {
            Action act = Game.WaitForSkip;
            act.BeginInvoke(null, null);
        }
        // ************** *************** ************* ********** **************




        // ************** переодичная очистка консоли **************
        // очистка экрана
        private static void Clear()
        {
            while (true)
            {
                Console.SetCursorPosition(0, 2);
                for (int i = 0; i < Game.iHeigthMax - 4 && !Game.mreEnd.WaitOne(0); i++)
                    Console.WriteLine(new string(' ', Game.iWidthMax));
                Thread.Sleep(1000);
            }
        }

        // поток для переодической очистки экрана
        private static void AsyncConsoleClear()
        {
            Action act = Game.Clear;
            act.BeginInvoke(null, null);
        }
        // ************** *************** ************* **************





        // проверка на конец игры
        private static void CheckForEnd()
        {
            if (TankComp.iArmor <= 0 || TankUser.iArmor <= 0)
                mreEnd.Set();
        }




        // вывод на экран победителя
        public static void LastScreen(bool isUser)
        {
            Thread.Sleep(1000);
            Console.Clear();

            // вывод по центру
            Console.SetCursorPosition(Game.iWidthMax / 2 - 5, 5);
            Console.WriteLine( isUser ? "User WON" : "Computer WON" );
        }

        // завершение асинхронного вызова
        private static void Finish(IAsyncResult result)
        {
            try
            {
                DelTextMoving dtm = (DelTextMoving)result.AsyncState;
                dtm.EndInvoke(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}