using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ListRecordings : ContentPage
    {
        private readonly AudioPlayer player;
        public static ObservableCollection<RecordedFileDetails> recordingList;
        private PermissionStatus StoragePermission;


        public ListRecordings(PermissionStatus permission)
        {
            InitializeComponent();
            this.BindingContext = this;
            StoragePermission = new PermissionStatus();
            StoragePermission = permission;
            recordingList = new ObservableCollection<RecordedFileDetails>();
            ErrorMessage.IsVisible = false;

            RecordingListView.ItemTapped += RecordingListView_ItemTapped;

            player = new AudioPlayer();
            RecordingListView.IsPullToRefreshEnabled = true;
            RecordingListView.RefreshCommand = new Command(() =>
            {
                RefreshData();
                RecordingListView.IsRefreshing = false;
            });


        }

        private async void DisplayRecordings(PermissionStatus storagePermission)
        {
            if (storagePermission == PermissionStatus.Granted)
            {
                if (DependencyService.Get<IFileHelper>().GetRecordingList().Count > 0)
                {
                    RecordingsFound();
                    
                }
                else
                {
                    NoRecordingsFound();
                }

            }
            else
            {
                if (await DependencyService.Get<IFileHelper>().AskPermissionWriteStorage() == PermissionStatus.Granted)
                {
                    if (DependencyService.Get<IFileHelper>().GetRecordingList().Count > 0)
                    {

                        RecordingsFound();
                    }
                    else
                    {
                        NoRecordingsFound();
                    }
                }
                else
                {
                    PermissionNotGranted();
                }

            }
        }

        private void RecordingsFound()
        {
            recordingList = DependencyService.Get<IFileHelper>().GetRecordingList();
            RecordingListView.ItemsSource = recordingList;
            GetAllRecordigs.IsVisible = false;
            ErrorMessage.IsVisible = false;
        }

        private void PermissionNotGranted()
        {
            ErrorMessage.Text = "Permisssion for \'storage\' or \'Files and Media\' is required for accessing recording files";
            ErrorMessage.IsVisible = true;
            GetAllRecordigs.IsVisible = true;
        }

        private void NoRecordingsFound()
        {
            ErrorMessage.Text = "No recordings found. Go back to Record somthing.";
            ErrorMessage.TextColor = Color.Green;
            ErrorMessage.IsVisible = true;
            GetAllRecordigs.IsVisible = false;
        }

        private void RefreshData()
        {
            RecordingListView.ItemsSource = null;
            DisplayRecordings(StoragePermission);
            
        }

        private async void RecordingListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            
            RecordedFileDetails recordedFileDetails = recordingList[e.ItemIndex];
            try
            {
                if(recordedFileDetails.FileAbsolutePath != null)
                {
                    player.Play(recordedFileDetails.FileAbsolutePath);
                }
            }
            catch(Exception ex)
            {
                await DisplayAlert("Failed to play recording", ex.ToString(), "OK");
            }
        }

        protected override bool  OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            try
            {
                player.Pause();
            }
            catch 
            {
                
            }
            
            return false;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            player.Pause();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            recordingList = null;
            RecordingListView.ItemsSource = null;
            DisplayRecordings(StoragePermission);
            
        }
        async void ShowFileInfo(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var contextItem = mi.BindingContext;
            RecordedFileDetails recordingFileDetails = (RecordedFileDetails)contextItem;
            await DisplayAlert("File Info", recordingFileDetails.FileAbsolutePath, "OK");
            
        }
        async void DeleteThisRecording(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var contextItem = mi.BindingContext;
            RecordedFileDetails recordingFileDetails = (RecordedFileDetails)contextItem;
            
            if(System.IO.File.Exists(recordingFileDetails.FileAbsolutePath))
            {
                if(await DisplayAlert("Delete Recording","Are you sure want to delete this recording: " + recordingFileDetails.FileName, "Yes Delete", "No" ))
                {
                    var result = DependencyService.Get<IFileHelper>().DeleteFile(recordingFileDetails.FileAbsolutePath);
                    await DisplayAlert("Delete Recording", result, "OK");
                }
                DisplayRecordings(StoragePermission);
                
            }

        }
         async void RenameThisRecording(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var contextItem = mi.BindingContext;
            RecordedFileDetails recordedFileDetails = (RecordedFileDetails)contextItem;
            if (System.IO.File.Exists(recordedFileDetails.FileAbsolutePath))
            {
                var renameRecordingPage = new RenameRecording( recordedFileDetails);
                
                await Navigation.PushAsync(renameRecordingPage);
                
            }

        }
        async void GetAllRecordings_Clicked(object sender, EventArgs args)
        {
            StoragePermission = await DependencyService.Get<IFileHelper>().CheckPermissionsWriteStorage();
            DisplayRecordings(StoragePermission);
            
        }
        async void GoBack(object sender, EventArgs args)
        {
            await Navigation.PopAsync();
        }
        

    }
}