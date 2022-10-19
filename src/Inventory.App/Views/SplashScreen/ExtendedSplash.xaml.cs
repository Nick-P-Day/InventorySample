using Inventory.Services;
using Inventory.ViewModels;
using Windows.ApplicationModel.Activation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Inventory.Views.SplashScreen
{

    public sealed partial class ExtendedSplash : Page
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private readonly Windows.ApplicationModel.Activation.SplashScreen splashScreen; // Variable to hold the splash screen object.
        private readonly Frame rootFrame;
        private readonly IActivatedEventArgs activatedEventArgs;

        public ExtendedSplash(IActivatedEventArgs e, bool loadState)
        {
            this.InitializeComponent();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

            splashScreen = e.SplashScreen;
            activatedEventArgs = e;

            if (splashScreen != null)
            {
                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splashScreen.ImageLocation;
            }

            Resize();
            rootFrame = new Frame();
            LoadDataAsync(activatedEventArgs);
        }

        private async void LoadDataAsync(IActivatedEventArgs e)
        {
            ActivationInfo activationInfo = ActivationService.GetActivationInfo(e);

            await Startup.ConfigureAsync();

            ShellArgs shellArgs = new ShellArgs
            {
                ViewModel = activationInfo.EntryViewModel,
                Parameter = activationInfo.EntryArgs,
                UserInfo = await TryGetUserInfoAsync(e as IActivatedEventArgsWithUser)
            };

#if SKIP_LOGIN
            rootFrame.Navigate(typeof(MainShellView), shellArgs);
            var loginService = ServiceLocator.Current.GetService<ILoginService>();
            loginService.IsAuthenticated = true;
#else
            rootFrame.Navigate(typeof(LoginView), shellArgs);
#endif

            Window.Current.Content = rootFrame;

            Window.Current.Activate();

        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        private void Resize()
        {
            if (splashScreen == null)
            {
                return;
            }

            // The splash image's not always perfectly centered. Therefore we need to set our image's position 
            // to match the original one to obtain a clean transition between both splash screens.

            this.splashImage.Height = splashScreen.ImageLocation.Height;
            this.splashImage.Width = splashScreen.ImageLocation.Width;

            this.splashImage.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top);
            this.splashImage.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left);

            this.progressRing.SetValue(Canvas.TopProperty, splashScreen.ImageLocation.Top + splashScreen.ImageLocation.Height + 50);
            this.progressRing.SetValue(Canvas.LeftProperty, splashScreen.ImageLocation.Left + (splashScreen.ImageLocation.Width / 2) - (this.progressRing.Width / 2));
        }

        private void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splashScreen != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splashScreen.ImageLocation;
                Resize();
            }
        }

        private async Task<UserInfo> TryGetUserInfoAsync(IActivatedEventArgsWithUser argsWithUser)
        {
            if (argsWithUser != null)
            {
                User user = argsWithUser.User;
                UserInfo userInfo = new UserInfo
                {
                    AccountName = await user.GetPropertyAsync(KnownUserProperties.AccountName) as String,
                    FirstName = await user.GetPropertyAsync(KnownUserProperties.FirstName) as String,
                    LastName = await user.GetPropertyAsync(KnownUserProperties.LastName) as String
                };
                if (!userInfo.IsEmpty)
                {
                    if (String.IsNullOrEmpty(userInfo.AccountName))
                    {
                        userInfo.AccountName = $"{userInfo.FirstName} {userInfo.LastName}";
                    }
                    var pictureStream = await user.GetPictureAsync(UserPictureSize.Size64x64);
                    if (pictureStream != null)
                    {
                        userInfo.PictureSource = await BitmapTools.LoadBitmapAsync(pictureStream);
                    }
                    return userInfo;
                }
            }
            return UserInfo.Default;
        }
    }
}
