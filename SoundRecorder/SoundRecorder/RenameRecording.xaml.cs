using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SoundRecorder
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RenameRecording : ContentPage
    {
        private static RecordedFileDetails recordedFileDetails;

        public RenameRecording( RecordedFileDetails rfd)
        {
            InitializeComponent();
            recordedFileDetails = rfd;
            if(rfd != null)
            {
                string oldName = recordedFileDetails.FileName.Replace("/", string.Empty).Replace(".wav", string.Empty);
                NewNameEntry.Text = oldName;
            }
            
            
            
        }
        async void OnRename(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(NewNameEntry.Text))
            {
                await DisplayAlert("Enter a Valid Name", "Name Can't be empty! and Can contains only alphabets and numeric numbers", "OK");
                return;
            }
            string newName = "/" + NewNameEntry.Text + ".wav";
            if(recordedFileDetails != null)
            {
                if (File.Exists(recordedFileDetails.FileAbsolutePath))
                {
                    var newPath = recordedFileDetails.FolderPath+newName;
                    if (File.Exists(newPath))
                    {
                        await DisplayAlert("Error", "Same file name already exists", "OK");
                        return;
                       
                    }
                    File.Copy(recordedFileDetails.FileAbsolutePath, newPath);
                    File.Delete(recordedFileDetails.FileAbsolutePath);
                    if(File.Exists(recordedFileDetails.FileAbsolutePath))
                    {
                        File.Delete(newPath);
                        await DisplayAlert("Sorry! ","Could't rename File. Please try to rename from File Manager", "OK");
                        
                    }
                    else
                    {
                        if (File.Exists(newPath))
                        {
                            
                            await DisplayAlert("Sucessfull!", "File is Renamed Sucessfully", "OK");
                        }
                    }
                    
                    

                    await Navigation.PopAsync();
                }
            }
            else
            {
                await Navigation.PopAsync();
            }
            
        }
        async void GoBack(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }
    }
}