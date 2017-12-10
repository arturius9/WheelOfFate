using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SupportWheelOfFateWebApi.Data
{
    public class WheelOfFateContext : DbContext, IWheelOfFateContext
    {

        #region ctor
        public WheelOfFateContext(DbContextOptions<WheelOfFateContext> options) : base(options)
        {
            SeedIfEmpty();
        }

        private void SeedIfEmpty()
        {
            if (People.Count() != 0) return;
            People.Add(new Person() { Id = 1, FirstName = "Alice", Surname = "A." });
            People.Add(new Person() { Id = 2, FirstName = "Bob", Surname = "B." });
            People.Add(new Person() { Id = 3, FirstName = "Celine", Surname = "C." });
            People.Add(new Person() { Id = 4, FirstName = "Damian", Surname = "D." });
            People.Add(new Person() { Id = 5, FirstName = "Eliza", Surname = "E." });
            People.Add(new Person() { Id = 6, FirstName = "Frank", Surname = "F." });
            People.Add(new Person() { Id = 7, FirstName = "Gregory", Surname = "G." });
            People.Add(new Person() { Id = 8, FirstName = "Henry", Surname = "H." });
            People.Add(new Person() { Id = 9, FirstName = "Ingrid", Surname = "I." });
            People.Add(new Person() { Id = 10, FirstName = "Jack", Surname = "J." });
            SaveChanges();
        }
        #endregion

        #region properties
        public DbSet<Person> People { get; set; }
        public DbSet<BAU> BAU { get; set; }
        #endregion

        #region overrides

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<BAU>().HasKey(table => new { table.Date, table.HalfOfTheDay });
        }

        void IWheelOfFateContext.SaveChanges()
        {
            SaveChanges();
        }
        #endregion

    }
}
