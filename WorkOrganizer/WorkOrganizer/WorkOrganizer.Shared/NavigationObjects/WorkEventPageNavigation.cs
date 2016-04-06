using System;
using System.Collections.Generic;
using System.Text;
using WorkOrganizer.Specs;

namespace WorkOrganizer.NavigationObjects
{
    public class WorkEventPageNavigation
    {
        public DateTime Date { get; private set; }
        public WorkEvent WorkEvent { get; private set; }

        public WorkEventPageNavigation(DateTime dt) : this(dt, null)
        {
        }

        public WorkEventPageNavigation(DateTime dt, WorkEvent we)
        {
            Date = dt;
            WorkEvent = we;
        }
    }
}
