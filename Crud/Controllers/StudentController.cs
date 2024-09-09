using Crud.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Crud.Controllers
{
    public class StudentController : Controller


    {


        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public StudentController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpGet]
        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@StudentName", student.StudentName);
                    parameters.Add("@Email", student.Email);
                    parameters.Add("@Phone", student.Phone);
                    parameters.Add("@Subscribe", student.Subscribe);

                    await connection.ExecuteAsync("AddStudent", parameters, commandType: CommandType.StoredProcedure);
                }

                return RedirectToAction("StudentList");
            }

            return View(student);
        }
        [HttpGet]
        public async Task<IActionResult> StudentList()
        {
            IEnumerable<Student> students;
            int totalCount;

            using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                students = await db.QueryAsync<Student>("GetStudentList", commandType: CommandType.StoredProcedure);
                totalCount = await db.QuerySingleAsync<int>("GetTotalCount", commandType: CommandType.StoredProcedure);
            }

            var viewModel = new StudentListViewModel
            {
                Students = students,
                TotalCount = totalCount
            };

            return View(viewModel);
        }





        // Edit a student
        [HttpGet]
        public async Task<IActionResult> EditStudent(int id)
        {
            Student student;
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@StudentId", id);

                student = await connection.QueryFirstOrDefaultAsync<Student>("GetStudentById", parameters, commandType: CommandType.StoredProcedure);
            }

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@StudentId", student.StudentID);
                    parameters.Add("@StudentName", student.StudentName);
                    parameters.Add("@Email", student.Email);
                    parameters.Add("@Phone", student.Phone);
                    parameters.Add("@Subscribe", student.Subscribe);

                    await connection.ExecuteAsync("EditStudent", parameters, commandType: CommandType.StoredProcedure);
                }

                return RedirectToAction("StudentList");
            }

            return View(student);
        }

        // Delete a student
        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@StudentId", id);

                await connection.ExecuteAsync("DeleteStudent", parameters, commandType: CommandType.StoredProcedure);
            }

            return RedirectToAction("StudentList");
        }






    }
}



