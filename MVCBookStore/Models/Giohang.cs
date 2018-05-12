using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCBookStore.Models
{
    public class Giohang
    {
        //Tao doi tuong data chua du lieu tu model dbBansach da tao
        dbBookStoreDataContext data = new dbBookStoreDataContext();
        public int iMasach { set; get; }
        public string sTensach { set; get; }
        public string sAnhbia { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }

        //Khoi tao gio hang theo Masach duoc truyen vao voi Soluong mac dinh la 1

        public Giohang(int Masach)
        {
            iMasach = Masach;
            SACH sach = data.SACHes.Single(n => n.Masach == iMasach);
            sTensach = sach.Tensach;
            sAnhbia = sach.Hinhminhhoa;
            dDongia = double.Parse(sach.Dongia.ToString());
            iSoluong = 1;
        }
    }
}