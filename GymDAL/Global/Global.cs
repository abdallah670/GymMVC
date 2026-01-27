global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using System.Threading.Tasks;
global using System.ComponentModel.DataAnnotations.Schema;
global using TestMVC.DAL.DB;
global using System.Linq.Expressions;
global using Microsoft.IdentityModel.Tokens;
global using System.Reflection;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity;
global using System.ComponentModel.DataAnnotations;
global using GymDAL.Entities.Users;
global using GymDAL.Entities.Communication;
global using GymDAL.Entities.Nutrition;
global using GymDAL.Entities.Workout;
global using GymDAL.Repo.Abstract;
global using GymDAL.Entities.Financial;
global using GymDAL.Entities.Core;
global using AutoMapper;

public interface IGlobal
{
    bool ToggleStatus(string DeletedBy);
}



