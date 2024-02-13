using LMSEarlyBird.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LMSEarlyBird.Data
{
    public class Seed
    {
        public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                //Roles
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.Administrator))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Administrator));
                if (!await roleManager.RoleExistsAsync(UserRoles.Student))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Student));
                if (!await roleManager.RoleExistsAsync(UserRoles.Teacher))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Teacher));
            }

        }

        public static void SeedCourse(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                if (!context.Courses.Any())
                {

                    Course newCourse = new Course()
                    {

                        CourseName = "Software Engineering",
                        CourseNumber = "3750",
                        CreditHours = 4,
                        StartTime = new TimeOnly(9, 30),
                        EndTime = new TimeOnly(11, 30),
                        DaysOfWeek = "TR",
                        Department = new Department() { DeptCode = "CS", DeptName = "Computer Science" },
                        Room = new Room()
                        {
                            RoomNumber = 318,
                            IsAvailable = false,
                            Building = new Building()
                            {
                                BuildingName = "Noorda"
                            }
                        }
                    };
                    context.Courses.Add(newCourse);
                    context.SaveChanges();
                }
            }
        }

        public static void SeedRooms(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                List<Room> roomList = new List<Room>();
                for(int i = 1; i < 18; i++)
                {
                    Room newRoom = new Room() { RoomNumber = 100 + i, IsAvailable = true, Building = context.Buildings.Find(1)};
                    roomList.Add(newRoom);
                }
                for (int i = 1; i < 18; i++)
                {
                    Room newRoom = new Room() { RoomNumber = 200 + i, IsAvailable = true, Building = context.Buildings.Find(1) };
                    roomList.Add(newRoom);
                }
                for (int i = 1; i < 18; i++)
                {
                    Room newRoom = new Room() { RoomNumber = 300 + i, IsAvailable = true, Building = context.Buildings.Find(1) };
                    roomList.Add(newRoom);
                }
                context.Rooms.AddRange(roomList);
                context.SaveChanges();
            }
        }

        public static void SeedDepartments(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                List<Department> deptList = new List<Department>();
                deptList.Add(new Department() { DeptCode = "ENG", DeptName = "English" });
                deptList.Add(new Department() { DeptCode = "MATH", DeptName = "Mathematics" });
                deptList.Add(new Department() { DeptCode = "FA", DeptName = "Fine Art" });
                deptList.Add(new Department() { DeptCode = "ART", DeptName = "Visual Arts" });

                context.Departments.AddRange(deptList);
                context.SaveChanges();
            }
        }
    }
}
