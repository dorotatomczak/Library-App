using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
   public class Reader
    {
        private string _login;
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        private int _readBooksAmount;
        public int ReadBooksAmount
        {
            get { return _readBooksAmount; }
            set { _readBooksAmount = value; }
        }
    }
}
