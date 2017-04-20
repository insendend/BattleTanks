using System;
using System.Threading;

namespace hmTank.Classes
{
    class TankComp : Tank
    {
        // свойство получение координаты левой границы танка
        public static int iTankLeftBorder { get; set; } = (Game.iWidthMax + 1) / 2 - (Tank.iTankLength + 1) / 2 + 2;

        // здоровье танка
        public static int iArmor { get; set; } = 12;




        // отрисовка танка
        public override void DrawTank()
        {
            lock (locker)
            {
                // ТАНК компьютера
                Console.SetCursorPosition(0, 0);

                // цвет отрисовки при разном уровне здоровья танка
                if (TankComp.iArmor > 8)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (TankComp.iArmor < 9 && TankComp.iArmor > 4)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(new string(' ', this.iSpaces) + "  [ ][ ][ ]  ");
                Console.WriteLine(new string(' ', this.iSpaces + 3) + "  [ ]  ");
                Console.ResetColor();
            }
        }

        // выстрел танка
        public override void Shoot(int pos)
        {
            // позиция первой отрисовки пули (сверху)
            int iStart = 3;

            // полет вниз
            while (iStart != Game.iHeigthMax - 2)
            {
                // критическая секция
                lock (locker)
                {
                    // прерывание работы метода при установки в сигнальное положение события "конец игры"
                    if (Game.mreEnd.WaitOne(0))
                        return;

                    Thread.Sleep(55);
                    Console.SetCursorPosition(pos, iStart);

                    // затираем предыдущее положение снаряда
                    Console.Write(' ');

                    // поднимаем курсор выше
                    Console.SetCursorPosition(pos, ++iStart);

                    // новое положение снаряда
                    Console.Write('o');

                    Thread.Sleep(20);
                }
            }

            // проверка на попадение по танку оппонента
            if (pos >= TankUser.iTankLeftBorder && pos <= TankUser.iTankLeftBorder + Tank.iTankLength)
            {
                // отображение символа попадания
                Console.SetCursorPosition(pos, iStart);
                Console.Write('Ж');
                
                // уменьшаем кол-во здоровья
                TankUser.iArmor -= new Random().Next(2,4);

                // проверка на критическое здоровье
                if (TankUser.iArmor <= 0)
                {
                    // установка в сигнальное положение события "конца игры"
                    Game.mreEnd.Set();
                   
                    // отображение победителя
                    Game.LastScreen(false);
                }
            }

            lock (locker)
            {
                // затираем последнее положение снаряда
                Thread.Sleep(50);
                Console.Write(' ');
            }

        }
    }
}