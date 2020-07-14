using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using AndroidX.Work;
using Java.Lang;
using System.Linq;
using XamarinAndroidWorkerDemos.Workers;

namespace XamarinAndroidWorkerDemos
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, IObserver
    {
        TextView _textView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            _textView = FindViewById<TextView>(Resource.Id.worker_status);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            ObserveWorkers();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_start_worker)
            {
                var simpleWorkerRequest =
                    new OneTimeWorkRequest.Builder(typeof(SimpleWorker))
                    .AddTag(SimpleWorker.TAG)
                    .Build();

                WorkManager.GetInstance(this).BeginUniqueWork(
                    SimpleWorker.TAG, ExistingWorkPolicy.Keep, simpleWorkerRequest)
                    .Enqueue();

                return true;
            }
            else if (id == Resource.Id.action_start_listenable_worker)
            {
                var simpleListenableWorkerRequest =
                    new OneTimeWorkRequest.Builder(typeof(SimpleListenableWorker))
                    .AddTag(SimpleListenableWorker.TAG)
                    .Build();

                WorkManager.GetInstance(this).BeginUniqueWork(
                    SimpleListenableWorker.TAG, ExistingWorkPolicy.Keep, simpleListenableWorkerRequest)
                    .Enqueue();

                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected void ObserveWorkers()
        {
            var workManager = WorkManager.GetInstance(this);

            var simpleWorkerObserver = workManager.GetWorkInfosByTagLiveData(SimpleWorker.TAG);
            simpleWorkerObserver.Observe(this, this);

            var simpleListenableWorkerObserver = workManager.GetWorkInfosByTagLiveData(SimpleListenableWorker.TAG);
            simpleListenableWorkerObserver.Observe(this, this);
        }

        public void OnChanged(Object p0)
        {
            var workInfos = p0.JavaCast<JavaList<WorkInfo>>();
            StringBuilder textViewText = default;

            RunOnUiThread(() =>
            {
                textViewText = new StringBuilder(_textView.Text);
            });

            foreach (var workInfo in workInfos)
            {
                //Ignore the default Xamarin Tag when getting the Tag.
                var name = workInfo.Tags.First(t => !t.Contains("."));
                var progress = workInfo.Progress?.GetInt("Progress", -1) ?? -1;
                if (progress == -1)
                {
                    textViewText.Append($"{System.Environment.NewLine}{name}:{workInfo.GetState()}");
                }
                else
                {
                    textViewText.Append($"{System.Environment.NewLine}{name}:{workInfo.GetState()} {progress}%");
                }
            }

            RunOnUiThread(() =>
            {
                _textView.Text = textViewText.ToString();
            });
        }
    }
}
