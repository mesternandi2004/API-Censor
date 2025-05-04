using CENZURAZO_CQW1QQ_MESTER.Models;
using Microsoft.EntityFrameworkCore;

namespace CENZURAZO_CQW1QQ_MESTER.Data
{
    public class CensorRepository : ICensorRepository
    {
        private readonly CensorDbContext db;

        public CensorRepository(CensorDbContext db)
        {
            this.db = db;
        }

        public void Create(ReplacementData data)
        {
            this.db.ReplacementDatas.Add(data);
            db.SaveChanges();
        }

        public IEnumerable<ReplacementData> Read()
        {
            return this.db.ReplacementDatas
                .Include(x => x.Alternatives);
        }

        public ReplacementData? Read(int id)
        {
            return this.db.ReplacementDatas
                .Include(x => x.Alternatives)
                .FirstOrDefault(x => x.ID == id);
        }

        public void Update(ReplacementData data)
        {
            var toUpdate = this.Read(data.ID);

            if (toUpdate == null) return;

            toUpdate.Word = data.Word;

            db.AlternativeWords.RemoveRange(toUpdate.Alternatives);

            toUpdate.Alternatives = data.Alternatives;

            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var toDelete = this.Read(id);

            if (toDelete == null) return;

            db.AlternativeWords.RemoveRange(toDelete.Alternatives);

            this.db.ReplacementDatas.Remove(toDelete);

            db.SaveChanges();
        }

        public IEnumerable<ReplacementData> GetAll()
        {
            return db.ReplacementDatas.Include(r => r.Alternatives).ToList();
        }

        public bool DeleteByWord(string word)
        {
            var wordEntry = db.ReplacementDatas
                .Include(r => r.Alternatives)
                .FirstOrDefault(r => r.Word == word);

            if (wordEntry == null) return false;

            db.ReplacementDatas.Remove(wordEntry);
            db.SaveChanges();
            return true;
        }
    }
}
