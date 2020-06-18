using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Db
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> users { set; get; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
        }
    }
    public class User {
        public int id { set; get; }
        public string name { set; get; }
        public string passwordHash { set; get; }

        public User() { }
        public User(String name, String password) {
            this.name = name;
            setPasswordAsHash(password);
        }

        private void setPasswordAsHash(String source)
        {
            using (System.Security.Cryptography.SHA256 sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                //From String to byte array
                byte[] sourceBytes = System.Text.Encoding.UTF8.GetBytes(source);
                byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
                passwordHash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
        }
    }
}
