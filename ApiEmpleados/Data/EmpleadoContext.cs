using ApiEmpleados.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiEmpleados.Data
{
    public class EmpleadoContext : DbContext
    {
        public EmpleadoContext(DbContextOptions<EmpleadoContext> options) : base(options) { }
        public DbSet<Empleado> Empleados { get; set; }
    }
}
