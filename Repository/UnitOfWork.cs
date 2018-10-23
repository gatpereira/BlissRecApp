using System;
using DBBliss;

namespace Repository
{
    public class UnitOfWork : IDisposable
    {
        private readonly BlissDBEntities ctx = new BlissDBEntities();

        private bool disposed = false;

        /// <summary>
        /// Dispose the context
        /// </summary>
        /// <param name="disposing">The <see cref="bool"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ctx.Dispose();
                }
            }
            disposed = true;
        }

        /// <summary>
        /// Call the dispose virtual method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Commits the changes to DB
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int Save()
        {
            return ctx.SaveChanges();
        }

        private GenericRepository<DT_QUESTION> questionRepository;
        private GenericRepository<DT_CHOICE> choiceRepository;

        public GenericRepository<DT_QUESTION> QuestionRepository
        {
            get
            {
                if (questionRepository == null)
                    questionRepository = new GenericRepository<DT_QUESTION>(ctx);
                return questionRepository;
            }
        }

        public GenericRepository<DT_CHOICE> ChoiceRepository
        {
            get
            {
                if (choiceRepository == null)
                    choiceRepository = new GenericRepository<DT_CHOICE>(ctx);
                return choiceRepository;
            }
        }


    }
}
