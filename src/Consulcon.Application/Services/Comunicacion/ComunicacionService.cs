using Consulcon.Application.DTOs.Comunicacion;
using Consulcon.Application.Interfaces.Comunicacion;

namespace Consulcon.Application.Services.Comunicacion;

public class ComunicacionService : IComunicacionService
{
    private readonly IRepository<ComunicadoBlog> _blogRepository;

    public ComunicacionService(IRepository<ComunicadoBlog> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<Result<IEnumerable<ComunicadoBlogDto>>> GetComunicadosByCondominioAsync(int condominioId)
    {
        var entities = await _blogRepository.FindAsync(b => b.IdCondominio == condominioId);
            return Result.Ok(entities.Select(e => new ComunicadoBlogDto
        {
            Id = e.IdBlog,
            IdCondominio = e.IdCondominio,
            FechaPublicacion = e.FechaPublicacion,
            Titulo = e.Titulo,
            ContenidoHtml = e.ContenidoHtml,
            UrlImagen = e.UrlImagen,
            UrlArchivoAdjunto = e.UrlArchivoAdjunto,
            Activo = e.Activo
        }));
    }

    public async Task<Result<ComunicadoBlogDto>> CreateComunicadoAsync(CreateComunicadoDto dto)
    {
        var entity = new ComunicadoBlog
        {
            IdCondominio = dto.IdCondominio,
            FechaPublicacion = DateTime.Now,
            Titulo = dto.Titulo,
            ContenidoHtml = dto.ContenidoHtml,
            UrlImagen = dto.UrlImagen,
            UrlArchivoAdjunto = dto.UrlArchivoAdjunto,
            Activo = true
        };

        await _blogRepository.AddAsync(entity);
        
        return Result.Ok(new ComunicadoBlogDto 
        { 
                Id = entity.IdBlog,
                Titulo = entity.Titulo,
                FechaPublicacion = entity.FechaPublicacion
        });
    }

    public async Task<Result<bool>> DeleteComunicadoAsync(int id)
    {
        var entity = await _blogRepository.GetByIdAsync(id);
        if (entity == null) return Result.Fail<bool>("Comunicado no encontrado");

        await _blogRepository.DeleteAsync(entity);
        return Result.Ok(true);
    }
}
