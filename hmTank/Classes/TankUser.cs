using System;
using System.Threading;

namespace hmTank.Classes
{
    class TankUser : Tank
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
                // ТАНК пользователя
                Console.SetCursorPosition(0, Game.iHeigthMax - 2);
                
                // цвет отрисовки при разном уровне здоровья танка
                if (TankUser.iArmor > 8)
                    Console.ForegroundColor = ConsoleColor.Green;
                else if (TankUser.iArmor < 9 && TankUser.iArmor > 4)
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else
                    Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(new string(' ', this.iSpaces + 3) + "  [ ]  ");               
                Console.WriteLine(new string(' ', this.iSpaces) + "  [ ][ ][ ]  ");
                Console.ResetColor();
            }
        }

        // выстрел танка
        public override void Shoot(int pos)
        {
            int iStart = Game.iHeigthMax - 3;

            // полет вверх
            while (iStart != 1)
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
                    Console.SetCursorPosition(pos, --iStart);

                    // новое положение снаряда
                    Console.Write('o');
                }
            }

            // проверка на попадение по танку оппонента
            if (pos >= TankComp.iTankLeftBorder && pos <= TankComp.iTankLeftBorder + Tank.iTankLength)
            {
                // отображение символа попадания
                Console.SetCursorPosition(pos, iStart);
                Console.Write('Ж');

                // уменьшаем кол-во здоровья
                TankComp.iArmor -= new Random().Next(2,4);

                if (TankComp.iArmor <= 0)
                {
                    // установка в сигнальное положение события "конца игры"
                    Game.mreEnd.Set();

                    // отображение победителя
                    Game.LastScreen(true);
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