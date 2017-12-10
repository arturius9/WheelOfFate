using System;
using System.Collections.Generic;
using SupportWheelOfFateWebApi.Data;

namespace SupportWheelOfFateWebApi.Business_Logic
{
    public interface IBusinessService
    {
        IEnumerable<BAU> GetAllBAUs();

        IEnumerable<BAU> GetBAU(DateTime date);

        int DeleteAllBAUs();
    }
}