using System;
using System.Collections;
using System.Collections.Generic;

public class DateData : IEnumerable<object[]>
{

    private IEnumerator<object[]> GetDates()
    {
        yield return new object[] { new DateTime(2017, 11, 23) };
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        return GetDates();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}