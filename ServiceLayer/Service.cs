using DataAccessLayer;
using System;

namespace ServiceLayer
{
    public class Service : IService
    {
        private readonly IDataRepository _dataRepository;

        public Service(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public void ProcessData()
        {
            string data = _dataRepository.GetData();
            Console.WriteLine($"Service is processing: {data}");
        }
    }
}