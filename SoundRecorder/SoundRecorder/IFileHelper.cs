using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SoundRecorder
{
    public interface IFileHelper
    {
        bool SaveFile(byte[] data);
        string DeleteFile(string absolutePath);

        ObservableCollection<RecordedFileDetails> GetRecordingList();
        Task<PermissionStatus> CheckPermissionMicrophone();
        Task<PermissionStatus> CheckPermissionsWriteStorage();
        Task<PermissionStatus> AskPermissionMicrophone();
        Task<PermissionStatus> AskPermissionWriteStorage();

        
        
        
    }
}
