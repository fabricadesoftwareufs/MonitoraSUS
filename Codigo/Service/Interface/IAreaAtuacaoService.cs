using Model;
using System.Collections.Generic;

namespace Service.Interface
{
    public interface IAreaAtuacaoService
    {
        List<AreaAtuacaoModel> GetAll();
		AreaAtuacaoModel GetById(int id);
    }
}
