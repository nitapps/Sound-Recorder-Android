using Plugin.AudioRecorder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;


namespace SoundRecorder
{
    public partial class MainPage : ContentPage

    {

        readonly AudioRecorderService recorder;
        readonly AudioPlayer player;
        int seconds = 0, minutes = 0;
        bool isTimerRunning = false;
       
        public MainPage()
        {
            InitializeComponent();
            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(15)
            };
            player = new AudioPlayer();
            player.FinishedPlaying += Player_FinishedPlaying;
            StopButton.IsEnabled = false;
        }

        private void Player_FinishedPlaying(object sender, EventArgs e)
        {
           

            
            RecordButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            PlayButton.IsEnabled = true;
            RecordingListButton.IsEnabled = true;
            lblSeconds.Text = "00";
            lblMinutes.Text = "00";
        }

        async void OnRecordAsync(object sender, EventArgs e)
        {
            if(!recorder.IsRecording)
            {
                
                var micPermission = await DependencyService.Get<IFileHelper>().AskPermissionMicrophone();
                if(micPermission == PermissionStatus.Granted)
                {
                    StartTimer();
                    var audioRecordTask = await recorder.StartRecording();

                    RecordButton.IsEnabled = false;
                    StopButton.IsEnabled = true;
                    PlayButton.IsEnabled = false;
                    RecordingListButton.IsEnabled = false;

                    await audioRecordTask;
                }
                else
                {
                    var askPermission = await DependencyService.Get<IFileHelper>().AskPermissionMicrophone();
                    if(askPermission == PermissionStatus.Granted)
                    {
                        var audioRecordTask = await recorder.StartRecording();

                        RecordButton.IsEnabled = false;
                        StopButton.IsEnabled = true;
                        PlayButton.IsEnabled = false;
                        RecordingListButton.IsEnabled = false;

                        await audioRecordTask;
                    }
                    else
                    {
                        isTimerRunning = false;
                        await DisplayAlert("Permisssions Not Granted", "Permisssions for 'Mic' and 'Storage or Files and Media' required for Recording and Storing", "OK");
                    }
                }
                
                
                
                
            }
            

        }

        private void StartTimer()
        {
            seconds = 0;
            minutes = 0;
            isTimerRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                seconds++;

                if (seconds.ToString().Length == 1)
                {
                    lblSeconds.Text = "0" + seconds.ToString();
                }
                else
                {
                    lblSeconds.Text = seconds.ToString();
                }
                if (seconds == 60)
                {
                    minutes++;
                    seconds = 0;

                    if (minutes.ToString().Length == 1)
                    {
                        lblMinutes.Text = "0" + minutes.ToString();
                    }
                    else
                    {
                        lblMinutes.Text = minutes.ToString();
                    }

                    lblSeconds.Text = "00";
                }
                return isTimerRunning;
            }); 
        }

        void OnPlay(object sender, EventArgs e)
        {
            try
            {
                if (recorder.GetAudioFilePath() != null)
                {
                    isTimerRunning = true;
                    player.Play(recorder.GetAudioFilePath());
                }
                
            }catch (Exception ex)
            {
                throw ex;
            }
            
        }
        async  void OnStopAsync(object sender, EventArgs e)
        {
            isTimerRunning = false;
            await recorder.StopRecording();

            RecordButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            PlayButton.IsEnabled = true;
            RecordingListButton.IsEnabled = true;
            lblSeconds.Text = "00";
            lblMinutes.Text = "00";
            var storagePermission = await DependencyService.Get<IFileHelper>().CheckPermissionsWriteStorage();
            if (storagePermission == PermissionStatus.Granted)
            {
                if (recorder.GetAudioFilePath() != null)
                {
                    byte[] data = File.ReadAllBytes(recorder.GetAudioFilePath());
                    if (data != null || data.Length > 0)
                    {
                        var result = DependencyService.Get<IFileHelper>().SaveFile(data);
                        if (result)
                        {
                            await DisplayAlert("Done", "Successfull", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Failed", "Recording Not saved", "OK");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Sorry Could,t record Audio", "Audio could not be recorded, Make sure you record some sounds , silence could not be recorded", "OK");
                }
            }
            else
            {
                var askStoragePermission = await DependencyService.Get<IFileHelper>().AskPermissionWriteStorage();
                if (askStoragePermission == PermissionStatus.Granted)
                {
                    
                    if (recorder.GetAudioFilePath() != null)
                    {
                        byte[] data = File.ReadAllBytes(recorder.GetAudioFilePath());
                        if (data != null || data.Length > 0)
                        {
                            var result = DependencyService.Get<IFileHelper>().SaveFile(data);
                            if (result)
                            {
                                await DisplayAlert("Done", "Successfull", "OK");
                            }
                            else
                            {
                                await DisplayAlert("Failed", "Recording Not saved", "OK");
                            }
                        }
                    }

                }
                else
                {
                    await DisplayAlert("Permisssions Not Granted", "Permisssions for 'Mic' and 'Storage or Files and Media' required for Recording and Storing", "OK");
                }
            }

        }
        async void GotoRecordingsPage(object sender, EventArgs e)
        {
            var permission = await DependencyService.Get<IFileHelper>().CheckPermissionsWriteStorage();
            await Navigation.PushAsync(new ListRecordings(permission));
        }
        
    }
}
