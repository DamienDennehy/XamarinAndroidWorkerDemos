using Android.Content;
using Android.Util;
using AndroidX.Concurrent.Futures;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Java.Lang;
using System.Threading.Tasks;

namespace XamarinAndroidWorkerDemos.Workers
{
    public class SimpleListenableWorker : ListenableWorker, CallbackToFutureAdapter.IResolver
    {
        public const string TAG = "SimpleListenableWorker";

        public SimpleListenableWorker(Context context, WorkerParameters workerParams) : 
            base(context, workerParams)
        {
        }

        public override IListenableFuture StartWork()
        {
            Log.Debug(TAG, "Started.");
            return CallbackToFutureAdapter.GetFuture(this);
        }

        public Object AttachCompleter(CallbackToFutureAdapter.Completer p0)
        {
            Log.Debug(TAG, $"Executing.");

            //Switch to background thread.
            Task.Run(async () =>
            {
                //Perform a process here, simulated by a delay for 5 seconds.

                var delaySeconds = 5;
                var progress = 0;
                var progressIncrement = 100 / delaySeconds;
                var dataBuilder = new Data.Builder();

                for (int i = 0; i < delaySeconds+1; i++)
                {
                    await Task.Delay(1000);
                    progress += progressIncrement;
                    dataBuilder.PutInt("Progress", progress);
                    SetProgressAsync(dataBuilder.Build());
                }

                Log.Debug(TAG, "Completed.");

                //Set a Success Result on the completer and return it.
                return p0.Set(Result.InvokeSuccess());
            });

            return TAG;
        }
    }
}