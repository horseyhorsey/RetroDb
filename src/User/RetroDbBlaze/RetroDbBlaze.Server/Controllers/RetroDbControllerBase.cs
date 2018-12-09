using Microsoft.AspNetCore.Mvc;
using RetroDb.Repo;

namespace RetroDbBlaze.Server.Controllers
{
    /// <summary>
    /// RetroDbBase for IUnitOfWork.
    /// </summary>
    public abstract class RetroDbControllerBase : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;

        public RetroDbControllerBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}
