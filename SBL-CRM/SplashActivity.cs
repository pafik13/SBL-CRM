using Android.OS;
using Android.App;
using Android.Util;
using Android.Views;
using Android.Content;

namespace SBLCRM
{
	//https://forums.xamarin.com/discussion/19362/xamarin-forms-splashscreen-in-android
	[Activity(Label = "SBL-CRM", Theme = "@style/MyTheme.Splash", Icon = "@mipmap/s5_logo_v2", MainLauncher = true, NoHistory = true)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			RequestWindowFeature (WindowFeatures.NoTitle);
			Window.AddFlags (WindowManagerFlags.KeepScreenOn);
			StartActivity(new Intent(this, typeof(MainActivity)));
		}
	}
}