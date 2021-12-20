using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CG.MVC5.Stefania.Models
{
    public static class FileImageMethod
    {
        public static void SaveFile(HttpPostedFileBase img, int id)
        {
            try
            {
                var pathImg =HttpContext.Current.Server.MapPath(@"~/Content/Images/" + id);
                if (!Directory.Exists(pathImg))
                {
                    Directory.CreateDirectory(pathImg);
                }
                bool flag = true;
                foreach (string file in Directory.GetFiles(pathImg))
                {
                    if (img.FileName == Path.GetFileNameWithoutExtension(file))
                    {
                        flag = flag && false;
                    }
                }
                if (flag)
                {
                  img.SaveAs(pathImg + "/" + Path.GetFileName(img.FileName));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void DeleteImage(string img,int id)
        {
            try
            {
                string fullPath = HttpContext.Current.Server.MapPath(@"~/Content/Images/" + id+"/"+img);
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                //cancello la cartella, se è vuota
                string directoryPath = HttpContext.Current.Server.MapPath(@"~/Content/Images/"+id);
                if (Directory.Exists(directoryPath) && !Directory.EnumerateFileSystemEntries(directoryPath).Any())
                    Directory.Delete(directoryPath);
            

        }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void DeleteDirectory(int id)
        {
            try
            {
                string directoryPath = HttpContext.Current.Server.MapPath(@"~/Content/Images/" + id);
                if (Directory.Exists(directoryPath))
                    Directory.Delete(directoryPath,true);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public static void DeleteAllFileDirectory(int id)
        {
            try
            {
                string path = HttpContext.Current.Server.MapPath(@"~/Content/Images/" + id);
                DirectoryInfo dir = new DirectoryInfo(path);
                foreach(FileInfo file in dir.GetFiles())
                {
                    file.Delete();
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
    
}