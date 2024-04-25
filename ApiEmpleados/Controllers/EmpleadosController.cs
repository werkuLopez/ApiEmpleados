using ApiEmpleados.Helpers;
using ApiEmpleados.Models;
using ApiEmpleados.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ApiEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private EmpleadosRepository repo;
        private HelperActionServicesOAuth helper;

        public EmpleadosController(EmpleadosRepository repo, HelperActionServicesOAuth helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>> LogIn(LoginModel model)
        {
            Empleado emp =
                await this.repo.LogInAsync(model.Username, model.Password);

            if (emp != null)
            {
                SigningCredentials credentials =

                    new SigningCredentials(
                        this.helper.GetKeyToken()
                        , SecurityAlgorithms.HmacSha256);


                JwtSecurityToken token =
                    new JwtSecurityToken(
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        notBefore: DateTime.UtcNow
                        );

                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().
                    WriteToken(token)
                });
            }
            return Unauthorized();
        }

        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> Empleados()
        {
            List<Empleado> empleados =
                await this.repo.GetEmpleadosAsync();

            return Ok(empleados);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Empleado>> Empleado(int id)
        {
            Empleado emp =
                await this.repo.GetEmpleadoByIdAsync(id);
            return Ok(emp);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> Oficios()
        {
            List<string> oficios =
                await this.repo.GetOficiosAsync();

            return Ok(oficios);
        }
        [Authorize]
        [HttpGet]
        [Route("[action]/{oficio}")]
        public async Task<ActionResult<List<Empleado>>> EmpleadosOficio(string oficio)
        {
            List<Empleado> empleados =
                await this.repo.GetEmpleadosByOficioAsync(oficio);

            return Ok(empleados);
        }
    }
}
