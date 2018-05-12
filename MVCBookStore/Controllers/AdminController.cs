using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBookStore.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace MVCBookStore.Controllers
{
    public class AdminController : Controller
    {
        dbBookStoreDataContext db = new dbBookStoreDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                    ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        //Sach
        public ActionResult Sach(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 7;
            return View(db.SACHes.ToList().OrderBy(n => n.Masach).ToPagedList(pageNumber, pageSize));
        }

        // Thêm mới sách
        [HttpGet]
        public ActionResult Themmoisach()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoisach(SACH sach, HttpPostedFileBase fileupload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //Kiểm tra đường dẫn file
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    // Lưu tên file, lưu ý bổ sung thư viện using System.IO;
                    var fileName = Path.GetFileName(fileupload.FileName);
                    // Lưu đường dẫn file
                    var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);
                    // Kiễm tra hình ảnh tồn tại
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileupload.SaveAs(path);
                    }
                    sach.Hinhminhhoa = fileName;
                    // Lưu vào CSDL
                    db.SACHes.InsertOnSubmit(sach);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sach");
            }
        }

        //Hiển thị sản phẩm
        public ActionResult Chitietsach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        //Xóa sản phẩm
        [HttpGet]
        public ActionResult Xoasach(int id)
        {
            //Lấy ra đối tượng sách cần xóa theo mã
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }
        [HttpPost, ActionName("Xoasach")]
        public ActionResult Xacnhanxoa(int id)
        {
            //Lấy ra đối tượng sách cần xóa theo mã
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.DeleteOnSubmit(sach);
            db.SubmitChanges();
            return RedirectToAction("Sach");
        }

        //Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            //Lấy ra đối tượng sách cần xóa theo mã
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View(sach);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            //Kiểm tra đường dẫn file
            if (fileUpload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            //Thêm vào CSDL
            else
            {
                if (ModelState.IsValid)
                {
                    //Lưu tên file, lưu ý bổ sung thư viên System.IO;
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //Lưu đường dẫn của file
                    var path = Path.Combine(Server.MapPath("~/Hinhsanpham"), fileName);
                    // Kiễm tra hình ảnh tồn tại
                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";                   
                    else
                    {
                        //Lưu hình ảnh vào đường dẫn
                        fileUpload.SaveAs(path);
                    }

                    SACH s = db.SACHes.Where(x => x.Masach == sach.Masach).Single<SACH>();
                    s.Tensach = sach.Tensach;
                    s.Donvitinh = sach.Donvitinh;
                    s.Dongia = sach.Dongia;
                    s.Mota = sach.Mota;
                    s.Hinhminhhoa = fileName;
                    s.MaCD = sach.MaCD;
                    s.MaNXB = sach.MaNXB;
                    s.Ngaycapnhat = sach.Ngaycapnhat;
                    s.Soluongban = sach.Soluongban;
                    s.solanxem = sach.solanxem;
                    s.moi = sach.moi;

                    sach.Hinhminhhoa = fileName;
                    // Lưu vào CSDL
                    UpdateModel(sach);
                    db.SubmitChanges();
                }
                return RedirectToAction("Sach");
            }
        }
    }
}