using System.Collections.Generic;
using RMDemo.DataLayer;
using RMDemo.Domain;

namespace RMDemo.ServiceLayer
{
   public class StudentRegistrationSerivce
   {
      private readonly IStudentRepository _repository;
      private readonly IStudentValidator _studentValidator;

      public StudentRegistrationSerivce(IStudentRepository repository,
         IStudentValidator studentValidator)
      {
         _repository = repository;
         _studentValidator = studentValidator;
      }

      public void AddStudent(string name, int i)
      {
         var student = new Student()
         {
            Name = name,
            Age = i
         };

         bool isValid = _studentValidator.Validate(student);

         if (isValid)
         {
            _repository.Save(student);
         }
      }

      public void AddStudents(IEnumerable<Student> list)
      {
         foreach (var student in list)
         {
            bool isValid = _studentValidator.Validate(student);

            if (isValid)
            {
               _repository.Save(student);
            }
         }
      }

      public void DeleteStudents(IEnumerable<int> studentIds)
      {
         var listToDelete = new List<Student>();

         foreach (var id in studentIds)
         {
            Student student = _repository.GetStudentById(id);
            listToDelete.Add(student);
         }

         _repository.DeleteList(listToDelete);
      }
   }
}