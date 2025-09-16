using System;

namespace UniRmmz
{
    public class RmmzError : Exception
    {
        public RmmzError(string error, string url = null, Action retryAction = null)
        {
            // TODO
        }
    }
}