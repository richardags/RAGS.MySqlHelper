using System;

namespace Debug
{
    public class DatabaseResult
    {
        public object data;
        public Exception error;

        public DatabaseResult(object data)
        {
            this.data = data;
        }
        public DatabaseResult(Exception error)
        {
            this.error = error;
        }
    }
}