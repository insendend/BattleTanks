using System;
using System.Runtime.Serialization;

namespace hmTank.Classes
{
    [Serializable]
    class MyExcep : ApplicationException
    {
        // конструктор по умолчанию
        public MyExcep() { }

        // конструтор с аварийным сообщением
        public MyExcep(string strMsg) : base(strMsg) { }

        // конструктор для обработки "внутренних" исключений
        public MyExcep(string strMsg, Exception ex) : base(strMsg, ex) { }

        // защищенный конструктор для сериализации
        protected MyExcep(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}