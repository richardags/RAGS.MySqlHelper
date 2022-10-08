using System;

namespace MySqlHelper
{
    public class MySqlHelperResult
    {
        public Exception error;
        public object data;
        public int affected_rows;

        public MySqlHelperResult(object data)
        {
            this.data = data;
        }
        public MySqlHelperResult(int affected_rows)
        {
            this.affected_rows = affected_rows;
        }
        public MySqlHelperResult(Exception error)
        {
            this.error = error;
        }
    }
}