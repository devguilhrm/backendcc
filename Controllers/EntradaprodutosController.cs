using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ATVbackend.DTOs;
using ATVbackend.Models;

namespace ATVbackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntradaprodutosController : ControllerBase
    {
        private readonly BdAulaBackendContext _context;

        public EntradaprodutosController(BdAulaBackendContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntradaProdutoDTO>>> GetAll()
        {
            var entradas = await _context.Entradaprodutos
                .Select(e => new EntradaProdutoDTO
                {
                    IdEntradaProduto = e.Identradaproduto,
                    IdItem = e.Iditem,
                    DataEntradaProduto = e.DataEntradaProduto,
                    QuantidadeEntrada = e.QuantidadeEntrada,
                    Unidade = e.Unidade,
                    ValorUnitarioEntrada = e.ValorUnitarioEntrada
                }).ToListAsync();

            return Ok(entradas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntradaProdutoDTO>> GetEntrada(int id)
        {
            var entrada = await _context.Entradaprodutos
                .Where(e => e.Identradaproduto == id)
                .Select(e => new EntradaProdutoDTO
                {
                    IdEntradaProduto = e.Identradaproduto,
                    IdItem = e.Iditem,
                    DataEntradaProduto = e.DataEntradaProduto,
                    QuantidadeEntrada = e.QuantidadeEntrada,
                    Unidade = e.Unidade,
                    ValorUnitarioEntrada = e.ValorUnitarioEntrada
                }).FirstOrDefaultAsync();

            if (entrada == null)
            {
                return NotFound(new { mensagem = "Entrada não encontrada" });
            }

            return Ok(entrada);
        }

        [HttpPost]
        public async Task<ActionResult<EntradaProdutoDTO>> Post(EntradaProdutoDTO dTO)
        {

            if (!await _context.Estoques.AnyAsync(e => e.Iditem == dTO.IdItem))
            {
                return BadRequest(new { mensagem = "Produto não encontrado no estoque" });
            }

            var ObjEntrada = new Entradaproduto
            {
                Iditem = dTO.IdItem,
                DataEntradaProduto = dTO.DataEntradaProduto,
                QuantidadeEntrada = dTO.QuantidadeEntrada,
                Unidade = dTO.Unidade,
                ValorUnitarioEntrada = dTO.ValorUnitarioEntrada
            };

            _context.Entradaprodutos.Add(ObjEntrada);


            await _context.SaveChangesAsync();

            dTO.IdEntradaProduto = ObjEntrada.Identradaproduto;

            return CreatedAtAction(
                nameof(GetEntrada),
                new { id = ObjEntrada.Identradaproduto }, dTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, EntradaProdutoDTO dTO)
        {
            var entrada = await _context.Entradaprodutos
                .FirstOrDefaultAsync(e => e.Identradaproduto == id);

            if (entrada == null)
            {
                return NotFound(new { mensagem = "Entrada não encontrada" });
            }

            if (dTO.IdItem != entrada.Iditem)
            {
                return BadRequest(new
                {
                    mensagem = "Não é possível trocar o produto de uma entrada. Exclua e cadastre novamente."
                });
            }

            var estoque = await _context.Estoques.FirstAsync(e => e.Iditem == entrada.Iditem);

            decimal novoSaldo = estoque.QuantidadeEstoque - entrada.QuantidadeEntrada + dTO.QuantidadeEntrada;

            if (novoSaldo < 0)
            {
                return Conflict(new
                {
                    mensagem = "Não é possível reduzir a quantidade: o estoque ficaria negativo"
                });
            }

            entrada.DataEntradaProduto = dTO.DataEntradaProduto;
            entrada.QuantidadeEntrada = dTO.QuantidadeEntrada;
            entrada.Unidade = dTO.Unidade;
            entrada.ValorUnitarioEntrada = dTO.ValorUnitarioEntrada;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(new
                {
                    mensagem = "Não foi possível atualizar a entrada",
                    erro = ex.InnerException?.Message ?? ex.Message
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entrada = await _context.Entradaprodutos
                .FirstOrDefaultAsync(e => e.Identradaproduto == id);

            if (entrada == null)
            {
                return NotFound(new { mensagem = "Entrada não encontrada" });
            }

            var estoque = await _context.Estoques.FirstAsync(e => e.Iditem == entrada.Iditem);

            if (estoque.QuantidadeEstoque < entrada.QuantidadeEntrada)
            {
                return Conflict(new
                {
                    mensagem = "Não é possível excluir: o estoque atual é menor que a quantidade desta entrada"
                });
            }

            _context.Entradaprodutos.Remove(entrada);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Conflict(new
                {
                    mensagem = "Não foi possível excluir a entrada",
                    erro = ex.InnerException?.Message ?? ex.Message
                });
            }

            return NoContent();
        }
    }
}