
//using System.IO;

//namespace TestMVC.Helper
//{
//    public static class Upload
//    {
//        public static string UploadFile(string FolderName,IFormFile file)
//        {
//            try
//            {
//                if (file == null || file.Length == 0)
//                    return null;

//                string FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", FolderName);

//                // ✅ FIX: Ensure directory exists
//                if (!Directory.Exists(FolderPath))
//                {
//                    Directory.CreateDirectory(FolderPath);
//                }
//                string FileName=Guid.NewGuid().ToString()+Path.GetFileName(file.FileName);
//                var FinalPath=Path.Combine(FolderPath,FileName);
//                using(var FileStream=new FileStream(FinalPath,FileMode.Create))
//                {
//                    file.CopyTo(FileStream);
                   
//                }
//                return FileName;

//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }
//            return null;
            
//        }
//        public static bool Delete(string FolderName, string fileName)
//        {
//            try
//            {
//                if (string.IsNullOrEmpty(fileName))
//                    return false;

//                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", FolderName, fileName);

//                if (File.Exists(filePath))
//                {
//                    File.Delete(filePath);
//                    return true;
//                }
//                return false;
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
          
//        }
//    }
//}
