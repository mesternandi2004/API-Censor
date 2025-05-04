using CENZURAZO_CQW1QQ_MESTER.Models;

namespace CENZURAZO_CQW1QQ_MESTER.Data
{
    public interface ICensorRepository
    {
        void Create(ReplacementData data);
        void Delete(int id);
        IEnumerable<ReplacementData> Read();
        ReplacementData? Read(int id);
        void Update(ReplacementData data);
        IEnumerable<ReplacementData> GetAll();  // Ez adja vissza az összes feketelistás adatot
        bool DeleteByWord(string word);
    }
}