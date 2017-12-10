using SupportWheelOfFateWebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SupportWheelOfFateWebApi.Business_Logic
{
    public class BusinessService : IBusinessService
    {
        private IWheelOfFateContext _context;
        private object syncObj = new object();

        public BusinessService(IWheelOfFateContext _wheelOfFateContext)
        {
            this._context = _wheelOfFateContext;
        }

        public int DeleteAllBAUs()
        {
            int removed = 0;
            foreach (var item in _context.BAU)
            {
                _context.BAU.Remove(item);
                removed++;
            }
            _context.SaveChanges();
            return removed;
        }

        public IEnumerable<BAU> GetAllBAUs()
        {
            return _context.BAU.Include(b => b.Person);
        }

        public IEnumerable<BAU> GetBAU(DateTime date)
        {
            lock (syncObj)
            {
                var result = _context.BAU.Include(b => b.Person).Where(table => table.Date == date);
                return result.Any() ? result : GenerateIfNotExist(date);
            }
        }

        private IEnumerable<BAU> GenerateIfNotExist(DateTime date)
        {
            //take employess who didn't complie BAU during last two weeks
            var bausFromLast2Weeks = _context.BAU.Include(b=>b.Person).Where(x => x.Date >= date.AddDays(-14) && x.Date <=date.AddDays(14)).ToList();

            var usedPersons = bausFromLast2Weeks.GroupBy(x => x.Person).Select(y => y.Key).Where(x=>x != null);
            var highlyRecomendedPersons = _context.People.Where(x => !usedPersons.Any() || !usedPersons.Contains(x, new PersonEqualityComparer())).ToList();

            bool personFor1ShiftFound = false;

            if (highlyRecomendedPersons.Any())
            {
                var bau1 = new BAU() { Date = date, HalfOfTheDay = 1, Person = highlyRecomendedPersons.First() };
                personFor1ShiftFound = true;
                _context.BAU.Add(bau1);
                yield return bau1;

                if (highlyRecomendedPersons.Count() > 1)
                {
                    var bau2 = new BAU() { Date = date, HalfOfTheDay = 2, Person = highlyRecomendedPersons.Take(2).Last() };
                    _context.BAU.Add(bau2);
                    yield return bau2;
                    _context.SaveChanges();
                    yield break; //we are done!
                }
            }

            //persons having BAU yesterday cannot be chosen
            var forbidenPersons = _context.BAU.Where(x => x.Date >= date.AddDays(-1) && x.Date <=date.AddDays(1)).Select(x => x.Person);

            //search for the best person for 1st shift (if not found yet)
            if (!personFor1ShiftFound)
            {
                var recomendedPersonFor1Shift = TryGetPossiblePerson(1,forbidenPersons, date);
                if (recomendedPersonFor1Shift != null)
                {
                    var bau1 = new BAU() { Date = date, HalfOfTheDay = 1, Person = recomendedPersonFor1Shift };
                    personFor1ShiftFound = true;
                    _context.BAU.Add(bau1);
                    yield return bau1;
                }
            }

            //search for the best person for 2st shift
            var recomendedPersonFor2Shift = TryGetPossiblePerson(2, forbidenPersons, date);
            if (recomendedPersonFor2Shift != null)
            {
                var bau2 = new BAU() { Date = date, HalfOfTheDay = 2, Person = recomendedPersonFor2Shift };
                _context.BAU.Add(bau2);
                yield return bau2;
            }
            
            _context.SaveChanges();
        }

        private Person TryGetPossiblePerson(int halfOfTheDay, IEnumerable<Person> forbidenPersons, DateTime now)
        {
            var bauFromTwoWeeksUpToYesderdaySymmetric = _context.BAU.Where(x => x.Date > now.AddDays(-14) && x.Date < now.AddDays(-1) || x.Date > now.AddDays(1) && x.Date < now.AddDays(14)).ToList();
            var mostRecomendedPeople = bauFromTwoWeeksUpToYesderdaySymmetric.Where(x => x.HalfOfTheDay != halfOfTheDay)
                                                                           .GroupBy(x => x.Person, new PersonEqualityComparer())
                                                                           .OrderBy(x => x.Count())
                                                                           .Select(x => x.Key);

            var notRecomendedPeople = bauFromTwoWeeksUpToYesderdaySymmetric.Where(x => x.HalfOfTheDay == halfOfTheDay)
                                                                          .Select(x => x.Person)
                                                                          .Distinct(new PersonEqualityComparer());

            if (mostRecomendedPeople.Any())
            {
                foreach (var person in mostRecomendedPeople)
                {
                    if (!forbidenPersons.Contains(person) && !notRecomendedPeople.Contains(person))                       
                        return person;
                }

                foreach (var person in mostRecomendedPeople)
                {
                    if (!forbidenPersons.Contains(person))
                        return person;
                }
            }
            return _context.People.Where(x => !forbidenPersons.Contains(x)).FirstOrDefault();
        }
    }
}

