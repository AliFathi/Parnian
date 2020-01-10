using Ionic.Zip;
using Newtonsoft.Json;
using Parnian.Areas.Kaveh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Parnian.Areas.Kaveh.Controllers
{
    public class FileManagerController : Controller
    {
        [HttpGet]
        public PartialViewResult _Partial()
        {
            return PartialView("_FileManager", new KView());
        }

        [HttpPost]
        public JsonResult AddNewItem(string type, string name, string parent)
        {
            KResult result = new KResult();

            if (!KCore.IsValidFileName(name))
            {
                result.status = KStatus.warning;
                result.message = "file/folder name is invalid: it either contains invalid characters or starts with a reserved name";
                return Json(result);
            }

            string fullName = KCore.PhysicalPath(parent + "/" + name);

            switch (type)
            {
                case "file":
                    if (!System.IO.File.Exists(fullName))
                    {
                        FileStream fs = null;
                        try { fs = System.IO.File.Create(fullName); }
                        catch (Exception ex) { result.status = KStatus.error; result.message = ex.Message; }
                        finally { if (fs != null) { fs.Close(); fs.Dispose(); } }
                    }
                    else { result.status = KStatus.error; result.message = "file " + name + " already exists."; }
                    break;
                case "folder":
                    try { Directory.CreateDirectory(fullName); }
                    catch (Exception ex) { result.status = KStatus.error; result.message = ex.Message; }
                    break;
                default:
                    result.status = KStatus.error;
                    result.message = "incorrect type";
                    break;
            }

            return Json(result);
        }

        [HttpGet]
        public ActionResult Download(string json, string downloadName)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(downloadName))
                downloadName = "download";

            KSelectListItem[] array = JsonConvert.DeserializeObject<KSelectListItem[]>(json);

            using (var outputStream = new MemoryStream())
            {
                using (ZipFile zip = new ZipFile(System.Text.Encoding.UTF8))
                {
                    foreach (KSelectListItem item in array)
                    {
                        string path = KCore.PhysicalPath(item.url);

                        switch (item.type)
                        {
                            case "file":
                                if (System.IO.File.Exists(path))
                                    zip.AddItem(path, "");
                                break;

                            case "folder":
                                if (Directory.Exists(path))
                                    zip.AddDirectory(path, item.id);
                                break;
                        }
                    }

                    zip.Save(outputStream);
                }

                outputStream.Position = 0;
                return File(outputStream, "application/zip", downloadName + ".zip");
            }

            //string path = Server.MapPath(url);
            //FileInfo file = new FileInfo(path);
            //string fileName = file.Name;
            //byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            //return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        public JsonResult GetViewBoxContent(string path = "~/", string exclude = "")
        {
            DirectoryInfo directory = new DirectoryInfo(KCore.PhysicalPath(path));

            if (!directory.Exists)
            {
                return Json(new KResult
                {
                    status = KStatus.error,
                    message = "directory not found"
                }, JsonRequestBehavior.AllowGet);
                //throw new DirectoryNotFoundException();
            }

            KViewBoxContent content = new KViewBoxContent
            {
                parentName = directory.Name,
                parentPath = path,
                list = new List<KViewBoxItem>()
            };

            foreach (FileSystemInfo fsi in directory.GetFileSystemInfos())
            {
                content.list.Add(GetInfo(fsi));
            }

            return Json(new KResult
            {
                status = KStatus.success,
                message = content.list.Count.ToString(),
                data = content
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNavPaneContent(string path = "~/", bool deepDive = false)
        {
            DirectoryInfo directory = new DirectoryInfo(KCore.PhysicalPath(path));

            if (!directory.Exists)
            {
                return Json(new KResult
                {
                    status = KStatus.error,
                    message = "directory not found"
                }, JsonRequestBehavior.AllowGet);
                //throw new DirectoryNotFoundException();
            }

            KNavPaneItem item = new KNavPaneItem
            {
                name = directory.Name,
                path = path,
                list = GetList(KCore.PhysicalPath(path)),
            };

            return Json(new KResult
            {
                status = KStatus.success,
                message = "",
                data = item
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Remove(string json)
        {
            KSelectListItem[] list = JsonConvert.DeserializeObject<KSelectListItem[]>(json);

            List<KResult> result = new List<KResult>();

            foreach (KSelectListItem item in list)
            {
                string fullName = KCore.PhysicalPath(item.url);
                KStatus status = KStatus.success;
                string message = "";
                switch (item.type)
                {
                    case "file":
                        FileInfo file = new FileInfo(fullName);
                        try { file.Delete(); }
                        catch (Exception ex) { status = KStatus.error; message = ex.Message; }
                        break;
                    case "folder":
                        DirectoryInfo dir = new DirectoryInfo(fullName);
                        try { dir.Delete(true); }
                        catch (Exception ex) { status = KStatus.error; message = ex.Message; }
                        break;
                    default:
                        status = KStatus.error;
                        message = "incorrect type";
                        break;
                }

                result.Add(new KResult
                {
                    status = status,
                    message = message,
                    data = item.id
                });
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult Rename(string newName, string json)
        {
            if (string.IsNullOrWhiteSpace(newName)) return Json(new KResult { status = KStatus.error, message = "new name is empty" });

            KSelectListItem[] list = JsonConvert.DeserializeObject<KSelectListItem[]>(json);

            List<KResult> result = new List<KResult>();

            foreach (KSelectListItem item in list)
            {
                string fullName = KCore.PhysicalPath(item.url);
                string newFullName;
                KStatus status = KStatus.success;
                string message = "";
                switch (item.type)
                {
                    case "file":
                        FileInfo file = new FileInfo(fullName);
                        newFullName = NewFileName(file.DirectoryName + "\\" + newName, file.Extension);
                        try { file.MoveTo(newFullName); }
                        catch (Exception ex) { status = KStatus.error; message = ex.Message; }
                        break;
                    case "folder":
                        DirectoryInfo dir = new DirectoryInfo(fullName);
                        newFullName = NewFolderName(dir.Parent.FullName + "\\" + newName);
                        try { dir.MoveTo(newFullName); }
                        catch (Exception ex) { status = KStatus.error; message = ex.Message; }
                        break;
                    default:
                        status = KStatus.error;
                        message = "incorrect type";
                        break;
                }

                result.Add(new KResult
                {
                    status = status,
                    message = message,
                    data = item.id
                });
            }

            return Json(result);
        }

        [HttpGet]
        public void Thumbnail(string path, int width = 91)
        {
            string physicalPath = KCore.PhysicalPath(path);

            if (!System.IO.File.Exists(physicalPath)) return;

            var image = new WebImage(physicalPath);

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

        [HttpPost]
        public JsonResult Upload(string parent = "~/")
        {
            string root = KCore.PhysicalPath(parent);

            if (!Directory.Exists(root))
            {
                return Json(new KResult
                {
                    status = KStatus.error,
                    message = "parent not exist"
                });
            }

            KResult result = new KResult();
            List<KResult> data = new List<KResult>();

            foreach (string key in Request.Files.AllKeys)
            {
                KResult fileResult = new KResult();
                HttpPostedFileBase file = Request.Files.Get(key);

                if (file != null && file.ContentLength > 0)
                {
                    string fileName = file.FileName;
                    string fullName = root + "\\" + fileName;

                    fileResult.data = fileName;

                    if (System.IO.File.Exists(fullName))
                    {
                        fileResult.status = KStatus.warning;
                        fileResult.message = "فایلی با نام و پسوند مشابه وجود دارد.";
                    }
                    else
                    {
                        try { file.SaveAs(fullName); /*fileResult.message = "فایل با موفقیت آپلود شد";*/ }
                        catch (Exception ex) { fileResult.status = KStatus.error; fileResult.message = ex.Message; }
                    }
                }
                else
                {
                    fileResult.status = KStatus.warning;
                    fileResult.message = "فایلی وجود ندارد.";
                }

                data.Add(fileResult);
            }

            result.data = data;

            return Json(result);
        }

        # region private methods

        private List<KNavPaneItem> GetList(string fullname)
        {
            DirectoryInfo directory = new DirectoryInfo(fullname);
            List<KNavPaneItem> list = new List<KNavPaneItem>();
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                if (!dir.Exists) continue;
                var _fullname = dir.FullName;
                list.Add(new KNavPaneItem
                {
                    name = dir.Name,
                    path = KCore.VirtualPath(_fullname),
                    list = GetList(_fullname)
                });
            }
            return list;
        }

        private KViewBoxItem GetInfo(FileSystemInfo fsi)
        {
            string ext = fsi.Extension.ToLower();
            string fullName = fsi.FullName;
            return new KViewBoxItem
            {
                name = fsi.Name,
                extension = ext,
                path = KCore.VirtualPath(fullName),
                creation = fsi.CreationTime.ToShortDateString(),
                imageDimensions = (KCore.ImageExtensions.Contains(ext)) ? GetImageDimensions(fullName) : "",
                length = (fsi is FileInfo) ? ((FileInfo)fsi).Length : 0,
                isDirectory = (fsi is DirectoryInfo) ? true : false
            };
        }

        private string GetImageDimensions(string fullname, string sign = "x")
        {
            var image = new WebImage(fullname);
            return image.Width.ToString() + sign + image.Height.ToString();
        }

        private string NewFolderName(string name, int index = 0)
        {
            string i = (index == 0) ? "" : " (" + index.ToString() + ")";

            string result = name + i;

            DirectoryInfo dir = new DirectoryInfo(result);

            if (dir.Exists) NewFolderName(name, index + 1);

            return result;
        }

        private string NewFileName(string name, string extension, int index = 0)
        {
            string i = (index == 0) ? "" : " (" + index.ToString() + ")";

            string result = name + i + extension;

            FileInfo file = new FileInfo(result);

            if (file.Exists) NewFileName(name, extension, index + 1);

            return result;
        }

        # endregion private methods
    }
}