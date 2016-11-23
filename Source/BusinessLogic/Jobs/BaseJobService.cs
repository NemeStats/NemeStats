using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BusinessLogic.Models;
using BusinessLogic.Models.Games;
using RollbarSharp;

namespace BusinessLogic.Jobs
{
    public abstract class BaseJobService
    {
        protected readonly IRollbarClient RollbarClient;

        protected BaseJobService(IRollbarClient rollbar)
        {
            RollbarClient = rollbar;
        }


    }
}