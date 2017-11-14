using BreadPlayer.Core.Common;
using BreadPlayer.Interfaces;
using System;
using Windows.ApplicationModel.Email;
using Windows.Storage;

namespace BreadPlayer.SettingsViews.ViewModels
{
    public class ContactViewModel : ObservableObject
    {
        public string Email { get; set; }
        private string _contentTitle = "Bug details";

        public string ContentTitle
        {
            get => _contentTitle;
            set => Set(ref _contentTitle, value);
        }

        public string Content { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ChangeContactTypeCommand { get; set; }

        public ContactViewModel()
        {
            SendCommand = new DelegateCommand(Send);
            ChangeContactTypeCommand = new RelayCommand(ChangeContactType);
        }

        private void ChangeContactType(object contactType)
        {
            switch (contactType.ToString())
            {
                case "Bug report":
                    ContentTitle = "Bug details:";
                    break;

                case "Suggestion":
                    ContentTitle = "Your suggestion:";
                    break;

                case "Question":
                    ContentTitle = "Ask anything:";
                    break;

                case "Other":
                    ContentTitle = "Say anything:";
                    break;

                case "Kudos":
                    ContentTitle = "Your appreciation note:";
                    break;
            }
        }

        private async void Send()
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("support@breadplayer.com"));
            emailMessage.To.Add(new EmailRecipient("enkaboot@gmail.com"));
            emailMessage.Body = Content;
            string extraInfo = "\n\r\n\rBreadPlayer Version: v" + DeviceInfoHelper.ApplicationVersion;
            extraInfo += "\nDevice: " + DeviceInfoHelper.DeviceModel;
            extraInfo += "\nOS Version: " + DeviceInfoHelper.SystemVersion;
            extraInfo += "\nOS Architecture: " + DeviceInfoHelper.SystemArchitecture;
            emailMessage.Body += extraInfo;
            emailMessage.Subject = ContentTitle.Replace(":", "");
            if (ContentTitle.Contains("Bug"))
            {
                StorageFolder storageFolder = await KnownFolders.MusicLibrary.GetFolderAsync(".breadplayerLogs");
                if (storageFolder != null)
                {
                    var attachmentFile = await storageFolder.GetFileAsync("BreadPlayer.log");
                    if (attachmentFile != null)
                    {
                        var stream = Windows.Storage.Streams.RandomAccessStreamReference.CreateFromFile(attachmentFile);
                        var attachment = new Windows.ApplicationModel.Email.EmailAttachment(
                                 attachmentFile.Name,
                                 stream);
                        emailMessage.Attachments.Add(attachment);
                    }
                }
            }
            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
    }
}