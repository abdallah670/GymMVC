//using  System.ComponentModel.DataAnnotations;
//using System.Linq;
//using TestMVC.DAL.DB;
//namespace TestMVC.Validation
//{
//    public class CheckUniqueName : ValidationAttribute
//    {
//        private readonly  _context;
//        public CheckUniqueName()
//        {
//            _context = new AppDbContext();
//        }
//        protected override ValidationResult IsValid(object? value,ValidationContext validationContext)
//        {
//            if (value is string name)
//            {
//                var exists = _context.Employees.Any(e => e.Name == name);
//                if (exists)
//                {
//                    ErrorMessage = "Employee with this name already exists";
//                    return new ValidationResult(ErrorMessage);
//                }
//            }
//            return ValidationResult.Success;

//        }
//    }
//}
