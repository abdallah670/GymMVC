using GymDAL.Entities.Core;
using GymDAL.Repo.Abstract.Core;
using GymDAL.Repo.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GymDAL.Repo.Implementation.Core
{
    public class FitnessGoalsRepository : Repository<FitnessGoals>, IFitnessGoalsRepository
    {
        public FitnessGoalsRepository(GymDbContext context) : base(context)
        {
        }


    }
}