using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Parnian.Controllers
{
    public class KavehController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        //Explore
        //---------------------------------------------------------------------------------------------
        public JsonResult Explore(string path, bool isVirtual = true, bool goThroughSubDirectories = false, bool getFiles = true, bool getDirectories = true)
        {
            string root = Server.MapPath("~/");
            string route = (String.IsNullOrEmpty(path)) ? root : ((isVirtual) ? Server.MapPath(path) : path);

            DirectoryInfo directory = new DirectoryInfo(route);
            Array array = null;

            if (getFiles && getDirectories) array = directory.GetFileSystemInfos();
            else if (getFiles) array = directory.GetFiles();
            else if (getDirectories) array = directory.GetDirectories();

            foreach (FileSystemInfo item in array)
            {
                iFileSystem ifs = new iFileSystem();
                ifs.name = item.Name;
                ifs.extension = item.Extension.ToLower();
                ifs.path = "/" + item.FullName.Remove(0, root.Length).Replace("\\", "/");
                ifs.creation = item.CreationTime.ToShortDateString();
                ifs.lastAccess = item.LastAccessTime.ToShortDateString();
                ifs.lastWrite = item.LastWriteTime.ToShortDateString();
                ifs.imageDimensions = getImageDimensions(ifs.extension, item.FullName);
                if (item is FileInfo)
                {
                    FileInfo file = (FileInfo)item;
                    ifs.length = file.Length;
                    ifs.isDirectory = false;
                }
                else if (item is DirectoryInfo)
                {
                    ifs.length = 0;
                    ifs.isDirectory = true;
                    if (goThroughSubDirectories) Explore(item.FullName, false, true);
                }

                iFileSystemList.Add(ifs);
            }
            
            return Json(iFileSystemList, JsonRequestBehavior.AllowGet);
        }

        string getImageDimensions(string ext, string fullname, string sign = "x")
        {
            if (imageExts.Contains(ext))
            {
                var image = new WebImage(fullname);
                return image.Width.ToString() + sign + image.Height.ToString();
            }
            else return null;
        }

        List<iFileSystem> iFileSystemList = new List<iFileSystem>();

        class iFileSystem
        {
            public string name;
            public string extension;
            public string path;
            public string creation;
            public string lastAccess;
            public string lastWrite;
            public string imageDimensions;

            public long length;

            public bool isDirectory;
        }

        List<String> imageExts = new List<String> { ".jpeg", ".jpg", ".png", ".ico", ".gif" };

        //Remove
        //---------------------------------------------------------------------------------------------
        [HttpPost]
        public string Remove(string path, string type)
        {
            if (!String.IsNullOrEmpty(path))
            {
                if (type == "file")
                {
                    FileInfo file = new FileInfo(Server.MapPath(path));
                    if (file.Exists)
                    {
                        file.Delete();
                        return "فایل پاک شد";
                    }
                    else return "این فایل وجود ندارد";
                }

                else if (type == "folder")
                {
                    DirectoryInfo directory = new DirectoryInfo(Server.MapPath(path));
                    if (directory.Exists)
                    {
                        directory.Delete(true);
                        return "فولدر پاک شد";
                    }
                    else return "این فولدر وجود ندارد";
                }
            }

            return "درخواست معتبر نیست";
        }

        //Rename
        //---------------------------------------------------------------------------------------------
        [HttpPost]
        public string Rename(string path, string type, string newName)
        {
            if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(newName))
            {
                if (type == "file")
                {
                    FileInfo file = new FileInfo(Server.MapPath(path));
                    if (file.Exists)
                    {
                        string parentPath = file.DirectoryName;
                        string newFullName = parentPath + "/" + newName;
                        FileInfo newFile = new FileInfo(newFullName);
                        if (newFile.Exists) return "تغییر نام انجام نشد چون فایلی با نام مشابه وجود دارد";
                        file.MoveTo(newFullName);
                        return "نام فایل عوض شد";
                    }
                    else return "این فایل وجود ندارد";
                }

                else if (type == "folder")
                {
                    DirectoryInfo directory = new DirectoryInfo(Server.MapPath(path));
                    if (directory.Exists)
                    {
                        string parentPath = directory.Parent.FullName;
                        string newFullName = parentPath + "/" + newName;
                        DirectoryInfo newDirectory = new DirectoryInfo(newFullName);
                        if (newDirectory.Exists) return "تغییر نام انجام نشد چون فولدری با نام مشابه وجود دارد";
                        directory.MoveTo(newFullName);
                        return "نام فولدر عوض شد";
                    }
                    else return "این فولدر وجود ندارد";
                }
                else return "درخواست معتبر نیست";
            }

            return "درخواست معتبر نیست";
        }

        //Download
        //---------------------------------------------------------------------------------------------
        public FileResult Download(string url)
        {
            string path = Server.MapPath(url);
            FileInfo file = new FileInfo(path);
            string fileName = file.Name;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        //Upload
        //---------------------------------------------------------------------------------------------
        [HttpPost]
        public string Upload(string rootPath, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string root = Server.MapPath(rootPath);
                DirectoryInfo directory = new DirectoryInfo(root);
                string fileName = file.FileName;
                foreach (FileInfo fi in directory.GetFiles())
                {
                    if (fi.Name == fileName) return "آپلود انجام نشد چون فایلی با نام و پسوند مشابه وجود دارد.";
                }
                string path = Server.MapPath(rootPath + fileName);
                file.SaveAs(path);
                return "فایل با موفقیت آپلود شد";
            }
            return "یا فایلی انتخاب نشده یا فایل انتخاب شده خالیست";
        }

        //Thumbnail
        //---------------------------------------------------------------------------------------------
        public void Thumbnail(string path, int width = 91)
        {
            var image = new WebImage(Server.MapPath(path));
            if (image.Width > width)
            {
                float fH = (width * image.Height) / image.Width;
                int H = (int)Math.Floor(fH);
                image.Resize(width, H).Crop(1, 1).Write();
            }
            else
            {
                image.Write();
            }
        }

        //---------------------------------------------------------------------------------------------
    }
}