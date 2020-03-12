using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace IntegrationTools.FileTools
{
    public class IOHelper
    {
        public bool CheckFile(string path)
        {
            return new FileInfo(path).Exists;
        }

        public string ReadFile(string path)
        {
            string data = string.Empty;
            try
            {
                using (StreamReader reader = new FileInfo(path).OpenText())
                {
                    data = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch
            {

            }
            return data;
        }

        public void Rename(string oldpath, string newpath)
        {
            File.Copy(oldpath, newpath);
            FileInfo fi = new FileInfo(oldpath);
            fi.Delete();
        }

        public void DeleteFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
                fi.Delete();
        }

        public bool CreateDirectory(string path)
        {
            bool isSuccess = true;
            try
            {
                DirectoryInfo pathDir = new DirectoryInfo(path);
                if (!pathDir.Exists) pathDir.Create();
            }
            catch
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool DeleteDirectory(string path, bool recursive = true)
        {
            bool isSuccess = true;
            try
            {
                DirectoryInfo pathDir = new DirectoryInfo(path);
                if (pathDir.Exists) pathDir.Delete(recursive);
            }
            catch
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public string CreateFile(string logpath, long maxbuffer)
        {
            string filePath = null;
            string name = DateTime.Now.ToString("yyyyMMdd");
            filePath = string.Format("{0}\\{1}.txt", logpath, name);
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists && fi.Length >= maxbuffer)
            {
                List<FileInfo> infos = GetFiles(logpath, string.Format("{0}*", name)).ToList();
                //get last iteration
                int max = infos.Max(f => f.Name.Length);
                infos = infos.Where(c => c.Name.Length == max).OrderBy(f => f.Name).ToList();
                FileInfo lst = infos[infos.Count - 1];
                string[] namedetails = lst.Name.Replace(".txt", string.Empty).Split('-');
                int iter = 1;
                if (namedetails.Length > 1)
                    iter = Convert.ToInt32(namedetails[1]) + 1;
                try
                {
                    Rename(fi.FullName, string.Format("{0}\\{1}-{2}.txt", logpath, name, iter));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return filePath;
        }


        public void AppendToFile(string path, string line)
        {
            try
            {
                using (StreamWriter w = File.AppendText(path))
                {
                    w.WriteLine(line);
                    w.Close();
                }
            }
            catch
            {

            }
        }

        public FileInfo[] GetFiles(string path, string pattern)
        {
            FileInfo[] fi = null;
            try
            {
                DirectoryInfo pathDir = new DirectoryInfo(path);
                if (pathDir.Exists)
                    fi = pathDir.GetFiles(pattern);
            }
            catch
            {
                return fi;
            }
            return fi;
        }
    }
}
