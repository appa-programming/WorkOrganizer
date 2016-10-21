namespace WorkOrganizer.Specs
{
    public enum DatabaseMessageState
    {
        OK,
        ERROR
    }

    public class DatabaseMessage
    {
        public DatabaseMessageState State { get; private set; }
        public string Error { get; private set; }

        public DatabaseMessage(DatabaseMessageState st, string err)
        {
            State = st;
            Error = err;
        }

        public DatabaseMessage()
        {
            State = DatabaseMessageState.OK;
        }
    }
}
