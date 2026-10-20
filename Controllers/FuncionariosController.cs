using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATVbackend.DTOs;
using ATVbackend.Models;

namespace ATVbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionariosController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public FuncionariosController(BdAulaBackendContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FuncionarioDTO>>> GetAll()
        {
            var funcionarios = await _context.Funcionarios
                .Select(f => new FuncionarioDTO
                {
                    IdPessoa = f.Idpessoa,
                    Cargo = f.Cargo,
                    DataAdmissao = f.DataAdmissao,
                    Pessoa = new PessoaDTO
                    {
                        IdPessoa = f.IdpessoaNavigation.Idpessoa,
                        Nome = f.IdpessoaNavigation.Nome,
                        cpf = f.IdpessoaNavigation.Cpf,
                        DataNascimento = f.IdpessoaNavigation.DataNascimento,
                        sexo = f.IdpessoaNavigation.Sexo,
                        email = f.IdpessoaNavigation.Email,
                        telefone = f.IdpessoaNavigation.Telefone,
                        AtivoPessoa = f.IdpessoaNavigation.AtivoPessoa,
                        Enderecos = f.IdpessoaNavigation.Enderecos.Select(e => new EnderecoDTO
                        {
                            IdPessoa = e.Idpessoa,
                            IdEndereco = e.Idendereco,
                            CEP = e.Cep,
                            Numero = e.Numero,
                            Complemento = e.Complemento
                        }).ToList()
                    }
                }).ToListAsync();

            return Ok(funcionarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FuncionarioDTO>> GetFuncionario(int id)
        {
            var funcionario = await _context.Funcionarios
                .Where(f => f.Idpessoa == id)
                .Select(f => new FuncionarioDTO
                {
                    IdPessoa = f.Idpessoa,
                    Cargo = f.Cargo,
                    DataAdmissao = f.DataAdmissao,
                    Pessoa = new PessoaDTO
                    {
                        IdPessoa = f.IdpessoaNavigation.Idpessoa,
                        Nome = f.IdpessoaNavigation.Nome,
                        cpf = f.IdpessoaNavigation.Cpf,
                        DataNascimento = f.IdpessoaNavigation.DataNascimento,
                        sexo = f.IdpessoaNavigation.Sexo,
                        email = f.IdpessoaNavigation.Email,
                        telefone = f.IdpessoaNavigation.Telefone,
                        AtivoPessoa = f.IdpessoaNavigation.AtivoPessoa,
                        Enderecos = f.IdpessoaNavigation.Enderecos.Select(e => new EnderecoDTO
                        {
                            IdPessoa = e.Idpessoa,
                            IdEndereco = e.Idendereco,
                            CEP = e.Cep,
                            Numero = e.Numero,
                            Complemento = e.Complemento
                        }).ToList()
                    }
                }).FirstOrDefaultAsync();

            if (funcionario == null)
            {
                return NotFound(new { mensagem = "Funcionário não encontrado" });
            }

            return Ok(funcionario);
        }

        [HttpPost]
        public async Task<ActionResult<FuncionarioDTO>> Post(FuncionarioDTO dTO)
        {
            if (await _context.Pessoas.AnyAsync(p => p.Cpf == dTO.Pessoa.cpf))
            {
                return BadRequest(new { mensagem = "Já existe uma pessoa com este CPF" });
            }

            var ObjPessoa = new Pessoa
            {
                Nome = dTO.Pessoa.Nome,
                Cpf = dTO.Pessoa.cpf,
                DataNascimento = dTO.Pessoa.DataNascimento,
                Sexo = dTO.Pessoa.sexo,
                Email = dTO.Pessoa.email,
                Telefone = dTO.Pessoa.telefone,
                AtivoPessoa = dTO.Pessoa.AtivoPessoa,
                Funcionario = new Funcionario
                {
                    Cargo = dTO.Cargo,
                    DataAdmissao = dTO.DataAdmissao
                },
                Enderecos = dTO.Pessoa.Enderecos.Select(e => new Endereco
                {
                    Cep = e.CEP,
                    Numero = e.Numero,
                    Complemento = e.Complemento
                }).ToList()
            };

            _context.Pessoas.Add(ObjPessoa);

            await _context.SaveChangesAsync();

            dTO.IdPessoa = ObjPessoa.Idpessoa;
            dTO.Pessoa.IdPessoa = ObjPessoa.Idpessoa;

            return CreatedAtAction(
                nameof(GetFuncionario),
                new { id = ObjPessoa.Idpessoa }, dTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, FuncionarioDTO dTO)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Enderecos)
                .Include(p => p.Funcionario)
                .FirstOrDefaultAsync(p => p.Idpessoa == id);

            if (pessoa == null || pessoa.Funcionario == null)
            {
                return NotFound(new { mensagem = "Funcionário não encontrado" });
            }

            if (await _context.Pessoas.AnyAsync(p => p.Cpf == dTO.Pessoa.cpf && p.Idpessoa != id))
            {
                return BadRequest(new { mensagem = "Já existe outra pessoa com este CPF" });
            }

            pessoa.Nome = dTO.Pessoa.Nome;
            pessoa.Cpf = dTO.Pessoa.cpf;
            pessoa.DataNascimento = dTO.Pessoa.DataNascimento;
            pessoa.Sexo = dTO.Pessoa.sexo;
            pessoa.Email = dTO.Pessoa.email;
            pessoa.Telefone = dTO.Pessoa.telefone;
            pessoa.AtivoPessoa = dTO.Pessoa.AtivoPessoa;

            pessoa.Funcionario.Cargo = dTO.Cargo;
            pessoa.Funcionario.DataAdmissao = dTO.DataAdmissao;

            foreach (var endPessoa in dTO.Pessoa.Enderecos)
            {
                if (endPessoa.IdEndereco > 0)
                {
                    var endereco = pessoa.Enderecos
                        .FirstOrDefault(e => e.Idendereco == endPessoa.IdEndereco);

                    if (endereco == null)
                    {
                        return BadRequest("O endereço não pertence a esse funcionário");
                    }

                    endereco.Numero = endPessoa.Numero;
                    endereco.Cep = endPessoa.CEP;
                    endereco.Complemento = endPessoa.Complemento;
                }
                else
                {
                    pessoa.Enderecos.Add(new Endereco
                    {
                        Numero = endPessoa.Numero,
                        Cep = endPessoa.CEP,
                        Complemento = endPessoa.Complemento
                    });
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Enderecos)
                .Include(p => p.Funcionario)
                .FirstOrDefaultAsync(p => p.Idpessoa == id);

            if (pessoa == null || pessoa.Funcionario == null)
            {
                return NotFound(new { mensagem = "Funcionário não encontrado" });
            }

            _context.Enderecos.RemoveRange(pessoa.Enderecos);
            _context.Funcionarios.Remove(pessoa.Funcionario);
            _context.Pessoas.Remove(pessoa);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensagem = "Não é possível excluir: o funcionário possui registros vinculados"
                });
            }

            return NoContent();
        }
    }
}