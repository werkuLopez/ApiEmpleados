using ApiEmpleados.Data;
using ApiEmpleados.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiEmpleados.Repositories
{
    public class EmpleadosRepository
    {
        private EmpleadoContext context;

        public EmpleadosRepository(EmpleadoContext context)
        {
            this.context = context;
        }

        private async Task<int> GetMaxIdEmpleadoAsync()
        {
            if (this.context.Empleados.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.Empleados.MaxAsync(x => x.IdEmpleado) + 1;
            }
        }
        public async Task<Empleado> InsertarAsync(Empleado empleado)
        {
            Empleado emp = new Empleado();
            emp.IdEmpleado = await this.GetMaxIdEmpleadoAsync();
            emp.Apellido = empleado.Apellido;
            emp.Salario = empleado.Salario;
            emp.Oficio = empleado.Oficio;
            emp.IdDept = empleado.IdDept;

            this.context.Empleados.Add(emp);
            await this.context.SaveChangesAsync();

            return emp;
        }

        public async Task EliminarAsync(int id)
        {
            Empleado emp =
                await this.context.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == id);

            this.context.Empleados.Remove(emp);
            await this.context.SaveChangesAsync();
        }

        public async Task<Empleado> GetEmpleadoByIdAsync(int id)
        {
            Empleado emp =
                await this.context.Empleados.FirstOrDefaultAsync(x => x.IdEmpleado == id);

            return emp;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }

        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Empleados
                            select datos.Oficio).Distinct();

            return await consulta.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosByOficioAsync(string oficio)
        {
            return await this.context.Empleados.Where(x => x.Oficio == oficio).ToListAsync();
        }

        public async Task<Empleado> LogInAsync(string username, string password)
        {
            return await this.context.Empleados.Where
                (x => x.Apellido == username && x.IdEmpleado == int.Parse(password)).FirstOrDefaultAsync();
        }
    }
}
