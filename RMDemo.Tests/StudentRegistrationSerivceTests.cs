using System.Collections.Generic;
using NUnit.Framework;
using RMDemo.DataLayer;
using RMDemo.Domain;
using RMDemo.ServiceLayer;
using Rhino.Mocks;

namespace RMDemo.Tests
{
   [TestFixture]
   public class StudentRegistrationSerivceTests
   {
      //SUT
      private StudentRegistrationSerivce _studentService;

      //Dependencies
      private IStudentRepository _repository;
      private IStudentValidator _studentValidator;

      [SetUp]
      public void Setup()
      {
         _repository = MockRepository.GenerateMock<IStudentRepository>();
         _studentValidator = MockRepository.GenerateMock<IStudentValidator>();
         _studentValidator.Stub(d => d.Validate(Arg<Student>.Is.NotNull))
            .Return(true);

         _studentService = new StudentRegistrationSerivce(_repository, _studentValidator);
      }

      [Test]
      public void AddStudent_should_call_repository_Save()
      {
         _studentService.AddStudent("mark", 32);
         _repository.AssertWasCalled(d => d.Save(Arg<Student>.Is.NotNull));
      }

      [Test]
      public void AddStudent_should_call_repo_Save_with_correct_Name_and_Age_in_Student_properties()
      {
         _studentService.AddStudent("Bob", 25);
         _repository.AssertWasCalled(d => d.Save(Arg<Student>.Matches(
            x => x.Age == 25 && x.Name == "Bob")));
      }

      [Test]
      public void AddStudent_should_call_studentValidator_Validate_with_student_as_input_param()
      {
         _studentService.AddStudent("Karen", 28);
         _studentValidator.AssertWasCalled(d => d.Validate(Arg<Student>.Matches(
            x => x.Age == 28 && x.Name == "Karen")));
      }

      [Test]
      public void AddStudent_should_not_call_Repo_Save_if_Student_Name_was_invalid()
      {
         _studentValidator.BackToRecord();
         _studentValidator.Stub(d => d.Validate(Arg<Student>.Matches(
            x => x.Age == 20 && x.Name == "Mark"))).Return(false);
         _studentValidator.Replay();

         _studentService.AddStudent("Mark", 20);
         _repository.AssertWasNotCalled(d => d.Save(Arg<Student>.Is.Anything));
      }

      [Test]
      public void AddStudents_should_call_Repo_Save_the_number_of_students_times()
      {
         var list = getStudentsList();
         _studentValidator.Stub(d => d.Validate(Arg<Student>.Is.Anything)).Return(true);
         _studentService.AddStudents(list);
         _repository.AssertWasCalled(d => d.Save(Arg<Student>.Is.NotNull), x => x.Repeat.Times(3));
      }

      private static IEnumerable<Student> getStudentsList()
      {
         var list = new List<Student>
                       {
                          new Student
                             {
                                Name = "mark",
                                Age = 32
                             },
                          new Student
                             {
                                Name = "bob",
                                Age = 23
                             },
                          new Student
                             {
                                Name = "Tom",
                                Age = 20
                             },
                       };
         return list;
      }

      [Test]
      public void AddStudents_should_not_call_Save_if_student_in_list_was_invalid()
      {
         _studentValidator.BackToRecord();
         var list = getStudentsList();
         _studentValidator.Stub(d => d.Validate(Arg<Student>.Matches(
            x => x.Age >= 21))).Return(true);
         _studentValidator.Replay();

         _studentService.AddStudents(list);
         _repository.AssertWasCalled(d => d.Save(Arg<Student>.Is.NotNull), x => x.Repeat.Times(2));
      }

      [Test]
      public void DeleteStudents_should_call_repo_DeleteList()
      {
         _studentService.DeleteStudents(new[]{ 1, 3, 6});
         _repository.AssertWasCalled(d => d.DeleteList(Arg<IEnumerable<Student>>.Is.NotNull));
      }

      [Test]
      public void DeleteStudents_should_call_repo_DeleteList_with_matching_Students()
      {
         var student1 = new Student
                              {
                                 Age = 25, Name = "Bob", Id = 1
                              };
         _repository.Stub(d => d.GetStudentById(1))
            .Return(student1);

         var student2 = new Student
                              {
                                 Age = 27, Name = "Mark", Id = 3
                              };
         _repository.Stub(d => d.GetStudentById(3))
            .Return(student2);

         var student3 = new Student
                              {
                                 Age = 35, Name = "Sam", Id = 6
                              };
         _repository.Stub(d => d.GetStudentById(6))
            .Return(student3);

         _studentService.DeleteStudents(new[] {1, 3, 6});
         _repository.AssertWasCalled(d => d.DeleteList(Arg<IEnumerable<Student>>.List.ContainsAll(
            new List<Student>
               {
                  student1,
                  student2,
                  student3,
               })));
      }
   }
}
