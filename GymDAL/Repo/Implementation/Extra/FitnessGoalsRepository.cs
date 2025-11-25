using GymDAL.Entities.External;
using GymDAL.Interfaces.External;
using GymDAL.Repo.Implementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GymDAL.Implementation.External
{
    public class FitnessGoalsRepository : Repository<FitnessGoals>, IFitnessGoalsRepository
    {
        private readonly GymDbContext _context;

        public FitnessGoalsRepository(GymDbContext context) : base(context)
        {
            _context = context;
        }


    }
}