using System;

namespace MySqlHelper
{
    public class MySqlHelperData
    {
        public Exception error;
        public object data;

        public MySqlHelperData(object data)
        {
            this.data = data;
        }
        public MySqlHelperData(Exception error)
        {
            this.error = error;
        }
    }
}