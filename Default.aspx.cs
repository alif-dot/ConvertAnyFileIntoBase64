using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ConvertPdfBase64
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region Specific type of file Convert
        //protected void btnConvert_Click(object sender, EventArgs e)
        //{
        //    string directoryPath = @"F:\Image\IVAS\SixTen";
        //    List<Return91SFAttFileSet> Return_9_1_SF_att_fileSet = new List<Return91SFAttFileSet>();

        //    // Get all PDF files in the directory
        //    string[] files = Directory.GetFiles(directoryPath, "*.pdf");

        //    foreach (string file in files)
        //    {
        //        FileInfo fileInfo = new FileInfo(file);

        //        // Check file size 
        //        if (fileInfo.Length <= 4 * 1024 * 1024)
        //        {
        //            // Convert to Base64
        //            string base64Content = ConvertToBase64(file);

        //            // Create JSON object
        //            Return91SFAttFileSet Return91SFAttFileSet = new Return91SFAttFileSet
        //            {
        //                MSGID = "",
        //                DocId = "",
        //                FileName = fileInfo.Name,
        //                Content = base64Content,
        //                FileType = "PDF",
        //                DocType = "10"
        //            };

        //            Return_9_1_SF_att_fileSet.Add(Return91SFAttFileSet);
        //        }
        //    }

        //    // Create JSON string
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    string jsonResult = serializer.Serialize(new { Return_9_1_SF_att_fileSet });

        //    // Output JSON string
        //    Response.Write(jsonResult);
        //}

        #endregion

        #region Read any type of file & compress to zip
        //protected void btnConvert_Click(object sender, EventArgs e)
        //{
        //    string periodKey = DateTime.Now.ToString("yyMM");
        //    string directoryPath = @"F:\Image\IVAS\SixTen\" + periodKey;
        //    List<Return91SFAttFileSet> Return_9_1_SF_att_fileSet = new List<Return91SFAttFileSet>();

        //    // Get all files in the directory
        //    string[] files = Directory.GetFiles(directoryPath);

        //    // Create a zip file
        //    string timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
        //    string zipFileName = $"CompressedFiles_{timestamp}.zip";
        //    string zipFilePath = Path.Combine(directoryPath, zipFileName);

        //    // Delete existing zip file if it exists
        //    if (File.Exists(zipFilePath))
        //    {
        //        File.Delete(zipFilePath);
        //    }

        //    using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        //    {
        //        HashSet<string> uniqueFile = new HashSet<string>(); // Track unique file
        //        long totalFileSize = 0; // Track total file size

        //        // Process each file
        //        foreach (string file in files)
        //        {
        //            FileInfo fileInfo = new FileInfo(file);

        //            // Check file size and uniqueness
        //            if (fileInfo.Length <= 4 * 1024 * 1024 && uniqueFile.Add(fileInfo.Name))
        //            {
        //                // Add the file to the zip archive
        //                string entryName = Path.GetFileName(file);
        //                archive.CreateEntryFromFile(file, entryName);

        //                // Update counters
        //                totalFileSize += fileInfo.Length;

        //                // Create JSON object
        //                Return91SFAttFileSet return91SFAttFileSet = new Return91SFAttFileSet
        //                {
        //                    MSGID = "",
        //                    FileName = fileInfo.Name,
        //                    FileType = Path.GetExtension(fileInfo.Name).TrimStart('.'),
        //                    DocType = "10"
        //                };

        //                Return_9_1_SF_att_fileSet.Add(return91SFAttFileSet);
        //            }
        //        }
        //    }

        //    // Convert the zip file to Base64
        //    string base64Content = ConvertToBase64(zipFilePath);

        //    // Create JSON string
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    string jsonResult = serializer.Serialize(new { Return_9_1_SF_att_fileSet = base64Content });

        //    // Output JSON string
        //    Response.Write(jsonResult);
        //}

        #endregion

        #region Generate Zip & TextFile
        protected void btnConvert_Click(object sender, EventArgs e)
        {
            string directoryPath = @"F:\Image\IVAS\SixTen";
            List<Return91SFAttFileSet> Return_9_1_SF_att_fileSet = new List<Return91SFAttFileSet>();

            // Get all PDF files in the directory
            string[] files = Directory.GetFiles(directoryPath, "*.pdf");

            // Create a timestamp for the filenames
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Generate unique filenames for zip and text files
            string zipFileName = $"ConvertedFiles_{timestamp}.zip";
            //string textFileName = $"JsonResult_{timestamp}.txt";

            // Path to save the zip and text files
            string zipFilePath = Path.Combine(directoryPath, zipFileName);
            string textFilePath = Path.Combine(directoryPath, "JsonResult.txt");

            // Create dictionary to store mapping between filenames and IDs
            Dictionary<string, int> filenameToIdMap = new Dictionary<string, int>();

            if (File.Exists(textFilePath))
            {
                // If text file exists, read existing JSON content and deserialize it
                string existingJson = File.ReadAllText(textFilePath);
                //Return_9_1_SF_att_fileSet = new JavaScriptSerializer().Deserialize<List<Return91SFAttFileSet>>(existingJson);

                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                jsonSerializer.MaxJsonLength = Int32.MaxValue; // Set maxJsonLength to maximum value
                Return_9_1_SF_att_fileSet = jsonSerializer.Deserialize<List<Return91SFAttFileSet>>(existingJson);

                // Populate filename to ID mapping
                foreach (var item in Return_9_1_SF_att_fileSet)
                {
                    filenameToIdMap[item.FileName] = item.DocId;
                }
            }

            using (ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    // Check file size 
                    if (fileInfo.Length <= 4 * 1024 * 1024)
                    {
                        // Add the file to the zip archive
                        string entryName = Path.GetFileName(file);
                        archive.CreateEntryFromFile(file, entryName);

                        // Convert to Base64
                        string base64Content = ConvertToBase64(file);
                        //int docId;
                        Return91SFAttFileSet rt = new Return91SFAttFileSet();
                        int docId = rt.DocId;

                        // If filename is already mapped to an ID, use that ID; otherwise, assign a new ID
                        if (filenameToIdMap.TryGetValue(fileInfo.Name, out docId))
                        {
                            // Create JSON object
                            Return91SFAttFileSet Return91SFAttFileSet = new Return91SFAttFileSet
                            {
                                MSGID = "",
                                //DocId = 1,
                                FileName = fileInfo.Name,
                                Content = base64Content,
                                FileType = "PDF",
                                DocType = "10"
                            };

                            Return_9_1_SF_att_fileSet.Add(Return91SFAttFileSet);
                        }
                        else
                        {
                            // Assign a new ID and add mapping
                            //docId = GetNextDocId(); // Use auto-incremented docId
                            //Return91SFAttFileSet rt = new Return91SFAttFileSet();
                            filenameToIdMap[fileInfo.Name] = rt.DocId;

                            // Create JSON object with new ID
                            Return91SFAttFileSet Return91SFAttFileSet = new Return91SFAttFileSet
                            {
                                MSGID = "",
                                //DocId = 1,
                                FileName = fileInfo.Name,
                                Content = base64Content,
                                FileType = "PDF",
                                DocType = "10"
                            };

                            Return_9_1_SF_att_fileSet.Add(Return91SFAttFileSet);
                        }
                    }
                }
            }

            // Create JSON string
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;
            string jsonResult = serializer.Serialize(new { Return_9_1_SF_att_fileSet });

            // Write JSON result to text file
            File.WriteAllText(textFilePath, jsonResult);

            // Response message
            Response.Write(jsonResult);
        }

        //private int GetNextDocId()
        //{
        //    return 1;
        //}

        #endregion

        private string ConvertToBase64(string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(fileBytes);
        }

        public class Return91SFAttFileSet
        {
            private static int _nextDocId = 1;
            private static object _lock = new object();

            public Return91SFAttFileSet()
            {
                lock (_lock)
                {
                    this.DocId = _nextDocId++;
                }
            }
            public string MSGID { get; set; }
            public int DocId { get; set; }
            public string FileName { get; set; }
            public string Content { get; set; }
            public string FileType { get; set; }
            public string DocType { get; set; }
        }
    }
}