using System;
using System.Collections.Generic;
using System.Text;

namespace SoundRecorder
{
    public class RecordedFileDetails
    {
        public string FileName { get; set; }
        public string FileAbsolutePath { get; set; }
        public string FolderPath { get; set; }

        public RecordedFileDetails(string fileAbsolutePath, string folderPath)
        {
            FileAbsolutePath = fileAbsolutePath;
            FolderPath = folderPath;
            FileName = fileAbsolutePath.Replace(folderPath, string.Empty);
        }
    }
}
