using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var memberService = new MemberService();
            var universityService = new UniversityService();

            while (true)
            {
                Console.WriteLine("Press 1: University Managment | Press 2 : Member Managment | Press 0 : Exit");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("-----------------Wniversity Managment Service------------------------");
                        Console.WriteLine("Press 1 : Add University | Press 2 : Update University | Press 3 : GetAll Universitys| Press 4: Delete University | Press 0 : Exit");
                        int universityChoice = int.Parse(Console.ReadLine());
                        switch (universityChoice)
                        {
                            case 1:
                                var university1 = NewUniversity();
                                universityService.AddUniversity(university1);
                                break;
                            case 2:
                                var university2 = NewUniversity();
                                universityService.UpdateUniversity(university2);
                                break;
                            case 3:
                                universityService.ShowAllUniversitys();
                                break;
                            case 4:
                                Console.WriteLine("Enter University Id for deleting");
                                int deleteId = int.Parse(Console.ReadLine());
                                universityService.DeleteUniversity(deleteId);
                                break;
                            case 0:
                                return;
                        }
                        break;
                    case 2:
                        Console.WriteLine("-----------------Wniversity Managment Service------------------------");
                        Console.WriteLine("Press 1 : Add Member | Press 2 : Update Member | Press 3 : GetAll Members| Press 4: Delete Member | Press 0 : Exit");
                        int memberChoice = int.Parse(Console.ReadLine());
                        switch (memberChoice)
                        {
                            case 1:
                                var member1 = NewMember();  
                                memberService.AddMember(member1);
                                break;
                            case 2:
                                var member2 = NewMember();
                                memberService.AddMember(member2);
                                break;
                            case 3:
                                memberService.ShowAllMembers();
                                break;
                            case 4:
                                Console.WriteLine("Enter Member Id for deleting");
                                int deleteId = int.Parse(Console.ReadLine());
                                memberService.DeleteMember(deleteId);             
                                break;
                            case 0:
                                return;
                        }
                        break;
                    case 0:
                        return;
                }
            }
        }

        public static University NewUniversity()
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter Name: ");
            string name = Console.ReadLine();

            Console.Write("Enter City: ");
            string city = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            return new University
            {
                Id = id,
                Name = name,
                City = city,
                Email = email
            };
        }

        public static Member NewMember()
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter First Name: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter Age: ");
            int age = int.Parse(Console.ReadLine());

            Console.Write("Enter University ID: ");
            int universityId = int.Parse(Console.ReadLine());

            Console.Write("Enter Type (Student = 0, Teacher = 1): ");
            MemberType type = (MemberType)int.Parse(Console.ReadLine());

            return new Member
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Age = age,
                UniversityId = universityId,
                Type = type
            };
        }
    }

    public enum MemberType
    {
        Student,
        Teacher
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public int UniversityId { get; set; }
        public MemberType Type { get; set; }

    }
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
    }


    public interface IRepository<T>
    {
        void Add(T member);
        List<T> GetAll();
        void Update(T member);
        void Delete(int id);
    }

    static class ConnectionSrting
    {
        public const string CONNECTION_STRING = "Data Source=.;Initial Catalog=UniversityDB;Integrated Security=True;Encrypt=False";
    }

    public class MemberRepo : IRepository<Member>
    {
        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "Delete * from Members where Id = @Id";
                    command.Parameters.Add(new SqlParameter("@Id", id));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Add(Member member)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "insert into Members values(@Id, @FirstName, @LastName, @Age, @UniversityId, @Type)";
                    command.Parameters.Add(new SqlParameter("@Id", member.Id));
                    command.Parameters.Add(new SqlParameter("@FirstName", member.FirstName));
                    command.Parameters.Add(new SqlParameter("@LastName", member.LastName));
                    command.Parameters.Add(new SqlParameter("@Age", member.Age));
                    command.Parameters.Add(new SqlParameter("@UniversityId", member.UniversityId));
                    command.Parameters.Add(new SqlParameter("@Type", member.Type));
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Member> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                List<Member> members = new List<Member>();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "select * from Member";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Member member = new Member();
                            member.Id = int.Parse(reader["ID"].ToString());
                            member.FirstName = reader["FirstName"].ToString();
                            member.LastName = reader["LastName"].ToString();
                            member.Age = int.Parse(reader["Age"].ToString());
                            member.UniversityId = int.Parse(reader["UniversityId"].ToString());
                            member.Type = (MemberType)Enum.Parse(typeof(MemberType), reader["Type"].ToString());

                            members.Add(member);
                        }
                    }
                }
                return members;
            }
        }

        public void Update(Member member)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "update Member set FirstName = @FirstName, LastName = @LastName, Age = @Age, UniversityId = @UniversityId, Type = @Type ";
                    command.Parameters.Add(new SqlParameter("@Firstame", member.FirstName));
                    command.Parameters.Add(new SqlParameter("@LastName", member.LastName));
                    command.Parameters.Add(new SqlParameter("@Age", member.Age));
                    command.Parameters.Add(new SqlParameter("@UniversityId", member.UniversityId));
                    command.Parameters.Add(new SqlParameter("@Type", member.Type));

                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public class UniversityRepo : IRepository<University>
    {
        public void Add(University university)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "insert into University values(@Id, @Name, @City, @Email)";
                    command.Parameters.Add(new SqlParameter("@Id", university.Id));
                    command.Parameters.Add(new SqlParameter("@Name", university.Name));
                    command.Parameters.Add(new SqlParameter("@City", university.City));
                    command.Parameters.Add(new SqlParameter("@Email", university.Email));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "Delete * from University where Id = @Id";
                    command.Parameters.Add(new SqlParameter("@Id", id));
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<University> GetAll()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                List<University> universitys = new List<University>();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "select * from University";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            University university = new University();
                            university.Id = int.Parse(reader["ID"].ToString());
                            university.Name = reader["NAme"].ToString();
                            university.City = reader["City"].ToString();
                            university.Email = reader["Email"].ToString();

                            universitys.Add(university);
                        }
                    }
                }
                return universitys;
            }
        }

        public void Update(University university)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionSrting.CONNECTION_STRING))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "update University set Name = @Name, City = @City, Email = @Email";
                    command.Parameters.Add(new SqlParameter("@Name", university.Name));
                    command.Parameters.Add(new SqlParameter("@City", university.City));
                    command.Parameters.Add(new SqlParameter("@Email", university.Email));

                    command.ExecuteNonQuery();
                }
            }
        }
    }

   public class MemberService
    {
        private MemberRepo _repo = new MemberRepo();
        public void AddMember(Member member)
        {
            var result = _repo.GetAll().FirstOrDefault(x => x.Id == member.Id);
            if (result != null)
            {
                Console.WriteLine($"Member with ID {member.Id} already exists.");
                return;
            }

            _repo.Add(member);
            Console.WriteLine("Member added successfully.");
        }

        public void ShowAllMembers()
        {
            var members = _repo.GetAll();
            if (members.Count == 0)
            {
                Console.WriteLine("No members found.");
                return;
            }
            foreach (var el in members)
            {
                Console.WriteLine($"{el.Id}: {el.FirstName} {el.LastName}, Age: {el.Age}, UniversityId: {el.UniversityId}, Type: {el.Type}");
            }
        }
        public void UpdateMember(Member member)
        {
            var result = _repo.GetAll().FirstOrDefault(x => x.Id == member.Id);
            if (result == null)
            {
                Console.WriteLine($"Member with ID {member.Id} does not exist.");
                return;
            }
            _repo.Update(member);
            Console.WriteLine("Member updated successfully.");
        }

        public void DeleteMember(int id)
        {
            var result = _repo.GetAll().FirstOrDefault(x => x.Id == id);
            if (result == null)
            {
                Console.WriteLine($"Member with ID {id} does not exist.");
                return;
            }

            _repo.Delete(id);
            Console.WriteLine("Member deleted successfully.");
        }
    }

    public class UniversityService
    {
        private UniversityRepo _repo = new UniversityRepo();
        public void AddUniversity(University university)
        {
            var result = _repo.GetAll().FirstOrDefault(x => x.Id == university.Id);
            if (result != null)
            {
                Console.WriteLine($"University with ID {university.Id} already exists.");
                return;
            }

            _repo.Add(university);
            Console.WriteLine("University added successfully.");
        }

        public void ShowAllUniversitys()
        {
            var universitys = _repo.GetAll();
            if (universitys.Count == 0)
            {
                Console.WriteLine("No members found.");
                return;
            }
            foreach (var el in universitys)
            {
                Console.WriteLine($"ID :{el.Id}, Name: {el.Name}, City: {el.City}, Email: {el.Email}");
            }
        }
        public void UpdateUniversity(University university)
        {
            var result = _repo.GetAll().FirstOrDefault(x => x.Id == university.Id);
            if (result == null)
            {
                Console.WriteLine($"University with ID {university.Id} does not exist.");
                return;
            }
            _repo.Update(university);
            Console.WriteLine("University updated successfully.");
        }

        public void DeleteUniversity(int id)
        {
            var result = _repo.GetAll().FirstOrDefault(x => x.Id == id);
            if (result == null)
            {
                Console.WriteLine($"University with ID {id} does not exist.");
                return;
            }

            _repo.Delete(id);
            Console.WriteLine("University deleted successfully.");
        }
    }
}
