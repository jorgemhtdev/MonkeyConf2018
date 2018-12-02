﻿namespace MonkeyConf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using MonkeyConf.Models;
    using MonkeyConf.Services.Camera;
    using MonkeyConf.Services.Prediction;
    using Xamarin.Forms;

    public class HotDogViewModel : BindableObject
    {
        private ImageSource _photo;
        private string _hotDogOrNot;
        private List<Prediction> _predictions;

        public ImageSource Photo
        {
            get { return _photo; }
            set
            {
                _photo = value;
                OnPropertyChanged();
            }
        }

        public string HotDogOrNot
        {
            get { return _hotDogOrNot; }
            set
            {
                _hotDogOrNot = value;
                OnPropertyChanged();
            }
        }

        public List<Prediction> Predictions
        {
            get { return _predictions; }
            set
            {
                _predictions = value;
                OnPropertyChanged();
            }
        }

        public ICommand PickPhotoCommand => new Command(async () => await PickPhotoAsync());
        public ICommand TakePhotoCommand => new Command(async () => await TakePhotoAsync());

        private async Task PickPhotoAsync()
        {
            var photo = await CameraService.Instance.PickPhotoAsync();

            if (photo != null)
            {
                Photo = ImageSource.FromStream(() => photo.GetStream());

                var predictionResult = await PredictionService.Instance.MakePredictionRequestAsync(photo);

                Predictions = predictionResult.Predictions;

                HotDogOrNot = "NOT HOTDOG";

                foreach (var item in predictionResult.Predictions)
                {
                    if (IsHotdog(item))
                    {
                        HotDogOrNot = "HOTDOG!";
                        break;
                    }
                }
            }
        }

        private async Task TakePhotoAsync()
        {
            var photo = await CameraService.Instance.TakePhotoAsync();

            if (photo != null)
            {
                Photo = ImageSource.FromStream(() => photo.GetStream());

                var predictionResult = await PredictionService.Instance.MakePredictionRequestAsync(photo);

                Predictions = predictionResult.Predictions;

                HotDogOrNot = "NOT HOTDOG";

                foreach (var item in predictionResult.Predictions)
                {
                    if (IsHotdog(item))
                    {
                        HotDogOrNot = "HOTDOG!";
                        break;
                    }
                }
            }
        }

        private bool IsHotdog(Prediction prediction)
        {
            bool isHotdogImage = false;

            if ((prediction.TagName.Equals("hotdog", StringComparison.InvariantCultureIgnoreCase))
                && (prediction.Probability >= 0.75))
                isHotdogImage = true;

            return isHotdogImage;
        }
    }
}