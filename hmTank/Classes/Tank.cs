using System;

namespace hmTank.Classes
{
    // под движения влево-вправо
    delegate void DelMoveLeftRight();

    abstract class Tank
    {
        // длина танка
        public static readonly int iTankLength = 9;

        // количество пробелов для отрисовки
        protected int iSpaces;

        // поцизия дула танка
        public int iBarrelPos { get; set; }

        // объект блокировки
        protected readonly object locker = new object();



        // инициализация танка
        public Tank()
        {
            // центр экрана
            this.iSpaces = (Game.iWidthMax + 1) / 2 - (Tank.iTankLength + 1) / 2;

            // позиция дула танка в центре
            this.iBarrelPos = Game.iWidthMax / 2 + 1;
        }




        // движение влево
        protected void MoveLeft()
        {
            if (this.iSpaces > 0)
            {
                // уменьшаем кол-во пробелов перед рисованием танка
                // пока не упрёмся в левую границу ( 0 пробелов )
                this.iSpaces--;

                // и позицию дула
                iBarrelPos--;

                // перерисовываем танк
                DrawTank();
            }
        }

        // движение вправо
        protected void MoveRight()
        {
            if (this.iSpaces < Game.iWidthMax - 10)
            {
                // увеличиваем кол-во пробелов перед рисованием танка
                // пока не упрёмся в правую границу ( iWidthMax-10 пробелов )
                this.iSpaces++;

                // и позицию дула
                iBarrelPos++;

                // перерисовываем танк
                DrawTank();
            }
        }



        // рисуем танк
        public abstract void DrawTank();

        // выстрел танка
        public abstract void Shoot(int pos);



        // поток под движение влево
        public void AsyncMoveLeft()
        {
            // настраиваем метод
            DelMoveLeftRight dmlr = this.MoveLeft;
            IAsyncResult res = dmlr.BeginInvoke(null, null);
        }

        // поток под движение вправо
        public void AsyncMoveRight()
        {
            // настраиваем метод
            DelMoveLeftRight dmlr = this.MoveRight;
            IAsyncResult res = dmlr.BeginInvoke(null, null);
        }

        // поток для выстрела
        public void AsyncShooting()
        {
            Action<int> a = Shoot;
            a.BeginInvoke(iBarrelPos, null, null);
        }
    }
}