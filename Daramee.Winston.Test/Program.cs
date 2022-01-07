using Daramee.Winston.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.Winston.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FileDialog ofd = new OpenFolderDialog()
            {
                //Filter = "이미지 파일(*.bmp;*.jpg;*.jpeg;*.png)|*.bmp;*.jpg;*.jpeg;*.png",
            };
            if (ofd.ShowDialog() == true)
            {
                Console.WriteLine(ofd.FileName);
            }
        }
    }
}