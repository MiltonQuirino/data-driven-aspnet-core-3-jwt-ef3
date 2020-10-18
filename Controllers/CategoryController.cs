using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Backoffice.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using Backoffice.Data;
using Microsoft.EntityFrameworkCore;

[Route("v1/categories")]
public class CategoryController : ControllerBase
{

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    public async Task<ActionResult<List<Category>>> Get([FromServices] DataContext context)
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(
        int id,
        [FromServices] DataContext context
    )
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);

        }
        catch (Exception ex)
        {

            return BadRequest(new { message = $"Categoria não encontrada. Trace : {ex.Message}" });
        }

    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Post(
        [FromBody] Category model,
        [FromServices] DataContext context
    )
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            context.Categories.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);

        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Não foi possível criar a categoria. trace: {ex.Message}" });
        }

    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Put(
        int id,
        [FromBody] Category model,
        [FromServices] DataContext context
    )
    {

        if (id != model.Id)
        {
            return NotFound(new { message = "Categoria não encontrada" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return model;
        }
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Não foi possive atualiza a categoria no banco de dados" });
        }
        catch (Exception ex)
        {

            return BadRequest(new { message = $"Não foi possível atualizar a categoria. trace: {ex.Message}" });
        }

    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Delete(
        int id,
        [FromServices] DataContext context
        )
    {
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
        {
            return NotFound(new { message = "Categoria não encontrada" });
        }

        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new { message = "Categoria removida com sucesso" });
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possivel remover a categoria" });
        }
    }

}