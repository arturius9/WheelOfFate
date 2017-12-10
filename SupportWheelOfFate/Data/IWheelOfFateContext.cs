using Microsoft.EntityFrameworkCore;

namespace SupportWheelOfFateWebApi.Data
{
    public interface IWheelOfFateContext
    {
        DbSet<BAU> BAU { get; set; }
        DbSet<Person> People { get; set; }

        void SaveChanges();
    }
}