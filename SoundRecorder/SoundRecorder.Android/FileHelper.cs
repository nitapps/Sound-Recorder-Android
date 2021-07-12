using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(SoundRecorder.Droid.FileHelper))]
namespace SoundRecorder.Droid
{
    public class FileHelper : IFileHelper
    {
        
        private static string FolderPath = GetFolderPath();

        public bool SaveFile(byte[] data)
        {
            var FileName = "SR_recording_" + DateTime.Now.ToShortDateString().Replace("/", "-") + "_" + DateTime.Now.ToShortTimeString().Replace(" ", string.Empty).Replace(":", "-") + DateTime.Now.Second + ".wav";
            string path = Path.Combine(FolderPath, FileName);
            File.WriteAllBytesAsync(path, data);
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
                
            }
        }
        public ObservableCollection<RecordedFileDetails> GetRecordingList()
        {
            Java.IO.File recordingDirectory = new Java.IO.File(FolderPath);
            ObservableCollection<RecordedFileDetails> recordingList = new ObservableCollection<RecordedFileDetails>();
            foreach(Java.IO.File f in recordingDirectory.ListFiles())
            {
                if (f.Exists() && f.Name.Contains(".wav"))
                {
                    recordingList.Add(new RecordedFileDetails(f.AbsolutePath, FolderPath));
                }
            }
            return recordingList;
        }

        
        private static string GetFolderPath()
        {
            if(Android.OS.BuildVersionCodes.Q >= Build.VERSION.SdkInt)
            {
                var FolderPath = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, "SoundRecorder");
                var dir = new Java.IO.File(FolderPath);
                if (dir.Exists())
                {
                    
                    return FolderPath;
                }
                else
                {
                    dir.Mkdir();
                    if (dir.Exists())
                    {

                        return FolderPath;
                    }
                }
            }
            
            
                FolderPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).ToString(), "SoundRecorder");
                var folder = new Java.IO.File(FolderPath);
                
                if (folder.Exists())
                {
                    return folder.AbsolutePath;
                }
                else
                {
                    folder.Mkdir();
                    if (folder.Exists())
                    {
                        return folder.AbsolutePath;
                    }
                }
            
            
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).ToString() ; 
            return path;
        }

        public async Task<PermissionStatus> CheckPermissionMicrophone()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            return status;
            
        }

        public async Task<PermissionStatus> CheckPermissionsWriteStorage()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            return status;
        }

        public async Task<PermissionStatus> AskPermissionMicrophone()
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            return status;
        }

        public async Task<PermissionStatus> AskPermissionWriteStorage()
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            
            return status;
        }

        public string DeleteFile(string absolutePath)
        {
            if (File.Exists(absolutePath))
            {
                var file = new Java.IO.File(absolutePath);
                file.Delete();
                if (File.Exists(absolutePath))
                {
                    return "File could not be deleted! Please try again or Try to Delete From file Manager. Recordings are Stored in Folder: \'" + FolderPath + "\'.";
                }
                    
                else
                {
                    return "File Deleted Successfully!";
                }
            }
            else
            {
                return "File not found!";
            }
        }
    }
}