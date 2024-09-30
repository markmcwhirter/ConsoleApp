namespace DataAccessLayer
{
    public class DataRepository : IDataRepository
    {
        public string GetData()
        {
            return "Data from DataRepository";
        }
    }
}