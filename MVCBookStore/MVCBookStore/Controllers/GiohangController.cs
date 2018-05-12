using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBookStore.Models;

namespace MVCBookStore.Controllers
{
    //Tao doi tuong data chua du lieu tu model dbBansach da tao
    
    public class GiohangController : Controller
    {
        dbBookStoreDataContext data = new dbBookStoreDataContext();

        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang==null)
            {
                //Neu gio hang chua ton tai thi khoi tao listGiohang
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }

        public ActionResult Index()
        {
            return View();
        }

        //Them hang vao gio
        public ActionResult ThemGiohang(int iMasach, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.iMasach == iMasach);
            if(sanpham==null)
            {
                sanpham = new Giohang(iMasach);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }
        // GET: Giohang
      private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang!=null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);
            }
            return iTongSoLuong;
        }

        private double TongTien()
        {
            double iTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);
            }
            return iTongTien;
        }

        public ActionResult Giohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count==0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }
        //Tao partial view de hien thi thong tin gio hang
        public ActionResult GiohangPartial()
        {
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return PartialView();
        }

        //Xoa Giohang
        public ActionResult XoaGiohang(int iMaSP)
        {
            //Lay gio hang tu session
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra sach da co trong session["Giohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if(sanpham!=null)
            {
                lstGiohang.RemoveAll(n => n.iMasach == iMaSP);
                return RedirectToAction("GioHang");
            }
            if(lstGiohang.Count==0)
            {
                return RedirectToAction("Index", "BookStore");
            }
            return RedirectToAction("Giohang");
        }

        //Cap nhat gio hang
        public ActionResult CapnhatGiohang(int iMaSP,FormCollection f)
        {
            //Lay gio hang tu session
            List<Giohang> lstGiohang = Laygiohang();
            //Kiem tra sach da co trong session["Giohang"]
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.iMasach == iMaSP);
            //Neu ton tai thi cho sua Soluong
            if (sanpham != null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            //Lay gio hang tu session
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "BookStore");
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if(Session["Taikhoan"]==null || Session["Taikhoan"].ToString()=="")
            {
                return RedirectToAction("Dangnhap", "Nguoidung");
            }
            if(Session["Giohang"]==null)
            {
                return RedirectToAction("Index", "BookStore");
            }
            //Lay gio hang tu Session
            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            //Them don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<Giohang> gh = Laygiohang();
            ddh.MaKH = kh.MaKH;
            ddh.NgayDH = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiaohang = DateTime.Parse(ngaygiao);
            ddh.HTGiaohang = false;
            ddh.Dagiao = false;
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                CTDATHANG ctdh = new CTDATHANG();
                ctdh.SoDH = ddh.SoDH;
                ctdh.Masach = item.iMasach;
                ctdh.Soluong = item.iSoluong;
                ctdh.Dongia = (decimal)item.dDongia;
                data.CTDATHANGs.InsertOnSubmit(ctdh);
            }
            data.SubmitChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }

        public ActionResult Xacnhandonhang()
        {
            return View();
        }
    }
}