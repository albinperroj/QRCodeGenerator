
using System;

namespace QRCodeGenerator.Models
{
    public class MyException : Exception
    {
        public string Msg { get; set; }
        public int Code { get; set; }
    }
}
