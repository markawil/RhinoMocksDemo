using System.Collections.Generic;
using RMDemo.Domain;

namespace RMDemo.DataLayer
{
   public interface IStudentRepository
   {
      void Save(Student student);

      void DeleteList(IEnumerable<Student> list);
      Student GetStudentById(int id);
   }
}