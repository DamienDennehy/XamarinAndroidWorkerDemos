using Android.Content;
using Android.Util;
using AndroidX.Work;
using System.Threading;

namespace XamarinAndroidWorkerDemos.Workers
{
    public class SimpleWorker : Worker
    {
        public const string TAG = "SimpleWorker";

        public SimpleWorker(Context context, WorkerParameters workerParams) : 
            base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            Log.Debug(TAG, "Started.");

            //Perform a process here, simulated by sleeping for 5 seconds.

            Thread.Sleep(5000);

            Log.Debug(TAG, "Completed.");

            return Result.InvokeSuccess();
        }
    }
}