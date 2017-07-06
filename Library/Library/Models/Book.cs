using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Book
    {
        private int _bookid;
        private string _title;
        private string _author;
        private int _pages;
        private string _genre;
        private bool _borrowed;
        private bool _reserved;

        #region Gets & Sets

        public int BookID
        {
            get { return _bookid; }
            set { _bookid = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public int Pages
        {
            get { return _pages; }
            set { _pages = value; }
        }

        public string Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        public bool Borrowed
        {
            get { return _borrowed; }
            set { _borrowed = value; }
        }

        public bool Reserved
        {
            get { return _reserved; }
            set { _reserved = value; }
        }

        #endregion
    }
}
